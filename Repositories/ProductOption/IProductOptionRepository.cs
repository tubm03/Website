using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.ProductOption
{
	public interface IProductOptionRepository
	{
		public Task<string> CreateProductOption(ProductOptionCreateRequestViewModel productOptionCreateRequest, int productId, int imageId);
		public int QuantityOfProductOption(int productOptionId);
		public void UpdateProductOption(int productOptionId, int quantity);

		public void UpdateIsSoldOut(int productOptionId, bool isSoldOut);
    }
}
