using PetStoreProject.Models;

namespace PetStoreProject.Areas.Employee.ViewModels
{
    public class ReturnRefunDetailViewModel
    {
        public ReturnRefund ReturnRefund { get; set; }
        public List<Image> Images { get; set; }
    }
}
