using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Size
{
    public interface ISizeRepository
    {
        public List<SizeViewModel> GetSizes();
        public int CreateSize(string sizeName);
    }
}
