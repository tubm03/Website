using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using X.PagedList;

namespace PetStoreProject.Repositories.Discount
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly PetStoreDBContext _context;

        public DiscountRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public string Create(Models.Discount discount)
        {
            var duplicate = _context.Discounts.Any(d => d.Code == discount.Code && !((d.StartDate > discount.EndDate) || (d.EndDate < discount.StartDate)));
            if (duplicate)
            {
                return "Fail";
            }
            else
            {
                var id = 0;
                try
                {
                    id = _context.Discounts.Max(d => d.DiscountId);
                    id = id + 1;
                }
                catch (System.InvalidOperationException)
                {
                    id = 1;
                }
                var dis = new Models.Discount
                {
                    DiscountId = id,
                    Code = discount.Code,
                    StartDate = discount.StartDate,
                    EndDate = discount.EndDate,
                    DiscountTypeId = discount.DiscountTypeId,
                    Value = discount.Value,
                    CreatedAt = DateTime.Now.ToString(),
                    Description = discount.Description,
                    MaxValue = discount.MaxValue,
                    MinPurchase = discount.MinPurchase,
                    Quantity = discount.Quantity,
                    MaxUse = discount.MaxUse,
                    Used = 0,
                    Status = true,
                    LevelID = discount.DiscountTypeId != 4 ? 0 : discount.LevelID
                };
                _context.Discounts.Add(dis);
                _context.SaveChanges();
                return "Success";
            }
        }

        public string Edit(Models.Discount discount)
        {
            var d = _context.Discounts.Find(discount.DiscountId);
            d.Status = false;
            _context.Discounts.Update(d);
            var id = _context.Discounts.Max(d => d.DiscountId) + 1;
            discount.DiscountId = id;
            discount.CreatedAt = DateTime.Now.ToString();
            discount.Status = true;
            discount.Used = d.Used;
            discount.LevelID = d.LevelID;
            _context.Discounts.Add(discount);
            _context.SaveChanges();
            return discount.Code;
        }

        public DiscountViewModel GetDiscount(int id)
        {
            var discount = (from d in _context.Discounts
                            join dt in _context.DiscountTypes on d.DiscountTypeId equals dt.DiscountTypeId
                            join l in _context.LoyaltyLevels on d.LevelID equals l.LevelID
                            where d.DiscountId == id
                            select new DiscountViewModel
                            {
                                Id = id,
                                Code = d.Code,
                                StartDate = d.StartDate,
                                EndDate = d.EndDate,
                                CreatedAt = d.CreatedAt,
                                DiscountType = new DiscountTypeViewModel
                                {
                                    Id = dt.DiscountTypeId,
                                    Name = dt.DiscountName
                                },
                                Value = d.Value,
                                MaxValue = d.MaxValue,
                                MinPurchase = d.MinPurchase,
                                Quantity = d.Quantity,
                                MaxUse = d.MaxUse,
                                Used = d.Used,
                                Description = d.Description,
                                Loyal = new LoyaltyLevel
                                {
                                    LevelID = (int)d.LevelID,
                                    LevelName = l.LevelName,
                                }
                            }).FirstOrDefault();
            return discount;
        }

        public IPagedList<DiscountViewModel> GetDiscounts(int page, int pageSize)
        {
            var discounts = (from d in _context.Discounts
                             join dt in _context.DiscountTypes on d.DiscountTypeId equals dt.DiscountTypeId
                             orderby d.DiscountId descending
                             select new DiscountViewModel
                             {
                                 Id = d.DiscountId,
                                 Code = d.Code,
                                 StartDate = d.StartDate,
                                 EndDate = d.EndDate,
                                 CreatedAt = d.CreatedAt,
                                 DiscountType = new DiscountTypeViewModel
                                 {
                                     Id = dt.DiscountTypeId,
                                     Name = dt.DiscountName
                                 },
                                 Value = d.Value,
                                 MaxValue = d.MaxValue,
                                 MinPurchase = d.MinPurchase,
                                 Quantity = d.Quantity,
                                 MaxUse = d.MaxUse,
                                 Used = d.Used,
                                 Status = d.Status
                             }).ToList();

            var now = DateOnly.FromDateTime(DateTime.Now);
            foreach (var item in discounts)
            {
                if (item.Status == true)
                {
                    if (item.DiscountType.Id == 3)
                    {
                        item.Status = true;
                        item.StatusString = "Đang diễn ra";
                    }
                    else if (item.StartDate <= now && now <= item.EndDate && item.Status == true)
                    {
                        if (item.Quantity < item.Used)
                        {
                            item.Status = false;
                            item.StatusString = "Hết lượt";
                        }
                        else
                        {
                            item.Status = true;
                            item.StatusString = "Đang diễn ra";

                        }
                    }
                    else
                    {
                        if (item.StartDate > now)
                        {
                            item.Status = true;
                            item.StatusString = "Chưa bắt đầu";
                        }
                        else
                        {
                            item.Status = false;
                            item.StatusString = "Đã kết thúc";
                        }
                    }
                }
                else
                {
                    item.StatusString = "Đã kết thúc";
                }

            }
            return discounts.ToPagedList(page, pageSize);
        }



        public List<DiscountViewModel> GetDiscounts(double total_amount, string email)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            var discountsQuery = from d in _context.Discounts
                                 join dt in _context.DiscountTypes on d.DiscountTypeId equals dt.DiscountTypeId
                                 where d.EndDate >= now && d.Status == true && d.StartDate <= now && d.LevelID == 0
                                 select new DiscountViewModel
                                 {
                                     Id = d.DiscountId,
                                     Code = d.Code,
                                     StartDate = d.StartDate,
                                     EndDate = d.EndDate,
                                     CreatedAt = d.CreatedAt,
                                     DiscountType = new DiscountTypeViewModel
                                     {
                                         Id = dt.DiscountTypeId,
                                         Name = dt.DiscountName
                                     },
                                     Value = d.Value,
                                     MaxValue = d.MaxValue,
                                     MinPurchase = d.MinPurchase,
                                     Quantity = d.Quantity,
                                     MaxUse = d.MaxUse,
                                     Used = d.Used,
                                     Status = d.Status
                                 };

            bool isFirstOrder = true;
            if (string.IsNullOrEmpty(email))
            {
                discountsQuery = discountsQuery.Where(d => d.DiscountType.Id != 3);
                isFirstOrder = false;
            }
            else
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
                if (customer != null)
                {
                    try
                    {
                        isFirstOrder = !_context.Orders.Any(o => o.CustomerId == customer.CustomerId);
                    }
                    catch
                    {
                        isFirstOrder = true;
                    }

                }
            }

            var discounts = discountsQuery.ToList();
            foreach (var item in discounts)
            {
                var number_used = (from o in _context.Orders
                                   join d in _context.Discounts on o.DiscountId equals d.DiscountId
                                   where d.Code == item.Code
                                   select o).Count();
                if (item.DiscountType.Id == 3 && !isFirstOrder)
                {
                    item.Status = false;
                    item.StatusString = "Chỉ áp dụng cho đơn hàng đầu tiên";
                }
                else if (number_used >= item.MaxUse)
                {
                    item.Status = false;
                    item.StatusString = "Lượt sử dụng mã giảm giá của bạn đã hết";
                }
                else if (item.Used >= item.Quantity)
                {
                    item.Status = false;
                    item.StatusString = "Đã hết lượt sử dụng";
                }
                else if ((double)item.MinPurchase > total_amount)
                {
                    item.Status = false;
                    item.StatusString = "Mua thêm " + ((double)item.MinPurchase - total_amount).ToString("#,###.###") + " VND sản phẩm để sử dụng";
                }
                else
                {
                    item.Status = true;
                    if (item.DiscountType.Id == 2)
                    {
                        item.Reduce = item.Value;
                    }
                    else
                    {
                        item.Reduce = item.Value / 100 * (decimal)total_amount > item.MaxValue ? item.MaxValue : item.Value / 100 * (decimal)total_amount;
                    }
                    item.Title = "-" + ((decimal)item.Reduce).ToString("#,###") + " VND";
                }
            }
            return discounts;
        }

        public void DeleteDiscount(int id)
        {
            var discount = _context.Discounts.Find(id);
            discount.Status = false;
            _context.SaveChanges();
        }

        public float GetDiscountPrice(double total_amount, int discountId)
        {
            var item = _context.Discounts.Find(discountId);
            decimal? reduce = 0;
            if (item.DiscountTypeId == 2)
            {
                reduce = item.Value;
            }
            else
            {
                reduce = item.Value / 100 * (decimal)total_amount > item.MaxValue ? item.MaxValue : item.Value / 100 * (decimal)total_amount;
            }
            return (float)reduce;
        }

        public List<DiscountViewModel> GetOwnDiscount(double total_amount, int customerId)
        {
            if (customerId != -1)
            {
                var now = DateOnly.FromDateTime(DateTime.Now);
                var discountsQuery = from d in _context.Discounts
                                     join dt in _context.DiscountTypes on d.DiscountTypeId equals dt.DiscountTypeId
                                     join c in _context.Customers on d.LevelID equals c.LoyaltyLevelID
                                     where d.EndDate >= now && d.Status == true && d.StartDate <= now && d.LevelID == c.LoyaltyLevelID && c.CustomerId == customerId
                                     select new DiscountViewModel
                                     {
                                         Id = d.DiscountId,
                                         Code = d.Code,
                                         StartDate = d.StartDate,
                                         EndDate = d.EndDate,
                                         CreatedAt = d.CreatedAt,
                                         DiscountType = new DiscountTypeViewModel
                                         {
                                             Id = dt.DiscountTypeId,
                                             Name = dt.DiscountName
                                         },
                                         Value = d.Value,
                                         MaxValue = d.MaxValue,
                                         MinPurchase = d.MinPurchase,
                                         Quantity = d.Quantity,
                                         MaxUse = d.MaxUse,
                                         Used = d.Used,
                                         Status = d.Status
                                     };

                var discounts = discountsQuery.ToList();
                foreach (var item in discounts)
                {
                    var number_used = (from o in _context.Orders
                                       join d in _context.Discounts on o.OwnDiscountId equals d.DiscountId
                                       where d.Code == item.Code
                                       select o).Count();

                    if (number_used >= item.MaxUse)
                    {
                        item.Status = false;
                        item.StatusString = "Lượt sử dụng mã giảm giá của bạn đã hết";
                    }
                    else if ((double)item.MinPurchase > total_amount)
                    {
                        item.Status = false;
                        item.StatusString = "Mua thêm " + ((double)item.MinPurchase - total_amount).ToString("#,###.###") + " VND sản phẩm để sử dụng";
                    }
                    else
                    {
                        item.Status = true;
                        if (item.DiscountType.Id == 2)
                        {
                            item.Reduce = item.Value;
                        }
                        else
                        {
                            item.Reduce = item.Value / 100 * (decimal)total_amount > item.MaxValue ? item.MaxValue : item.Value / 100 * (decimal)total_amount;
                        }
                        item.Title = "-" + ((decimal)item.Reduce).ToString("#,###") + " VND";
                    }
                }
                return discounts;
            }
            return new List<DiscountViewModel>();
        }
    }

}
