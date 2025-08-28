using Microsoft.AspNetCore.Mvc;

namespace WebApps.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Quotation() {
            return View();
        }

        public IActionResult QuotationDetails()
        {
            return View();
        }
    }
}
