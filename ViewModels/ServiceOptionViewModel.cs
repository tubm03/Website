namespace PetStoreProject.ViewModels
{
    public class ServiceOptionViewModel
    {
        public int ServiceOptionId { get; set; }

        public int ServiceId { get; set; }

        public string PetType { get; set; }

        public string Weight { get; set; }

        public float Price { get; set; }

        public bool IsDelete { get; set; }

        public List<string> PetTypes { get; set; }

        public List<string> Weights { get; set; }

        public int OrderedQuantity { get; set; }

        public int UsedQuantity { get; set; }
    }
}
