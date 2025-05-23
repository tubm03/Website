using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Filters;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.Discount;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderItem;
using PetStoreProject.Repositories.ReturnRefund;
using PetStoreProject.Repositories.Service;
using PetStoreProject.ViewModels;
using System.Security.Claims;


namespace PetStoreProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;
        private readonly EmailService _emailService;
        private readonly ICustomerRepository _customer;
        private readonly IServiceRepository _service;
        private readonly IOrderRepository _order;
        private readonly IDiscountRepository _discount;
        private readonly IOrderItemRepository _orderItem;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IReturnRefundRepository _returnRefund;
        private readonly PetStoreDBContext _dbContext;

        public AccountController(IAccountRepository accountRepo, EmailService emailService, ICustomerRepository customer,
            IServiceRepository service, IOrderRepository order, IDiscountRepository discount, IOrderItemRepository orderItem,
            ICloudinaryService cloudinaryService, IReturnRefundRepository returnRefund, PetStoreDBContext dbContext)
        {
            _account = accountRepo;
            _emailService = emailService;
            _customer = customer;
            _service = service;
            _order = order;
            _discount = discount;
            _orderItem = orderItem;
            _cloudinaryService = cloudinaryService;
            _returnRefund = returnRefund;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Account account)
        {
            Account acc = _account.GetAccount(account.Email, account.Password);
            if (acc != null)
            {
                HttpContext.Session.SetString("userEmail", acc.Email);
                var role = _account.GetUserRoles(acc.Email);
                if (role == "Admin")

                {
                    var userName = _account.GetUserName(acc.Email, "Admin");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (role == "Employee")
                {
                    var userName = _account.GetUserName(acc.Email, "Employee");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("ListOrderService", "Service", new { area = "Employee" });
                }
                else if (role == "Shipper")
                {
                    var userName = _account.GetUserName(acc.Email, "Shipper");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("List", "Order", new { area = "Shipper" });
                }
                else
                {
                    var userName = _account.GetUserName(acc.Email, "Customer");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Mess"] = "Email hoặc mật khẩu không chính xác.";
                ViewBag.Email = account.Email;
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(ViewModels.RegisterViewModel registerInfor)
        {
            if (ModelState.IsValid)
            {
                bool isEmailExist = _account.CheckEmailExist(registerInfor.Email);
                bool isPhoneValid = PhoneNumber.isValid(registerInfor.Phone);
                if (isEmailExist)
                {
                    ViewBag.EmailMess = "Email này đã được liên kết với một tài khoản khác. " +
                        "Vui lòng đăng nhập hoặc sử dụng một email khác.";
                    return View();
                }
                else if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View();
                }
                else
                {
                    _account.AddNewCustomer(registerInfor);
                    HttpContext.Session.SetString("userEmail", registerInfor.Email);
                    HttpContext.Session.SetString("userName", registerInfor.FullName);
                    return RedirectToAction("Index", "Home", new { success = "True" });
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            bool isExistEmail = _account.CheckEmailExist(email);
            if (isExistEmail)
            {
                ViewBag.SuccessMess = "Yêu cầu đặt lại mật khẩu đã được gửi đến email của bạn. " +
                    "Nếu không nhận được email, vui lòng nhập lại địa chỉ email và gửi lại yêu cầu.";

                var resetLink = Url.Action("ResetPassword", "Account", new { userEmail = email }, Request.Scheme);

                var subject = "Đặt lại mật khẩu cho tài khoản khách hàng";

                var body = "<h2 style=\"font-weight:normal;font-size:24px;margin:0 0 10px\">Đặt lại mật khẩu</h2>" +
                    "<p style=\"color:#777;line-height:150%;font-size:16px;margin:0\">Đặt lại mật khẩu cho tài khoản khách hàng " +
                    "tại Animart Store. Nếu bạn không cần đặt mật khẩu mới, bạn có thể yên tâm xóa email này đi.</p> <br>" +
                    $"<p style=\"font-size:20px; margin:10px 0 0;\">Hãy bấm vào <a href=\"{resetLink}\">đặt lại mật khẩu</a></p>";

                _emailService.SendEmail(email, subject, body);
                return View();
            }
            else
            {
                ViewBag.ErrorMess = "Email không hợp lệ. Vui lòng nhập lại địa chỉ email.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string userEmail)
        {
            var ResetPasswordVM = new ResetPasswordViewModel { Email = userEmail };
            return View(ResetPasswordVM);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel ResetPasswordVM)
        {
            if (ModelState.IsValid)
            {
                _account.ResetPassword(ResetPasswordVM);
                HttpContext.Session.SetString("userEmail", ResetPasswordVM.Email);
                var role = _account.GetUserRoles(ResetPasswordVM.Email);
                if (role == "Admin")
                {
                    var userName = _account.GetUserName(ResetPasswordVM.Email, "Admin");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (role == "Employee")
                {
                    var userName = _account.GetUserName(ResetPasswordVM.Email, "Employee");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("ListOrderService", "Service", new { area = "Employee" });
                }
                else if (role == "Shipper")
                {
                    var userName = _account.GetUserName(ResetPasswordVM.Email, "Shipper");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("List", "Shipper", new { area = "Shipper" });
                }
                else
                {
                    var userName = _account.GetUserName(ResetPasswordVM.Email, "Customer");
                    HttpContext.Session.SetString("userName", userName);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View(ResetPasswordVM);
            }
        }

        public IActionResult LoginGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
                Items = { { "prompt", "select_account" } },
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public IActionResult GoogleFailure()
        {
            TempData["Mess"] = "Đăng nhập tài khoản Google không thành công. Vui lòng thử lại.";
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var userInfo = claims.ToDictionary(claim => claim.Type, claim => claim.Value);

                var fullName = userInfo[ClaimTypes.Name];
                var email = userInfo[ClaimTypes.Email];

                bool isEmailExist = _account.CheckEmailExist(email);
                if (isEmailExist)
                {
                    HttpContext.Session.SetString("userEmail", email);
                    var role = _account.GetUserRoles(email);
                    if (role == "Admin")
                    {
                        var userName = _account.GetUserName(email, "Admin");
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (role == "Employee")
                    {
                        var userName = _account.GetUserName(email, "Employee");
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("ListOrderService", "Service", new { area = "Employee" });
                    }
                    else if (role == "Shipper")
                    {
                        var userName = _account.GetUserName(email, "Shipper");
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("List", "Shipper", new { area = "Shipper" });
                    }
                    else
                    {
                        var userName = _account.GetUserName(email, "Customer");
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    var resgister = new ViewModels.RegisterViewModel { FullName = fullName, Email = email };
                    _account.AddNewCustomer(resgister);
                    HttpContext.Session.SetString("userEmail", email);
                    HttpContext.Session.SetString("userName", fullName);
                    return RedirectToAction("Index", "Home", new { success = "True" });
                }
            }

            TempData["Mess"] = "Đăng nhập tài khoản Google không thành công. Vui lòng thử lại.";
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied(string[] allowedRoles)
        {
            List<string> roles = allowedRoles.ToList();
            return View(roles);
        }


        [RoleAuthorize("Customer")]
        public IActionResult Profile()
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customer = _customer.GetCustomer(email);
            LoyaltyLevel l = _dbContext.LoyaltyLevels.FirstOrDefault(l => l.LevelID == customer.LoyaltyLevelID);
            if (l == null)
            {
                TempData["Rank"] = "---";
            }
            else
            {
                TempData["Rank"] = l.LevelName;
            }
            return View();
        }


        [RoleAuthorize("Customer")]
        [HttpGet]
        public IActionResult ProfileDetail()
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customer = _customer.GetCustomer(email);
            var customerVM = new CustomerViewModel
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Gender = customer.Gender,
                DoB = customer.DoB?.ToString("dd/MM/yyyy"),
                Address = customer.Address,
                Phone = customer.Phone,
                Email = customer.Email,
                AccountId = customer.AccountId,
            };
            return View(customerVM);
        }


        [RoleAuthorize("Customer")]
        [HttpPost]
        public IActionResult ProfileDetail(CustomerViewModel customer)
        {
            if (ModelState.IsValid)
            {
                var oldEmail = HttpContext.Session.GetString("userEmail");
                if (oldEmail != customer.Email)
                {
                    bool isEmailExist = _account.CheckEmailExist(customer.Email);
                    if (isEmailExist)
                    {
                        ViewBag.EmailMess = "Địa chỉ email này đã được liên kết với một tài khoản khác. Vui lòng nhập một email khác.";
                        return View(customer);
                    }
                }
                bool isPhoneValid = PhoneNumber.isValid(customer.Phone);
                if (isPhoneValid == false)
                {
                    ViewBag.PhoneMess = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                    return View(customer);
                }

                HttpContext.Session.SetString("userEmail", customer.Email);
                HttpContext.Session.SetString("userName", customer.FullName);
                _customer.UpdateProfile(customer);
                return View(customer);
            }
            else
            {
                return View(customer);
            }
        }


        [RoleAuthorize("Customer")]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var email = HttpContext.Session.GetString("userEmail");
            var ChangePasswordVM = new ChangePasswordViewModel { Email = email };
            string? oldPassword = _account.GetOldPassword(email);
            if (oldPassword != null)
            {
                ChangePasswordVM.OldPassword = oldPassword;
            }
            else
            {
                ChangePasswordVM.OldPassword = null;
            }
            return View(ChangePasswordVM);
        }


        [RoleAuthorize("Customer")]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel ChangePasswordVM)
        {
            if (ChangePasswordVM.OldPassword != null)
            {
                var passwordStored = _account.GetOldPassword(ChangePasswordVM.Email);
                bool isValid = BCrypt.Net.BCrypt.Verify(ChangePasswordVM.OldPassword, passwordStored);
                if (isValid == false)
                {
                    ViewBag.Message = "Mật khẩu cũ không chính xác. Vui lòng thử lại.";
                    return View(ChangePasswordVM);
                }
            }

            if (ModelState.IsValid)
            {
                _account.ChangePassword(ChangePasswordVM);
                return View(ChangePasswordVM);
            }
            else
            {
                return View(ChangePasswordVM);
            }
        }


        [RoleAuthorize("Customer")]
        [HttpGet]
        public ActionResult OrderServiceHistory()
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customer = _customer.GetCustomer(email);
            var orderedServices = _service.GetOrderedServicesOfCustomer(customer.CustomerId);
            return View(orderedServices);
        }

        [RoleAuthorize("Customer")]
        [HttpGet]
        public ActionResult OrderServiceDetail(int orderServiceId)
        {
            var orderService = _service.GetOrderServiceDetail(orderServiceId);
            ViewData["WorkingTime"] = _service.GetWorkingTimeByDateForUpdate(orderService.OrderDate, orderService.OrderTime);
            ViewData["Services"] = _service.GetListServicesForUpdate(orderServiceId);
            ViewData["PetTypes"] = _service.GetFistServiceOptionForUpdate(orderService.ServiceId, orderServiceId).PetTypes;
            ViewData["Weights"] = _service.GetFirstServiceAndListWeightOfPetTypeForUpdate(orderService.ServiceId, orderService.PetType, orderServiceId).Weights;
            return View(orderService);
        }

        [RoleAuthorize("Customer")]
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
                ViewData["UpdateSuccess"] = "Thông tin lịch hẹn của bạn đã được cập nhật thành công. Chúng tôi sẽ sớm liên hệ với bạn để xác nhận. Cảm ơn bạn đã tin tưởng và đặt lịch dịch vụ của chúng tôi!";
                return View(orderServiceInfo);
            }
            else
            {
                return View(orderServiceInfo);
            }
        }

        [RoleAuthorize("Customer")]
        [HttpGet]
        public List<TimeOnly> GetWorkingTimeByDate(string date, int orderServiceId)
        {
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
        public ServiceOptionViewModel GetServiceOptionByChangeService(int serviceId, int orderServiceId)
        {
            return _service.GetFistServiceOptionForUpdate(serviceId, orderServiceId);
        }

        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangePetType(int serviceId, string petType, int orderServiceId)
        {
            return _service.GetFirstServiceAndListWeightOfPetTypeForUpdate(serviceId, petType, orderServiceId);
        }

        [HttpGet]
        public ServiceOptionViewModel GetServiceOptionByChangeWeight(int serviceId, string petType, string weight)
        {
            return _service.GetNewServiceOptionBySelectWeight(serviceId, petType, weight);
        }

        [RoleAuthorize("Customer")]
        [HttpGet]
        public void CancelOrderService(int orderServiceId)
        {
            _service.DeleteOrderService(orderServiceId);
        }

        [RoleAuthorize("Customer")]
        [HttpPost]
        public IActionResult OrderHistory(OrderModel orderCondition)
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customer = _customer.GetCustomer(email);

            orderCondition.UserId = customer.CustomerId;
            orderCondition.pageSize = 0;

            var listOrderHistory = _order.GetOrderDetailByCondition(orderCondition);
            foreach (var order in listOrderHistory)
            {
                if (order.DiscountId != null && order.DiscountId != 0)
                {
                    var priceReduce = _discount.GetDiscountPrice(order.TotalAmount, order.DiscountId.Value);
                    order.TotalAmount = order.TotalAmount - priceReduce;
                }
                if (order.OwnDiscountId != null && order.OwnDiscountId != 0)
                {
                    var priceReduce = _discount.GetDiscountPrice(order.TotalAmount, order.OwnDiscountId.Value);
                    order.TotalAmount = order.TotalAmount - priceReduce;
                }

                order.TotalAmount += order.ShippingFee;
            }
            return View(listOrderHistory);
        }

        [RoleAuthorize("Customer")]
        [HttpPost]
        public IActionResult OrderHistoryDetail(string orderId)
        {
            long id = long.Parse(orderId);
            var order = _order.GetOrderDetailById(id);
            if (order.DiscountId.HasValue)
            {
                var discount = _discount.GetDiscount(order.DiscountId.Value);
                ViewBag.discount = discount;
            }
            else
            {
                ViewBag.discount = null;
            }
            var checkoutDetail = new CheckoutViewModel
            {
                OrderId = long.Parse(order.OrderId),
                OrderEmail = order.Email,
                OrderName = order.FullName,
                OrderPhone = order.Phone,
                ConsigneeAddressDetail = order.ShipAddress,
                ConsigneeName = order.ConsigneeName,
                ConsigneePhone = order.ConsigneePhone,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                ConsigneeWard = "",
                ConsigneeProvince = "",
                ConsigneeDistrict = "",
                OrderDate = order.OrderDate,
                ShippingFee = order.ShippingFee,
                Status = order.Status,
                ReturnId = order.ReturnId,
                OwnDiscountId = order.OwnDiscountId
            };
            var listItemOrder = _orderItem.GetOrderItemByOrderId(long.Parse(order.OrderId));


            var totalAmount = 0.0;

            foreach (var item in listItemOrder)
            {
                var price = item.Price * item.Quantity;
                if (item.Promotion != null)
                {
                    price = price * (1 - (float)item.Promotion.Value / 100);

                }
                totalAmount += price;
            }
            var priceDiscount = 0.0;
            var ownDiscount = 0.0;
            if (order.DiscountId.HasValue)
            {
                priceDiscount = _discount.GetDiscountPrice(totalAmount, order.DiscountId.Value);
            }

            if (order.OwnDiscountId.HasValue && order.OwnDiscountId.Value != 0)
            {
                ownDiscount = _discount.GetDiscountPrice(totalAmount - priceDiscount, order.OwnDiscountId.Value);
            }
            ViewBag.priceDiscount = priceDiscount + ownDiscount;
            ViewBag.listItemOrder = listItemOrder;

            return View(checkoutDetail);
        }

        [RoleAuthorize("Customer")]
        [HttpPost]
        public IActionResult UpdateStatusOrder(long orderId, string status)
        {
            _order.UpdateStatusOrder(orderId, status, 0);
            return Json(new { success = true });
        }

        [RoleAuthorize("Customer")]
        [HttpPost]
        public async Task<IActionResult> ReturnRefund(CreateReturnRefund returnRefund)
        {
            var email = HttpContext.Session.GetString("userEmail");
            var customer = _customer.GetCustomer(email);

            await _returnRefund.CreateNewReturnRefund(returnRefund);

            if (returnRefund.OrderId != 0)
            {
                var order = _order.GetOrderDetailById(returnRefund.OrderId);
                var status = "Trả hàng";
                _order.UpdateStatusOrder(returnRefund.OrderId, status, 0);
                var returnLast = _returnRefund.GetReturnRefunds().Last();
                _order.UpdateReturnOrder(returnRefund.OrderId, returnLast.ReturnId);
            }
            return Json(new { success = true });
        }
    }
}
