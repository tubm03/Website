using PetStoreProject.Models;

namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class DiscountViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string? Title { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? CreatedAt { get; set; }
        public DiscountTypeViewModel DiscountType { get; set; }
        public decimal? Value { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinPurchase { get; set; }
        public int? Quantity { get; set; }
        public int? MaxUse { get; set; }
        public int? Used { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public string? StatusString { get; set; }
        public decimal? Reduce { get; set; }
        public LoyaltyLevel Loyal { get; set; }
    }
}
