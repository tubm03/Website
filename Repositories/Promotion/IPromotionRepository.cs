using PetStoreProject.Areas.Admin.ViewModels;
using X.PagedList;

namespace PetStoreProject.Repositories.Promotion
{
    public interface IPromotionRepository
    {
        public void CreatePromotion(PromotionCreateRequest promotion);
        public IPagedList<PromotionViewModel> GetPromotions(int page, int pageSize);
        public PromotionViewModel GetPromotion(int id);
        public void UpdatePromotion(PromotionCreateRequest promotion);
        public void DeletePromotion(int id);
    }
}
