using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using VM = PetStoreProject.ViewModels;
using System.Globalization;
using VMAdmin = PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Accounts
{
    public class AccountRepository : IAccountRepository
    {
        private readonly PetStoreDBContext _context;

        public AccountRepository(PetStoreDBContext dbContext)
        {
            _context = dbContext;
        }

        public Account GetAccount(string email, string password)
        {
            var account = (from acc in _context.Accounts
                           where acc.Email == email && acc.IsDelete == false
                           select acc).FirstOrDefault();
            if (account == null)
            {
                return null;
            }

            if (account.Password == null)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, account.Password);

            if (isPasswordValid == false)
            {
                return null;
            }
            else
            {
                return account;
            }
        }

        public bool CheckEmailExist(string email)
        {
            var account = (from acc in _context.Accounts
                           where acc.Email == email
                           select acc).FirstOrDefault();

            if (account != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddNewCustomer(VM.RegisterViewModel registerInfor)
        {
            Account account = new Account();
            account.Email = registerInfor.Email;
            if (registerInfor.Password != null)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(registerInfor.Password);
            }
            else
            {
                account.Password = null;
            }

            account.RoleId = 3;

            account.IsDelete = false;

            _context.Accounts.Add(account);

            _context.SaveChanges();

            var accountId = (from a in _context.Accounts
                             where a.Email == registerInfor.Email
                             select a.AccountId).FirstOrDefault();

            var customer = new Customer
            {
                FullName = registerInfor.FullName,
                Phone = registerInfor.Phone,
                Email = registerInfor.Email,
                AccountId = accountId,
            };

            _context.Customers.Add(customer);

            _context.SaveChanges();
        }

        public void ResetPassword(ResetPasswordViewModel ResetPasswordVM)
        {
            var account = _context.Accounts.FirstOrDefault(acc => acc.Email == ResetPasswordVM.Email);

            if (account != null)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(ResetPasswordVM.Password);

                _context.SaveChanges();
            }
        }

        public string? GetOldPassword(string email)
        {
            var password = (from a in _context.Accounts
                            where a.Email == email
                            select a.Password).FirstOrDefault();
            return password;
        }

        public void ChangePassword(ChangePasswordViewModel ChangePasswordVM)
        {
            var account = (from a in _context.Accounts
                           where a.Email == ChangePasswordVM.Email
                           select a).FirstOrDefault();

            account.Password = BCrypt.Net.BCrypt.HashPassword(ChangePasswordVM.NewPassword);

            _context.SaveChanges();
        }

        public string GetUserRoles(string email)
        {
            var role = (from a in _context.Accounts
                        join r in _context.Roles on a.RoleId equals r.RoleId
                        where a.Email == email
                        select r.Name).FirstOrDefault();

            return role;
        }

        public string GetUserName(string email, string userRole)
        {
            string? userName;
            if (userRole == "Admin")
            {
                userName = (from a in _context.Accounts
                            join ad in _context.Admins on a.AccountId equals ad.AccountId
                            where a.Email == email
                            select ad.FullName).FirstOrDefault();
            }
            else if (userRole == "Employee")
            {
                userName = (from a in _context.Accounts
                            join e in _context.Employees on a.AccountId equals e.AccountId
                            where a.Email == email
                            select e.FullName).FirstOrDefault();
            }
            else if (userRole == "Shipper")
            {
                userName = (from a in _context.Accounts
                            join s in _context.Shippers on a.AccountId equals s.AccountId
                            where a.Email == email
                            select s.FullName).FirstOrDefault();
            }
            else
            {
                userName = (from a in _context.Accounts
                            join c in _context.Customers on a.AccountId equals c.AccountId
                            where a.Email == email
                            select c.FullName).FirstOrDefault();
            }
            return userName;
        }

        public List<VMAdmin.EmployeeDetailViewModel> GetEmployees()
        {
            var accountEmployees = (from e in _context.Employees
                                    join a in _context.Accounts on e.AccountId equals a.AccountId
                                    select new EmployeeDetailViewModel
                                    {
                                        AccountDetail = new AccountDetailViewModel
                                        {
                                            AccountId = e.AccountId,
                                            UserId = e.EmployeeId,
                                            FullName = e.FullName,
                                            Phone = e.Phone,
                                            DoB = e.DoB,
                                            Address = e.Address.Trim(),
                                            Email = e.Email,
                                            Gender = e.Gender,
                                            Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                            IsDelete = e.IsDelete
                                        },

                                        TotalOrderService = _context.OrderServices.Count(os => os.EmployeeId == e.EmployeeId),

                                        TotalResponseFeedback = _context.ResponseFeedbacks.Count(rFb => rFb.EmployeeId == e.EmployeeId)
                                    }).ToList();
            return accountEmployees;
        }

        public List<CustomerDetailViewModel> GetCustomers()
        {
            var accountCustomers = (from c in _context.Customers
                                    join a in _context.Accounts on c.AccountId equals a.AccountId
                                    select new CustomerDetailViewModel
                                    {
                                        AccountDetail = new AccountDetailViewModel
                                        {
                                            AccountId = c.AccountId,
                                            UserId = c.CustomerId,
                                            FullName = c.FullName,
                                            Phone = c.Phone,
                                            DoB = c.DoB,
                                            Address = c.Address.Trim(),
                                            Email = c.Email,
                                            Gender = c.Gender,
                                            Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                            IsDelete = c.IsDelete
                                        },

                                        TotalOrder = _context.Orders.Count(o => o.CustomerId == c.CustomerId),

                                        TotalOrderService = _context.OrderServices.Count(os => os.CustomerId == c.CustomerId),

                                    }).ToList();
            return accountCustomers;
        }

        public List<AccountDetailViewModel> GetAccountAdmins()
        {
            var accountAdmins = (from ad in _context.Admins
                                 join a in _context.Accounts on ad.AccountId equals a.AccountId
                                 select new AccountDetailViewModel
                                 {
                                     AccountId = ad.AccountId,
                                     UserId = ad.AdminId,
                                     FullName = ad.FullName,
                                     Phone = ad.Phone,
                                     DoB = ad.DoB,
                                     Address = ad.Address.Trim(),
                                     Email = ad.Email,
                                     Gender = ad.Gender,
                                     Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                     IsDelete = ad.IsDelete
                                 }).ToList();
            return accountAdmins;
        }

        public AccountDetailViewModel GetAccountCustomers(int customerId)
        {
            var accountCustomer = (from c in _context.Customers
                                    join a in _context.Accounts on c.AccountId equals a.AccountId
                                    where c.CustomerId == customerId
                                    select new AccountDetailViewModel
                                    {
                                        AccountId = c.AccountId,
                                        UserId = c.CustomerId,
                                        FullName = c.FullName,
                                        Phone = c.Phone,
                                        DoB = c.DoB,
                                        Address = c.Address.Trim(),
                                        Email = c.Email,
                                        Gender = c.Gender,
                                        Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                        IsDelete = c.IsDelete
                                    }).FirstOrDefault();
            return accountCustomer;

        }

        public List<CustomerDetailViewModel> GetAccountCustomers(int pageIndex, int pageSize, int userType, string searchName, string sortName, string selectedStatus)
        {
            List<CustomerDetailViewModel> accounts = GetCustomers();

            if (sortName == "abc")
            {
                accounts = accounts.OrderBy(a => a.AccountDetail.FullName).ToList();
            }
            else if (sortName == "zxy")
            {
                accounts = accounts.OrderByDescending(a => a.AccountDetail.FullName).ToList();
            }

            if (searchName != "")
            {
                accounts = accounts.Where(acc => acc.AccountDetail.FullName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (selectedStatus == "0")
            {
                accounts = accounts.Where(acc => acc.AccountDetail.IsDelete == false).ToList();
            }
            else if (selectedStatus == "1")
            {
                accounts = accounts.Where(acc => acc.AccountDetail.IsDelete == true).ToList();
            }

            accounts = accounts.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return accounts;
        }
        
        public List<VMAdmin.EmployeeDetailViewModel> GetAccountEmployees(int pageIndex, int pageSize, int userType, string searchName, string sortName, string selectedStatus)
        {
            List<EmployeeDetailViewModel> accounts = GetEmployees();

            if (sortName == "abc")
            {
                accounts = accounts.OrderBy(a => a.AccountDetail.FullName).ToList();
            }
            else if (sortName == "zxy")
            {
                accounts = accounts.OrderByDescending(a => a.AccountDetail.FullName).ToList();
            }

            if (searchName != "")
            {
                accounts = accounts.Where(acc => acc.AccountDetail.FullName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (selectedStatus == "0")
            {
                accounts = accounts.Where(acc => acc.AccountDetail.IsDelete == false).ToList();
            }
            else if (selectedStatus == "1")
            {
                accounts = accounts.Where(acc => acc.AccountDetail.IsDelete == true).ToList();
            }

            accounts = accounts.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return accounts;
        }
        
        public int GetAccountCount(int userType)
        {
            int countAccounts = 0;
            switch (userType)
            {
                case 1:
                    countAccounts = GetAccountAdmins().Count;
                    break;
                case 2:
                    countAccounts = GetEmployees().Count;
                    break;
                case 3:
                    countAccounts = GetCustomers().Count;
                    break;
            }
            return countAccounts;
        }

        public void AddNewEmployment(Areas.Admin.ViewModels.AccountViewModel accountViewModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var account = new Models.Account
                    {
                        Email = accountViewModel.Email,
                        Password = accountViewModel.Password != null ? BCrypt.Net.BCrypt.HashPassword(accountViewModel.Password) : null,
                        RoleId = 2,
                        IsDelete = false
                    };

                    _context.Accounts.Add(account);
                    _context.SaveChanges();

                    var accountId = _context.Accounts
                        .FirstOrDefault(a => a.Email == accountViewModel.Email)?.AccountId ?? 0;

                    if (accountId != 0)
                    {
                        DateOnly dateOfBirth;
                        if (!DateOnly.TryParse(accountViewModel.DoB, out dateOfBirth))
                        {
                            Console.WriteLine("The date string is not in a valid format.");
                            return;
                        }

                        var employee = new Models.Employee
                        {
                            FullName = accountViewModel.FullName,
                            DoB = dateOfBirth,
                            Phone = accountViewModel.Phone,
                            Address = accountViewModel.Address,
                            Gender = accountViewModel.Gender,
                            IsDelete = false,
                            AccountId = accountId,
                            Email = accountViewModel.Email
                        };

                        _context.Employees.Add(employee);
                        _context.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Failed to add new employment.");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public bool IsExistAccount(int accountId)
        {
            bool checkExist = true;
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
            if (account == null || account.IsDelete == true)
            {
                checkExist = false;
            }
            return checkExist;
        }

        public int UpdateStatusDeleteEmployee(int accountId)
        {
            int status = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var account = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                    var employee = _context.Employees.FirstOrDefault(e => e.AccountId == accountId);
                    if (account != null && employee != null)
                    {
                        account.IsDelete = true;
                        employee.IsDelete = true;
                        _context.SaveChanges();
                    }
                    transaction.Commit();
                    status = 1;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return status;
        }

    }
}
