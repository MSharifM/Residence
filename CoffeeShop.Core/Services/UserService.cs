using CoffeeShop.Core.DTOs.Account;
using CoffeeShop.Core.DTOs.UserPanel;
using CoffeeShop.Core.Sender;
using CoffeeShop.Core.Services.Interfaces;
using CoffeeShop.DataLayer.Context;
using CoffeeShop.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoffeeShop.Core.Services
{
    public class UserService : IUserService
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private AppDbContext _dbContext;

        public UserService(AppDbContext dbContext, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Account

        public async Task<bool> IsExistEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
                return true;

            return false;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model, string baseUrl)
        {
            var user = new User()
            {
                Email = model.Email,
                UserName = model.Email,
            };

            if (await IsExistEmailAsync(model.Email))
                return IdentityResult.Failed(new IdentityError { Description = "ایمیل تکراری است" });

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
                await SendEmailConfirmationMessageAsync(user, baseUrl);

            return result;
        }

        public bool IsUserSignIn(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }

        public async Task<SignInResult> SignInAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, true);

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
            subject: "فعالسازی حساب کاربری - Coffee Shop",
            body: $@"
<!DOCTYPE html>
<html lang='fa' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>فعالسازی حساب کاربری</title>
</head>
<body style='margin:0; padding:0; font-family: Tahoma, Arial, sans-serif; background-color:#f6f0e6; direction:rtl;'>
    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='background-color:#f6f0e6; padding:20px;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='600' cellspacing='0' cellpadding='0' style='background-color:#ffffff; border-radius:10px; box-shadow:0 2px 10px rgba(0,0,0,0.1); max-width:600px;'>
                    <!-- Header -->
                    <tr>
                        <td style='padding:30px; text-align:center; background-color:#6f4e37; border-radius:10px 10px 0 0;'>
                            <h1 style='color:#fff; margin:0; font-size:24px;'>خوش آمدید به Coffee Shop!</h1>
                            <p style='color:#fff; margin:10px 0 0 0; font-size:14px;'>حساب کاربری شما با موفقیت ایجاد شد.</p>
                        </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                        <td style='padding:40px 30px; text-align:center;'>
                            <h2 style='color:#333; margin:0 0 20px 0; font-size:20px;'>برای فعالسازی حساب خود روی دکمه زیر کلیک کنید:</h2>
                            <p style='color:#555; margin:0 0 30px 0; font-size:16px; line-height:1.5;'>
                                با فعالسازی حساب، می‌توانید سفارش‌های خوشمزه خود را سریع‌تر ثبت کنید و از پیشنهادات ویژه کافی‌شاپ بهره‌مند شوید.
                                این لینک تا ۱ ساعت معتبر است.
                            </p>

                            <a href='{confirmationLink}'
                               style='display:inline-block; padding:15px 30px; background-color:#6f4e37; color:#fff; text-decoration:none; border-radius:5px; font-size:16px; font-weight:bold;'>
                                فعال‌سازی حساب
                            </a>

                            <p style='color:#555; margin:30px 0 0 0; font-size:14px; font-style:italic;'>
                                اگر دکمه کار نکرد، لینک زیر را کپی کنید: <br>
                                <a href='{confirmationLink}' style='color:#6f4e37; text-decoration:underline;'>{confirmationLink}</a>
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='padding:20px 30px; background-color:#f2ede7; border-radius:0 0 10px 10px; text-align:center;'>
                            <p style='color:#999; margin:0; font-size:12px; line-height:1.4;'>
                                سوالی دارید؟ با ما تماس بگیرید: <br>
                                ایمیل: support@coffeeshop.com | تلفن: ۰۲۱-۱۲۳۴۵۶۷۸
                            </p>
                            <p style='color:#999; margin:10px 0 0 0; font-size:12px;'>
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

        #endregion User common methods

        #region UserPanel

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await GetUserByUserNameAsync(model.UserName);
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "کاربر یافت نشد" });

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            return result;
        }

        public async Task<EditProfileViewModel?> EditProfileAsync(string oldUserName, EditProfileViewModel model)
        {
            var user = await GetUserByUserNameAsync(oldUserName);

            if (user == null)
            {
                model.Message = "خطا";
                return model;
            }
            if (await _dbContext.Users.AnyAsync(u => u.Email == model.Email && u.Id != user.Id))
            {
                model.Message = "خطا: ایمیل تکراری است";
                return model;
            }

            user.NormalizedUserName = model.Email.ToUpper();
            user.UserName = model.Email;
            if (model.Email != user.Email)
            {
                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();
                user.EmailConfirmed = false;
                //ToDo send confirm mail
            }
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            //Update session
            await _signInManager.RefreshSignInAsync(user);

            model.Message = "اطلاعات با موفقیت تغییر یافت";
            return model;
        }

        #endregion UserPanel
    }
}