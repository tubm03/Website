using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.OrderService
{
    public class OrderServiceRepository : IOrderServiceRepository
    {
        private readonly PetStoreDBContext _context;

        public OrderServiceRepository(PetStoreDBContext dBContext)
        {
            _context = dBContext;
        }
        public List<OrderServicesDetailViewModel> GetOrderServiceDetailByUserId(int userId)
        {
            var orderServices = (from os in _context.OrderServices
                                 join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                                 where os.CustomerId == userId
                                 select new OrderServicesDetailViewModel
                                 {
                                     OrderServiceId = os.OrderServiceId,
                                     CustomerId = userId,
                                     Name = os.Name,
                                     Phone = os.Phone,
                                     OrderDate = os.OrderDate,
                                     OrderTime = os.OrderTime,
                                     Message = os.Message,
                                     Status = os.Status,
                                     IsDeleted = os.IsDelete,
                                     employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == os.EmployeeId),
                                     Price = so.Price,
                                     ServiceId = so.ServiceId
                                 }).ToList();
            return orderServices;
        }

        public List<OrderServicesDetailViewModel> GetOrderServicesByCondition(OrderServiceModel orderServiceModel)
        {
            var orderServices = GetOrderServiceDetailByUserId(orderServiceModel.UserId);

            if (int.TryParse(orderServiceModel.SearchOrderServiceId, out int searchOrderServiceId))
            {
                orderServices = orderServices.Where(o => o.OrderServiceId == searchOrderServiceId).ToList();
            }

            if (orderServiceModel.SearchName != null)
            {
                orderServices = orderServices.Where(o => o.Name.ToLower().Contains(orderServiceModel.SearchName.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(orderServiceModel.SearchDate))
            {
                if (DateOnly.TryParse(orderServiceModel.SearchDate, out DateOnly dateService))
                {
                    orderServices = orderServices.Where(o => o.OrderDate == dateService).ToList();
                }
            }

            var seTime = orderServiceModel.SearchTime;
            if (seTime != null)
            {
                int timeHour = int.Parse(seTime);
                if (timeHour > 0 && timeHour <= 24)
                {
                    orderServices = orderServices.Where(o => o.OrderTime.HasValue && o.OrderTime.Value.Hour == timeHour).ToList();
                }
            }

            if (orderServiceModel.Status != null)
            {
                orderServices = orderServices.Where(o => o.Status.Contains(orderServiceModel.Status)).ToList();
            }

            if (orderServiceModel.SortOrderServiceId == "abc")
                orderServices = orderServices.OrderBy(o => o.OrderServiceId).ToList();
            else if (orderServiceModel.SortOrderServiceId == "zxy")
                orderServices = orderServices.OrderByDescending(o => o.OrderServiceId).ToList();

            if (orderServiceModel.SortName == "abc")
                orderServices = orderServices.OrderBy(o => o.Name).ToList();
            else if (orderServiceModel.SortName == "zxy")
                orderServices = orderServices.OrderByDescending(o => o.Name).ToList();

            if (orderServiceModel.SortDate == "abc")
                orderServices = orderServices.OrderBy(o => o.OrderDate).ToList();
            else if (orderServiceModel.SortDate == "zxy")
                orderServices = orderServices.OrderByDescending(o => o.OrderDate).ToList();

            if (orderServiceModel.SortTime == "abc")
                orderServices = orderServices.OrderBy(o => o.OrderTime).ToList();
            else if (orderServiceModel.SortTime == "zxy")
                orderServices = orderServices.OrderByDescending(o => o.OrderTime).ToList();

            if (orderServiceModel.SortServiceId == "abc")
                orderServices = orderServices.OrderBy(o => o.ServiceId).ToList();
            else if (orderServiceModel.SortServiceId == "zxy")
                orderServices = orderServices.OrderByDescending(o => o.ServiceId).ToList();

            if (orderServiceModel.SortPrice == "abc")
                orderServices = orderServices.OrderBy(o => o.Price).ToList();
            else if (orderServiceModel.SortPrice == "zxy")
                orderServices = orderServices.OrderByDescending(o => o.Price).ToList();

            orderServices = orderServices.Skip((orderServiceModel.PageIndex - 1) * orderServiceModel.PageSize).Take(orderServiceModel.PageSize).ToList();

            return orderServices;
        }

        public int GetCountOrderService(int customerId)
        {
            int count = GetOrderServiceDetailByUserId(customerId).Count;

            return count;
        }
    }
}
