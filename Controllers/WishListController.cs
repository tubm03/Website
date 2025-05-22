using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Filters;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.WishList;

namespace PetStoreProject.Controllers
{
    [RoleAuthorize("Customer")]
    public class WishListController : Controller
    {
        private readonly ICustomerRepository _customer;
        private readonly IWishListRepository _wishList;

        public WishListController(ICustomerRepository customer, IWishListRepository wishList)
        {
            _customer = customer;
            _wishList = wishList;
        }

        public IActionResult Detail()
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customerId = _customer.GetCustomerId(email);
            var listWishList = _wishList.wishListVMs(customerId);
            return View(listWishList);
        }

        [HttpPost]
        public IActionResult Delete(int productID)
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customerId = _customer.GetCustomerId(email);
            _wishList.DeleteFromWishList(customerId, productID);
            return RedirectToAction("Detail");
        }
    }
}
