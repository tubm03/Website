using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Discount;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class DiscountController : Controller
    {
        private readonly IDiscountRepository _discount;

        public DiscountController(IDiscountRepository discount)
        {
            _discount = discount;
        }


        public IActionResult List(int? page, int? pageSize)
        {
            page = page == null ? 1 : page;
            pageSize = pageSize == null ? 10 : pageSize;
            var discount = _discount.GetDiscounts((int)page, (int)pageSize);
            return View(discount);
        }
    }
}
