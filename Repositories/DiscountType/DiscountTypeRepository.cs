using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.DiscountType
{
    public class DiscountTypeRepository : IDiscountTypeRepository
    {
        private readonly PetStoreDBContext _context;

        public DiscountTypeRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public List<DiscountTypeViewModel> GetDiscountTypes()
        {
            var discountTypes = _context.DiscountTypes.Select(d => new DiscountTypeViewModel
            {
                Id = d.DiscountTypeId,
                Name = d.DiscountName,
            }).ToList();
            return discountTypes;
        }

        public List<LoyaltyLevel> GetRolaTypes()
        {
            return _context.LoyaltyLevels.ToList();
        }
    }
}
