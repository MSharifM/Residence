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

        public async Task<IActionResult> Detail(int id)
        {
            var model = await _residenceService.GetResidenceDetailById(id);
            return View(model);
        }
    }
}