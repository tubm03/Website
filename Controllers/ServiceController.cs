using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Filters;
using PetStoreProject.Helpers;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.FeedBack;
using PetStoreProject.Repositories.Service;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _service;
        private readonly ICustomerRepository _customer;
        private readonly IAccountRepository _account;
        private readonly IFeedbackRepository _feedback;
        public ServiceController(IServiceRepository service, ICustomerRepository customer, IAccountRepository account, IFeedbackRepository feedback)
        {
            _service = service;
            _customer = customer;
            _account = account;
            _feedback = feedback;
        }

        public IActionResult List()
        {
            ViewData["ServiceTypes"] = _service.GetListServiceTypes();
            var services = _service.GetListServices();
            return View(services);
        }

        [HttpGet("/Service/Detail/{serviceId}")]
        public IActionResult Detail(int serviceId)
        {
            ViewData["ServiceDetail"] = _service.GetServiceDetail(serviceId);
            ViewData["FirstServiceOption"] = _service.GetFistServiceOption(serviceId);
            ViewData["OtherServices"] = _service.GetOtherServices(serviceId);
            ViewData["listFeedback"] = _feedback.GetListFeedBackForService(serviceId);
            return View();
        }

        [HttpPost]
        public ServiceOptionViewModel GetOptionViewModel(int serviceId, string petType)
        {
            return _service.GetFirstServiceAndListWeightOfPetType(serviceId, petType);
        }

        [HttpPost]
        public ServiceOptionViewModel GetServiceOptionPrice(int serviceId, string petType, string weight)
        {
            return _service.GetNewServiceOptionBySelectWeight(serviceId, petType, weight);
        }

        [HttpGet]
        public ActionResult BookService(int serviceOptionId)
        {
            var bookingService = _service.GetBookingServiceInFo(serviceOptionId);
            var userEmail = HttpContext.Session.GetString("userEmail");
            if (userEmail != null)
            {
                var userRole = _account.GetUserRoles(userEmail);
                if (userRole != "Customer")
                {
                    return new ContentResult
                    {
                        Content = "Access Denied",
                        StatusCode = 403,
                    };
                }
                var userInfo = _customer.GetCustomer(userEmail);
                bookingService.CustomerId = userInfo.CustomerId;
                bookingService.Name = userInfo.FullName;
                bookingService.Phone = userInfo?.Phone;
            }
            ViewData["WorkingTime"] = _service.GetWorkingTime(bookingService.ServiceId);
            return View(bookingService);
        }

        [HttpPost]
        public ActionResult BookService(BookServiceViewModel bookServiceInfo)
        {
            if (bookServiceInfo.OrderDate != null)
            {
                ViewData["WorkingTime"] = _service.GetWorkingTimeByDate(bookServiceInfo.OrderDate);
            }
            else {
                ViewData["WorkingTime"] = _service.GetWorkingTime(bookServiceInfo.ServiceId);
            }

            if (ModelState.IsValid)
            {
                bool isPhoneValid = PhoneNumber.isValid(bookServiceInfo.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View(bookServiceInfo);
                }

                _service.AddOrderService(bookServiceInfo);
                ViewData["WorkingTime"] = _service.GetWorkingTimeByDate(bookServiceInfo.OrderDate);
                ViewData["BookSuccess"] = "Cửa hàng ANIMART đã nhận được đặt hẹn của bạn và sẽ sớm liên hệ với bạn để xác nhận. Cảm ơn bạn đã tin tưởng và đặt lịch dịch vụ của chúng tôi!";
                return View(bookServiceInfo);
            }
            else
            {
                return View(bookServiceInfo);
            }
        }

        [HttpGet]
        public List<TimeOnly> GetWorkingTimeByDate(string date)
        {
            return _service.GetWorkingTimeByDate(date);
        }
    }
}
