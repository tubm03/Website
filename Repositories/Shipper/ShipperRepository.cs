using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Shipper
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly PetStoreDBContext _context;

        public ShipperRepository(PetStoreDBContext dBContext)
        {
            _context = dBContext;
        }

        public Models.Shipper GetShipperByDistricts(string districtName)
        {
            var district = _context.Districts.FirstOrDefault(d => d.Name.Contains(districtName));
            if (district == null)
            {
                return null;
            }
            return _context.Shippers.FirstOrDefault(s => s.ShipperId == district.ShipperId);
        }

        public List<Models.Shipper> GetShippers()
        {
            var shippers = _context.Shippers.ToList();
            return shippers;
        }
        public List<ShipperViewModel> GetTotalAccountShippers(ShipperFilterViewModel shipperFilerVM)
        {
            string? shipperName = string.IsNullOrEmpty(shipperFilerVM.Name) ? null : shipperFilerVM.Name;
            int? districtID = string.IsNullOrEmpty(shipperFilerVM.DistrictId) ? null : Int32.Parse(shipperFilerVM.DistrictId);
            bool? status = string.IsNullOrEmpty(shipperFilerVM.Status) ? null : bool.Parse(shipperFilerVM.Status);

            var shippers = (from s in _context.Shippers
                            join a in _context.Accounts on s.AccountId equals a.AccountId
                            join d in _context.Districts on s.ShipperId equals d.ShipperId
                            where (shipperName == null || s.FullName.Contains(shipperName))
                            && (districtID == null || d.DistrictId == districtID)
                            && (status == null || s.IsDelete == status)
                            select new ShipperViewModel
                            {
                                AccountId = s.AccountId,
                                ShipperId = s.ShipperId,
                                FullName = s.FullName,
                                Gender = s.Gender,
                                Phone = s.Phone,
                                DoB = s.DoB,
                                Address = s.Address,
                                Email = s.Email,
                                Role = _context.Roles.FirstOrDefault(r => r.RoleId == a.RoleId),
                                IsDelete = s.IsDelete,
                            }).Distinct().ToList();

            return shippers;
        }

        public List<ShipperViewModel> GetAccountShippers(ShipperFilterViewModel shipperFilerVM, int pageIndex, int pageSize)
        {
            List<ShipperViewModel> shippers = GetTotalAccountShippers(shipperFilerVM);

            if (shippers != null)
            {
                foreach (var shipper in shippers)
                {
                    shipper.Districts = _context.Districts.Where(d => d.ShipperId == shipper.ShipperId).ToList();
                    shipper.TotalDeliveredQuantity = _context.Orders.Where(o => o.ShipperId == shipper.ShipperId).Count();
                }

                if (shipperFilerVM.SortName != null)
                {
                    if (shipperFilerVM.SortName == "Ascending")
                        shippers = shippers.OrderBy(s => s.FullName).ToList();
                    else
                        shippers = shippers.OrderByDescending(s => s.FullName).ToList();
                }

                if (shipperFilerVM.SortDeliveryQuantity != null)
                {
                    if (shipperFilerVM.SortDeliveryQuantity == "Ascending")
                        shippers = shippers.OrderBy(s => s.TotalDeliveredQuantity).ToList();
                    else
                        shippers = shippers.OrderByDescending(s => s.TotalDeliveredQuantity).ToList();
                }
            }

            var listDisplay = shippers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return listDisplay;
        }

        public Models.Shipper GetShipperByID(int? id)
        {
            return _context.Shippers.FirstOrDefault(s => s.ShipperId == id);
        }

        public bool CheckAssignDuplicatedDistrict(AccountViewModel shipper)
        {
            List<Models.District> districtsDuplicated = new List<Models.District>();
            if (shipper.ShipperId == null)
            {
                districtsDuplicated = (from d in _context.Districts
                                       join s in _context.Shippers on d.ShipperId equals s.ShipperId
                                       where  shipper.Districts.Contains(d.DistrictId) && s.IsDelete == false
                                       select d).ToList();
            }
            else
            {
                districtsDuplicated = (from d in _context.Districts
                                       join s in _context.Shippers on d.ShipperId equals s.ShipperId
                                       where shipper.Districts.Contains(d.DistrictId) && s.IsDelete == false && s.ShipperId != shipper.ShipperId
                                       select d).ToList();
            }

            if (districtsDuplicated.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddNewShipper(AccountViewModel shipper)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Account account = new Account();
                    account.Email = shipper.Email;
                    account.Password = shipper.Password;
                    account.RoleId = 4;
                    account.IsDelete = false;

                    _context.Accounts.Add(account);
                    _context.SaveChanges();

                    Models.Shipper newShipper = new Models.Shipper();
                    newShipper.FullName = shipper.FullName;
                    newShipper.Gender = shipper.Gender;

                    DateOnly dateOfBirth = DateOnly.Parse(shipper.DoB);
                    newShipper.DoB = dateOfBirth;

                    newShipper.Phone = shipper.Phone;
                    newShipper.Email = shipper.Email;
                    newShipper.Address = shipper.Address;
                    newShipper.IsDelete = false;
                    newShipper.AccountId = account.AccountId;

                    _context.Shippers.Add(newShipper);
                    _context.SaveChanges();

                    if (shipper.Districts != null)
                    {
                        foreach (var districtId in shipper.Districts)
                        {
                            var district = _context.Districts.FirstOrDefault(d => d.DistrictId == districtId);
                            district.ShipperId = newShipper.ShipperId;
                        }
                    }
                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public void UpdateShipper(AccountViewModel shipper)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var oldShipper = _context.Shippers.FirstOrDefault(s => s.ShipperId == shipper.ShipperId);
                    oldShipper.FullName = shipper.FullName;
                    oldShipper.Gender = shipper.Gender;
                    DateOnly dateOfBirth = DateOnly.Parse(shipper.DoB);
                    oldShipper.DoB = dateOfBirth;
                    oldShipper.Phone = shipper.Phone;
                    oldShipper.Address = shipper.Address;
                    oldShipper.Email = shipper.Email;

                    var account = _context.Accounts.FirstOrDefault(a => a.AccountId == oldShipper.AccountId);
                    account.Email = shipper.Email;

                    var oldDistricts = _context.Districts.Where(d => d.ShipperId == oldShipper.ShipperId).ToList();
                    foreach (var item in oldDistricts)
                    {
                        item.ShipperId = null;
                    }
                    _context.SaveChanges();

                    foreach (var districtID in shipper.Districts)
                    {
                        var district = _context.Districts.FirstOrDefault(d => d.DistrictId == districtID);
                        district.ShipperId = (int) shipper.ShipperId ;
                    }

                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public void DeleteShipperAccount(int id)
        {
            var shipper = _context.Shippers.FirstOrDefault(s => s.ShipperId == id);
            shipper.IsDelete = true;
            var accountShipper = _context.Accounts.FirstOrDefault(s => s.AccountId == shipper.AccountId);
            accountShipper.IsDelete = true;

            _context.SaveChanges();
        }
    }

}
