using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Shipper.ViewModels;
using PetStoreProject.Repositories.Order;

namespace PetStoreProject.Areas.Shipper.Controllers
{
    [Area("Shipper")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _order;

        public OrderController(IOrderRepository order)
        {
            _order = order;
        }

        [HttpGet]
        public IActionResult List()
        {
            var shipperEmail = HttpContext.Session.GetString("userEmail");
            var orderFilterVM = new OrderFilterViewModel();
            int pageIndex = 1;
            int pageSize = 5;

            ViewData["ListOrder"] = _order.GetOrderForShipper(shipperEmail, orderFilterVM, pageIndex, pageSize);
            var totalOrders = _order.GetTotalOrderForShipper(shipperEmail, orderFilterVM).Count;
            ViewData["NumberOfPage"] = (int)Math.Ceiling((double)totalOrders / pageSize);
            return View();
        }

        [HttpPost]
        public IActionResult List(OrderFilterViewModel orderFilterVM, int pageIndex, int pageSize)
        {
            var shipperEmail = HttpContext.Session.GetString("userEmail");
            var orders = _order.GetOrderForShipper(shipperEmail, orderFilterVM, pageIndex, pageSize);
            var totalOrders = _order.GetTotalOrderForShipper(shipperEmail, orderFilterVM).Count;
            var numberPage = (int)Math.Ceiling((double)totalOrders / pageSize);

            return Json(new
            {
                listOrder = orders,
                numberOfPage = numberPage,
            });
        }

        [HttpPost]
        public IActionResult ConfirmDelivery(string orderId, string imageData, string status)
        {
            _order.ConfirmDelivery(orderId, imageData, status);

            return Json(new { message = "success" });
        }
    }
}
