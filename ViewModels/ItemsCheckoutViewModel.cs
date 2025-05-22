using PetStoreProject.Models;

namespace PetStoreProject.ViewModels
{
    public class ItemsCheckoutViewModel
    {
        public long OrderId { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string ImgUrl { get; set; }
        public int ProductOptionId { get; set; }
        public int ProductId { get; set; }
        public Promotion? Promotion { get; set; }
    }
}
