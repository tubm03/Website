using Microsoft.AspNetCore.Mvc;

namespace PetStoreProject.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
