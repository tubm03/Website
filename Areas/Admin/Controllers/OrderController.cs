using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Discount;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderItem;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {

        private readonly IOrderRepository _order;
        private readonly IDiscountRepository _discount;
        private readonly IOrderItemRepository _orderItem;

        public OrderController(IOrderRepository order, IDiscountRepository discount, IOrderItemRepository orderItem)
        {
            _order = order;
            _discount = discount;
            _orderItem = orderItem;
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
                if (order.OwnDiscountId.HasValue && order.OwnDiscountId.Value != 0)
                {
                    var priceReduce = _discount.GetDiscountPrice(order.TotalAmount, order.OwnDiscountId.Value);
                    order.TotalAmount = order.TotalAmount - priceReduce;
                }
                order.TotalAmount += order.ShippingFee;
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

        [HttpPost]
        public IActionResult Detail(OrderDetailViewModel order)
        {
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
                OwnDiscountId = order.OwnDiscountId
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
            var priceDiscount = 0.0;
            var ownDiscount = 0.0;
            if (order.DiscountId.HasValue)
            {
                priceDiscount = _discount.GetDiscountPrice(totalAmount, order.DiscountId.Value);
            }

            if (order.OwnDiscountId.HasValue && order.OwnDiscountId.Value != 0)
            {
                ownDiscount = _discount.GetDiscountPrice(totalAmount - priceDiscount, order.OwnDiscountId.Value);
            }

            ViewBag.priceDiscount = priceDiscount + ownDiscount;

            ViewBag.listItemOrder = listItemOrder;
            return View(checkoutDetail);
        }
    }

}

