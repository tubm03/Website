using PetStoreProject.Models;
using PetStoreProject.ViewModels;
using System.Globalization;

namespace PetStoreProject.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly PetStoreDBContext _context;

        public CustomerRepository(PetStoreDBContext dbContext)
        {
            _context = dbContext;
        }

        public int GetCustomerId(string email)
        {
            var customerId = (from c in _context.Customers
                              where c.Email == email
                              select c.CustomerId).FirstOrDefault();
            return customerId;
        }

        public Customer? GetCustomer(string email)
        {
            var customer = (from user in _context.Customers
                            where user.Email == email
                            select user).FirstOrDefault();
            return customer;
        }

        public void UpdateProfile(CustomerViewModel customer)
        {
            var oldCustomer = (from user in _context.Customers
                               where user.CustomerId == customer.CustomerId
                               select user).FirstOrDefault();

            oldCustomer.FullName = customer.FullName;
            oldCustomer.Gender = customer.Gender;
            oldCustomer.Phone = customer.Phone;
            oldCustomer.Email = customer.Email;
            oldCustomer.Address = customer.Address;

            if (customer.DoB != null)
            {
                DateTime dobDateTime = DateTime.ParseExact(customer.DoB, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                oldCustomer.DoB = DateOnly.FromDateTime(dobDateTime);
            }
            else
            {
                oldCustomer.DoB = null;
            }

            var oldAccount = (from acc in _context.Accounts
                              where acc.AccountId == customer.AccountId
                              select acc).FirstOrDefault();

            oldAccount.Email = customer.Email;

            _context.SaveChanges();
        }

        public int GetTotalNumberCustomer()
        {
            var numberCustomer = _context.Customers.Count();

            var phoneOfGuestOrderProduct = (from o in _context.Orders
                                           where o.CustomerId == null
                                           select o.Phone).ToList();
            var phoneOfGuestOrderService = (from o in _context.OrderServices
                                            where o.CustomerId == null
                                            select o.Phone).ToList();
            var numberGuest = phoneOfGuestOrderService.Union(phoneOfGuestOrderProduct).Distinct().Count();

            return numberCustomer + numberGuest;
        }
    }
}
