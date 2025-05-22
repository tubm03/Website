namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class LoyalCustomerViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string LoyalName { get; set; }
        public bool IsActive { get; set; }
    }
}
