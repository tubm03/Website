using PetStoreProject.Models;
using System.ComponentModel.DataAnnotations;
namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ProductCreateRequestViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public ProductCategory ProductCategory { get; set; }

        public Brand Brand { get; set; }

        [Required]
        public List<ProductOptionCreateRequestViewModel> ProductOptions { get; set; }
    }
}
