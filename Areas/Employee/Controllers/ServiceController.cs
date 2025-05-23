using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Repositories.Service;
using PetStoreProject.ViewModels;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Employee;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _service;
        private readonly IEmployeeRepository _employee;

        public ServiceController(IServiceRepository service, IEmployeeRepository employee)
        {
            _service = service;
            _employee = employee;
        }

        [HttpGet]
        public IActionResult ListService()
        {
            var serviceFilterVM = new ServiceFilterViewModel();
            ViewData["ListServiceType"] = _service.GetListServiceTypes();
            ViewData["ListService"] = _service.GetListServiceByConditions(serviceFilterVM, 1, 7);
            var totalService = _service.GetTotalCountListServicesByConditions(serviceFilterVM);
            ViewData["numberOfPage"] = (int)Math.Ceiling((double)totalService / 7);
            return View();
        }

        [HttpPost]
        public Object ListService(ServiceFilterViewModel serviceFilterVM, int pageIndex, int pageSize)
        {
            var listService = _service.GetListServiceByConditions(serviceFilterVM, pageIndex, pageSize);

            var totalService = _service.GetTotalCountListServicesByConditions(serviceFilterVM);

            var numberOfPage = (int)Math.Ceiling((double)totalService / 7);

            return Json(new
            {
                listService = listService,
                numberOfPage = numberOfPage
            });
        }

        [HttpGet("/Employee/Service/ServiceDetail/{serviceId}")]
        public IActionResult ServiceDetail(int serviceId)
        {
            ViewData["ListServiceId"] = _service.GetAllServiceId();
            ViewData["ServiceDetail"] = _service.GetServiceDetail(serviceId);
            ViewData["FirstServiceOption"] = _service.GetFistServiceOptionForAdmin(serviceId);
            ViewData["ListServiceOption"] = _service.GetServiceOptions(serviceId);
            return View();
        }

        public IActionResult ListOrderService()
        {
            ViewData["Services"] = _service.GetListServices();
            var orderServicePending = new OrderedServiceViewModel() { Status = "Chưa xác nhận" };
            ViewData["listOrderedPending"] = _service.GetOrderedServicesByConditions(orderServicePending, 1, 5);
            var totalPendingItems = _service.GetTotalCountOrderedServicesByConditions(orderServicePending);
            ViewData["numberPageOfListPending"] = (int)Math.Ceiling((double)totalPendingItems / 5);

            var orderServiceConfirmed = new OrderedServiceViewModel() { Status = "Đã xác nhận" };
            ViewData["listOrderedConfirmed"] = _service.GetOrderedServicesByConditions(orderServiceConfirmed, 1, 5);
            var totalConfirmedItems = _service.GetTotalCountOrderedServicesByConditions(orderServiceConfirmed);
            ViewData["numberPageOfListConfirmed"] = (int)Math.Ceiling((double)totalConfirmedItems / 5);

            var orderServicePaid = new OrderedServiceViewModel() { Status = "Đã thanh toán" };
            ViewData["listOrderedPaid"] = _service.GetOrderedServicesByConditions(orderServicePaid, 1, 5);
            var totalPaidItems = _service.GetTotalCountOrderedServicesByConditions(orderServicePaid);
            ViewData["numberPageOfListPaid"] = (int)Math.Ceiling((double)totalPaidItems / 5);
            return View();
        }

        [HttpPost]
        public Object OrderedSeviceByConditions(OrderedServiceViewModel orderServiceVM, int pageIndex, int pageSize)
        {
            var orderedServices = _service.GetOrderedServicesByConditions(orderServiceVM, pageIndex, pageSize);

            var totalItems = _service.GetTotalCountOrderedServicesByConditions(orderServiceVM);

            var numberPage = (int)Math.Ceiling((double)totalItems / 5);

            return Json(new
            {
                orderedServices = orderedServices,
                numberPage = numberPage
            });
        }

        [HttpGet]
        public IActionResult OrderServiceDetail(int orderServiceId)
        {
            var orderService = _service.GetOrderServiceDetail(orderServiceId);
            ViewData["WorkingTime"] = _service.GetWorkingTimeByDateForUpdate(orderService.OrderDate, orderService.OrderTime);
            ViewData["Services"] = _service.GetListServicesForUpdate(orderServiceId);
            ViewData["PetTypes"] = _service.GetFistServiceOptionForUpdate(orderService.ServiceId, orderServiceId).PetTypes;
            ViewData["Weights"] = _service.GetFirstServiceAndListWeightOfPetTypeForUpdate(orderService.ServiceId, orderService.PetType, orderServiceId).Weights;
            return View(orderService);
        }

        [HttpPost]
        public ActionResult OrderServiceDetail(BookServiceViewModel orderServiceInfo)
        {
            var orderServiceInDB = _service.GetOrderServiceDetail((int)orderServiceInfo.OrderServiceId);
            if (orderServiceInfo.OrderDate == null)
            {
                ViewData["WorkingTime"] = _service.GetWorkingTime(orderServiceInfo.ServiceId);
            }
            else if (orderServiceInfo.OrderDate == orderServiceInDB.OrderDate)
            {
                ViewData["WorkingTime"] = _service.GetWorkingTimeByDateForUpdate(orderServiceInDB.OrderDate, orderServiceInDB.OrderTime);
            }
            else if (orderServiceInfo.OrderDate != orderServiceInDB.OrderDate)
            {
                ViewData["WorkingTime"] = _service.GetWorkingTimeByDate(orderServiceInfo.OrderDate);
            }

            ViewData["Services"] = _service.GetListServicesForUpdate((int)orderServiceInfo.OrderServiceId);
            ViewData["PetTypes"] = _service.GetFistServiceOptionForUpdate(orderServiceInfo.ServiceId, (int)orderServiceInfo.OrderServiceId).PetTypes;
            ViewData["Weights"] = _service.GetFirstServiceAndListWeightOfPetTypeForUpdate(orderServiceInfo.ServiceId, orderServiceInfo.PetType, (int)orderServiceInfo.OrderServiceId).Weights;
            if (ModelState.IsValid)
            {
                bool isPhoneValid = PhoneNumber.isValid(orderServiceInfo.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View(orderServiceInfo);
                }

                _service.UpdateOrderService(orderServiceInfo);
                ViewData["WorkingTime"] = _service.GetWorkingTimeByDateForUpdate(orderServiceInfo.OrderDate, orderServiceInfo.OrderTime);
                ViewData["UpdateSuccess"] = $"Thông tin lịch hẹn của " +
                    $"<strong style='font-size: 16px;'>{orderServiceInfo.Name}</strong> " +
                    $"đã được cập nhật thành công.";
                return View(orderServiceInfo);
            }
            else
            {
                return View(orderServiceInfo);
            }
        }

        [HttpGet]
        public IActionResult AddNewOrderService(string statusCreation)
        {
            var firstServiceOption = _service.GetFistServiceOption(1);
            ViewData["WorkingTime"] = _service.GetWorkingTime(1);
            ViewData["Services"] = _service.GetListServices();
            ViewData["PetTypes"] = firstServiceOption.PetTypes;
            ViewData["Weights"] = firstServiceOption.Weights;

            var orderService = new BookServiceViewModel();
            orderService.ServiceOptionId = firstServiceOption.ServiceOptionId;
            orderService.ServiceId = firstServiceOption.ServiceId;
            orderService.Price = firstServiceOption.Price.ToString("#,###") + " VND";

            ViewBag.Status = "PendingOrder";
            if (statusCreation == "Confirmed")
            {
                ViewBag.Status = "ConfirmedOrder";
                orderService.OrderDate = DateTime.Today.ToString("dd/MM/yyyy");
                orderService.OrderTime = roundOrderTime();
                orderService.Status = "Đã xác nhận";
            }

            return View(orderService);
        }

        [HttpPost]
        public IActionResult AddNewOrderService(BookServiceViewModel orderServiceInfo, string statusCreation)
        {
            ViewBag.Status = "PendingOrder";
            if (statusCreation == "Confirmed")
            {
                ViewBag.Status = "ConfirmedOrder";
            }

            if (orderServiceInfo.OrderDate != null)
            {
                ViewData["WorkingTime"] = _service.GetWorkingTimeByDate(orderServiceInfo.OrderDate);
            }
            else
            {
                ViewData["WorkingTime"] = _service.GetWorkingTime(orderServiceInfo.ServiceId);
            }

            ViewData["Services"] = _service.GetListServices();
            ViewData["PetTypes"] = _service.GetFistServiceOption(orderServiceInfo.ServiceId).PetTypes;
            ViewData["Weights"] = _service.GetFirstServiceAndListWeightOfPetType(orderServiceInfo.ServiceId, orderServiceInfo.PetType).Weights;
            if (ModelState.IsValid)
            {
                bool isPhoneValid = PhoneNumber.isValid(orderServiceInfo.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View(orderServiceInfo);
                }

                _service.AddOrderService(orderServiceInfo);
                ViewData["WorkingTime"] = _service.GetWorkingTimeByDate(orderServiceInfo.OrderDate);
                ViewData["CreateSuccess"] = $"Thông tin lịch hẹn của " +
                    $"<strong style='font-size: 16px;'>{orderServiceInfo.Name}</strong> " +
                    $"đã được tạo mới thành công.";
                return View(orderServiceInfo);
            }
            else
            {
                return View(orderServiceInfo);
            }
        }

        [HttpGet]
        public List<TimeOnly> GetWorkingTimeByDate(string date, int orderServiceId)
        {
            if(orderServiceId == 0)
            {
                return _service.GetWorkingTimeByDate(date);
            }
            var orderService = _service.GetOrderServiceDetail(orderServiceId);
            if (orderService.OrderDate == date)
            {
                return _service.GetWorkingTimeByDateForUpdate(date, orderService.OrderTime);
            }
            else
            {
                return _service.GetWorkingTimeByDate(date);
            }
        }

        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangeService(int serviceId)
        {
            return _service.GetFistServiceOption(serviceId);
        }

        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangePetType(int serviceId, string petType)
        {
            return _service.GetFirstServiceAndListWeightOfPetType(serviceId, petType);
        }

        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangeWeight(int serviceId, string petType, string weight)
        {
            return _service.GetNewServiceOptionBySelectWeight(serviceId, petType, weight);
        }

        [HttpGet]
        public BookServiceViewModel UpdateStatusOrderService(int orderServiceId, string status)
        {
            int employeeId = 0;
            if (status == "Đã thanh toán")
            {
                var email = HttpContext.Session.GetString("userEmail");
                employeeId = _employee.GetEmployee(email).EmployeeId;
            }
            _service.UpdateStatusOrderService(orderServiceId, status, employeeId);
            return _service.GetOrderServiceDetail(orderServiceId);
        }

        [HttpGet]
        public void CancelOrderService(int orderServiceId)
        {
            _service.DeleteOrderService(orderServiceId);
        }

        public TimeOnly roundOrderTime()
        {
            TimeOnly roundTime;
            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

            if (now.Minute == 0)
            {
                roundTime = now;
            }
            else if (now.Minute <= 30)
            {
                roundTime = new TimeOnly(now.Hour, 30);
            }
            else
            {
                roundTime = new TimeOnly(now.Hour + 1, 0);
            }
            return roundTime;
        }
    }
}
