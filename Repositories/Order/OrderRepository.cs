using CloudinaryDotNet.Actions;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Shipper.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.Repositories.ProductOption;
using System.Globalization;
using System.Net;

namespace PetStoreProject.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly IProductOptionRepository _productOptionRepository;
        private readonly ICloudinaryService _cloudinary;

        public OrderRepository(PetStoreDBContext dBContext, IProductOptionRepository productOptionRepository, ICloudinaryService cloudinary)
        {
            _context = dBContext;
            _productOptionRepository = productOptionRepository;
            _cloudinary = cloudinary;
        }
        public List<OrderDetailViewModel> GetOrderDetailByCustomerId(int customerId)
        {
            var orders = new List<OrderDetailViewModel>();
            if (customerId > 0)
            {
                orders = (from o in _context.Orders
                          where o.CustomerId == customerId
                          select new OrderDetailViewModel
                          {
                              CustomerId = o.CustomerId,
                              Email = o.Email,
                              FullName = o.FullName,
                              Phone = o.Phone,
                              ShipAddress = o.ShipAddress,
                              OrderDate = o.OrderDate,
                              TotalAmount = o.TotalAmount,
                              PaymentMethod = o.PaymetMethod,
                              OrderId = o.OrderId.ToString(),
                              ConsigneeName = o.ConsigneeFullName,
                              ConsigneePhone = o.ConsigneePhone,
                              totalOrderItems = _context.OrderItems.Count(ot => ot.OrderId == o.OrderId),
                              DiscountId = o.DiscountId,
                              Status = o.Status,
                              ShippingFee = o.ShippingFee,
                              ReturnId = o.ReturnId,
                              OwnDiscountId = o.OwnDiscountId
                          }).ToList();
            }
            else
            {
                orders = (from o in _context.Orders
                          select new OrderDetailViewModel
                          {
                              CustomerId = o.CustomerId,
                              Email = o.Email,
                              FullName = o.FullName,
                              Phone = o.Phone,
                              ShipAddress = o.ShipAddress,
                              OrderDate = o.OrderDate,
                              TotalAmount = o.TotalAmount,
                              PaymentMethod = o.PaymetMethod,
                              OrderId = o.OrderId.ToString(),
                              ConsigneeName = o.ConsigneeFullName,
                              ConsigneePhone = o.ConsigneePhone,
                              totalOrderItems = _context.OrderItems.Count(ot => ot.OrderId == o.OrderId),
                              DiscountId = o.DiscountId,
                              Status = o.Status,
                              ShippingFee = o.ShippingFee,
                              ReturnId = o.ReturnId,
                              OwnDiscountId = o.OwnDiscountId
                          }).ToList();
            }
            return orders;
        }

        public List<OrderDetailViewModel> GetOrderDetailByCondition(OrderModel orderModel)
        {
            var orders = GetOrderDetailExcuteCondition(orderModel);
            if (orderModel.pageSize == 0)
            {
                return orders;
            }
            orders = orders.Skip((orderModel.pageIndex - 1) * orderModel.pageSize).Take(orderModel.pageSize).ToList();

            return orders;
        }

        public List<OrderDetailViewModel> GetOrderDetailExcuteCondition(OrderModel orderModel)
        {
            var orders = GetOrderDetailByCustomerId(orderModel.UserId);

            if (long.TryParse(orderModel.SearchOrderId, out long searchOrderId))
            {
                orders = orders.Where(o => o.OrderId.Equals(searchOrderId.ToString())).ToList();
            }

            if (orderModel.SearchName != null)
            {
                orders = orders.Where(o => o.FullName.ToLower().Contains(orderModel.SearchName.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(orderModel.SearchDate))
            {
                if (DateOnly.TryParse(orderModel.SearchDate, out DateOnly searchDate))
                {
                    orders = orders.Where(o => DateOnly.FromDateTime(o.OrderDate) == searchDate).ToList();
                }
            }

            if (orderModel.SearchConsigneeName != null)
            {
                orders = orders.Where(o => o.ConsigneeName.ToLower().Contains(orderModel.SearchConsigneeName.ToLower())).ToList();
            }

            if (orderModel.SortConsigneeName == "Chờ xác nhận")
                orders = orders.Where(o => o.Status.Equals("Chờ xác nhận")).ToList();
            else if (orderModel.SortConsigneeName == "Đã hủy")
                orders = orders.Where(o => o.Status.Equals("Đã hủy")).ToList();
            else if (orderModel.SortConsigneeName == "Đã giao hàng")
                orders = orders.Where(o => o.Status.Equals("Đã giao hàng")).ToList();
            else if (orderModel.SortConsigneeName == "Chờ giao hàng")
                orders = orders.Where(o => o.Status.Equals("Chờ giao hàng")).ToList();
            else if (orderModel.SortConsigneeName == "Đã nhận hàng")
                orders = orders.Where(o => o.Status.Equals("Đã nhận hàng")).ToList();
            else if (orderModel.SortConsigneeName == "Đã hoàn thành")
                orders = orders.Where(o => o.Status.Equals("Đã hoàn thành")).ToList();
            else if (orderModel.SortConsigneeName == "Trả hàng")
                orders = orders.Where(o => o.Status.Equals("Trả hàng")).ToList();


            if (orderModel.SortOrderId == "abc")
                orders = orders.OrderBy(o => o.OrderId).ToList();
            else if (orderModel.SortOrderId == "zxy")
                orders = orders.OrderByDescending(o => o.OrderId).ToList();

            if (orderModel.SortName == "abc")
                orders = orders.OrderBy(o => o.FullName).ToList();
            else if (orderModel.SortName == "zxy")
                orders = orders.OrderByDescending(o => o.FullName).ToList();

            if (orderModel.SortTotalItems == "abc")
                orders = orders.OrderBy(o => o.totalOrderItems).ToList();
            else if (orderModel.SortTotalItems == "zxy")
                orders = orders.OrderByDescending(o => o.totalOrderItems).ToList();

            if (orderModel.SortDate == "abc")
                orders = orders.OrderBy(o => o.OrderDate).ToList();
            else if (orderModel.SortDate == "zxy")
                orders = orders.OrderByDescending(o => o.OrderDate).ToList();

            if (orderModel.SortPrice == "abc")
                orders = orders.OrderBy(o => o.TotalAmount).ToList();
            else if (orderModel.SortPrice == "zxy")
                orders = orders.OrderByDescending(o => o.TotalAmount).ToList();



            return orders;
        }

        public int GetCountOrder(OrderModel orderCondition)
        {
            int countOrder = GetOrderDetailExcuteCondition(orderCondition).Count;

            return countOrder;
        }

        public void AddOrder(Models.Order order)
        {
            if (order.DiscountId == 0)
            {
                order.DiscountId = null;
            }
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public float GetTotalProductSale()
        {
            var totalAmount = (from o in _context.Orders
                               select o.TotalAmount).Sum();
            return totalAmount;
        }

        public OrderDetailViewModel? GetOrderDetailById(long orderId)
        {
            OrderDetailViewModel? order = _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => new OrderDetailViewModel
                {
                    CustomerId = o.CustomerId,
                    Email = o.Email,
                    FullName = o.FullName,
                    Phone = o.Phone,
                    ShipAddress = o.ShipAddress,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymetMethod,
                    OrderId = o.OrderId.ToString(),
                    ConsigneeName = o.ConsigneeFullName,
                    ConsigneePhone = o.ConsigneePhone,
                    totalOrderItems = _context.OrderItems.Count(ot => ot.OrderId == o.OrderId),
                    DiscountId = o.DiscountId,
                    Status = o.Status,
                    ShippingFee = o.ShippingFee,
                    ReturnId = o.ReturnId,
                    OwnDiscountId = o.OwnDiscountId
                })
                .FirstOrDefault();

            return order;
        }

        public void UpdateStatusOrder(long orderId, string status, int shipper)
        {
            if (shipper != 0 && shipper != -1)
            {
                var order = _context.Orders.FirstOrDefault(order => order.OrderId == orderId);
                order.Status = status;
                order.ShipperId = shipper;
                _context.SaveChanges();
            }
            else if (shipper == -1)
            {
                var order = _context.Orders.FirstOrDefault(order => order.OrderId == orderId);
                order.Status = status;
                _context.SaveChanges();
            }
            else
            {
                var order = _context.Orders.FirstOrDefault(order => order.OrderId == orderId);
                order.Status = status;
                order.ShipperId = null;
                _context.SaveChanges();
                if (status == "Đã hủy")
                {
                    var orderItem = _context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
                    foreach (var item in orderItem)
                    {
                        var productOption = _context.ProductOptions.FirstOrDefault(po => po.ProductOptionId == item.ProductOptionId);
                        productOption.Quantity += item.Quantity;
                        _context.SaveChanges();
                    }
                }
            }
        }

        public void UpdateReturnOrder(long orderId, int returnId)
        {
            var order = _context.Orders.FirstOrDefault(order => order.OrderId == orderId);
            order.ReturnId = returnId;
            _context.SaveChanges();
        }

        public List<OrderViewModel> GetTotalOrderForShipper(string shipperEmail, OrderFilterViewModel orderFilterVM)
        {
            var shipperId = _context.Shippers.FirstOrDefault(s => s.Email == shipperEmail).ShipperId;

            string? orderId = string.IsNullOrEmpty(orderFilterVM.OrderId) ? null : orderFilterVM.OrderId;
            string? name = string.IsNullOrEmpty(orderFilterVM.Name) ? null : orderFilterVM.Name;
            string? phone = string.IsNullOrEmpty(orderFilterVM.Phone) ? null : orderFilterVM.Phone;
            DateTime? orderDate = null;
            if (DateTime.TryParseExact(orderFilterVM.OrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                orderDate = parsedDate;
            }

            var orders = (from o in _context.Orders
                          where o.ShipperId == shipperId
                          && (orderId == null || o.OrderId.ToString().Contains(orderId))
                          && (name == null || o.ConsigneeFullName.ToLower().Contains(name.ToLower()))
                          && (phone == null || o.Phone.Contains(phone))
                          && (orderDate == null || o.OrderDate.Date == orderDate.Value.Date)
                          && (orderFilterVM.Status == null || o.Status == orderFilterVM.Status)
                          && (orderFilterVM.PaymetMethod == null || o.PaymetMethod == orderFilterVM.PaymetMethod)
                          select new OrderViewModel
                          {
                              OrderId = o.OrderId.ToString(),
                              CustomerId = o.CustomerId,
                              ConsigneeFullName = o.ConsigneeFullName,
                              ConsigneePhone = o.ConsigneePhone,
                              ShipAddress = o.ShipAddress,
                              PaymetMethod = o.PaymetMethod,
                              TotalAmount = o.TotalAmount,
                              Status = o.Status,
                              OrderDate = o.OrderDate,
                              DeliveredTime = o.DeliveredTime,
                              ShippingFee = o.ShippingFee,
                          }).ToList();
            return orders;
        }

        public List<OrderViewModel> GetOrderForShipper(string shipperEmail, OrderFilterViewModel orderFilterVM, int pageIndex, int pageSize)
        {
            List<OrderViewModel> orders = GetTotalOrderForShipper(shipperEmail, orderFilterVM);

            if (orders != null)
            {
                if (orderFilterVM.SortOrderId != null)
                {
                    if (orderFilterVM.SortOrderId == "Ascending")
                        orders = orders.OrderBy(o => o.OrderId).ToList();
                    else
                        orders = orders.OrderByDescending(o => o.OrderId).ToList();
                }

                if (orderFilterVM.SortName != null)
                {
                    if (orderFilterVM.SortName == "Ascending")
                        orders = orders.OrderBy(o => o.ConsigneeFullName).ToList();
                    else
                        orders = orders.OrderByDescending(o => o.ConsigneeFullName).ToList();
                }

                if (orderFilterVM.SortOrderDate != null)
                {
                    if (orderFilterVM.SortOrderDate == "Ascending")
                        orders = orders.OrderBy(o => o.OrderDate).ToList();
                    else
                        orders = orders.OrderByDescending(o => o.OrderDate).ToList();
                }

                if (orderFilterVM.SortTotalAmount != null)
                {
                    if (orderFilterVM.SortTotalAmount == "Ascending")
                        orders = orders.OrderBy(o => o.TotalAmount).ToList();
                    else
                        orders = orders.OrderByDescending(o => o.TotalAmount).ToList();
                }
            }

            var orderDisplay = orders.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return orderDisplay;
        }

        public async Task ConfirmDelivery(string orderId, string imageData, string status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId.ToString().Equals(orderId));
            order.Status = status;
            order.DeliveredTime = DateTime.Now;

            await _context.SaveChangesAsync();

            int maxImageId = (from i in _context.Images
                              orderby i.ImageId descending
                              select i.ImageId).FirstOrDefault();
            maxImageId++;
            ImageUploadResult result = await _cloudinary.UploadImage(imageData, "image_" + maxImageId);
            Models.Image image = new Models.Image()
            {
                ImageId = maxImageId,
                OrderId = long.Parse(orderId),
                ImageUrl = result.Url.ToString()
            };

            _context.Images.Add(image);

            await _context.SaveChangesAsync();
        }
    }
}
