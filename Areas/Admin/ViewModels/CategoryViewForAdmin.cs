namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class CategoryViewForAdmin
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public int QuantityOfSold { get; set; }
        public bool isDeleted { get; set; }
    }
}
