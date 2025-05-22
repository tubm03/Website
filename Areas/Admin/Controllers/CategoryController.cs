using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Category;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _category;
        private readonly IBrandRepository _brand;

        public CategoryController(ICategoryRepository category, IBrandRepository brand)
        {
            _category = category;
            _brand = brand;
        }

        [HttpGet]
        public IActionResult List()
        {
            var brands = _brand.GetListBrand();
            ViewData["brands"] = brands;
            return View();
        }
    }
}

