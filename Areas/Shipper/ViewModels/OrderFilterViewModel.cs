namespace PetStoreProject.Areas.Shipper.ViewModels
{
    public class OrderFilterViewModel
    {
        public string? OrderId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? OrderDate { get; set; }
        public string? PaymetMethod { get; set; }
        public string? Status { get; set; }
        public string? SortOrderId { get; set; }
        public string? SortName { get; set; }
        public string? SortOrderDate { get; set; }
        public string? SortTotalAmount { get; set; }
    }
}
