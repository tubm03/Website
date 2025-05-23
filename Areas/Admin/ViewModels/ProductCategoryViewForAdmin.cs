namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductCategoryViewForAdmin
    {
        public int Id { get; set; }
        public string ProductCateName { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public int TotalProducts { get; set; }
        public int QuantitySoldProduct { get; set; }
        public bool? IsDelete { get; set; }
    }
}
