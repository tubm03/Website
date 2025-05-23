using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Repositories.Brand;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {

        private readonly IBrandRepository _brandRepository;

        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [HttpPost]
        public JsonResult Create(string brandName)
        {
            var brandId = _brandRepository.CreateBrand(brandName);
            return Json(new { brandId = brandId });
        }

        [HttpPut]
        public JsonResult Update(string brandName, int brandId)
        {
            var result = _brandRepository.UpdateBrand(brandId, brandName);
            return Json(new { result = result });
        }

        [HttpDelete]
        public JsonResult Delete(int brandId)
        {
            var result = _brandRepository.DeleteBrand(brandId);
            return Json(new { result = result });
        }
    }
}
