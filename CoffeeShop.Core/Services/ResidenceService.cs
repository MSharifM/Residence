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

        private async Task<IEnumerable<ResidenceBoxDetailViewModel>> GetBestResidenceAsync()
        {
            string query = "Select * from residence_rate order by Stars DESC limit 10";
            var result = await _dbContext.QueryAsync<ResidenceBoxDetailViewModel>(query);
            return result;
        }

        public async Task<HomePageViewModel> GetHomePageViewModelsAsync()
        {
            HomePageViewModel result = new HomePageViewModel();
            result.PopularResidencesList = await GetBestResidenceAsync();

            return result;
        }
    }
}