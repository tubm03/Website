using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductCategory;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _product;
        private readonly ICategoryRepository _category;
        private readonly IProductCategoryRepository _productCategory;

        public ProductController(IProductRepository product, ICategoryRepository category,
            IProductCategoryRepository productCategor)
        {
            _product = product;
            _category = category;
            _productCategory = productCategor;
        }

        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> fetchProduct(int? categoryId, int? productCateId, string? key,
            bool? sortPrice, bool? sortSoldQuantity, bool? isInStock, bool? isDelete,
            int pageNumber = 1, int pageSize = 10)
        {
            var categories = _category.GetCategories();
            categoryId = categoryId == 0 ? null : categoryId;

            var productCategories = _productCategory.GetProductCategories(categoryId, true);
            pageSize = Math.Min(pageSize, 30);

            ListProductForAdmin listProductForAdmin = await _product.productViewForAdmins(pageNumber, pageSize,
                                                                                        categoryId, productCateId, key);

            var products = listProductForAdmin.products;
            var totalProduct = listProductForAdmin.totalProducts;

            var totalPageNumber = totalProduct / pageSize + 1;

            return Json(new
            {
                products = products,
                pageNumber = pageNumber,
                pageSize = pageSize,
                totalPageNumber = totalPageNumber,
                categories = categories,
                productCategories = productCategories,
                key = key
            });
        }

        [HttpGet]
        public IActionResult Detail(int productId)
        {
            var product = _product.GetProductDetailForAdmin(productId);
            if (product != null && product.ProductOptions != null && product.ProductOptions[0].Attribute.AttributeId != 1)
            {
                var uniqueAttributes = new HashSet<int>();
                var attributes = new List<Models.Attribute>();

                foreach (var option in product.ProductOptions)
                {
                    if (option.Attribute != null && uniqueAttributes.Add(option.Attribute.AttributeId))
                    {
                        attributes.Add(option.Attribute);
                    }
                }
                ViewData["attributes"] = attributes;
            }
            ViewData["product"] = product;
            return View();
        }
    }
}
