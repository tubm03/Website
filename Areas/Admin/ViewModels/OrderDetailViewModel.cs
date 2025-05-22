namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class OrderDetailViewModel
    {
        public string OrderId { get; set; }

        public int? CustomerId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string? Email { get; set; }

        public string ConsigneeName { get; set; }

        public string ConsigneePhone { get; set; }

        public string ShipAddress { get; set; }

        public string PaymentMethod { get; set; }

        public float TotalAmount { get; set; }

        public DateTime OrderDate { get; set; }

        public int totalOrderItems { get; set; } = 0;

        public int? DiscountId { get; set; }

        public int? OwnDiscountId { get; set; }

        public string? Status { get; set; }
        public float ShippingFee { get; set; }

        public int? ReturnId { get; set; }
    }
}
