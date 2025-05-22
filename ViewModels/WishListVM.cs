using PetStoreProject.Models;

namespace PetStoreProject.ViewModels
{
    public class WishListVM
    {
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public string Brand { get; set; }
		public string? img_url { get; set; }
        public float price { get; set; }
    }
}
