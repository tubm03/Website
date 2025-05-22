using PetStoreProject.Models;

namespace PetStoreProject.Repositories.District
{
    public class DistrictRepository: IDistrictRepository
    {
        public readonly PetStoreDBContext _context;

        public DistrictRepository(PetStoreDBContext context) {
            _context = context;
        }

        public List<Models.District> GetDistricts()
        {
            return _context.Districts.ToList();
        }
    }
}
