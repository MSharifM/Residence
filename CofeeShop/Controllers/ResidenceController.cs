using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    public class ResidenceController : Controller
    {
        public IActionResult Detail(int id)
        {
            return View();
        }
    }
}