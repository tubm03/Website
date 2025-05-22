using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Service
{
    public interface IServiceRepository
    {
        public List<ServiceViewModel> GetListServices();

        public List<int> GetAllServiceId();

        public ServiceDetailViewModel GetServiceDetail(int serviceId);

        public List<ServiceOptionViewModel> GetServiceOptions(int serviceId);

        public ServiceOptionViewModel GetFistServiceOption(int serviceId);

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetType(int serviceId, string petType);

        public List<ServiceViewModel> GetListServicesForUpdate(int orderServiceId);

        public ServiceOptionViewModel GetFistServiceOptionForUpdate(int serviceId, int orderServiceId);

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetTypeForUpdate(int serviceId, string petType, int orderServiceId);

        public ServiceOptionViewModel GetFistServiceOptionForAdmin(int serviceId);

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetTypeForAdmin(int serviceId, string petType);

        public ServiceOptionViewModel GetNewServiceOptionBySelectWeight(int serviceId, string petType, string weight);

        public List<string> GetAllWeightOfPet();

        public BookServiceViewModel GetBookingServiceInFo(int serviceOptionId);

        public List<WorkingTime> GetAllWorkingTime();

        public List<int> GetWorkingTimeId(int serviceId);

        public List<TimeOnly> GetWorkingTime(int serviceId);

        public List<TimeOnly> GetWorkingTimeByDate(string date);

        public List<TimeOnly> GetWorkingTimeByDateForUpdate(string date, TimeOnly orderTime);

        public List<string> GetListServiceTypes();

        public List<ServiceViewModel> GetOtherServices(int serviceId);

        public List<BookServiceViewModel> GetOrderedServicesOfCustomer(int customerId);

        public BookServiceViewModel GetOrderServiceDetail(int orderServiceId);

        public void AddOrderService(BookServiceViewModel bookServiceInfo);

        public void UpdateOrderService(BookServiceViewModel orderService);

        public void DeleteOrderService(int orderServiceId);

        public Task AddNewService(ServiceAdditionViewModel serviceAddition);

        public Task AddWorkingTime(ServiceAdditionViewModel serviceAddition, int serviceId);

        public Task AddServiceOption(ServiceAdditionViewModel serviceAddition, int serviceId);

        public Task AddImageService(ServiceAdditionViewModel serviceAddition, int serviceId);

        public Task UpdateService(ServiceAdditionViewModel serviceAddition);

        public Task UpdateServiceOptions(ServiceAdditionViewModel serviceAddition);

        public void DeleteService(int serviceId);

        public void UpdateStatusOrderService(int orderServiceId, string status, int employeeId);

        public List<BookServiceViewModel> GetOrderedServicesByConditions(OrderedServiceViewModel orderServiceVM,
             int pageIndex, int pageSize);

        public int GetTotalCountOrderedServicesByConditions(OrderedServiceViewModel orderServiceVM);

        public List<ServiceTableViewModel> GetListServiceByConditions(ServiceFilterViewModel serviceFilterVM,
            int pageIndex, int pageSize);

        public int GetTotalCountListServicesByConditions(ServiceFilterViewModel serviceFilterVM);

        public float GetTotalServiceSale();

        public List<ServiceTableViewModel> GetTopSellingService(string startDate, string endDate);

        public List<float> GetServiceSaleOfMonth(int month, int year);
    }
}
