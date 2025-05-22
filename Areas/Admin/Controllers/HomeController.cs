using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.Service;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IOrderRepository _order;

        private readonly IServiceRepository _service;

        private readonly ICustomerRepository _customer;

        private readonly IProductRepository _product;

        public HomeController(IOrderRepository order, IServiceRepository service, ICustomerRepository customer, IProductRepository product)
        {
            _order = order;
            _service = service;
            _customer = customer;
            _product = product;
        }

        public IActionResult Index()
        {
            var dashBoard = new DashBoardViewModel();
            dashBoard.TotalProductSale = _order.GetTotalProductSale();
            dashBoard.TotalServiceSale = _service.GetTotalServiceSale();
            dashBoard.TotalSale = dashBoard.TotalProductSale + dashBoard.TotalServiceSale;
            dashBoard.TotalCustomers = _customer.GetTotalNumberCustomer();
            dashBoard.Products = _product.GetTopSellingProduct("", "");
            dashBoard.Services = _service.GetTopSellingService("", "");
            ViewData["DashBoard"] = dashBoard;
            return View();
        }

        [HttpPost]
        public List<ProductViewForAdmin> GetProductBestSelling(string startDate, string endDate)
        {
            var products = _product.GetTopSellingProduct(startDate, endDate);
            return products;
        }

        [HttpPost]
        public List<ServiceTableViewModel> GetServiceBestSelling(string startDate, string endDate)
        {
            var services = _service.GetTopSellingService(startDate, endDate);
            return services;
        }

        [HttpPost]
        public Object StatisticsSaleOfMonth(int month, int year)
        {
            var dataProduct = _product.GetProductSaleOfMonth(month, year);
            var dataService = _service.GetServiceSaleOfMonth(month, year);
            return Json(new
            {
                DataProduct = dataProduct,
                TotalProductOfMonth = dataProduct.Sum(d => d),
                DataService = dataService,
                TotalServiceOfMonth = dataService.Sum(d => d),
            });
        }
    }
}
