using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Shipper.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Order
{
    public interface IOrderRepository
    {
        public List<OrderDetailViewModel> GetOrderDetailByCondition(OrderModel orderModel);

        public int GetCountOrder(OrderModel orderCondition);

        public void AddOrder(Models.Order order);

        public float GetTotalProductSale();

        public OrderDetailViewModel? GetOrderDetailById(long orderId);

        public void UpdateStatusOrder(long orderId, string status, int shipper);

        public void UpdateReturnOrder(long orderId, int returnId);

        public List<OrderViewModel> GetTotalOrderForShipper(string shipperEmail, OrderFilterViewModel orderFilterVM);

        public List<OrderViewModel> GetOrderForShipper(string shipperEmail, OrderFilterViewModel orderFilterVM, int pageIndex, int pageSize);

        public Task ConfirmDelivery(string orderId, string imageData, string status);
    }
}
