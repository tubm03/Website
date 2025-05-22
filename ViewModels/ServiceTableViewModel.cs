namespace PetStoreProject.ViewModels
{
    public class ServiceTableViewModel
    {
        public string ImageUrl { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int ServiceId { get; set; }

        public float Price { get; set; }

        public int UsedQuantity { get; set; }

        public float TotalSale { get; set; }

        public string Type { get; set; } = null!;

        public bool? IsDelete { get; set; }
    }
}
