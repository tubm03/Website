using PetStoreProject.Areas.Admin.ViewModels;
using X.PagedList;

namespace PetStoreProject.Repositories.Discount
{
    public interface IDiscountRepository
    {
        public string Create(Models.Discount discount);
        public IPagedList<DiscountViewModel> GetDiscounts(int page, int pageSize);
        public DiscountViewModel GetDiscount(int id);
        public string Edit(Models.Discount discount);
        public List<DiscountViewModel> GetDiscounts(double total_amount, string email);

        public List<DiscountViewModel> GetOwnDiscount(double total_amount, int customerId);
        public float GetDiscountPrice(double total_amount, int discountId);

        public void DeleteDiscount(int id);
    }
}
