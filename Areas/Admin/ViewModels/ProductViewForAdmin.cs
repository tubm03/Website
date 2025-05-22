namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductViewForAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public float Price { get; set; }
        public string? Brand { get; set; }
        public bool IsSoldOut { get; set; }
        public int SoldQuantity { get; set; }
        public float? TotalSale { get; set; }
        public bool? IsDelete { get; set; }
        public int Quantity { get; set; }
        public ViewModels.CategoryViewModel Category { get; set; }
        public ViewModels.ProductCategoryViewModel ProductCategory { get; set; }
    }
}
