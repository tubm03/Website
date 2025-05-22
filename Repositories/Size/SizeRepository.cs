using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Size
{
    public class SizeRepository : ISizeRepository
    {
        private readonly PetStoreDBContext _context;

        public SizeRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public int CreateSize(string sizeName)
        {
            var size = new Models.Size
            {
                Name = sizeName
            };
            _context.Sizes.Add(size);
            _context.SaveChanges();
            return size.SizeId;
        }

        public List<SizeViewModel> GetSizes()
        {
            var sizes = _context.Sizes.Select(s => new SizeViewModel
            {
                Id = s.SizeId,
                Name = s.Name
            }).ToList();
            return sizes;
        }
    }
}
