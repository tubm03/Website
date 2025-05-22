using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using X.PagedList;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoyalCustomerController : Controller
    {
        private readonly PetStoreDBContext _dbContext;

        public LoyalCustomerController(PetStoreDBContext petStoreDBContext)
        {
            _dbContext = petStoreDBContext;
        }
        public IActionResult Index(int? page, int? pageSize, string phone)
        {
            List<LoyalCustomerViewModel> listLC = GetListLoyalCustomers();

            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            if (phone != null)
            {
                listLC = listLC.Where(x => x.PhoneNumber == phone).ToList();
            }
            return View(listLC.ToPagedList((int)page, (int)pageSize));
        }

        public IActionResult Refresh()
        {
            var totalTransactions = _dbContext.Orders
                                    .Where(o => o.CustomerId != null)
                                    .GroupBy(o => o.CustomerId)
                                    .Select(g => new
                                    {
                                        CustomerId = g.Key,
                                        TotalAmount = g.Sum(o => o.TotalAmount)
                                    }).ToList();

            foreach (var transaction in totalTransactions)
            {
                Customer customer = _dbContext.Customers.FirstOrDefault(c => c.CustomerId == transaction.CustomerId);
                if (customer != null)
                {
                    customer.TotalAmountSpent = (decimal?)transaction.TotalAmount;
                    List<LoyaltyLevel> listL = _dbContext.LoyaltyLevels.ToList();
                    foreach (var level in listL)
                    {
                        if (level.MinTotalAmount < (decimal?)transaction.TotalAmount && level.MaxTotalAmount >= (decimal?)transaction.TotalAmount)
                        {
                            customer.LoyaltyLevelID = level.LevelID;
                        }
                    }
                }
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult UpdateStatus(int CustomerId)
        {
            var c = _dbContext.Customers.FirstOrDefault(c => c.CustomerId == CustomerId);
            if (c != null)
            {
                try
                {
                    c.isLoyalty = !c.isLoyalty;
                    _dbContext.SaveChanges();
                    return Ok(new
                    {
                        success = true,
                        message = "Cập nhật thành cônggggg"
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating status: {ex.Message}");
                }
            }
            return BadRequest(new
            {
                success = false,
                message = "Cập nhật không thành công"
            });
        }

        public List<LoyalCustomerViewModel> GetListLoyalCustomers() => (from c in _dbContext.Customers
                                                                        join l in _dbContext.LoyaltyLevels on c.LoyaltyLevelID equals l.LevelID
                                                                        select new LoyalCustomerViewModel
                                                                        {
                                                                            CustomerId = c.CustomerId,
                                                                            CustomerName = c.FullName,
                                                                            PhoneNumber = c.Phone,
                                                                            LoyalName = l.LevelName,
                                                                            IsActive = (bool)c.isLoyalty,
                                                                        }).ToList();
    }
}
