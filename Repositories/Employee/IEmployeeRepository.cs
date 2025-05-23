using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Employee
{
    public interface IEmployeeRepository
    {
        public Models.Employee GetEmployee(string email);

        public void UpdateProfileEmployee(UserViewModel employee);
    }
}
