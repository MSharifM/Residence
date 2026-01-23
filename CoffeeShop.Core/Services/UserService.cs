using CoffeeShop.Core.Convertor;
using CoffeeShop.Core.DTOs.Account;
using CoffeeShop.Core.DTOs.UserPanel;
using CoffeeShop.Core.Sender;
using CoffeeShop.Core.Services.Interfaces;
using CoffeeShop.DataLayer.Context;
using CoffeeShop.DataLayer.Entities;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;
using System.Security.Claims;

namespace CoffeeShop.Core.Services
{
    public class UserService : IUserService
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private AppDbContext _dbContext;
        private IDbConnection _dbContextDapper;

        public UserService(AppDbContext dbContext, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContextDapper = new MySqlConnection(configuration.GetConnectionString("ResidenceConnection"));
        }

        #region Account

        public async Task<bool> IsExistEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
                return true;

            return false;
        }

        public async Task<bool> IsExistUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is not null)
                return true;

            return false;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model, string baseUrl)
        {
            var user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber1,
                PhoneNumber2 = model.PhoneNumber2,
            };

            if (await IsExistEmailAsync(model.Email) || await IsExistUserNameAsync(model.UserName))
                return IdentityResult.Failed(new IdentityError { Description = "ایمیل یا نام کاربری تکراری است" });

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.AccountNumber)) // Role = Host
                {
                    await _userManager.AddToRoleAsync(user, "Host");
                    await AddHostToDbAsync(user.Id, model.AccountNumber);
                }

                await SendEmailConfirmationMessageAsync(user, baseUrl);
            }

            return result;
        }

        private async Task AddHostToDbAsync(string userId, string AccountNumber)
        {
            string query = $"""
                            INSERT INTO H_host (UserId, Acc_Number) VALUES
                            ('{userId}', '{AccountNumber}')
                            """;
            var result = await _dbContextDapper.QueryAsync<string>(query);
        }

        public bool IsUserSignIn(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }

        public async Task<SignInResult> SignInAsync(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, true, true);
            //TODO: role management
            return result;
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        private async Task SendEmailConfirmationMessageAsync(User user, string baseUrl)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //Create Url
            var confirmationLink = $"{baseUrl}/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            #region Send email

            SendEmail.Send(
                to: user.Email,
                subject: "فعالسازی حساب کاربری - ResidenceYab",
                body: $@"<!DOCTYPE html>
<html lang=""fa"" dir=""rtl"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>تأیید ایمیل - اقامتگاه</title>
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"">
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }}
        body {{
            background-color: #f8f9fa;
            color: #333;
            line-height: 1.6;
            padding: 20px;
            background-image: linear-gradient(135deg, #f5f7fa 0%, #e4edf5 100%);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }}

        .email-container {{
            max-width: 600px;
            width: 100%;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 20px;
            overflow: hidden;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
            border: 1px solid #eaeaea;
        }}

        .header {{
            background: linear-gradient(to right, #2d7d7a, #1a5f5c);
            color: white;
            padding: 30px;
            text-align: center;
            position: relative;
            overflow: hidden;
        }}

        .header::before {{
            content: """";
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: radial-gradient(circle, rgba(255,255,255,0.1) 1px, transparent 1px);
            background-size: 20px 20px;
            opacity: 0.3;
        }}

        .logo {{
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 15px;
            position: relative;
            z-index: 2;
        }}

        .logo-icon {{
            font-size: 32px;
            margin-left: 10px;
        }}

        .logo-text {{
            font-size: 26px;
            font-weight: 700;
            letter-spacing: -0.5px;
        }}

        .header h1 {{
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 10px;
            position: relative;
            z-index: 2;
        }}

        .header p {{
            font-size: 16px;
            opacity: 0.9;
            position: relative;
            z-index: 2;
        }}

        .content {{
            padding: 40px;
        }}

        .welcome-text {{
            font-size: 18px;
            margin-bottom: 25px;
            color: #2d7d7a;
            font-weight: 600;
            text-align: center;
        }}

        .message {{
            background-color: #f8fafc;
            border-right: 4px solid #2d7d7a;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 30px;
            font-size: 16px;
            color: #444;
        }}

        .activation-box {{
            background-color: #f0f9f8;
            border-radius: 12px;
            padding: 25px;
            text-align: center;
            margin: 30px 0;
            border: 1px dashed #2d7d7a;
        }}

        .activation-title {{
            font-size: 18px;
            color: #1a5f5c;
            margin-bottom: 15px;
            font-weight: 600;
        }}

        .activation-button {{
            display: inline-block;
            background: linear-gradient(to right, #2d7d7a, #1a5f5c);
            color: white;
            text-decoration: none;
            padding: 16px 40px;
            border-radius: 50px;
            font-size: 18px;
            font-weight: 600;
            margin: 20px 0;
            transition: all 0.3s ease;
            box-shadow: 0 5px 15px rgba(42, 124, 121, 0.2);
        }}

        .activation-button:hover {{
            transform: translateY(-3px);
            box-shadow: 0 8px 20px rgba(42, 124, 121, 0.3);
            background: linear-gradient(to right, #1a5f5c, #2d7d7a);
        }}

        .activation-link {{
            display: block;
            background-color: white;
            padding: 15px;
            border-radius: 8px;
            margin-top: 20px;
            word-break: break-all;
            font-size: 14px;
            color: #555;
            border: 1px solid #e0e0e0;
            direction: ltr;
            text-align: center;
        }}

        .instructions {{
            background-color: #f8fafc;
            padding: 20px;
            border-radius: 10px;
            margin-top: 30px;
            border-right: 3px solid #e2e8f0;
        }}

        .instructions h3 {{
            color: #1a5f5c;
            margin-bottom: 15px;
            font-size: 18px;
        }}

        .instructions ol {{
            padding-right: 20px;
            margin-bottom: 0;
        }}

        .instructions li {{
            margin-bottom: 10px;
        }}

        .footer {{
            background-color: #f8f9fa;
            padding: 25px;
            text-align: center;
            color: #666;
            border-top: 1px solid #eaeaea;
        }}

        .footer-links {{
            display: flex;
            justify-content: center;
            margin-top: 15px;
            flex-wrap: wrap;
        }}

        .footer-link {{
            color: #2d7d7a;
            text-decoration: none;
            margin: 0 10px;
            font-size: 14px;
            transition: color 0.2s;
        }}

        .footer-link:hover {{
            color: #1a5f5c;
            text-decoration: underline;
        }}

        .social-icons {{
            display: flex;
            justify-content: center;
            margin-top: 20px;
        }}

        .social-icon {{
            width: 36px;
            height: 36px;
            border-radius: 50%;
            background-color: #e8f0ee;
            color: #2d7d7a;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 8px;
            text-decoration: none;
            transition: all 0.3s;
        }}

        .social-icon:hover {{
            background-color: #2d7d7a;
            color: white;
            transform: translateY(-3px);
        }}

        .note {{
            font-size: 14px;
            color: #888;
            margin-top: 20px;
            line-height: 1.5;
            padding: 15px;
            background-color: #fff9e6;
            border-radius: 8px;
            border-right: 3px solid #ffd166;
        }}

        @media (max-width: 640px) {{
            .content, .header, .footer {{
                padding: 25px 20px;
            }}

            .activation-button {{
                padding: 14px 30px;
                font-size: 16px;
            }}

            .header h1 {{
                font-size: 22px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <div class=""logo"">
                <div class=""logo-icon""><i class=""fas fa-home""></i></div>
                <div class=""logo-text"">اقامتگاه</div>
            </div>
            <h1>تأیید آدرس ایمیل شما</h1>
            <p>لطفاً ایمیل خود را برای فعال‌سازی حساب کاربری تأیید کنید</p>
        </div>

        <div class=""content"">
            <div class=""welcome-text"">سلام کاربر عزیز، به خانواده اقامتگاه خوش آمدید!</div>

            <div class=""message"">
                از اینکه در سایت رزرو اقامتگاه ما ثبت‌نام کردید متشکریم. برای تکمیل فرآیند ثبت‌نام و فعال‌سازی حساب کاربری خود، لطفاً آدرس ایمیل خود را تأیید کنید.
            </div>

            <div class=""activation-box"">
                <div class=""activation-title"">برای فعال‌سازی حساب کاربری خود، روی دکمه زیر کلیک کنید:</div>

                <a href=""{confirmationLink}"" class=""activation-button"">
                    <i class=""fas fa-check-circle""></i> تأیید ایمیل
                </a>

                <div style=""margin: 15px 0; color: #666; font-size: 15px;"">یا لینک زیر را در مرورگر خود کپی کنید:</div>

                <div class=""activation-link"">
                    {confirmationLink}
                </div>
            </div>

            <div class=""note"">
                <i class=""fas fa-info-circle""></i> توجه: این لینک تنها به مدت 24 ساعت معتبر است. اگر پس از 24 ساعت اقدامی نکرده‌اید، می‌توانید در صفحه ورود به سایت، درخواست ارسال مجدد ایمیل تأیید را بدهید.
            </div>

            <div class=""instructions"">
                <h3>راهنمای تأیید ایمیل:</h3>
                <ol>
                    <li>روی دکمه ""تأیید ایمیل"" در بالا کلیک کنید</li>
                    <li>صفحه جدیدی باز می‌شود که تأیید موفقیت‌آمیز را نشان می‌دهد</li>
                    <li>می‌توانید بلافاصله به حساب کاربری خود وارد شوید</li>
                    <li>پس از تأیید ایمیل، می‌توانید رزرو اقامتگاه مورد نظر خود را انجام دهید</li>
                </ol>
            </div>
        </div>

        <div class=""footer"">
            <p>اگر شما در سایت اقامتگاه ثبت‌نام نکرده‌اید، این ایمیل را نادیده بگیرید.</p>

            <p style=""margin-top: 20px; font-size: 14px; color: #888;"">© 2023 اقامتگاه. تمامی حقوق محفوظ است.</p>
        </div>
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function() {{
            const verifyButton = document.querySelector('.activation-button');

            verifyButton.addEventListener('mouseenter', function() {{
                this.style.transform = 'translateY(-3px)';
            }});

            verifyButton.addEventListener('mouseleave', function() {{
                this.style.transform = 'translateY(0)';
            }});
        }});
    </script>
</body>
</html>
");

            #endregion Send email
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<bool> IsEmailConfirmedAsync(string? userName = null, string? userId = null, string? email = null)
        {
            var user = await FindUserAsync(userName, userId, email);
            return user?.EmailConfirmed ?? false;
        }

        private async Task<User?> FindUserAsync(string? userName, string? userId, string? email)
        {
            if (!string.IsNullOrEmpty(userName))
                return await GetUserByUserNameAsync(userName);

            if (!string.IsNullOrEmpty(userId))
                return await GetUserByIdAsync(userId);

            if (!string.IsNullOrEmpty(email))
                return await GetUserByEmailAsync(email);

            return null;
        }

        public async Task SendResetPasswordEmailAsync(User user, string baseUrl)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Create Url
            var resetLink = $"{baseUrl}/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            SendEmail.Send(
                to: user.Email,
                subject: "بازیابی رمز عبور",
                body: $@"<!DOCTYPE html>
<html lang='fa' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>بازیابی رمز عبور</title>
</head>
<body style='margin: 0; padding: 0; font-family: Tahoma, Arial, sans-serif; background-color: #f4f4f4; direction: rtl;'>
    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='background-color: #f4f4f4; padding: 20px;'>
        <tr>
            <td align='center'>
                <!-- هدر -->
                <table role='presentation' width='600' cellspacing='0' cellpadding='0' style='background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); max-width: 600px;'>
                    <tr>
                        <td style='padding: 30px; text-align: center; background-color: #e94e77; border-radius: 10px 10px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: bold;'>بازیابی رمز عبور - Coffee Shop</h1>
                            <p style='color: #ffffff; margin: 10px 0 0 0; font-size: 14px;'>درخواست بازیابی رمز عبور دریافت شد.</p>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px; text-align: center;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 20px;'>برای بازیابی رمز عبور، روی دکمه زیر کلیک کنید:</h2>
                            <p style='color: #666666; margin: 0 0 30px 0; font-size: 16px; line-height: 1.5;'>لطفاً برای تنظیم رمز عبور جدید، روی دکمه زیر کلیک کنید. این لینک تا ۱ ساعت معتبر است.</p>

                            <!-- دکمه لینک -->
                            <a href='{resetLink}'
                               style='display: inline-block; padding: 15px 30px; background-color: #e94e77; color: #ffffff; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold;'>
                                بازیابی رمز عبور
                            </a>

                            <p style='color: #666666; margin: 30px 0 0 0; font-size: 14px; font-style: italic;'>
                                اگر دکمه کار نکرد، لینک زیر را کپی کنید: <br>
                                <a href='{resetLink}' style='color: #e94e77; text-decoration: underline;'>{resetLink}</a>
                            </p>
                            <p style='color: #666666; margin: 20px 0 0 0; font-size: 14px;'>
                                اگر این درخواست از شما نیست، لطفاً این ایمیل را نادیده بگیرید.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 20px 30px; background-color: #f8f9fa; border-radius: 0 0 10px 10px; text-align: center;'>
                            <p style='color: #999999; margin: 0; font-size: 12px; line-height: 1.4;'>
                                اگر سؤالی دارید، با ما تماس بگیرید: <br>
                                ایمیل: support@coffeeshop.com | تلفن: ۰۲۱-۱۲۳۴۵۶۷۸
                            </p>
                            <p style='color: #999999; margin: 10px 0 0 0; font-size: 12px;'>
                                © ۲۰۲۵ Coffee Shop. تمامی حقوق محفوظ است.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>");
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        #endregion Account

        #region User common methods

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> SendEmailConfirmAgain()
        {
            return true;
        }

        #endregion User common methods

        #region UserPanel

        private async Task<bool> ChangePasswordAsync(ChangePasswordViewModel model, User user)
        {
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> EditProfileAsync(string userId, UserInformationViewModel model)
        {
            var user = await GetUserByIdAsync(userId);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.Phone;
            user.UserName = model.UserName;
            user.NormalizedUserName = model.UserName.ToUpper().Trim();
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            //Update session
            await _signInManager.RefreshSignInAsync(user);

            if (model.ChangePassword != null)
            {
                if (!string.IsNullOrEmpty(model.ChangePassword.NewPassword) &&
                    !string.IsNullOrEmpty(model.ChangePassword.OldPassword) &&
                    !string.IsNullOrEmpty(model.ChangePassword.RePassword))
                {
                    if (await ChangePasswordAsync(model.ChangePassword, user))
                        return true;

                    return false;
                }
            }

            return true;
        }

        public async Task<IEnumerable<ReservesViewModel>> GetFutureUserReserves(string userName)
        {
            string query = $"""
                            select distinct dateofstart as StartDate, dateofend as EndDate,amountpaid as Price,r.situation,residencename, cityname as ResidenceCity
                            from reservation as r
                            join client_reserve_comment as crc on r.reservationid = crc.reservationid
                            join aspnetusers as u on crc.userid = u.id
                            join residence as re on r.residenceid = re.residenceid
                            join city as c on re.cityid = c.cityid
                            where r.dateofstart > current_date() and u.username = '{userName}'
                            order by StartDate;
                            """;
            var result = await _dbContextDapper.QueryAsync<ReservesViewModel>(query);
            return result;
        }

        public async Task<ReservesViewModel> GetLastUserReserve(string userName)
        {
            string query = $"""
                            select distinct dateofstart as StartDate, dateofend as EndDate,amountpaid as Price,r.situation,residencename, cityname as ResidenceCity, re.residenceid as ResidenceId
                            from reservation as r
                            join client_reserve_comment as crc on r.reservationid = crc.reservationid
                            join aspnetusers as u on crc.userid = u.id
                            join residence as re on r.residenceid = re.residenceid
                            join city as c on re.cityid = c.cityid
                            where r.dateofstart <= current_date() and u.username = '{userName}'
                            order by EndDate Desc
                            limit 1;
                            """;
            var result = await _dbContextDapper.QueryAsync<ReservesViewModel>(query);
            if (result.Count() != 0)
                return result.Single();
            else
                return new ReservesViewModel();
        }

        public async Task<bool> AddCommentForResidence(AddCommentViewModel model, int residenceId, string userId)
        {
            try
            {
                string query = @"
                                INSERT INTO Comments (CommentDescription, ResidenceId, Rate, CreateDate, CommentStatus)
                                VALUES (@Description, @ResidenceId, @Rate, @CreateDate, 'Ok');
                                SELECT LAST_INSERT_ID();
                               ";

                var commentId = await _dbContextDapper.QuerySingleAsync<int>(query, new
                {
                    Description = model.Description,
                    ResidenceId = residenceId,
                    Rate = model.Rate,
                    CreateDate = DateTime.Now
                });

                string queryInsertToClientReserveComment = @"
                            INSERT INTO Admins_comment (UserID, CommentId) VALUES (@AdminId, @CommentId);
                            INSERT INTO Client_Reserve_Comment (UserID, CommentId, ReservationId) VALUES (@UserId, @CommentId, @ResidenceId);
                            ";

                await _dbContextDapper.ExecuteAsync(queryInsertToClientReserveComment, new
                {
                    AdminId = 11,
                    UserId = userId,
                    CommentId = commentId,
                    ResidenceId = residenceId
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        #endregion UserPanel

        #region HostPanel

        public async Task<string?> IsHost(string userName)
        {
            string query = $"""
                            select acc_number
                            from (select id from aspnetusers as aspu where aspu.username = '{userName}') as u
                            join h_host as h on u.id = h.userid
                            """;

            string? result = await _dbContextDapper.QueryFirstOrDefaultAsync<string>(query);

            return result;
        }

        public async Task<IEnumerable<HostListResidencesViewModel>> GetListResidencesNameForHostAsync(
            string hostUserName)
        {
            string query = $"""
                            SELECT residencename as Name, residenceid
                            FROM residence as r
                            join aspnetusers as u on r.userid = u.id
                            where u.username = '{hostUserName}'
                            """;
            var result = await _dbContextDapper.QueryAsync<HostListResidencesViewModel>(query);
            return result;
        }

        public async Task<IEnumerable<ListFutureReservesForHostViewModel>> GetFutureReservesForHostByResidenceIdAsync(
            int residenceId)
        {
            string query = $"""
                            select re.ResidenceName, r.DateOfStart as StartDate, aspu.PhoneNumber
                            from (select * from reservation where residenceid = '{residenceId}' and DateOfStart > current_date()) as r
                            join residence as re on r.residenceid = re.residenceid
                            join client_reserve_comment as crc on r.reservationid = crc.reservationid
                            join aspnetusers as aspu on aspu.id=crc.userid
                            """;
            var result = await _dbContextDapper.QueryAsync<ListFutureReservesForHostViewModel>(query);
            return result;
        }

        public async Task<IEnumerable<ListCompletedReservesForHostViewModel>> GetCompletedReservesForHostByResidenceIdAsync(
                int residenceId)
        {
            string query = $"""
                            select re.ResidenceName , r.DateOfEnd as EndDate ,r.AmountPaid as Price
                            from (select * from reservation where residenceid = '{residenceId}' and DateOfEnd < current_date()) as r
                            join residence as re on r.residenceid = re.residenceid
                            """;
            var result = await _dbContextDapper.QueryAsync<ListCompletedReservesForHostViewModel>(query);
            return result;
        }

        public async Task<IEnumerable<ListResidenceCommentsForHostViewModel>> GetResidenceCommentsForHostByResidenceIdAsync(
                int residenceId)
        {
            string query = $"""
                            select aspu.UserName , c.CommentDescription as Description , c.Rate
                            from (select * from comments where ResidenceId = '{residenceId}') as c
                            join client_reserve_comment as crc on c.commentid = crc.CommentId
                            join aspnetusers as aspu on crc.UserID = aspu.id
                            """;
            var result = await _dbContextDapper.QueryAsync<ListResidenceCommentsForHostViewModel>(query);
            return result;
        }

        public async Task<List<ListResidenceSalaryViewModel>> GetHostSalary(string userName)
        {
            string query = $"""
                            select  ResidenceName, YEAR(createPay)  AS year, MONTH(createPay) AS month, SUM(p.price*0.1) as Salary
                            from H_host as h
                            join aspnetusers as u on h.UserId = u.Id
                            join residence as re on re.UserId = h.UserId
                            join reservation as r on r.ResidenceId = re.ResidenceId
                            join payment as p on p.PayId = r.PayId
                            where u.username = @userName
                            group by residenceName, YEAR(createPay), MONTH(createPay)
                            """;

            var result = await _dbContextDapper.QueryAsync<ListResidenceSalaryViewModel>(query, new { userName });

            var fixedResult = result.Select(p => new ListResidenceSalaryViewModel
            {
                ResidenceName = p.ResidenceName,
                Year = p.Year,
                Month = p.Month,
                Salary = p.Salary,
                FixedDate = $"{p.Year} - {p.Month.ToMonthName()}"
            }).ToList();

            return fixedResult;
        }

        public async Task AddOrUpdateHostAccountNumber(string number, string userName)
        {
            var userId = (await GetUserByUserNameAsync(userName)).Id;

            string query = @"
                            INSERT INTO h_host (userid, acc_number)
                            VALUES (@UserId, @Acc_Number)
                            ON DUPLICATE KEY UPDATE acc_number = @Acc_Number;
                            ";

            await _dbContextDapper.ExecuteAsync(query, new
            {
                UserId = userId,
                Acc_Number = number,
            });
        }

        #endregion HostPanel

        public async Task<IEnumerable<StripListViewModel>> GetUserStrips(string userName)
        {
            string query = $"""
                             select distinct dateofstart as StartDate, dateofend as EndDate,amountpaid as Price,r.situation,residencename, cityname as City, re.residenceid as ResidenceId
                             from reservation as r
                             join client_reserve_comment as crc on r.reservationid = crc.reservationid
                             join aspnetusers as u on crc.userid = u.id
                             join residence as re on r.residenceid = re.residenceid
                             join city as c on re.cityid = c.cityid
                             where u.username = '{userName}'
                             order by EndDate Desc
                             """;

            var result = await _dbContextDapper.QueryAsync<StripListViewModel>(query);

            return result;
        }
    }
}