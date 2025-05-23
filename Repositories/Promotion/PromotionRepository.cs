using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;
using X.PagedList;

namespace PetStoreProject.Repositories.Promotion
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly PetStoreDBContext _context;

        public PromotionRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public void CreatePromotion(PromotionCreateRequest promotion)
        {
            var id = 1;

            try
            {
                id = _context.Promotions.Max(x => x.PromotionId) + 1;
            }
            catch
            {
                id = 1;
            }

            var p = new Models.Promotion()
            {
                PromotionId = id,
                Name = promotion.Name,
                Value = promotion.Value,
                StartDate = promotion.StartDate.ToString(),
                EndDate = promotion.EndDate.ToString(),
                BrandId = promotion.BrandId,
                ProductCateId = promotion.ProductCateId,
                CreatedAt = DateTime.Now.ToString(),
                Status = true
            };
            _context.Promotions.Add(p);
            _context.SaveChanges();
        }

        public PromotionViewModel GetPromotion(int id)
        {
            var promotion = _context.Promotions
                                    .Where(p => p.PromotionId == id)
                                    .Select(p => new PromotionViewModel
                                    {
                                        Id = p.PromotionId,
                                        Name = p.Name,
                                        Value = p.Value,
                                        StartDate = p.StartDate,
                                        EndDate = p.EndDate,
                                        Brand = _context.Brands.FirstOrDefault(b => b.BrandId == p.BrandId) ?? new Models.Brand
                                        {
                                            BrandId = 0,
                                            Name = "Tất cả"
                                        },
                                        ProductCategory = _context.ProductCategories.FirstOrDefault(pc => pc.ProductCateId == p.ProductCateId) ?? new Models.ProductCategory
                                        {
                                            ProductCateId = 0,
                                            Name = "Tất cả"
                                        },
                                        CreatedAt = p.CreatedAt,
                                    })
                                    .FirstOrDefault();

            return promotion;
        }


        public IPagedList<PromotionViewModel> GetPromotions(int page, int pageSize)
        {
            var promotion = _context.Promotions.OrderByDescending(p => p.PromotionId).ToList();
            var promotionViewModel = new List<PromotionViewModel>();
            foreach (var item in promotion)
            {
                var now = DateTime.Now;
                var p = new PromotionViewModel()
                {
                    Id = item.PromotionId,
                    Name = item.Name,
                    Value = item.Value,
                    StartDate = item.StartDate.Trim(),
                    EndDate = item.EndDate.Trim(),
                    Brand = _context.Brands.Find(item.BrandId) == null ? (new Models.Brand()
                    {
                        BrandId = 0,
                        Name = "Tất cả"
                    }) : _context.Brands.Find(item.BrandId),
                    ProductCategory = _context.ProductCategories.Find(item.ProductCateId) == null ? (new Models.ProductCategory()
                    {
                        ProductCateId = 0,
                        Name = "Tất cả"
                    }) : _context.ProductCategories.Find(item.ProductCateId),
                    CreatedAt = item.CreatedAt,
                    Status = item.Status
                };

                if (DateTime.Parse(p.StartDate) <= now && now <= DateTime.Parse(p.EndDate) && p.Status == true)
                {
                    p.Status = true;
                    p.StatusString = "Đang diễn ra";
                }
                else if (DateTime.Parse(p.StartDate) > now && p.Status == true)
                {
                    p.Status = true;
                    p.StatusString = "Sắp diễn ra";
                }
                else
                {
                    p.Status = false;
                    p.StatusString = "Đã kết thúc";
                }
                promotionViewModel.Add(p);
            }
            return promotionViewModel.ToPagedList(page, pageSize);
        }

        public void UpdatePromotion(PromotionCreateRequest promotion)
        {
            var p = _context.Promotions.Find(promotion.Id);
            p.Status = false;
            _context.SaveChanges();
            CreatePromotion(promotion);
        }

        public List<int> GetBrandIdsByProductCateId(int productCateId)
        {
            List<int> listBrandIds = null;
            listBrandIds = _context.Promotions
                            .Where(p => p.ProductCateId == productCateId)
                            .Select(p => p.BrandId ?? 0).ToList();

            return listBrandIds;
        }

        public void DeletePromotion(int id)
        {
            var promotion = _context.Promotions.Find(id);
            promotion.Status = false;
            _context.SaveChanges();
        }
    }
}
