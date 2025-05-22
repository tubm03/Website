using PetStoreProject.Models;

namespace PetStoreProject.Areas.Admin.ViewModels
{
	public class ProductDetailForAdmin
	{
		public int ProductId { get; set; }
		public string? Name { get; set; }
		public Brand Brand { get; set; }
		public string? Description { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsSoldOut { get; set; }
		public Category Category { get; set; }
		public ProductCategory ProductCategory { get; set; }
		public List<ProductOptionDetailForAdmin> ProductOptions { get; set; }
	}
}
