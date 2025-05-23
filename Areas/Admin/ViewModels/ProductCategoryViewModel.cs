namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsDelete { get; set; }
    }
}
