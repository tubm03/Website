using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.OrderService
{
    public interface IOrderServiceRepository
    {
        public List<OrderServicesDetailViewModel> GetOrderServicesByCondition(OrderServiceModel orderServiceModel);

        public int GetCountOrderService(int customerId);

    }
}
