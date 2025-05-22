
using PetStoreProject.Models;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductOption;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.OrderItem
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly IProductRepository _product;
        private readonly IProductOptionRepository _productOption;
        public OrderItemRepository(PetStoreDBContext context, IProductRepository product, IProductOptionRepository productOption)
        {
            _context = context;
            _product = product;
            _productOption = productOption;
        }
        public void AddOrderItem(Models.OrderItem orderItem)
        {
            if(orderItem.PromotionId == 0)
            {
                orderItem.PromotionId = null;
            }
            var quantity = _productOption.QuantityOfProductOption(orderItem.ProductOptionId);
            if (quantity < orderItem.Quantity)
            {
                throw new Exception("Not enough quantity");
            }else
            {
                var quantityAfter = quantity - orderItem.Quantity;
                _productOption.UpdateProductOption(orderItem.ProductOptionId, quantityAfter);
            }
            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();
        }

        public List<ItemsCheckoutViewModel> GetOrderItemByOrderId(long orderId)
        {
            var listOrderItems = new List<ItemsCheckoutViewModel>();

            var orderItems = _context.OrderItems
                                     .Where(orderItem => orderItem.OrderId == orderId)
                                     .Select(orderItem => new
                                     {
                                         orderItem.OrderId,
                                         orderItem.ProductOptionId,
                                         orderItem.Price,
                                         orderItem.Quantity,
                                         ProductName = orderItem.ProductOption.Product.Name, // Include Product Name directly
                                         OptionAttributeName = orderItem.ProductOption.Attribute != null ? orderItem.ProductOption.Attribute.Name : "",
                                         OptionSizeName = orderItem.ProductOption.Size != null ? orderItem.ProductOption.Size.Name : "",
                                         ImageUrl = orderItem.ProductOption.Image != null ? orderItem.ProductOption.Image.ImageUrl : "",
                                         orderItem.ProductOption.ProductId,
                                         orderItem.Promotion
                                     })
                                     .ToList();

            foreach (var item in orderItems)
            {
                listOrderItems.Add(new ItemsCheckoutViewModel
                {
                    OrderId = item.OrderId,
                    Name = item.ProductName,
                    Option = item.OptionAttributeName + item.OptionSizeName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ImgUrl = item.ImageUrl,
                    ProductOptionId = item.ProductOptionId,
                    ProductId = item.ProductId,
                    Promotion = item.Promotion
                });
            }

            return listOrderItems;
        }


    }
}
