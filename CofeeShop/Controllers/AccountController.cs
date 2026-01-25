using CoffeeShop.Core.DTOs.Account;
using CoffeeShop.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("Authentication")]
        public IActionResult Authentication(string? returnUrl = null, bool? resetPassword = false)
        {
            if (_userService.IsUserSignIn(User))
                return RedirectToAction("Index", "Home");

            var model = new AuthenticationViewModel();

            ViewData["ResetPassword"] = resetPassword;
            ViewData["returnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (_userService.IsUserSignIn(User))
                return RedirectToAction("Index", "Home");

            var viewModel = new AuthenticationViewModel();
            viewModel.Login = model;

            ViewData["returnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View("Authentication", viewModel);

            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ViewData["LoginError"] = "رمزعبور یا ایمیل اشتباه است";
                return View("Authentication", viewModel);
            }

            var isConfirmed = user.EmailConfirmed;
            if (!isConfirmed)
            {
                ViewData["isConfirmed"] = false;
                return View("Authentication", viewModel);
            }

            var result = await _userService.SignInAsync(user.UserName, model.Password); // Login by username, password
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                ViewData["LoginError"] = "اکانت شما به دلیل پنج بار ورود ناموفق به مدت پنج دقیق قفل شده است";
                return View("Authentication", viewModel);
            }

            ViewData["LoginError"] = "رمزعبور یا نام کاربری اشتباه است";

            return View("Authentication", viewModel);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (_userService.IsUserSignIn(User))
                return RedirectToAction("Index", "Home");

            var viewModel = new AuthenticationViewModel();
            viewModel.Register = model;

            if (!ModelState.IsValid)
                return View("Authentication", viewModel);

            var result = await _userService.RegisterAsync(model, $"{Request.Scheme}://{Request.Host}");

            if (result.Succeeded)
            {
                ViewData["RegisterMessage"] = "لینک فعال‌سازی به ایمیل شما ارسال شد. لطفاً ایمیل‌تان را بررسی کنید.";
                return View("Authentication", viewModel);
            }

            var errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            ViewData["RegisterError"] = errors;

            return View("Authentication", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _userService.LogOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            if (_userService.IsUserSignIn(User))
                return RedirectToAction("Index", "Home");

            //EmailAlreadyConfirmed
            if (await _userService.IsEmailConfirmedAsync(null, userId))
            {
                ViewData["ConfirmStatus"] = "AlreadyConfirmed";
                ViewData["Message"] = @"<h3>ایمیل شما قبلاً تایید شده</h3>
                <p>حساب کاربری شما از قبل فعال می‌باشد</p>";
                ViewData["Discription"] = @"<p><strong>توجه!</strong> آدرس ایمیل شما قبلاً تایید شده است.</p>
                <p>نیازی به تایید مجدد نمی‌باشد و می‌توانید از حساب کاربری خود استفاده کنید.</p>";

                return View();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            //user id is false
            if (user is null)
            {
                ViewData["ConfirmStatus"] = "Error";
                ViewData["Message"] = @"<h2>خطا در تایید ایمیل</h2>
                <p>متأسفانه لینک تایید معتبر نمی‌باشد یا منقضی شده است</p>";
                ViewData["Discription"] = @" <p><strong>خطا!</strong> لینک تایید ایمیل معتبر نمی‌باشد.</p>
                <p>ممکن است لینک منقضی شده باشد یا قبلاً استفاده شده باشد.</p>";
                return View();
            }
            var result = await _userService.ConfirmEmailAsync(user, token);

            //Confirm is success
            if (result.Succeeded)
            {
                ViewData["ConfirmStatus"] = "Success";
                ViewData["Message"] = @"<h2>تایید ایمیل با موفقیت انجام شد</h2>
                <p>حساب کاربری شما فعال شد و می‌توانید از تمام امکانات سایت استفاده کنید</p>";
                ViewData["Discription"] = @"<p><strong>تبریک!</strong> عملیات تایید ایمیل با موفقیت انجام شد.</p>
                <p>هم اکنون می‌توانید وارد حساب کاربری خود شوید و از خدمات ما استفاده کنید.</p>";
            }
            else //Confirm is failed
            {
                ViewData["ConfirmStatus"] = "Error";
                ViewData["Message"] = @"<h2>خطا در تایید ایمیل</h2>
                <p>متأسفانه لینک تایید معتبر نمی‌باشد یا منقضی شده است</p>";
                ViewData["Discription"] = @" <p><strong>خطا!</strong> لینک تایید ایمیل معتبر نمی‌باشد.</p>
                <p>ممکن است لینک منقضی شده باشد یا قبلاً استفاده شده باشد.</p>";
            }

            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                model.ResultMessage = "در صورت وجود ایمیل، ایمیل بازیابی رمز عبور برای شما ارسال شد.";
                model.Status = "Success";
                return View(model);
            }
            if (!await _userService.IsEmailConfirmedAsync(email: model.Email))
            {
                model.ResultMessage = "حساب کاربری شما تایید نشده است. ابتدا حساب کاربری خود را فعال کنید.";
                model.Status = "Error";
                //ToDo send confirm userName again
                return View(model);
            }

            await _userService.SendResetPasswordEmailAsync(user, $"{Request.Scheme}://{Request.Host}");
            model.ResultMessage = "در صورت وجود ایمیل، ایمیل بازیابی رمز عبور برای شما ارسال شد.";
            model.Status = "Success";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            if (_userService.IsUserSignIn(User))
                return RedirectToAction("Index", "Home");

            var user = await _userService.GetUserByIdAsync(userId);
            //User id is false
            if (user is null)
            {
                ViewData["LinkStatus"] = "Error";
                return View();
            }

            var model = new ResetPasswordViewModel()
            {
                UserId = userId,
                Token = token,
                Password = "",
                ConfirmPassword = ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetUserByIdAsync(model.UserId);
            if (user is null)
            {
                ViewData["LinkStatus"] = "Error";
                return View(model);
            }

            var result = await _userService.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction("Authentication", new { resetPassword = true });

            ViewData["LinkStatus"] = "Error";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userService.IsExistEmailAsync(email);
            if (user is false)
                return Json(true);

            return Json("ایمیل تکراری است.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsUserNameInUse(string userName)
        {
            var user = await _userService.IsExistUserNameAsync(userName);
            if (user is false)
                return Json(true);

            return Json("نام کاربری تکراری است.");
        }
    }
}