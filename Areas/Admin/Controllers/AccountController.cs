using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Filters;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Admin;
using PetStoreProject.Repositories.District;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderService;
using PetStoreProject.Repositories.Shipper;
using PetStoreProject.ViewModels;
using System.Drawing.Printing;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _account;
        private readonly EmailService _emailService;
        private readonly IAdminRepository _admin;
        private readonly IOrderRepository _order;
        private readonly IOrderServiceRepository _orderService;
        private readonly IShipperRepository _shipper;
        private readonly IDistrictRepository _district;

        public AccountController(IAccountRepository account, EmailService emailService, IAdminRepository admin, IOrderRepository order, IOrderServiceRepository orderService, IShipperRepository shipper, IDistrictRepository district)
        {
            _account = account;
            _emailService = emailService;
            _admin = admin;
            _order = order;
            _orderService = orderService;
            _shipper = shipper;
            _district = district;
        }

        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public IActionResult List(int? pageIndex, int? pageSize, string? searchName, string? sortName, string? selectStatus)
        {
            var pageIndexLocal = pageIndex ?? 1;

            var pageSizeLocal = pageSize ?? 10;

            var accounts = _account.GetAccountEmployees(pageIndexLocal, pageSizeLocal, 2, searchName ?? "", sortName ?? "", selectStatus ?? "");

            var totalAccount = _account.GetAccountCount(2);

            var numberPage = (int)Math.Ceiling(totalAccount / (double)pageSizeLocal);

            return new JsonResult(new
            {
                userType = 2,
                accounts,
                totalAccount,
                currentPage = pageIndexLocal,
                pageSize = pageSizeLocal,
                numberPage
            });
        }

        [HttpPost]
        public IActionResult AddAccount(AccountViewModel accountViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isExistEmail = _account.CheckEmailExist(accountViewModel.Email);
                bool isValidPhone = PhoneNumber.isValid(accountViewModel.Phone);

                if (isExistEmail)
                {
                    var emailMess = "Email này đã tồn tại trong hệ thống!\n Vui lòng sử dụng email khác để tạo tài khoản.";

                    return Json(new { success = false, errors = "email", message = emailMess });
                }
                else if (isValidPhone == false)
                {
                    var phoneMess = "Số điện thoại không hợp lệ! Vui lòng nhập lại.";

                    return Json(new { success = false, errors = "phone", message = phoneMess });
                }
                else
                {
                    var password = GeneratePassword.GenerateAutoPassword(10);

                    var emailTitle = "Thông báo! Mật khẩu ứng dụng.";

                    var emailBody = "Mật khẩu: " + password;

                    _emailService.SendEmail(accountViewModel.Email, emailTitle, emailBody);

                    accountViewModel.Password = password;

                    _account.AddNewEmployment(accountViewModel);

                    return Json(new { success = true });
                }
            }
            else
            {
                var errors = ModelState.ToDictionary(kvp => kvp.Key,
                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors });
            }
        }

        [HttpPost]
        public IActionResult DeleteAccount(int accountId, string passwordAdmin)
        {
            var isExistAccount = _account.IsExistAccount(accountId);

            if (isExistAccount == false)
            {
                return Json(new { success = false, isExistAccout = false });
            }
            else
            {
                var emailAdmin = HttpContext.Session.GetString("userEmail");

                Account account = _account.GetAccount(emailAdmin, passwordAdmin);

                if (account == null)
                {
                    return Json(new { success = false, passwordAdmin = false });
                }
                else
                {
                    var status = _account.UpdateStatusDeleteEmployee(accountId);

                    return Json(new { success = status });
                }
            }
        }

        [HttpGet]
        public IActionResult ListCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ListCustomer(int? pageIndex, int? pageSize, string? searchName, string? sortName, string? selectStatus)
        {
            var pageIndexLocal = pageIndex ?? 1;

            var pageSizeLocal = pageSize ?? 10;

            var accounts = _account.GetAccountCustomers(pageIndexLocal, pageSizeLocal, 3, searchName ?? "", sortName ?? "", selectStatus ?? "");

            var totalAccount = _account.GetAccountCount(3);

            var numberPage = (int)Math.Ceiling((double)totalAccount / pageSizeLocal);


            return new JsonResult(new
            {
                userType = 3,
                accounts,
                totalAccount,
                currentPage = pageIndexLocal,
                pageSize = pageSizeLocal,
                numberPage
            });
        }

        [HttpGet]
        public IActionResult ListShipper()
        {
            var shipperFilterVM = new ShipperFilterViewModel();
            int pageIndex = 1;
            int pageSize = 5;

            ViewData["ListDistrict"] = _district.GetDistricts();
            ViewData["ListShipper"] = _shipper.GetAccountShippers(shipperFilterVM, pageIndex, pageSize);
            var totalShippers = _shipper.GetTotalAccountShippers(shipperFilterVM).Count;
            ViewData["NumberOfPage"] = (int)Math.Ceiling((double)totalShippers / pageSize);
            return View();
        }

        [HttpPost]
        public Object ListShipper(ShipperFilterViewModel shipperFilterVM, int pageIndex, int pageSize)
        {
            var shippers = _shipper.GetAccountShippers(shipperFilterVM, pageIndex, pageSize);
            var totalShippers = _shipper.GetTotalAccountShippers(shipperFilterVM).Count;
            var numberPage = (int)Math.Ceiling((double)totalShippers / pageSize);
            return Json(new
            {
                listShipper = shippers,
                numberOfPage = numberPage,
            });
        }

        [HttpPost]
        public IActionResult AddShipper(AccountViewModel shipper)
        {
            if (ModelState.IsValid)
            {
                bool isExistEmail = _account.CheckEmailExist(shipper.Email);
                bool isAssignDuplicateDistrict = _shipper.CheckAssignDuplicatedDistrict(shipper);
                bool isValidPhone = PhoneNumber.isValid(shipper.Phone);

                if (isExistEmail)
                {
                    var emailMess = "Email này đã tồn tại trong hệ thống!\n Vui lòng sử dụng email khác.";

                    return Json(new { success = false, errors = "email", message = emailMess });
                }
                else if (isValidPhone == false)
                {
                    var phoneMess = "Số điện thoại không hợp lệ! Vui lòng nhập lại.";

                    return Json(new { success = false, errors = "phone", message = phoneMess });
                }
                else if (isAssignDuplicateDistrict)
                {
                    var districtMess = "Hai nhân viên giao hàng không thể phụ trách cùng một quận!";
                    return Json(new { success = false, errors = "district", message = districtMess });
                }
                else
                {
                    var password = GeneratePassword.GenerateAutoPassword(10);

                    var emailTitle = "Thông báo! Mật khẩu ứng dụng.";

                    var emailBody = "Mật khẩu: " + password;

                    _emailService.SendEmail(shipper.Email, emailTitle, emailBody);

                    shipper.Password = password;

                    _shipper.AddNewShipper(shipper);

                    return Json(new { success = true });
                }
            }
            else
            {
                var errors = ModelState.ToDictionary(kvp => kvp.Key,
                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors });
            }
        }

        [HttpPost]
        public IActionResult UpdateShipper(AccountViewModel shipper)
        {
            if (ModelState.IsValid)
            {
                bool isExistEmail = false;
                Models.Shipper shipperDetail = _shipper.GetShipperByID(shipper.ShipperId);
                if (shipperDetail.Email != shipper.Email)
                {
                    isExistEmail = _account.CheckEmailExist(shipper.Email);
                }
                bool isAssignDuplicateDistrict = _shipper.CheckAssignDuplicatedDistrict(shipper);
                bool isValidPhone = PhoneNumber.isValid(shipper.Phone);

                if (isExistEmail)
                {
                    var emailMess = "Email này đã tồn tại trong hệ thống!\n Vui lòng sử dụng email khác.";

                    return Json(new { success = false, errors = "email", message = emailMess });
                }
                else if (isValidPhone == false)
                {
                    var phoneMess = "Số điện thoại không hợp lệ! Vui lòng nhập lại.";

                    return Json(new { success = false, errors = "phone", message = phoneMess });
                }
                else if (isAssignDuplicateDistrict)
                {
                    var districtMess = "Hai nhân viên giao hàng không thể phụ trách cùng một quận!";
                    return Json(new { success = false, errors = "district", message = districtMess });
                }
                else
                {
                    _shipper.UpdateShipper(shipper);

                    return Json(new { success = true });
                }
            }
            else
            {
                var errors = ModelState.ToDictionary(kvp => kvp.Key,
                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors });
            }
        }

        public IActionResult SelectDistrictIframe()
        {
            ViewData["ListDistrict"] = _district.GetDistricts();
            return View();
        }

        [HttpPost]
        public IActionResult DeleteShipper(string password, int shipperId)
        {
            var emailAdmin = HttpContext.Session.GetString("userEmail");
            Account accountAdmin = _account.GetAccount(emailAdmin, password);
            if (accountAdmin == null)
            {
                return Json(new { message = "Fail" });
            }
            else
            {
                _shipper.DeleteShipperAccount(shipperId);
                return Json(new { message = "Success" });
            }
        }

        [HttpGet]
        public IActionResult CustomerDetail(int userId)
        {
            var account = _account.GetAccountCustomers(userId);

            return View(account);
        }

        [HttpPost]
        public IActionResult OrderHistory(OrderModel orderModel)
        {

            var orders = _order.GetOrderDetailByCondition(orderModel);

            var totalOrders = _order.GetCountOrder(orderModel);

            ViewBag.searchOrderId = orderModel.SearchOrderId;
            ViewBag.searchName = orderModel.SearchName;
            ViewBag.searchDateOrder = orderModel.SearchDate;
            ViewBag.totalOrders = totalOrders;

            var numberPage = (int)Math.Ceiling((double)totalOrders / orderModel.pageSize);
            ViewBag.numberPage = numberPage;
            ViewBag.pageIndex = orderModel.pageIndex;
            ViewBag.pageSize = orderModel.pageSize;

            // Export Excel
            string json = JsonConvert.SerializeObject(orders);

            ViewBag.json = json;

            return View(orders);
        }

        [HttpPost]
        public IActionResult OrderServiceHistory(OrderServiceModel orderServiceModel)
        {
            List<OrderServicesDetailViewModel> orderServices = _orderService.GetOrderServicesByCondition(orderServiceModel);

            var totalOrderServices = _orderService.GetCountOrderService(orderServiceModel.UserId);

            ViewBag.SearchOrderId = orderServiceModel.SearchOrderServiceId;
            ViewBag.SearchName = orderServiceModel.SearchName;
            ViewBag.SearchDate = orderServiceModel.SearchDate;
            ViewBag.SearchTime = orderServiceModel.SearchTime;
            ViewBag.Status = orderServiceModel.Status;

            ViewBag.totalOrderServices = totalOrderServices;

            var numberPage = (int)Math.Ceiling((double)totalOrderServices / orderServiceModel.PageSize);
            ViewBag.numberPage = numberPage;
            ViewBag.pageIndex = orderServiceModel.PageIndex;
            ViewBag.pageSize = orderServiceModel.PageSize;

            // Export Excel
            string json = JsonConvert.SerializeObject(orderServices);

            ViewBag.json = json;

            return View(orderServices);
        }

        [HttpGet]
        public IActionResult ProfileAccount()
        {
            var email = HttpContext.Session.GetString("userEmail");

            var admin = _admin.GetAdmin(email);
            if (admin == null)
            {
                return NotFound("Admin not found.");
            }
            var adminViewModel = new UserViewModel
            {
                UserId = admin.AdminId,
                FullName = admin.FullName,
                DoB = admin.DoB,
                Gender = admin.Gender,
                Phone = admin.Phone,
                Address = admin.Address,
                Email = email,
                AccountId = admin.AccountId,
                RoleName = "Quản trị viên"
            };
            return View("_ProfileUser", adminViewModel);
        }

        [HttpPost]
        public IActionResult ProfileAccount(UserViewModel admin)
        {
            if (ModelState.IsValid)
            {
                var errors = new Dictionary<string, string>();

                var oldEmail = HttpContext.Session.GetString("userEmail");
                if (oldEmail != admin.Email)
                {
                    bool isEmailExist = _account.CheckEmailExist(admin.Email);
                    if (isEmailExist)
                    {
                        errors["Email"] = "Địa chỉ email này đã được liên kết với một tài khoản khác. Vui lòng nhập một email khác.";
                    }
                }

                bool isPhoneValid = PhoneNumber.isValid(admin.Phone);
                if (isPhoneValid == false)
                {
                    errors["Phone"] = "Số điện thoại không hợp lệ. Vui lòng nhập lại.";
                }

                if (admin.Address == null)
                {
                    errors["Address"] = "Địa chỉ không hợp lệ. Vui lòng nhập lại.";
                }

                if (admin.DoB >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
                {
                    errors["DoB"] = "Ngày sinh phải trước ngày hiện tại.";
                }

                if (errors.Any())
                {
                    return new JsonResult(new { isSuccess = false, errors });
                }

                HttpContext.Session.SetString("userEmail", admin.Email);
                HttpContext.Session.SetString("userName", admin.FullName);
                _admin.UpdateProfileAdmin(admin);
                return new JsonResult(new { isSuccess = true, message = "Cập nhật thành công", updatedData = admin });
            }
            else
            {
                var modelStateErrors = ModelState
                                        .Where(ms => ms.Value.Errors.Any())
                                        .ToDictionary(
                                            ms => ms.Key,
                                            ms => ms.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "Lỗi không xác định"
                                        );
                return new JsonResult(new { isSuccess = false, errors = modelStateErrors });
            }
        }

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
            return View("_ChangePasswordUser", ChangePasswordVM);
        }

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
                    return View("_ChangePasswordUser", ChangePasswordVM);
                }
                else
                {
                    if (ChangePasswordVM.NewPassword.Equals(ChangePasswordVM.OldPassword))
                    {
                        ViewBag.MessageNewPass = "Bạn đã sử dụng mật khẩu này gần đây. Vui lòng sử dụng mật khẩu khác.";
                        return View("_ChangePasswordUser", ChangePasswordVM);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                ViewBag.StatusChangePassword = "Thành công";
                _account.ChangePassword(ChangePasswordVM);
                return new JsonResult(new { success = true, message = "Thay đổi mật khẩu thành công." });
            }
            else
            {
                ViewBag.StatusChangePassword = "Thất bại";
                return View("_ChangePasswordUser", ChangePasswordVM);
            }
        }
    }
}
