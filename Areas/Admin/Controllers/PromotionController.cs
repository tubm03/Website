using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.ProductCategory;
using PetStoreProject.Repositories.Promotion;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionController : Controller
    {
        private readonly IBrandRepository _brand;
        private readonly IProductCategoryRepository _productCategory;
        private readonly IPromotionRepository _promotion;

        public PromotionController(IBrandRepository brand, IProductCategoryRepository productCategory, IPromotionRepository promotion)
        {
            _brand = brand;
            _productCategory = productCategory;
            _promotion = promotion;
        }

        public IActionResult Create()
        {
            var brand = _brand.GetBrands();
            var productCategory = _productCategory.GetProductCategories(null, false);
            ViewData["Brands"] = brand;
            ViewData["ProductCategories"] = productCategory;
            return View();
        }

        [HttpPost]
        public JsonResult Create(PromotionCreateRequest promotion)
        {
            _promotion.CreatePromotion(promotion);
            return Json("OK");
        }

        public IActionResult List(int? page, int? pageSize)
        {
            page = page == null ? 1 : page;
            pageSize = pageSize == null ? 10 : pageSize;
            var promotions = _promotion.GetPromotions((int)page, (int)pageSize);
            return View(promotions);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var promotion = _promotion.GetPromotion(id);
            var brand = _brand.GetBrands();
            var productCategory = _productCategory.GetProductCategories(null, false);
            ViewData["Brands"] = brand;
            ViewData["ProductCategories"] = productCategory;
            ViewData["Promotion"] = promotion;
            return View();
        }

        [HttpPost]
        public JsonResult Edit(PromotionCreateRequest promotion)
        {
            _promotion.UpdatePromotion(promotion);
            return Json("OK");
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            _promotion.DeletePromotion(id);
            return Json("OK");
        }
    }
}
