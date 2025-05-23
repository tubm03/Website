using PetStoreProject.Models;
namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductOptionDetailForAdmin
    {
        public int Id { get; set; }
        public PetStoreProject.Models.Attribute Attribute { get; set; }
        public Size Size { get; set; }
        public float Price { get; set; }
        public Image Image { get; set; }
        public bool IsSoldOut { get; set; }
        public bool? IsDelete { get; set; }
        public int SoldQuantity { get; set; }
        public int Quantity { get; set; }
    }
}
