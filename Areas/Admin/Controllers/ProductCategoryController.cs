using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.ProductCategory;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IBrandRepository _brand;
        private readonly ICategoryRepository _category;

        public ProductCategoryController(IProductCategoryRepository productCategoryRepository,
            IBrandRepository brand, ICategoryRepository category)
        {
            _productCategoryRepository = productCategoryRepository;
            _brand = brand;
            _category = category;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var productCategories = _productCategoryRepository.GetListProductCategory();
            ViewData["ProductCategories"] = productCategories;
            var brands = await _brand.GetListBrand();
            ViewData["brands"] = brands;
            var categories = _category.GetCategories();
            ViewData["categories"] = categories;
            return View();
        }
        [HttpPost]
        public IActionResult ListCate(int? page, int pageSize = 8)
        {
            int PageIndex = page ?? 1;  
            int PageSize = pageSize;    
            var allCategories = _productCategoryRepository.GetListProductCategory();
            int totalRow = allCategories.Count();  
            var startPage = (PageIndex - 1) * PageSize;
            int numberPage = (int)Math.Ceiling((float)totalRow / PageSize);  

            var data = allCategories.Skip(startPage).Take(PageSize).ToList();


            return new JsonResult(new
            {
                data = allCategories,
                TotalItems = totalRow,
                currentPage = PageIndex,
                PageSize = PageSize,
            });
        }


        [HttpPost]
        public JsonResult Create(string ProductCategoryName, int CategoryId)
        {
            var productCateId = _productCategoryRepository.CreateProductCategory(ProductCategoryName, CategoryId);
            return Json(new { productCateId = productCateId });

        }
        


        [HttpDelete]
        public JsonResult Delete(int ProductCategoryId)
        {
            var result = _productCategoryRepository.DeleteProductCategory(ProductCategoryId);
            return Json(new { result = result });
        }

        [HttpPut]
        public JsonResult Update(string ProductCategoryName, int ProductCategoryId, bool IsDelete, int CategoryId)
        {
            var result = _productCategoryRepository.UpdateProductCategory(ProductCategoryId, ProductCategoryName, CategoryId, IsDelete);
            return Json(new { result = result });
        }
    }
}
