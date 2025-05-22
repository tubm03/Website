using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Promotion;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class PromotionController : Controller
    {
        private readonly IPromotionRepository _promotion;

        public PromotionController(IPromotionRepository promotion)
        {
            _promotion = promotion;
        }

        public IActionResult List(int? page, int? pageSize)
        {
            page = page == null ? 1 : page;
            pageSize = pageSize == null ? 10 : pageSize;
            var promotions = _promotion.GetPromotions((int)page, (int)pageSize);
            return View(promotions);
        }
    }
}
