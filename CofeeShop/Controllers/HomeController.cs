using System.Diagnostics;
using CoffeeShop.Core.DTOs.Residence;
using CoffeeShop.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Models;

namespace CoffeeShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IResidenceService _residenceService;

    public HomeController(ILogger<HomeController> logger, IResidenceService residenceService)
    {
        _logger = logger;
        _residenceService = residenceService;
    }

    public async Task<IActionResult> Index()
    {
        HomePageViewModel model = await _residenceService.GetHomePageViewModelsAsync();

        return View(model);
    }

    [Route("Questions")]
    public IActionResult Questions()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}