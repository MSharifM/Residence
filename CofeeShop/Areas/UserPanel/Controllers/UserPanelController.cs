using CoffeeShop.Core.DTOs.UserPanel;
using CoffeeShop.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    public class HomeController : Controller
    {
        private IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(int residenceId = 0)
        {
            var user = await _userService.GetUserByUserNameAsync(User.Identity.Name);
            UserPanelViewModel model = new UserPanelViewModel();
            model.InformationViewModel = new UserInformationViewModel()
            {
                UserName = user.UserName,
                Phone = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            model.FutureReserves = await _userService.GetFutureUserReserves(User.Identity.Name);
            model.LastReserve = await _userService.GetLastUserReserve(User.Identity.Name);
            model.Comment = new AddCommentViewModel(); //Prevent null error

            var isHost = await _userService.IsHost(User.Identity.Name);
            if (isHost)
                model.HostListResidences = await _userService.GetListResidencesNameForHostAsync(User.Identity.Name);

            ViewData["UserId"] = user.Id;
            ViewData["ResidenceId"] = residenceId;
            ViewData["IsHost"] = isHost;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(string id, UserInformationViewModel model)
        {
            if (!ModelState.IsValid)
                return Redirect($"/Home/Index");

            await _userService.EditProfileAsync(id, model);

            return Redirect($"/Home/Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentViewModel model, int residenceId, string userId)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");
            var result = await _userService.AddCommentForResidence(model, residenceId, userId);

            return RedirectToAction("Index");
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

        #endregion HostPanel

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            model.UserName = User.Identity.Name;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _userService.ChangePasswordAsync(model);
            if (result.Succeeded)
                ViewData["isChanged"] = true;
            else
                ModelState.AddModelError("", "رمزعبور فعلی اشتباه است");

            return View(model);
        }
    }
}