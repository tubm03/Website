using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class ServiceViewModel
    {
        public int ServiceId { get; set; }

        public string Name { get; set; } = null!;

        public string? Type { get; set; }

        public string? SupDescription { get; set; }

        public float Price { get; set; }

        public string? ImageUrl { get; set; }
    }
}
