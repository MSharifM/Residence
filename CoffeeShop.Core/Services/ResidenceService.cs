using CoffeeShop.Core.DTOs.Residence;
using CoffeeShop.Core.Services.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace CoffeeShop.Core.Services
{
    public class ResidenceService : IResidenceService
    {
        private IDbConnection _dbContext;

        public ResidenceService(IConfiguration configuration)
        {
            this._dbContext = new MySqlConnection(configuration.GetConnectionString("ResidenceConnection"));
        }

        private async Task<IEnumerable<ResidenceBoxDetailViewModel>> GetMostPopularResidenceAsync()
        {
            string query = "Select * from residence_rate order by Stars DESC limit 10";
            var result = await _dbContext.QueryAsync<ResidenceBoxDetailViewModel>(query);
            return result;
        }

        private async Task<IEnumerable<ResidenceBoxDetailViewModel>> GetLuxuryResidenceAsync()
        {
            string query = "select * from residence_rate order by (price) desc limit 3;";
            var result = await _dbContext.QueryAsync<ResidenceBoxDetailViewModel>(query);
            return result;
        }

        private async Task<IEnumerable<ResidenceBoxDetailViewModel>> GetSuggestedResidenceAsync()
        {
            string query = """
                           select *
                           from residence_rate
                           where price < (select avg(price) from residence_rate)
                           and stars > (select avg(stars) from residence_rate);
                           """;
            var result = await _dbContext.QueryAsync<ResidenceBoxDetailViewModel>(query);
            return result;
        }

        private async Task<IEnumerable<ResidenceCommentsViewModel>> GetResidenceCommentsAsync(int id)
        {
            string query = $"""
                           select CommentDescription , Rate , c.CreateDate as Date , UserName as Name
                           from comments as c natural join client_reserve_comment as crc join aspnetusers as aspu on crc.userId = aspu.Id
                           where residenceId = {id};
                           """;
            var result = await _dbContext.QueryAsync<ResidenceCommentsViewModel>(query);
            return result;
        }

        private async Task<IEnumerable<string>> GetResidenceImagesAsync(int id)
        {
            string query = $"""
                           select imagename as ImageName
                           from residence as r natural join images
                           where r.residenceId = {id}
                           """;
            var result = await _dbContext.QueryAsync<string>(query);
            return result;
        }

        private async Task<IEnumerable<ResidenceOptionsViewModel>> GetResidenceOptionsAsync(int id)
        {
            string query = $"""
                           SELECT Option_Name as OptionName, Option_Description as OptionDescription
                           FROM option_residence as opr natural join h_option
                           where opr.ResidenceId = {id};
                           """;
            var result = await _dbContext.QueryAsync<ResidenceOptionsViewModel>(query);
            return result;
        }

        public async Task<ResidenceDetailViewModel> GetResidenceDetailById(int id)
        {
            string query = $"""
                           SELECT
                           	    r.ResidenceId as ResidenceId,
                                r.ResidenceName AS Name,
                                r.PostalCode as PostalCode,
                                c2.CityName AS City,
                                Street ,
                                Capacity ,
                                avg_comments.Stars AS Stars,
                                avg_comments.count AS CountRate,
                                r.MainImage AS MainImage,
                                r.Price AS Price,
                                Description ,
                                CASE
                                   WHEN r.Situation = 'active' THEN 1
                                   ELSE 0
                                END AS IsActive
                           FROM Residence r
                           JOIN City c2 ON r.CityId = c2.CityId
                           LEFT JOIN (
                                SELECT c.ResidenceId, AVG(c.Rate) AS Stars , count(c.Rate) as count
                                FROM Comments AS c
                                GROUP BY c.ResidenceId
                           ) AS avg_comments ON r.ResidenceId = avg_comments.ResidenceId
                           where r.residenceId = {id}
                           """;
            var residence = await _dbContext.QueryAsync<ResidenceDetailViewModel>(query);
            var result = residence.Single();

            result.Comments = await GetResidenceCommentsAsync(id);
            result.ImageNames = await GetResidenceImagesAsync(id);
            result.Options = await GetResidenceOptionsAsync(id);

            return result;
        }

        public async Task<HomePageViewModel> GetHomePageViewModelsAsync()
        {
            HomePageViewModel result = new HomePageViewModel();
            result.PopularResidencesList = await GetMostPopularResidenceAsync();
            result.LuxResidencesList = await GetLuxuryResidenceAsync();
            result.SuggestResidencesList = await GetSuggestedResidenceAsync();

            return result;
        }
    }
}