using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Admin
{
    public interface IAdminRepository
    {
        public Models.Admin? GetAdmin(string email);
        public void UpdateProfileAdmin(UserViewModel admin);
    }
}
