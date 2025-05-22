using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.OrderItem
{
	public interface IOrderItemRepository
	{
		public void AddOrderItem(Models.OrderItem orderItem);

		public List<ItemsCheckoutViewModel> GetOrderItemByOrderId(long orderId);

    }
}
