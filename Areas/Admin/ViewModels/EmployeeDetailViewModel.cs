using PetStoreProject.ViewModels;

namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class EmployeeDetailViewModel
    {
        public AccountDetailViewModel AccountDetail { get; set; }

        public int TotalResponseFeedback{ get; set; }

        public int TotalOrderService { get; set; }
    }
}
