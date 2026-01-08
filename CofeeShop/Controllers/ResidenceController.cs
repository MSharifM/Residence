using CoffeeShop.Core.DTOs.Residence;
using CoffeeShop.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    public class ResidenceController : Controller
    {
        private readonly IResidenceService _residenceService;

        public ResidenceController(IResidenceService residenceService)
        {
            _residenceService = residenceService;
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

        public async Task<IActionResult> Reservation(DateTime startDate, DateTime endDate, string residenceId)
        {
            return View();
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