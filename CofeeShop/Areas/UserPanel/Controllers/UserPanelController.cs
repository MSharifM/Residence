using CoffeeShop.Core.DTOs.UserPanel;
using CoffeeShop.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class HomeController : Controller
    {
        private IUserService _userService;
        private IResidenceService _residenceService;

        public HomeController(IUserService userService, IResidenceService residenceService)
        {
            _userService = userService;
            _residenceService = residenceService;
        }

        public async Task<IActionResult> Index(int residenceId = 0)
        {
            var user = await _userService.GetUserByUserNameAsync(User.Identity.Name);
            UserPanelViewModel model = new UserPanelViewModel()
            {
                InformationViewModel = new UserInformationViewModel()
                {
                    UserName = user.UserName,
                    Phone = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ChangePassword = new ChangePasswordViewModel(),
                },
                FutureReserves = await _userService.GetFutureUserReserves(User.Identity.Name),
                LastReserve = await _userService.GetLastUserReserve(User.Identity.Name),
                Comment = new AddCommentViewModel(), //Prevent null error
            };

            var hostAccountNumber = await _userService.IsHost(User.Identity.Name);
            if (!string.IsNullOrEmpty(hostAccountNumber))
                model.HostListResidences = await _userService.GetListResidencesNameForHostAsync(User.Identity.Name);

            ViewData["UserId"] = user.Id;
            ViewData["ResidenceId"] = residenceId;
            ViewData["HostAccountNumber"] = hostAccountNumber;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(string id, UserInformationViewModel model, ChangePasswordViewModel changePassword)
        {
            model.ChangePassword = changePassword;
            var passwordChangeSucceeded = await _userService.EditProfileAsync(id, model);
            if (passwordChangeSucceeded)
                return Redirect($"/Home/Index");

            //TODO: Set error
            ViewData["Error"] = "تغییر رمز موفق نبود. دوباره تلاش کنید" +
                                "رمز فعلی اشتباه وارد شده";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentViewModel model, int residenceId, string userId)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("MyStrips");
            var result = await _userService.AddCommentForResidence(model, residenceId, userId);

            if (result)
                return RedirectToAction("Index");
            else
                return RedirectToAction("MyStrips");
        }

        #region HostPanel

        //ToDo: Manage security
        [HttpGet]
        public async Task<IActionResult> GetFutureReservesForHost(int residenceId)
        {
            var reserves = await _userService.GetFutureReservesForHostByResidenceIdAsync(residenceId);
            return Json(reserves);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletedReservesForHost(int residenceId)
        {
            var reserves = await _userService.GetCompletedReservesForHostByResidenceIdAsync(residenceId);
            return Json(reserves);
        }

        [HttpGet]
        public async Task<IActionResult> GetResidenceCommentsForHost(int residenceId)
        {
            var reserves = await _userService.GetResidenceCommentsForHostByResidenceIdAsync(residenceId);
            return Json(reserves);
        }

        [HttpGet]
        public async Task<IActionResult> GetHostSalary()
        {
            var model = await _userService.GetHostSalary(User.Identity.Name);
            return Json(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdateHostAccountNumber(string accountNumber)
        {
            if (!string.IsNullOrEmpty(accountNumber))
                await _userService.AddOrUpdateHostAccountNumber(accountNumber, User.Identity.Name);

            return RedirectToAction("Index");
        }

        #endregion HostPanel

        public async Task<IActionResult> MyStrips()
        {
            var models = await _userService.GetUserStrips(User.Identity.Name);

            ViewData["UserId"] = (await _userService.GetUserByUserNameAsync(User.Identity.Name)).Id;

            return View(models);
        }
    }
}