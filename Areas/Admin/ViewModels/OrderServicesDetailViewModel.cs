using PetStoreProject.Models;
using EmployeeModel = PetStoreProject.Models;
namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class OrderServicesDetailViewModel
    {
        public int OrderServiceId { get; set; }

        public int? CustomerId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public DateOnly OrderDate { get; set; }

        public TimeOnly? OrderTime { get; set; }

        public string? Message { get; set; }

        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public EmployeeModel.Employee? employee { get; set; }

        public double Price { get; set; }

        public int ServiceId { get; set; }
    }
}
