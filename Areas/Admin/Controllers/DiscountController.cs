using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Discount;
using PetStoreProject.Repositories.DiscountType;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountController : Controller
    {
        private readonly IDiscountTypeRepository _discountType;
        private readonly IDiscountRepository _discount;

        public DiscountController(IDiscountRepository discount, IDiscountTypeRepository discountType)
        {
            _discountType = discountType;
            _discount = discount;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var discountType = _discountType.GetDiscountTypes();
            var loyalType = _discountType.GetRolaTypes();
            ViewData["discountType"] = discountType;
            ViewData["loyalType"] = loyalType;
            return View();
        }

        [HttpPost]
        public JsonResult Create(Discount discount)
        {
            var result = _discount.Create(discount);
            return Json(result);
        }

        public IActionResult List(int? page, int? pageSize)
        {
            page = page == null ? 1 : page;
            pageSize = pageSize == null ? 10 : pageSize;
            var discount = _discount.GetDiscounts((int)page, (int)pageSize);
            return View(discount);
        }

        public IActionResult Edit(int id)
        {
            var discount = _discount.GetDiscount(id);
            var discountType = _discountType.GetDiscountTypes();
            var loyalType = _discountType.GetRolaTypes();
            ViewData["loyalType"] = loyalType;
            ViewData["discountType"] = discountType;
            ViewData["discount"] = discount;
            return View();
        }

        [HttpPost]
        public JsonResult Edit(Discount discount)
        {
            var result = _discount.Edit(discount);
            return Json(result);
        }

        public JsonResult Delete(int id)
        {
            _discount.DeleteDiscount(id);
            return Json("OK");
        }
    }
}
