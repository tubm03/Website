using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Employee
{
    public class EmployeeReporistory : IEmployeeRepository
    {
        private readonly PetStoreDBContext _context;

        public EmployeeReporistory(PetStoreDBContext dBContext)
        {
            _context = dBContext;
        }

        public Models.Employee GetEmployee(string email)
        {
            var employee = (from e in _context.Employees
                            where e.Email == email
                            select e).FirstOrDefault();
            return employee;
        }

        public void UpdateProfileEmployee(UserViewModel employee)
        {
            if (employee != null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var employeeOld = (from e in _context.Employees
                                           where e.EmployeeId == employee.UserId
                                           select e).FirstOrDefault();
                        employeeOld.FullName = employee.FullName;
                        employeeOld.Email = employee.Email;
                        employeeOld.Address = employee.Address;
                        employeeOld.DoB = employee.DoB;
                        employeeOld.Phone = employee.Phone;
                        employeeOld.Gender = employee.Gender;


                        var oldAccount = (from a in _context.Accounts
                                          where a.AccountId == employee.AccountId
                                          select a).FirstOrDefault();
                        oldAccount.Email = employee.Email;
                        _context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
    }
}
