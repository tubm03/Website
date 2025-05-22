using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Repositories.Attribute;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductCategory;
using PetStoreProject.Repositories.Size;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _product;
        private readonly ICategoryRepository _category;
        private readonly IProductCategoryRepository _productCategory;
        private readonly IAttributeRepository _attribute;
        private readonly ISizeRepository _size;
        private readonly IBrandRepository _brand;

        public ProductController(IProductRepository product, ICategoryRepository category,
            IProductCategoryRepository productCategory,
            IBrandRepository brand, IAttributeRepository attribute, ISizeRepository size)
        {
            _product = product;
            _category = category;
            _productCategory = productCategory;
            _brand = brand;
            _attribute = attribute;
            _size = size;
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

            var totalPageNumber = Math.Ceiling((decimal)totalProduct / pageSize);

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
        public IActionResult Create()
        {
            var categories = _category.GetCategories();
            var productCategories = _productCategory.GetProductCategories(null, false);
            var attributes = _attribute.GetAttributes();
            var sizes = _size.GetSizes();
            var brands = _brand.GetBrands();
            ViewData["categories"] = categories;
            ViewData["productCategories"] = productCategories;
            ViewData["attributes"] = attributes;
            ViewData["sizes"] = sizes;
            ViewData["brands"] = brands;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateRequest(string product)
        {
            try
            {
                if (product == null)
                {
                    throw new Exception();
                }
                var productCreateRequest = JsonConvert.DeserializeObject<ProductCreateRequestViewModel>(product);

                var result = await _product.CreateProduct(productCreateRequest);

                return Json(new
                {
                    result = result.Trim()
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = "Không thể lưu sản phẩm."
                });
            }
        }

        [HttpGet]
        public IActionResult Detail(int productId)
        {
            var product = _product.GetProductDetailForAdmin(productId);
            if (product != null && product.ProductOptions != null)
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

        [HttpPost]
        public JsonResult GetDetail(int productId)
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
            return Json(new { product = product });
        }

        [HttpGet]
        public ActionResult Update(int productId)
        {
            var product = _product.GetProductDetailForAdmin(productId);
            var sizes = _size.GetSizes();
            HashSet<int> uniqueImages = new HashSet<int>();
            List<Models.Image> images = new List<Models.Image>();
            foreach (var option in product.ProductOptions)
            {
                if (option.Image != null && uniqueImages.Add(option.Image.ImageId))
                {
                    images.Add(new Models.Image
                    {
                        ImageId = option.Image.ImageId,
                        /*ImageUrl = await _cloudinary.GetBase64Image(option.Image.ImageUrl.Trim())*/
                        ImageUrl = option.Image.ImageUrl.Trim()
                    });
                }
            }
            ViewData["images"] = images;
            ViewData["sizes"] = sizes;
            ViewData["product"] = product;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UpdateRequest(string updateProductRequest)
        {

            try
            {
                var productUpdateRequest = JsonConvert.DeserializeObject<ProductDetailForAdmin>(updateProductRequest);
                var result = await _product.UpdateProduct(productUpdateRequest);
                return Json(new
                {
                    result = result
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = e.Message
                });
            }
        }

        [HttpPost]
        public JsonResult Delete(int productId)
        {
            _product.DeleteProduct(productId);
            return Json(new
            {
                result = "ok"
            });
        }

    }

}
