using CoffeeShop.Core.DTOs.Residence;
using CoffeeShop.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    public class ResidenceController : Controller
    {
        private readonly IResidenceService _residenceService;
        private readonly IUserService _userService;

        public ResidenceController(IResidenceService residenceService, IUserService userService)
        {
            _residenceService = residenceService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string? search = "", int page = 0)
        {
            var model = await _residenceService.GetAllResidences(search, page);

            ViewData["CurrentPage"] = page;
            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var model = await _residenceService.GetResidenceDetailById(id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Reservation(DateTime startDate, DateTime endDate, int residenceId)
        {
            var model = await _residenceService.GetDetailForReserve(residenceId, User.Identity.Name, startDate, endDate);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Reservation(ReserveResidenceViewModel model, int residenceId)
        {
            if (!ModelState.IsValid)
                return View(model);

            string userId = (await _userService.GetUserByUserNameAsync(User.Identity.Name)).Id;

            var succeeded = await _residenceService.ReserveSubmitAsync(model, residenceId, userId);

            if (succeeded)
                return RedirectToAction("Index", "/UserPanel");
            else
            {
                ViewData["Error"] = "این تاریخ از قبل رزرو شده است";
                return View(model);
            }
        }

        public async Task<IActionResult> GetResidenceDetailForHost(int residenceId)
        {
            var result = await _residenceService.GetResidenceDetailForHost(residenceId);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditResidenceDetail(ResidenceDetailForHostPanelViewModel model, int residenceId)
        {
            await _residenceService.UpdateResidenceDetail(model, residenceId);
            return RedirectToAction("Index", "/UserPanel");
        }
    }
}