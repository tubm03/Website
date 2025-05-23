using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly PetStoreDBContext _context;
        public AdminRepository(PetStoreDBContext dBContext)
        {
            _context = dBContext;
        }
        public Models.Admin? GetAdmin(string email)
        {
            var admin = (from a in _context.Admins
                        where a.Email == email
                        select a).FirstOrDefault();
            return admin;
        }

        public void UpdateProfileAdmin(UserViewModel admin)
        {
            if (admin != null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var adminOld = (from a in _context.Admins
                                        where a.AdminId == admin.UserId
                                        select a).FirstOrDefault();
                        adminOld.FullName = admin.FullName;
                        adminOld.Email = admin.Email;
                        adminOld.Address = admin.Address;
                        adminOld.DoB = admin.DoB;
                        adminOld.Phone = admin.Phone;
                        adminOld.Gender = admin.Gender;


                        var oldAccount = (from a in _context.Accounts
                                          where a.AccountId == admin.AccountId
                                          select a).FirstOrDefault();
                        oldAccount.Email = admin.Email;
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
