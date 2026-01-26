using CoffeeShop.Core.DTOs.Residence;
using CoffeeShop.Core.Generator;
using CoffeeShop.Core.Services.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Http;
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

        private async Task<IEnumerable<string>> GetResidenceImagesAsync(int residenceId)
        {
            string query = $"""
                            select imagename as ImageName
                            from residence as r natural join images
                            where r.residenceId = {residenceId}
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

        private async Task<List<ReservedDatesViewModel>> GetListReservedDates(int residenceId)
        {
            string query = $"""
                            select dateofstart as startDate, dateofend as endDate
                            from reservation as r
                            where residenceId = {residenceId}
                            """;

            var result = await _dbContext.QueryAsync<ReservedDatesViewModel>(query);

            return result.ToList();
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
                            FROM Residence as r
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
            result.ReservedDates = await GetListReservedDates(id);

            return result;
        }

        public async Task<ResidenceDetailForHostPanelViewModel?> GetResidenceDetailForHost(int residenceId)
        {
            string query = $"""
                            select residencename as Name, Street, PostalCode, Price, description, Capacity, CASE
                               WHEN Situation = 'active' THEN 1
                               ELSE 0
                            END AS Status
                            from residence
                            where residenceid = {residenceId};
                            """;
            var result = await _dbContext.QueryFirstOrDefaultAsync<ResidenceDetailForHostPanelViewModel>(query);
            return result;
        }

        #endregion ResidenceDetail

        #region Manage Residence For Host

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

        public async Task<EditResidenceImagesViewModel> GetResidenceImagesForEditAsync(int residenceId)
        {
            string query = @"
                            SELECT MainImage, ResidenceName
                            FROM residence
                            WHERE residenceid = @ResidenceId
                            ";

            var result = await _dbContext.QueryFirstOrDefaultAsync(query, new { ResidenceId = residenceId });

            List<string> images = new List<string>();
            if (result.MainImage != "no_photo.jpg")
                images.Add("/residence_images/" + result.MainImage);

            var otherImages = (await GetResidenceImagesAsync(residenceId)).ToList();
            images.AddRange((otherImages.Select(item => "/residence_images/otherImages/" + item)));

            var model = new EditResidenceImagesViewModel()
            {
                ResidenceId = residenceId,
                ResidenceName = result.ResidenceName,
                ExistingImages = images
            };

            return model;
        }

        private async Task DeleteResidenceImage(string imageName)
        {
            string query = @"
                            delete from images where imagename = @ImageName ;
                            ";

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/residence_images/otherImages/", imageName);
            File.Delete(imagePath);

            await _dbContext.ExecuteAsync(query, param: new { ImageName = imageName });
        }

        private async Task<string?> SaveImageFile(IFormFile file, bool isMain = false)
        {
            if (file != null)
            {
                string imageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(file.FileName);

                string imagePath = "";
                if (isMain)
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/residence_images/", imageName);
                else
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/residence_images/otherImages/", imageName);

                await using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return imageName;
            }

            return null;
        }

        private async Task<string> GetResidenceMainImage(int residenceId)
        {
            string query = $"""
                            select mainImage from residence
                            where residenceId = {residenceId}
                            """;

            var mainImage = await _dbContext.QuerySingleAsync<string>(query);

            return mainImage;
        }

        private async Task AddResidenceImage(IFormFile image, int residenceId)
        {
            string query = @"
                            INSERT INTO Images (ResidenceId, ImageName)
                            VALUES (@ResidenceId, @ImageName)
                            ";

            string? imageName;
            try
            {
                imageName = await SaveImageFile(image);
                if (imageName is null) return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            await _dbContext.ExecuteAsync(query, new
            {
                ResidenceId = residenceId,
                ImageName = imageName,
            });
        }

        private async Task UpdateMainImage(int residenceId, string imageName)
        {
            string query = $"""
                            UPDATE residence
                            SET `MainImage` = '{imageName}'
                            WHERE (`ResidenceId` = '{residenceId}');
                            """;

            await _dbContext.ExecuteAsync(query);
        }

        public async Task EditImageResidence(List<IFormFile>? newResidenceImages, int residenceId, List<int>? removedResidencesIndex)
        {
            var images = (await GetResidenceImagesAsync(residenceId)).ToList();
            if (removedResidencesIndex != null)
                foreach (var item in removedResidencesIndex)
                {
                    await DeleteResidenceImage(images[item - 1]); // -1 because in image list started by main image
                }

            if (await GetResidenceMainImage(residenceId) == "no_photo.jpg")
            {
                if (newResidenceImages != null && newResidenceImages.Any())
                {
                    //Save the first image as main image
                    var imageName = await SaveImageFile(newResidenceImages.First(), true);
                    newResidenceImages.RemoveAt(0);
                    if (imageName != null) await UpdateMainImage(residenceId, imageName);
                }
            }

            if (newResidenceImages != null)
                foreach (var item in newResidenceImages)
                {
                    //TODO: refactor: Convert to 1 query
                    await AddResidenceImage(item, residenceId);
                }
        }

        public async Task<List<string>> GetAllOptions()
        {
            string query = @"
                            select option_name from h_option;
                            ";

            var options = await _dbContext.QueryAsync<string>(query);

            return options.ToList();
        }

        private async Task<int> GetCityIdByName(string cityName)
        {
            string query = @"
                            select cityId from city where cityName = @CityName;
                            ";

            int cityId = await _dbContext.QuerySingleAsync<int>(query, new { CityName = cityName });

            return cityId;
        }

        private async Task InsertResidenceOptions(int residenceId, List<string> options)
        {
            string query = @"
                            INSERT INTO Option_Residence (ResidenceId, Option_Name)
                            VALUES (@ResidenceId, @OptionName)
                            ";

            //TODO: refactor: Convert to 1 query
            foreach (var item in options)
            {
                await _dbContext.ExecuteAsync(query, new
                {
                    ResidenceId = residenceId,
                    OptionName = item,
                });
            }
        }

        public async Task<int> AddResidence(AddResidenceViewModel model)
        {
            string query = @"
                            INSERT INTO Residence (UserId, CityId, Capacity, Street, PostalCode, ResidenceName, ResidenceType, Star, Situation, CreateDate, MainImage, Description, Price)
                            VALUES (@UserId, @CityId, @Capacity, @Street, @PostalCode, @ResidenceName, @ResidenceType, '5', @Situation, @CreateDate, 'no_photo.jpg', @Description, @Price);
                            SELECT LAST_INSERT_ID();
                            ";

            int residenceId = await _dbContext.QuerySingleAsync<int>(query, new
            {
                UserId = model.UserId,
                CityId = await GetCityIdByName(model.CityName),
                Capacity = model.Capacity,
                Street = model.Street,
                PostalCode = model.PostalCode,
                ResidenceName = model.ResidenceName,
                ResidenceType = model.Type,
                Situation = model.IsActive,
                CreateDate = DateTime.Now,
                Description = model.Description,
                Price = model.PricePerDate,
            });

            if (model.Options != null && model.Options.Any())
                await InsertResidenceOptions(residenceId, model.Options);

            return residenceId;
        }

        public async Task DeleteResidence(int residenceId, string userId)
        {
            string query = $"""
                            delete from residence
                            where residenceId = '{residenceId}' and userId = '{userId}';
                            """;

            await _dbContext.ExecuteAsync(query);
        }

        #endregion Manage Residence For Host

        #region AllResidences

        public async Task<AllResidencesViewModel> GetAllResidences(string? search, int page = 0)
        {
            var residences = new AllResidencesViewModel()
            {
                ResidenceBoxDetail = await GetAllBoxResidences(search, page, 6),
                CountPage = await CountResidencePages(search, 6),
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

        private async Task<int> CountResidencePages(string? searchName, int countBoxOnPage = 12)
        {
            string query = $"""
                            SELECT count(*) as Count
                            FROM residence_rate
                            where Name like '%{searchName}%'
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
                            from (select * from aspnetusers where username = '{userName}') as u
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
                            INSERT INTO Payment (CreatePay, Price)
                            VALUES (@CreatePay, @Price);
                            SELECT LAST_INSERT_ID();
                            ";

            var paymentId = await _dbContext.QuerySingleAsync<int>(query, new
            {
                CreatePay = DateTime.Now,
                Price = price,
            });

            return paymentId;
        }

        private async Task<int> InsertIntoReserve(ReserveResidenceViewModel model, int paymentId, int residenceId)
        {
            string query = @"
                            INSERT INTO Reservation (ResidenceId, PayId, DateOfStart, DateOfEnd
                                , NumberOfGuests, AmountPaid, Situation)
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
                            INSERT INTO Client_Reserve_Comment (UserID, ReservationId)
                            VALUES (@UserID, @ReservationId);
                           ";

            await _dbContext.ExecuteAsync(query, new
            {
                UserID = userId,
                ReservationId = reservationId
            });
        }

        private async Task InsertNewClients(List<ClientListViewModel> newClients, string userId)
        {
            foreach (var item in newClients)
            {
                string query = @"
                            INSERT INTO ClientList (UserId, PIN, FirstName, LastName, BirthDate, Sex)
                            VALUES (@UserId, @PIN, @FirstName, @LastName, @BirthDate, @Sex)
                            ";

                await _dbContext.ExecuteAsync(query, new
                {
                    UserID = userId,
                    PIN = item.Pin,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    BirthDate = item.BirthDate,
                    Sex = item.IsMan
                });
            }
        }

        private async Task InsertIntoClientListReserve(ReserveResidenceViewModel model, int reservationId, string userId)
        {
            //Remove additional clients
            var clients = model.Clients;
            foreach (var item in model.RemovedClientIndices)
            {
                clients.RemoveAt(item);
            }

            clients.AddRange(model.NewClients);

            //Insert clients into ClientList_Reserve table
            foreach (var item in clients)
            {
                string query = @"
                            INSERT INTO ClientList_Reserve (ReservationId, UserId, PIN)
                            VALUES (@ReservationId, @UserId, @PIN)
                            ";

                await _dbContext.ExecuteAsync(query, new
                {
                    ReservationId = reservationId,
                    UserId = userId,
                    PIN = item.Pin
                });
            }
        }

        public async Task<bool> ReserveSubmitAsync(ReserveResidenceViewModel model, int residenceId, string userId)
        {
            try
            {
                int paymentId = await InsertIntoPayment(model.Price);
                int reserveId = await InsertIntoReserve(model, paymentId, residenceId);
                await InsertIntoClientReserveComment(userId, reserveId);
                await InsertNewClients(model.NewClients, userId);
                await InsertIntoClientListReserve(model, reserveId, userId);
                //TODO: Convert to transaction

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