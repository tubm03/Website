using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.ProductCategory
{
    public interface IProductCategoryRepository
    {
        public List<ProductCategoryViewModel> GetProductCategories(int? categoryId, bool getDeleted);
        public List<ProductCategoryViewForAdmin> GetListProductCategory();
        public int CreateProductCategory(string ProductCategoryName, int CategoryId);
        public int DeleteProductCategory(int ProductCategoryId);
        public int UpdateProductCategory(int ProductCategoryId, string ProductCategoryName, int CategoryId, bool isDelete);
    }
}
