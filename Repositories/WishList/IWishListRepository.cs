using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.WishList
{
	public interface IWishListRepository
	{
        public List<WishListVM> wishListVMs(int customerId);

		public void DeleteFromWishList(int customerId, int productID);
    }
}
