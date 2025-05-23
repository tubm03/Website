using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PetStoreDBContext _context;

        public CategoryRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public List<CategoryViewModel> GetCategories()
        {
            var categories = _context.Categories.ToList();

            List<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();
            foreach (var category in categories)
            {
                categoryViewModels.Add(new CategoryViewModel
                {
                    Id = category.CategoryId,
                    Name = category.Name
                });
            }
            return categoryViewModels;
        }

        public Dictionary<string, List<Models.ProductCategory>> GetAllCategory()
        {
            var cateDictionary = new Dictionary<string, List<Models.ProductCategory>>(); 

            var listDogFood = (from pc in _context.ProductCategories
                               join c in _context.Categories on pc.CategoryId equals c.CategoryId
                               join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                               where c.Name == "Thức ăn" || c.Name == "Thức ăn cho chó"
                               select pc).Distinct().ToList();

            var listDogAccessory = (from pc in _context.ProductCategories
                                    join c in _context.Categories on pc.CategoryId equals c.CategoryId
                                    join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                                    where c.Name == "Phụ kiện" || c.Name == "Phụ kiện cho chó"
                                    select pc).Distinct().ToList();

            var listCatFood = (from pc in _context.ProductCategories
                               join c in _context.Categories on pc.CategoryId equals c.CategoryId
                               join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                               where c.Name == "Thức ăn" || c.Name == "Thức ăn cho mèo"
                               select pc).Distinct().ToList();

            var listCatAccessory = (from pc in _context.ProductCategories
                                    join c in _context.Categories on pc.CategoryId equals c.CategoryId
                                    join p in _context.Products on pc.ProductCateId equals p.ProductCateId
                                    where c.Name == "Phụ kiện" || c.Name == "Phụ kiện cho mèo"
                                    select pc).Distinct().ToList();

            cateDictionary.Add("DogFood", listDogFood);
            cateDictionary.Add("DogAccessory", listDogAccessory);
            cateDictionary.Add("CatFood", listCatFood);
            cateDictionary.Add("CatAccessory", listCatAccessory);

            return cateDictionary;
        }
    }
}
