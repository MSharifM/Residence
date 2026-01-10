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

        #region HomePage

        private async Task<IEnumerable<ResidenceBoxDetailViewModel>> GetMostPopularResidenceAsync()
        {
            string query = "Select * from residence_rate order by Stars DESC limit 10";
            var result = await _dbContext.QueryAsync<ResidenceBoxDetailViewModel>(query);
            return result;
        }

        private async Task<IEnumerable<ResidenceBoxDetailViewModel>> GetEspecialResidenceAsync()
        {
            string query = $"""
                            select rr.*
                            from reservation as r
                            join residence as re on r.ResidenceId = re.ResidenceId
                            join residence_rate as rr on r.ResidenceId = rr.ResidenceId
                            group by(r.ResidenceId)
                            order by count(r.reservationid) desc
                            limit 3
                            """;
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

        public async Task<HomePageViewModel> GetHomePageViewModelsAsync()
        {
            HomePageViewModel result = new HomePageViewModel();
            result.PopularResidencesList = await GetMostPopularResidenceAsync();
            result.LuxResidencesList = await GetLuxuryResidenceAsync();
            result.SuggestResidencesList = await GetSuggestedResidenceAsync();
            result.EspecialResidencesList = await GetEspecialResidenceAsync();

            return result;
        }

        #endregion HomePage

        #region ResidenceDetail

        private async Task<IEnumerable<ResidenceCommentsViewModel>> GetResidenceCommentsAsync(int id)
        {
            string query = $"""
                           select CommentDescription , Rate , c.CreateDate as Date , UserName as Name
                           from comments as c natural join client_reserve_comment as crc join aspnetusers as aspu on crc.userId = aspu.Id
                           where residenceId = {id} and c.commentstatus = 'Ok';
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

        public async Task<ResidenceDetailForHostPanelViewModel> GetResidenceDetailForHost(int residenceId)
        {
            string query = $"""
                            select residencename as Name, Street, PostalCode, Price, description, Capacity, CASE
                               WHEN Situation = 'active' THEN 1
                               ELSE 0
                            END AS Status
                            from residence
                            where residenceid = {residenceId};
                            """;
            var result = await _dbContext.QuerySingleAsync<ResidenceDetailForHostPanelViewModel>(query);
            return result;
        }

        public async Task UpdateResidenceDetail(ResidenceDetailForHostPanelViewModel model, int residenceId)
        {
            int isActive = 2; // index 2 => inactive in mysql
            if (model.Status)
                isActive = 1; // index 1 => active in mysql

            string query = $"""
                            UPDATE `residencedb`.`residence`
                            SET `residencename` = '{model.Name}', postalcode='{model.PostalCode}',
                            street='{model.Street}',description='{model.Description}',situation={isActive},
                            price='{model.Price}',capacity={model.Capacity}
                            WHERE (`ResidenceId` = '{residenceId}');
                            """;
            await _dbContext.ExecuteAsync(query);
        }

        #endregion ResidenceDetail

        #region AllResidences

        public async Task<AllResidencesViewModel> GetAllResidences(string? search, int page = 0)
        {
            var residences = new AllResidencesViewModel()
            {
                ResidenceBoxDetail = await GetAllBoxResidences(search, page),
                CountPage = await CountResidencePages(),
            };
            return residences;
        }

        private async Task<IEnumerable<ResidenceBoxDetailViewModel>> GetAllBoxResidences(string? searchName
            , int page = 0, int countBoxOnPage = 12)
        {
            string query = $"""
                            SELECT *
                            FROM residence_rate
                            where Name like '%{searchName}%'
                            LIMIT {countBoxOnPage} OFFSET {countBoxOnPage * page}
                            """;
            var result = await _dbContext.QueryAsync<ResidenceBoxDetailViewModel>(query);
            return result;
        }

        private async Task<int> CountResidencePages(int countBoxOnPage = 12)
        {
            string query = $"""
                            SELECT count(*) as Count
                            FROM residence_rate
                            """;
            var residences = await _dbContext.QueryAsync<int>(query);
            return (int)Math.Ceiling((double)residences.Single() / countBoxOnPage);
        }

        #endregion AllResidences

        #region Reservation

        private async Task<List<ClientListViewModel>> GetAllClientListForReserve(string userName)
        {
            string query = $"""
                            select PIN, c.FirstName, c.LastName, BirthDate, c.sex as IsMan
                            from (select * from aspnetusers where username = 'hassandsn') as u
                            join clientlist as c on u.id = c.userid;
                            """;
            var result = await _dbContext.QueryAsync<ClientListViewModel>(query);
            return result.ToList();
        }

        private async Task<Tuple<string, decimal>> GetResidenceDetailForReserve(int residenceId)
        {
            string query = $"""
                            select ResidenceName, price as PricePerDay
                            from residence as r
                            where residenceid = '{residenceId}'
                            """;
            var result = await _dbContext.QuerySingleAsync(query);
            return new Tuple<string, decimal>(result.ResidenceName, result.PricePerDay);
        }

        public async Task<ReserveResidenceViewModel> GetDetailForReserve(int residenceId, string userName,
            DateTime startDate, DateTime endDate)
        {
            var residence = await GetResidenceDetailForReserve(residenceId);
            var result = new ReserveResidenceViewModel
            (
                residence.Item1,
                residence.Item2,
                new ReserveDraftViewModel(startDate, endDate),
                await GetAllClientListForReserve(userName)
            );

            return result;
        }

        private async Task<int> InsertIntoPayment(decimal price)
        {
            string query = @"
                            INSERT INTO Payment (CreateDate, Price)
                            VALUES (@CreateDate, @Price);
                            SELECT LAST_INSERT_ID();
                            ";

            var paymentId = await _dbContext.QuerySingleAsync<int>(query, new
            {
                CreateDate = DateTime.Now,
                Price = price,
            });

            return paymentId;
        }

        private async Task<int> InsertIntoReserve(ReserveResidenceViewModel model, int paymentId, int residenceId)
        {
            string query = @"
                            INSERT INTO Reservation (ResidenceId, PayId, DateOfStart, DateOfEnd
                                , NumberOfGuest, AmountPaid, Situation)
                            VALUES (@ResidenceId, @PayId, @DateOfStart, @DateOfEnd, @NumberOfGuest,
                                    @AmountPaid, 'approved');
                            SELECT LAST_INSERT_ID();
                           ";

            var reserveId = await _dbContext.QuerySingleAsync<int>(query, new
            {
                ResidenceId = residenceId,
                PayId = paymentId,
                DateOfStart = model.ReserveDraft.StartDate,
                DateOfEnd = model.ReserveDraft.EndDate,
                NumberOfGuest = model.Clients.Count(),
                AmountPaid = model.Price
            });

            return reserveId;
        }

        private async Task InsertIntoClientReserveComment(string userId, int reservationId)
        {
            string query = @"
                            INSERT INTO Reservation (UserID, CommentId)
                            VALUES (@UserID, @CommentId);
                           ";

            await _dbContext.QuerySingleAsync<int>(query, new
            {
                UserID = userId,
                CommentId = reservationId,
            });
        }

        public async Task<bool> ReserveSubmitAsync(ReserveResidenceViewModel model, int residenceId, string userId)
        {
            try
            {
                int paymentId = await InsertIntoPayment(model.Price);
                int reserveId = await InsertIntoReserve(model, paymentId, residenceId);
                await InsertIntoClientReserveComment(userId, reserveId);
                // TODO: Add new clients to db

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        #endregion Reservation
    }
}