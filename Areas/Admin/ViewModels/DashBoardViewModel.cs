using PetStoreProject.ViewModels;

namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class DashBoardViewModel
    {
        public float TotalProductSale { get; set; }

        public float TotalServiceSale { get; set; }
        
        public float TotalSale { get; set; }

        public float TotalCustomers { get; set; }

        public List<ProductViewForAdmin> Products { get; set; }

        public List<ServiceTableViewModel> Services { get; set; }
    }
}
