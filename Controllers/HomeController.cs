using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.News;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.Service;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly PetStoreDBContext _context;
        private readonly IProductRepository _product;
        private readonly ICustomerRepository _customer;
        private readonly IServiceRepository _service;
        private readonly INewsRepository _news;
        private readonly ICategoryRepository _category;

        public HomeController(PetStoreDBContext dbContext, IProductRepository product, ICustomerRepository customer, IServiceRepository service, INewsRepository news, ICategoryRepository category)
        {
            _context = dbContext;
            _product = product;
            _customer = customer;
            _service = service;
            _news = news;
            _category = category;

        }

        public int GetCustomerId()
        {
            var email = HttpContext.Session.GetString("userEmail");
            if (email != null)
            {
                var customerID = _customer.GetCustomerId(email);
                return customerID;
            }
            else
            {
                return -1;
            }
        }

        public List<HomeProductViewModel> GetListItemToDisplayed(List<HomeProductViewModel> Items)
        {
            List<HomeProductViewModel> itemsDisplayed = new List<HomeProductViewModel>();
            foreach (var item in Items)
            {
                var product = _product.GetImageAndPriceOfHomeProduct(item);
                if (product != null)
                {
                    itemsDisplayed.Add(product);
                    if (itemsDisplayed.Count == 8)
                    {
                        break;
                    }
                }
            }
            return itemsDisplayed;
        }

        public IActionResult Index(string? success)
        {
            if (success != null)
            {
                ViewBag.Success = success;
            }

            List<int> listPID = _product.GetProductIDInWishList(GetCustomerId());
            ViewData["listPID"] = listPID;

            var dogFoods = _product.GetProductsOfHome(3, 14);
            var dogAccessories = _product.GetProductsOfHome(2, null);


            var homeVM = new HomeViewModel
            {
                NumberOfDogFoods = _product.GetNumberOfDogFoods(),
                NumberOfDogAccessories = _product.GetNumberOfDogAccessories(),
                NumberOfCatFoods = _product.GetNumberOfCatFoods(),
                NumberOfCatAccessories = _product.GetNumberOfCatAccessories(),
                DogFoodsDisplayed = GetListItemToDisplayed(dogFoods),

                DogAccessoriesDisplayed = GetListItemToDisplayed(dogAccessories),

                ServicesDisplayed = _service.GetListServices(),
                NewsDisplayed = _news.GetListNewsForHomePage()
            };
            return View(homeVM);
        }

        [HttpPost]
        public Object GetCategoryMenu()
        {
            var categories = _category.GetAllCategory();

            return Json(new
            {
                dogFoodCategory = categories["DogFood"],
                dogAccessoryCategory = categories["DogAccessory"],
                catFoodCategory = categories["CatFood"],
                catAccessoryCategory = categories["CatAccessory"],
                services = _service.GetListServices()
            });
        }
    }
}
