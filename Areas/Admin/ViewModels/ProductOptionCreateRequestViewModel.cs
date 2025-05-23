namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductOptionCreateRequestViewModel
    {
        public Models.Image Image { get; set; }
        public Models.Size Size { get; set; }
        public Models.Attribute Attribute { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }
}
