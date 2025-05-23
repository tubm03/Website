using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Discount;
using PetStoreProject.Repositories.Image;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderItem;
using PetStoreProject.Repositories.ReturnRefund;
using PetStoreProject.Repositories.Shipper;
using PetStoreProject.ViewModels;
using System.Diagnostics.Eventing.Reader;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class OrderController : Controller
    {
        private readonly PetStoreDBContext _context;
        private readonly IOrderRepository _order;
        private readonly IDiscountRepository _discount;
        private readonly IOrderItemRepository _orderItem;
        private readonly IShipperRepository _shipper;
        private readonly IImageRepository _image;
        private readonly IReturnRefundRepository _returnRefund;

        public OrderController(IOrderRepository order, IDiscountRepository discount, IOrderItemRepository orderItem, IShipperRepository shipper, 
            PetStoreDBContext context, IImageRepository image, IReturnRefundRepository returnRefund)
        {
            _order = order;
            _discount = discount;
            _orderItem = orderItem;
            _shipper = shipper;
            _context = context;
            _image = image;
            _returnRefund = returnRefund;
        }

        [HttpGet]
        public IActionResult ListOrderHistory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ListOrderHistory(OrderModel orderCondition)
        {
            var listOrderHistory = _order.GetOrderDetailByCondition(orderCondition);

            foreach (var order in listOrderHistory)
            {
                if (order.DiscountId.HasValue)
                {
                    var priceReduce = _discount.GetDiscountPrice(order.TotalAmount, order.DiscountId.Value);
                    order.TotalAmount = order.TotalAmount - priceReduce;
                }
            }

            var totalOrder = _order.GetCountOrder(orderCondition);

            var totalPage = (int)Math.Ceiling((double)totalOrder / (double)10);

            var searchDate = orderCondition.SearchDate;
            ViewBag.searchDate = searchDate;

            return Json(new
            {
                OrderHistory = listOrderHistory,
                PageIndex = orderCondition.pageIndex,
                TotalOrder = totalOrder,
                NumberPage = totalPage,
            });
        }

        [HttpGet]
        public IActionResult Detail(string orderId)
        {
            long id = long.Parse(orderId);
            var order = _order.GetOrderDetailById(id);
            if (order.DiscountId.HasValue)
            {
                var discount = _discount.GetDiscount(order.DiscountId.Value);
                ViewBag.discount = discount;
            }
            else
            {
                ViewBag.discount = null;
            }
            var checkoutDetail = new CheckoutViewModel
            {
                UserId = order.CustomerId,
                OrderId = long.Parse(order.OrderId),
                OrderEmail = order.Email,
                OrderName = order.FullName,
                OrderPhone = order.Phone,
                ConsigneeAddressDetail = order.ShipAddress,
                ConsigneeName = order.ConsigneeName,
                ConsigneePhone = order.ConsigneePhone,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                ConsigneeWard = "",
                ConsigneeProvince = "",
                ConsigneeDistrict = "",
                OrderDate = order.OrderDate,
                Status = order.Status,
                DiscountId = order.DiscountId,
                ShippingFee = order.ShippingFee,
                ReturnId = order.ReturnId
            };

            var listItemOrder = _orderItem.GetOrderItemByOrderId(long.Parse(order.OrderId));


            var totalAmount = 0.0;

            foreach (var item in listItemOrder)
            {
                var price = item.Price * item.Quantity;
                if (item.Promotion != null)
                {
                    price = price * (1 - (float)item.Promotion.Value / 100);
                }

                totalAmount += price;
            }
            if (order.DiscountId.HasValue)
            {
                var priceDiscount = _discount.GetDiscountPrice(totalAmount, order.DiscountId.Value);
                ViewBag.priceDiscount = priceDiscount;
            }

            var shippers = _shipper.GetShippers();
            ViewBag.shippers = shippers;

            var district = order.ShipAddress.Split(",")[1].Trim();

            var shipper = _shipper.GetShipperByDistricts(district);
            ViewBag.shipper = shipper;

            var districts = _context.Districts.ToList();
            ViewBag.districts = districts;


            ViewBag.listItemOrder = listItemOrder;
            return View(checkoutDetail);
        }

        [HttpPost]
        public IActionResult Detail(string orderId, string status, int shipperId)
        {
            long id = long.Parse(orderId);
            _order.UpdateStatusOrder(id, status, shipperId);
            return Json(new { success = true });
        }

        [HttpPost]        
        public IActionResult DetailRefund(int returnId)
        {
            var returnRefund = _returnRefund.GetReturnRefundById(returnId);
            var imgs = _image.GetImagesByReturnRefundId(returnId);
            return Json(new { reasonReturn = returnRefund.ReasonReturn, returnID = returnId, images = imgs, statusReturn = returnRefund.Status, responseContent = returnRefund.ResponseContent });
        }

        [HttpPost]
        public IActionResult UpdateReturnRefund(int returnId, string statusResponse, string responseContent)
        {
            _returnRefund.UpdateReturnRefund(returnId, statusResponse, responseContent);
            return Json(new { success = true });
        }
    }
}
