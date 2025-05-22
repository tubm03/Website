using PetStoreProject.ViewModels;

namespace PetStoreProject.Areas.Employee.ViewModels
{
    public class CustomerDetailViewModel
    {
        public AccountDetailViewModel AccountDetail { get; set; }

        public int TotalOrder { get; set; }

        public int TotalOrderService { get; set; }
    }
}
