using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.DiscountType
{
    public interface IDiscountTypeRepository
    {
        public List<DiscountTypeViewModel> GetDiscountTypes();
        public List<LoyaltyLevel> GetRolaTypes();
    }
}
