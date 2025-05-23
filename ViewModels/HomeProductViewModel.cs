using PetStoreProject.Models;

namespace PetStoreProject.ViewModels
{
    public class HomeProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public string ImageUrl { get; set; }
        public int BrandId { get; set; }
        public Promotion? Promotion { get; set; }   
    }
}
