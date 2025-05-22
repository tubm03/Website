using PetStoreProject.Models;

namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ShipperViewModel
    {
        public int AccountId { get; set; }

        public int ShipperId { get; set; }

        public string FullName { get; set; } = null!;

        public bool? Gender { get; set; }

        public string? Phone { get; set; }

        public DateOnly? DoB { get; set; }

        public string? Address { get; set; }

        public string Email { get; set; } = null!;

        public Role Role { get; set; }

        public bool? IsDelete { get; set; }

        public List<District> Districts { get; set; }

        public int TotalDeliveredQuantity { get; set; }
    }
}
