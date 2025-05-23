using PetStoreProject.ViewModels;
using AdminVM = PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Product
{
    public interface IProductRepository
    {
        public List<FeedbackViewModels> GetListFeedBack(int productId);
        public ProductDetailViewModel GetDetail(int productId);
        public List<RelatedProductViewModel> getRelatedProduct(int productId, int customerId);
        //public List<ProductDetailViewModel> GetProductDetailDoGet(List<int> cateId, int productCateId);
        //public List<ProductDetailViewModel> GetProductDetailDoPost(List<int> cateId, int productCateId);
        public List<PetStoreProject.Models.Brand> GetBrandsByCategoryIdsAndProductCateId(List<int> cateId, int productCateId);
        public List<PetStoreProject.Models.Size> GetSizesByCategoryIdsAndProductCateId(List<int> cateId, int productCateId);
        public List<int> GetProductIDInWishList(int customerID);
        void AddToFavorites(int userId, int productId);
        void RemoveFromFavorites(int userId, int productId);

        public SearchResultViewModel GetListProductsByKeyWords(string key, int page);

        public int GetNumberOfDogFoods();
        public int GetNumberOfDogAccessories();
        public int GetNumberOfCatFoods();
        public int GetNumberOfCatAccessories();
        public List<HomeProductViewModel> GetProductsOfHome(int cateId, int? productCateId);
        public HomeProductViewModel GetImageAndPriceOfHomeProduct(HomeProductViewModel product);

        public Task<AdminVM.ListProductForAdmin> productViewForAdmins(int pageNumber, int pageSize, int? categoryId,
            int? productCateId, string? key);

        public int GetTotalProducts(List<AdminVM.ProductViewForAdmin> products);

        public Task<string> CreateProduct(AdminVM.ProductCreateRequestViewModel productCreateRequest);

        public AdminVM.ProductDetailForAdmin GetProductDetailForAdmin(int productId);

        public Task<string> UpdateProduct(AdminVM.ProductDetailForAdmin productUpdateRequest);

        public void DeleteProduct(int productId);

        public Task<List<ProductDetailViewModel>> GetProductDetail(List<int> cateId, int productCateId, int customerId);

        public List<AdminVM.ProductViewForAdmin> GetTopSellingProduct(string startDate, string endDate);

        public List<float> GetProductSaleOfMonth(int month, int year);
    }
}
