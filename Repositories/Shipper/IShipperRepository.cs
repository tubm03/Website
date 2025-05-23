using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Repositories.Shipper
{
    public interface IShipperRepository
    {
        public Models.Shipper GetShipperByDistricts(string districtName);

        public List<Models.Shipper> GetShippers();

        public List<ShipperViewModel> GetTotalAccountShippers(ShipperFilterViewModel shipperFilerVM);

        public List<ShipperViewModel> GetAccountShippers(ShipperFilterViewModel shipperFilerVM, int pageIndex, int pageSize);

        public Models.Shipper GetShipperByID(int? id);

        public bool CheckAssignDuplicatedDistrict(AccountViewModel shipper);

        public void AddNewShipper(AccountViewModel shipper);

        public void UpdateShipper(AccountViewModel shipper);

        public void DeleteShipperAccount(int id);
    }
}
