using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.Areas.Shipper.ViewModels
{
    public class OrderViewModel
    {

        public string OrderId { get; set; }

        public int? CustomerId { get; set; }

        public string? ConsigneeFullName { get; set; }

        public string? ConsigneePhone { get; set; }

        public string ShipAddress { get; set; } = null!;

        public string PaymetMethod { get; set; } = null!;

        public float TotalAmount { get; set; }

        public string? Status { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveredTime { get; set; }

        public float ShippingFee { get; set; }
    }
}
