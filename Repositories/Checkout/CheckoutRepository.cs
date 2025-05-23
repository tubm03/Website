using PetStoreProject.Models;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderItem;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductOption;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Checkout
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly IOrderRepository _order;
        private readonly IOrderItemRepository _orderItem;
        private readonly IProductOptionRepository _productOption;
        private readonly IProductRepository _product;


        public CheckoutRepository(PetStoreDBContext context, IOrderRepository order, IOrderItemRepository orderItem, IProductOptionRepository productOption, IProductRepository product)
        {
            _context = context;
            _order = order;
            _orderItem = orderItem;
            _productOption = productOption;
            _product = product;
        }

        public async Task<string> ProcessCheckOut(CheckoutViewModel checkoutInfo, int? customerId, string email)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Models.Order order = new Models.Order()
                    {
                        OrderId = checkoutInfo.OrderId,
                        FullName = checkoutInfo.OrderName,
                        CustomerId = null,
                        Email = null,
                        Phone = checkoutInfo.OrderPhone,
                        TotalAmount = checkoutInfo.TotalAmount,
                        OrderDate = DateTime.Now,
                        ConsigneeFullName = checkoutInfo.ConsigneeName,
                        ConsigneePhone = checkoutInfo.ConsigneePhone,
                        PaymetMethod = checkoutInfo.PaymentMethod,
                        ShipAddress = checkoutInfo.ConsigneeProvince + ", " + checkoutInfo.ConsigneeDistrict + ", "
                        + checkoutInfo.ConsigneeWard,
                        DiscountId = checkoutInfo.DiscountId,
                        ShippingFee = checkoutInfo.ShippingFee,
                        Status = checkoutInfo.Status,
                        OwnDiscountId = checkoutInfo.OwnDiscountId
                    };

                    if (checkoutInfo.ConsigneeAddressDetail != null) order.ShipAddress += ", " + checkoutInfo.ConsigneeAddressDetail;


                    if (customerId.HasValue) order.CustomerId = customerId;


                    if (checkoutInfo.OrderEmail != null)
                    {
                        order.Email = checkoutInfo.OrderEmail;
                    }
                    else if (email != null)
                    {
                        order.Email = email;
                    }

                    _order.AddOrder(order);

                    foreach (var item in checkoutInfo.OrderItems)
                    {
                        var productOptionId = item.ProductOptionId;

                        var quantityInStock = _productOption.QuantityOfProductOption(productOptionId);
                        if (quantityInStock - item.Quantity == 0)
                        {
                            _productOption.UpdateIsSoldOut(productOptionId, true);
                        }

                        if (quantityInStock < item.Quantity)
                        {

                            var productId = _context.ProductOptions.Where(p => p.ProductOptionId == productOptionId).FirstOrDefault().ProductId;
                            var product = _context.Products.Where(p => p.ProductId == productId).FirstOrDefault();

                            await transaction.RollbackAsync();
                            return "Sản phẩm " + product.Name + " với lựa chọn của quý khách không đủ số lượng trong kho!";
                        }
                        else
                        {
                            Models.OrderItem orderItem = new Models.OrderItem()
                            {
                                OrderId = checkoutInfo.OrderId,
                                ProductOptionId = item.ProductOptionId,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                PromotionId = item.PromotionId,
                            };

                            _orderItem.AddOrderItem(orderItem);
                        }
                    }
                    await transaction.CommitAsync();
                    return "Thanh toán thành công";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return $"Lỗi: {ex.Message}";
                }
            }
        }
    }
}
