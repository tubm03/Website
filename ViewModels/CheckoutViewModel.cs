using PetStoreProject.Models;

namespace PetStoreProject.ViewModels
{
    public class CheckoutViewModel
    {
        public int CheckoutId { get; set; }

        public long OrderId { get; set; }

        public int? UserId { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public string OrderName { get; set; }

        public string OrderPhone { get; set; }

        public string? OrderEmail { get; set; }

        public DateTime OrderDate { get; set; }

        public string ConsigneeName { get; set; }

        public string ConsigneePhone { get; set; }

        public string ConsigneeProvince { get; set; }

        public string ConsigneeDistrict { get; set; }

        public string ConsigneeWard { get; set; }

        public string ConsigneeAddressDetail { get; set; }

        public string PaymentMethod { get; set; }

        public float TotalAmount { get; set; }

        public int? DiscountId { get; set; }

        public int? OwnDiscountId { get; set; }

        public float ShippingFee { get; set; }

        public string? Status { get; set; }
        public int? ReturnId { get; set; }
    }
}
