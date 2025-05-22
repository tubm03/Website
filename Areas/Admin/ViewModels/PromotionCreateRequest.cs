namespace PetStoreProject.Areas.Admin.ViewModels
{
	public class PromotionCreateRequest
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int BrandId { get; set; }
		public int ProductCateId { get; set; }
	}
}
