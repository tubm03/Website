using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Cart
{
	public interface ICartRepository
	{
		public CartItemViewModel GetCartItemVM(int productOptionId, int quantity);
		public List<CartItemViewModel> GetListCartItemsVM(int customerId);

		public void AddItemsToCart(int productOptionId, int quantity, int customerID);

		public void UpdateQuantityToCartItem(int productOptionId, int quantity, int customerID);

		public void DeleteCartItem(int productOptionId, int customer);

		public bool isExistProductOption(int newProductOptionID, int customerID);

		public void UpdateNewCartItem(int oldProductOptionId, int newProductOptionId, int quantity, int customerID);

		public CartItemViewModel? findCartItemViewModel(int productOptionId, int customerID);

		public Models.Promotion GetItemPromotion(int itemId);
	}
}
