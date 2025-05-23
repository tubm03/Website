using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.ProductCategory
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly PetStoreDBContext _context;

        public ProductCategoryRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public int CreateProductCategory(string ProductCategoryName, int CategoryId)
        {
            var productCateId = _context.ProductCategories.Select(pc => pc.ProductCateId).Max() + 1;
            var productCategory = new Models.ProductCategory
            {
                ProductCateId = productCateId,
                Name = ProductCategoryName,
                CategoryId = CategoryId
            };
            _context.ProductCategories.Add(productCategory);
            _context.SaveChanges();
            return productCategory.ProductCateId;
        }

        public int DeleteProductCategory(int ProductCategoryId)
        {
            var productCategory = _context.ProductCategories.Find(ProductCategoryId);
            if (productCategory != null)
            {
                productCategory.IsDelete = true;
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }

        public List<ProductCategoryViewForAdmin> GetListProductCategory()
        {
            var ProductsCategories = (from pc in _context.ProductCategories
                                      join c in _context.Categories on pc.CategoryId equals c.CategoryId
                                      where pc.IsDelete == false
                                      select new ProductCategoryViewForAdmin()
                                      {
                                          Id = pc.ProductCateId,
                                          ProductCateName = pc.Name,
                                          CategoryName = c.Name,
                                          IsDelete = pc.IsDelete,
                                          CategoryId = pc.CategoryId
                                      }).ToList();

            foreach (var item in ProductsCategories)
            {
                var totalProducts = _context.Products.Where(p => p.ProductCateId == item.Id).Count();

                item.TotalProducts = totalProducts;

                var totalQuantitySold = (from pc in ProductsCategories
                                         join p in _context.Products on pc.Id equals p.ProductCateId
                                         join po in _context.ProductOptions on p.ProductId equals po.ProductId
                                         join od in _context.OrderItems on po.ProductOptionId equals od.ProductOptionId
                                         where pc.Id == item.Id
                                         select od.Quantity).Sum();

                item.QuantitySoldProduct = totalQuantitySold;
            }

            return ProductsCategories;
        }

        public List<ProductCategoryViewModel> GetProductCategories(int? categoryId, bool getDeleted)
        {
            var productCategories = _context.ProductCategories.Select(pc => new ProductCategoryViewModel
            {
                Id = pc.ProductCateId,
                Name = pc.Name.Trim(),
                CategoryId = pc.CategoryId,
                IsDelete = pc.IsDelete
            }).ToList();

            if (!getDeleted)
            {
                productCategories = productCategories.Where(pc => pc.IsDelete == false).ToList();
            }

            if (categoryId != null)
            {
                productCategories = productCategories.Where(pc => pc.CategoryId == categoryId).ToList();
            }
            return productCategories;

        }

        public int UpdateProductCategory(int ProductCategoryId, string ProductCategoryName, int CategoryId, bool isDelete)
        {
            var productCategory = _context.ProductCategories.Find(ProductCategoryId);
            if (productCategory != null)
            {
                productCategory.Name = ProductCategoryName;
                productCategory.CategoryId = CategoryId;
                productCategory.IsDelete = isDelete;
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }
    }
}
