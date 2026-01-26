using CoffeeShop.Core.DTOs.Residence;
using CoffeeShop.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    [Authorize]
    public class ResidenceController : Controller
    {
        private readonly IResidenceService _residenceService;
        private readonly IUserService _userService;

        public ResidenceController(IResidenceService residenceService, IUserService userService)
        {
            _residenceService = residenceService;
            _userService = userService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search = "", int page = 0)
        {
            var model = await _residenceService.GetAllResidences(search, page);

            ViewData["CurrentPage"] = page;
            ViewData["Search"] = search;

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var model = await _residenceService.GetResidenceDetailById(id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Reservation(DateTime startDate, DateTime endDate, int residenceId)
        {
            var model = await _residenceService.GetDetailForReserve(residenceId, User.Identity.Name, startDate, endDate);
            ViewData["ResidenceId"] = residenceId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Reservation(ReserveResidenceViewModel model, int residenceId)
        {
            if (!(model.NewClients.Any() || model.Clients.Any()))
            {
                ViewData["ResidenceId"] = residenceId;
                return View(model);
            }

            string userId = (await _userService.GetUserByUserNameAsync(User.Identity.Name)).Id;

            var succeeded = await _residenceService.ReserveSubmitAsync(model, residenceId, userId);

            if (succeeded)
                return RedirectToAction("Index", "Home", new { area = "UserPanel" });

            ViewData["Error"] = "این تاریخ از قبل رزرو شده است";
            ViewData["ResidenceId"] = residenceId;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetResidenceDetailForHost(int residenceId)
        {
            var result = await _residenceService.GetResidenceDetailForHost(residenceId);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResidenceDetail(ResidenceDetailForHostPanelViewModel model, int residenceId)
        {
            await _residenceService.UpdateResidenceDetail(model, residenceId);

            return RedirectToAction("Index", "Home", new { area = "UserPanel" });
        }

        [HttpGet]
        public async Task<IActionResult> EditResidenceImages(int residenceId)
        {
            var isHost = await _userService.IsHost(User.Identity.Name);
            if (string.IsNullOrEmpty(isHost))
                return RedirectToAction("Index", "Home", new { area = "UserPanel", needUpgradeRole = true });

            var model = await _residenceService.GetResidenceImagesForEditAsync(residenceId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResidenceImages(EditResidenceImagesViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _residenceService.EditImageResidence(model.NewImages, model.ResidenceId, model.RemovedImages);

            return RedirectToAction("EditResidenceImages", new { residenceId = model.ResidenceId });
        }

        [HttpGet]
        public async Task<IActionResult> AddResidence()
        {
            var isHost = await _userService.IsHost(User.Identity.Name);
            if (string.IsNullOrEmpty(isHost))
                return RedirectToAction("Index", "Home", new { area = "UserPanel", needUpgradeRole = true });

            ViewData["Options"] = await _residenceService.GetAllOptions();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResidence(AddResidenceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Options"] = await _residenceService.GetAllOptions();
                return View(model);
            }

            model.UserId = (await _userService.GetUserByUserNameAsync(User.Identity.Name)).Id;
            var residenceId = await _residenceService.AddResidence(model);

            //Redirect to EditResidenceImages to add photos
            return RedirectToAction("EditResidenceImages", new { residenceId = residenceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResidence(int id)
        {
            var userId = (await _userService.GetUserByUserNameAsync(User.Identity.Name)).Id;
            await _residenceService.DeleteResidence(id, userId);

            return RedirectToAction("Index", "Home", new { area = "UserPanel" });
        }
    }
}