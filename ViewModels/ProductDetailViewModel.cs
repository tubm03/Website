using PetStoreProject.Models;
using Attribute = PetStoreProject.Models.Attribute;

namespace PetStoreProject.ViewModels
{
	public class ProductDetailViewModel
	{
		public int ProductId { get; set; }
		public string Name { get; set; }
		public string Brand { get; set; }
		public int BrandId { get; set; }
		public int ProductCateId { get; set; }
		public string Description { get; set; }
		public List<ProductOptionViewModel> productOption { get; set; }
		public List<Image> images { get; set; }
		public List<Attribute>? attributes { get; set; }
		public List<Size>? sizes { get; set; }
		public bool? IsSoldOut { get; set; }
		public bool? IsDeleted { get; set; }
		public Promotion? Promotion { get; set; }
		public Promotion? PromotionUpcoming { get; set; }
		public int Quantity { get; set; }
	}
}
