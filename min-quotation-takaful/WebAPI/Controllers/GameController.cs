using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
