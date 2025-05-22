using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Customers
{
    public interface ICustomerRepository
    {
        public int GetCustomerId(string email);

        public Customer? GetCustomer(string email);

        public void UpdateProfile(CustomerViewModel customer);

        public int GetTotalNumberCustomer();
    }
}