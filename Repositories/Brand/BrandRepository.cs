using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Brand
{
    public class BrandRepository : IBrandRepository
    {
        private readonly PetStoreDBContext _context;

        public BrandRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public int CreateBrand(string BrandName)
        {
            var brand = new Models.Brand
            {
                Name = BrandName,
                IsDelete = false
            };

            _context.Brands.Add(brand);
            _context.SaveChanges();
            return brand.BrandId;
        }

        public int DeleteBrand(int brandId)
        {
            var brand = _context.Brands.Find(brandId);
            if (brand != null)
            {
                brand.IsDelete = true;
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }

        public List<BrandViewModel> GetBrands()
        {
            var brands = _context.Brands.Where(b => b.IsDelete == false).ToList();

            List<BrandViewModel> brandViewModels = new List<BrandViewModel>();
            foreach (var brand in brands)
            {
                brandViewModels.Add(new BrandViewModel
                {
                    Id = brand.BrandId,
                    Name = brand.Name,
                });
            }
            return brandViewModels;
        }

        public async Task<List<BrandViewForAdmin>> GetListBrand()
        {
            var brandData = await (from b in _context.Brands
                                   join p in _context.Products on b.BrandId equals p.BrandId into bp
                                   from subp in bp.DefaultIfEmpty()
                                   join po in _context.ProductOptions on subp.ProductId equals po.ProductId into pop
                                   from subpo in pop.DefaultIfEmpty()
                                   join od in _context.OrderItems on subpo.ProductOptionId equals od.ProductOptionId into odo
                                   from subod in odo.DefaultIfEmpty()
                                   where b.IsDelete == false
                                   group new { b, subp, subod } by new { b.BrandId, b.Name } into g
                                   select new
                                   {
                                       BrandId = g.Key.BrandId,
                                       Name = g.Key.Name,
                                       Quantity = g.Count(x => x.subp != null),
                                       QuantityOfSold = g.Count(x => x.subod != null)
                                   }).ToListAsync();

            var brands = brandData.Select(x => new BrandViewForAdmin
            {
                Id = x.BrandId,
                Name = x.Name,
                Quantity = x.Quantity,
                QuantityOfSold = x.QuantityOfSold
            }).OrderBy(b => b.Name).ToList();

            return brands;
        }

        public string UpdateBrand(int brandId, string brandName)
        {
            var brand = _context.Brands.Find(brandId);
            if (brand == null)
            {
                return "Brand not found!";
            }
            else
            {
                brand.Name = brandName;
                _context.Entry(brand).State = EntityState.Modified;
                _context.SaveChanges();
                return "Success!";
            }
        }
    }
}
