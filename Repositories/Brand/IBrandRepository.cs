using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Brand
{
    public interface IBrandRepository
    {
        public List<BrandViewModel> GetBrands();
        public Task<List<BrandViewForAdmin>> GetListBrand();
        public int CreateBrand(string BrandName);
        public string UpdateBrand(int brandId, string brandName);

        public int DeleteBrand(int brandId);
    }
}
