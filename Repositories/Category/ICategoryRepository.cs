using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Category
{
    public interface ICategoryRepository
    {
        public List<CategoryViewModel> GetCategories();

        public Dictionary<string, List<Models.ProductCategory>> GetAllCategory();
    }

}
