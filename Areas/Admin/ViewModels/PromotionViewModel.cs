namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class PromotionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Value { get; set; }
        public decimal? MaxValue { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Models.Brand Brand { get; set; }
        public Models.ProductCategory ProductCategory { get; set; }
        public string CreatedAt { get; set; }
        public bool? Status { get; set; }
        public string StatusString { get; set; }
    }
}
