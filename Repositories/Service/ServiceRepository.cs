using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;
using Quartz;
using System.Globalization;

namespace PetStoreProject.Repositories.Service
{
    public class ServiceRepository : IServiceRepository, IJob
    {
        private readonly PetStoreDBContext _context;
        private readonly ICloudinaryService _cloudinary;

        public ServiceRepository(PetStoreDBContext context, ICloudinaryService cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public List<ServiceViewModel> GetListServices()
        {
            var services = (from s in _context.Services
                            where s.IsDelete == false
                            select new ServiceViewModel
                            {
                                ServiceId = s.ServiceId,
                                Name = s.Name,
                                Type = s.Type,
                                SupDescription = s.SupDescription,
                            }).ToList();

            foreach (var service in services)
            {
                var Image = _context.Images.Where(i => i.ServiceId == service.ServiceId).FirstOrDefault();
                service.ImageUrl = Image.ImageUrl;
                var serviceOption = (from s in _context.Services
                                     join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                                     where s.ServiceId == service.ServiceId
                                     orderby so.Price ascending
                                     select so).FirstOrDefault();
                service.Price = serviceOption.Price;
            }

            return services;
        }

        public List<int> GetAllServiceId()
        {
            var serviceIds = (from s in _context.Services
                              select s.ServiceId).ToList();
            return serviceIds;
        }

        public ServiceDetailViewModel GetServiceDetail(int serviceId)
        {
            var service = (from s in _context.Services
                           where s.ServiceId == serviceId
                           select new ServiceDetailViewModel
                           {
                               ServiceId = s.ServiceId,
                               Name = s.Name,
                               Type = s.Type,
                               SubDescription = s.SupDescription,
                               Description = s.Description,
                               IsDelete = s.IsDelete,
                           }).FirstOrDefault();

            var images = (from s in _context.Services
                          join i in _context.Images on s.ServiceId equals i.ServiceId
                          where s.ServiceId == serviceId
                          select i).ToList();

            service.Images = images;
            return service;
        }

        public ServiceOptionViewModel GetFistServiceOption(int serviceId)
        {
            var listPetType = (from s in _context.Services
                               join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                               where s.ServiceId == serviceId && so.IsDelete == false
                               select so.PetType).Distinct().ToList();

            var petType = listPetType.FirstOrDefault();

            var firstServiceOption = GetFirstServiceAndListWeightOfPetType(serviceId, petType);

            firstServiceOption.PetTypes = listPetType;

            return firstServiceOption;
        }

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetType(int serviceId, string petType)
        {
            var firstServiceOption = (from so in _context.ServiceOptions
                                      where so.ServiceId == serviceId && so.PetType == petType && so.IsDelete == false
                                      select so).FirstOrDefault();
            var listWeight = (from so in _context.ServiceOptions
                              where so.ServiceId == serviceId && so.PetType == petType && so.IsDelete == false
                              orderby so.ServiceOptionId ascending
                              select so.Weight).ToList();
            return new ServiceOptionViewModel
            {
                ServiceId = firstServiceOption.ServiceId,
                ServiceOptionId = firstServiceOption.ServiceOptionId,
                PetType = firstServiceOption.PetType,
                Weight = firstServiceOption.Weight,
                Price = firstServiceOption.Price,
                IsDelete = firstServiceOption.IsDelete,
                Weights = listWeight
            };
        }

        public List<ServiceViewModel> GetListServicesForUpdate(int orderServiceId)
        {
            var serviceOfOrder = (from o in _context.OrderServices
                                  join so in _context.ServiceOptions on o.ServiceOptionId equals so.ServiceOptionId
                                  join s in _context.Services on so.ServiceId equals s.ServiceId 
                                  where o.OrderServiceId == orderServiceId
                                  select s).FirstOrDefault();

            var services = (from s in _context.Services
                            where s.IsDelete == false
                            select new ServiceViewModel
                            {
                                ServiceId = s.ServiceId,
                                Name = s.Name,
                            }).ToList();

            if(!services.Select(s => s.Name).Contains(serviceOfOrder.Name))
            {
                services.Add(new ServiceViewModel
                {
                    ServiceId = serviceOfOrder.ServiceId,
                    Name = serviceOfOrder.Name
                });
            }

            return services;
        }

        public ServiceOptionViewModel GetFistServiceOptionForUpdate(int serviceId, int orderServiceId)
        {
            var orderDetail = (from o in _context.OrderServices
                                  join so in _context.ServiceOptions on o.ServiceOptionId equals so.ServiceOptionId
                                  where o.OrderServiceId == orderServiceId
                                  select so).FirstOrDefault();

            var listPetType = (from s in _context.Services
                               join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                               where s.ServiceId == serviceId && so.IsDelete == false
                               select so.PetType).Distinct().ToList();
            
            if(serviceId == orderDetail.ServiceId)
            {
                if (listPetType.IsNullOrEmpty() || !listPetType.Contains(orderDetail.PetType))
                {
                    listPetType.Add(orderDetail.PetType);
                }
            }

            var petType = listPetType.FirstOrDefault();

            var firstServiceOption = GetFirstServiceAndListWeightOfPetTypeForUpdate(serviceId, petType, orderServiceId);

            firstServiceOption.PetTypes = listPetType;

            return firstServiceOption;
        }

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetTypeForUpdate(int serviceId, string petType, int orderServiceId)
        {
            var orderDetail = (from o in _context.OrderServices
                               join so in _context.ServiceOptions on o.ServiceOptionId equals so.ServiceOptionId
                               where o.OrderServiceId == orderServiceId
                               select so).FirstOrDefault();

            var firstServiceOption = (from so in _context.ServiceOptions
                                      where so.ServiceId == serviceId && so.PetType == petType && so.IsDelete == false
                                      select so).FirstOrDefault();

            if (firstServiceOption == null)
            {
                firstServiceOption = orderDetail;
            }

            var listWeight = (from so in _context.ServiceOptions
                              where so.ServiceId == serviceId && so.PetType == petType && so.IsDelete == false
                              orderby so.ServiceOptionId ascending
                              select so.Weight).ToList();

            if (orderDetail.ServiceId == serviceId && orderDetail.PetType == petType)
            {
                if(listWeight.IsNullOrEmpty() || !listWeight.Contains(orderDetail.Weight))
                {
                    listWeight.Add(orderDetail.Weight);
                }
            }

            return new ServiceOptionViewModel
            {
                ServiceId = firstServiceOption.ServiceId,
                ServiceOptionId = firstServiceOption.ServiceOptionId,
                PetType = firstServiceOption.PetType,
                Weight = firstServiceOption.Weight,
                Price = firstServiceOption.Price,
                IsDelete = firstServiceOption.IsDelete,
                Weights = listWeight
            };
        }

        public ServiceOptionViewModel GetFistServiceOptionForAdmin(int serviceId)
        {
            var listPetType = (from s in _context.Services
                               join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                               where s.ServiceId == serviceId
                               select so.PetType).Distinct().ToList();

            var petType = listPetType.FirstOrDefault();

            var firstServiceOption = GetFirstServiceAndListWeightOfPetTypeForAdmin(serviceId, petType);

            firstServiceOption.PetTypes = listPetType;

            return firstServiceOption;
        }

        public ServiceOptionViewModel GetFirstServiceAndListWeightOfPetTypeForAdmin(int serviceId, string petType)
        {
            var firstServiceOption = (from so in _context.ServiceOptions
                                      where so.ServiceId == serviceId && so.PetType == petType
                                      select so).FirstOrDefault();
            var listWeight = (from so in _context.ServiceOptions
                              where so.ServiceId == serviceId && so.PetType == petType
                              orderby so.ServiceOptionId ascending
                              select so.Weight).ToList();
            return new ServiceOptionViewModel
            {
                ServiceId = firstServiceOption.ServiceId,
                ServiceOptionId = firstServiceOption.ServiceOptionId,
                PetType = firstServiceOption.PetType,
                Weight = firstServiceOption.Weight,
                Price = firstServiceOption.Price,
                IsDelete = firstServiceOption.IsDelete,
                Weights = listWeight
            };
        }

        public ServiceOptionViewModel GetNewServiceOptionBySelectWeight(int serviceId, string petType, string weight)
        {
            var serviceOption = (from so in _context.ServiceOptions
                                 where so.ServiceId == serviceId && so.PetType == petType && so.Weight == weight
                                 select new ServiceOptionViewModel
                                 {
                                     ServiceOptionId = so.ServiceOptionId,
                                     Price = so.Price,
                                     IsDelete = so.IsDelete,
                                 }).FirstOrDefault();

            return serviceOption;
        }

        public List<string> GetAllWeightOfPet()
        {
            var weights = (from so in _context.ServiceOptions
                           select so.Weight).Distinct().ToList();
            return weights;
        }

        public List<ServiceOptionViewModel> GetServiceOptions(int serviceId)
        {
            var serviceOptions = (from so in _context.ServiceOptions
                                  where so.ServiceId == serviceId
                                  select new ServiceOptionViewModel
                                  {
                                      ServiceOptionId = so.ServiceOptionId,
                                      ServiceId = so.ServiceId,
                                      PetType = so.PetType,
                                      Weight = so.Weight,
                                      Price = so.Price,
                                      IsDelete = so.IsDelete,
                                  }).ToList();
            foreach (var serviceOption in serviceOptions)
            {
                serviceOption.OrderedQuantity = _context.OrderServices.Where
                    (os => os.ServiceOptionId == serviceOption.ServiceOptionId).ToList().Count;

                serviceOption.UsedQuantity = _context.OrderServices.Where(os =>
                os.ServiceOptionId == serviceOption.ServiceOptionId && os.Status == "Đã thanh toán").ToList().Count;
            }

            return serviceOptions;
        }

        public BookServiceViewModel GetBookingServiceInFo(int serviceOptionId)
        {
            var bookService = (from s in _context.Services
                               join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                               where so.ServiceOptionId == serviceOptionId
                               select new BookServiceViewModel
                               {
                                   ServiceOptionId = so.ServiceOptionId,
                                   ServiceId = so.ServiceId,
                                   ServiceName = s.Name,
                                   PetType = so.PetType,
                                   Weight = so.Weight,
                                   Price = so.Price.ToString("#,###") + "VND",
                               }).FirstOrDefault();

            return bookService;
        }

        public List<WorkingTime> GetAllWorkingTime()
        {
            var workingTimes = (from w in _context.WorkingTimes
                                select w).ToList();
            return workingTimes;
        }

        public List<int> GetWorkingTimeId(int serviceId)
        {
            var workingTimeIds = (from s in _context.Services
                                  join ts in _context.TimeServices on s.ServiceId equals ts.ServiceId
                                  join wt in _context.WorkingTimes on ts.WorkingTimeId equals wt.WorkingTimeId
                                  where s.ServiceId == serviceId
                                  select wt.WorkingTimeId).ToList();
            return workingTimeIds;
        }

        public List<TimeOnly> GetWorkingTime(int serviceId)
        {
            var workingTime = (from s in _context.Services
                               join ts in _context.TimeServices on s.ServiceId equals ts.ServiceId
                               join wt in _context.WorkingTimes on ts.WorkingTimeId equals wt.WorkingTimeId
                               where s.ServiceId == serviceId
                               select wt.Time).ToList();
            return workingTime;
        }

        public List<TimeOnly> GetWorkingTimeByDate(string date)
        {
            List<TimeOnly> listTime = new List<TimeOnly>();
            var workingTimes = _context.WorkingTimes.Select(wt => wt.Time).ToList();
            var numberOfEmployee = _context.Employees.Where(e => e.IsDelete == false).ToList().Count;
            DateOnly? orderDate = null;
            if (DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateParsed))
            {
                orderDate = DateOnly.FromDateTime(dateParsed);
            }
            else
            {
                return workingTimes;
            }
            var orderTimeInDate = (from os in _context.OrderServices
                                   where os.OrderDate == orderDate
                                   && (os.Status == "Chưa xác nhận" || os.Status == "Đã xác nhận")
                                   select os.OrderTime).ToList();
            foreach (var workingTime in workingTimes)
            {
                var count = orderTimeInDate.Count(ot => ot == workingTime);
                if (count < numberOfEmployee)
                {
                    listTime.Add(workingTime);
                }
            }

            return listTime;
        }

        public List<TimeOnly> GetWorkingTimeByDateForUpdate(string date, TimeOnly orderTime)
        {
            List<TimeOnly> listTime = new List<TimeOnly>();
            var workingTimes = _context.WorkingTimes.Select(wt => wt.Time).ToList();
            var numberOfEmployee = _context.Employees.Where(e => e.IsDelete == false).ToList().Count;
            DateOnly? orderDate = null;
            if (DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateParsed))
            {
                orderDate = DateOnly.FromDateTime(dateParsed);
            }
            else
            {
                return workingTimes;
            }
            var orderTimeInDate = (from os in _context.OrderServices
                                   where os.OrderDate == orderDate
                                   && (os.Status == "Chưa xác nhận" || os.Status == "Đã xác nhận")
                                   select os.OrderTime).ToList();
            foreach (var workingTime in workingTimes)
            {
                var count = orderTimeInDate.Count(ot => ot == workingTime);
                if (workingTime == orderTime || count < numberOfEmployee)
                {
                    listTime.Add(workingTime);
                }
            }

            return listTime;
        }

        public void AddOrderService(BookServiceViewModel bookServiceInfo)
        {
            DateTime ordDate = DateTime.ParseExact(bookServiceInfo.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateOnly orderDate = DateOnly.FromDateTime(ordDate);
            var orderServiceDuplicate = (from os in _context.OrderServices
                                         where os.Name == bookServiceInfo.Name
                                         && os.Phone == bookServiceInfo.Phone
                                         && os.OrderDate == orderDate
                                         && os.OrderTime == bookServiceInfo.OrderTime
                                         && os.ServiceOptionId == bookServiceInfo.ServiceOptionId
                                         && os.Status == "Chưa xác nhận"
                                         select os).FirstOrDefault();
            if (orderServiceDuplicate != null)
            {
                return;
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Models.OrderService orderService = new Models.OrderService();
                    orderService.CustomerId = bookServiceInfo?.CustomerId;
                    orderService.Name = bookServiceInfo.Name;
                    orderService.Phone = bookServiceInfo.Phone;
                    orderService.OrderDate = orderDate;

                    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
                    orderService.DateCreated = today;

                    orderService.OrderTime = bookServiceInfo.OrderTime;
                    orderService.ServiceOptionId = bookServiceInfo.ServiceOptionId;
                    orderService.Message = bookServiceInfo?.Message;

                    if (bookServiceInfo.Status.IsNullOrEmpty())
                    {
                        orderService.Status = "Chưa xác nhận";
                    }
                    else
                    {
                        orderService.Status = bookServiceInfo.Status;
                    }

                    orderService.IsDelete = false;

                    _context.Add(orderService);

                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public List<ServiceViewModel> GetOtherServices(int serviceId)
        {
            var services = (from s in _context.Services
                            where s.IsDelete == false && s.ServiceId != serviceId
                            select new ServiceViewModel
                            {
                                ServiceId = s.ServiceId,
                                Name = s.Name,
                                Type = s.Type,
                                SupDescription = s.SupDescription,
                            }).ToList();

            foreach (var service in services)
            {
                var Image = _context.Images.Where(i => i.ServiceId == service.ServiceId).FirstOrDefault();
                service.ImageUrl = Image.ImageUrl;
                var serviceOption = (from s in _context.Services
                                     join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                                     where s.ServiceId == service.ServiceId
                                     orderby so.Price ascending
                                     select so).FirstOrDefault();
                service.Price = serviceOption.Price;
            }

            return services;
        }

        public List<BookServiceViewModel> GetOrderedServicesOfCustomer(int customerId)
        {
            var orderedServices = (from os in _context.OrderServices
                                   join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                                   join s in _context.Services on so.ServiceId equals s.ServiceId
                                   where os.CustomerId == customerId
                                   select new BookServiceViewModel
                                   {
                                       OrderServiceId = os.OrderServiceId,
                                       ServiceName = s.Name,
                                       DateCreated = os.DateCreated.ToString("dd/MM/yyyy"),
                                       OrderDate = os.OrderDate.ToString("dd/MM/yyyy"),
                                       Status = os.Status
                                   }).ToList();
            return orderedServices;
        }

        public BookServiceViewModel GetOrderServiceDetail(int orderServiceId)
        {
            var orderServiceDetail = (from os in _context.OrderServices
                                      join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                                      join s in _context.Services on so.ServiceId equals s.ServiceId
                                      where os.OrderServiceId == orderServiceId
                                      select new BookServiceViewModel
                                      {
                                          OrderServiceId = os.OrderServiceId,
                                          ServiceId = s.ServiceId,
                                          ServiceOptionId = os.ServiceOptionId,
                                          Name = os.Name,
                                          Phone = os.Phone,
                                          OrderDate = os.OrderDate.ToString("dd/MM/yyyy"),
                                          DateCreated = os.DateCreated.ToString("dd/MM/yyyy"),
                                          OrderTime = os.OrderTime,
                                          ServiceName = s.Name,
                                          PetType = so.PetType,
                                          Weight = so.Weight,
                                          Message = os.Message,
                                          Status = os.Status,
                                      }).FirstOrDefault();

            if (orderServiceDetail.Status == "Chưa xác nhận")
            {
                orderServiceDetail.Price = _context.ServiceOptions.FirstOrDefault(
                    s => s.ServiceOptionId == orderServiceDetail.ServiceOptionId)?.Price.ToString("#,###") + " VND";
            }
            else
            {
                orderServiceDetail.Price = _context.OrderServices.FirstOrDefault(
                    o => o.OrderServiceId == orderServiceDetail.OrderServiceId)?.Price?.ToString("#,###") + " VND";
            }

            var employeeId = _context.OrderServices.Where(os => os.OrderServiceId == orderServiceId).FirstOrDefault()?.EmployeeId;
            if (employeeId != null)
            {
                orderServiceDetail.EmployeeName = _context.Employees.Where(e => e.EmployeeId == employeeId).FirstOrDefault()?.FullName;
            }

            return orderServiceDetail;
        }

        public void UpdateOrderService(BookServiceViewModel orderService)
        {
            var order = (from os in _context.OrderServices
                         where os.OrderServiceId == orderService.OrderServiceId
                         select os).FirstOrDefault();

            order.Name = orderService.Name;
            order.Phone = orderService.Phone;

            DateTime orderDate = DateTime.ParseExact(orderService.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            order.OrderDate = DateOnly.FromDateTime(orderDate);

            order.OrderTime = orderService.OrderTime;

            order.ServiceOptionId = orderService.ServiceOptionId;

            if (orderService.Status == "Đã xác nhận")
            {
                order.Price = _context.ServiceOptions.FirstOrDefault(s => s.ServiceOptionId == orderService.ServiceOptionId)?.Price;
            }

            order.Message = orderService.Message;

            _context.SaveChanges();
        }

        public void DeleteOrderService(int orderServiceId)
        {
            var orderService = (from os in _context.OrderServices
                                where os.OrderServiceId == orderServiceId
                                select os).FirstOrDefault();
            orderService.Status = "Đã hủy";

            orderService.IsDelete = true;

            _context.SaveChanges();
        }

        public void UpdateStatusOrderService(int orderServiceId, string status, int employeeId)
        {
            var orderService = (from os in _context.OrderServices
                                where os.OrderServiceId == orderServiceId
                                select os).FirstOrDefault();
            orderService.Price = _context.ServiceOptions.FirstOrDefault(s => s.ServiceOptionId == orderService.ServiceOptionId)?.Price;

            orderService.Status = status;

            if (employeeId != 0)
            {
                orderService.EmployeeId = employeeId;
            }

            _context.SaveChanges();
        }

        public async Task AddNewService(ServiceAdditionViewModel serviceAddition)
        {
            Models.Service service = new Models.Service();
            service.Name = serviceAddition.Name;
            service.Type = serviceAddition.Type;
            service.SupDescription = serviceAddition.Subdescription;
            service.Description = serviceAddition.Description;
            service.IsDelete = false;

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            var serviceId = service.ServiceId;
            await AddWorkingTime(serviceAddition, serviceId);
            await AddServiceOption(serviceAddition, serviceId);
            await AddImageService(serviceAddition, serviceId);

            await _context.SaveChangesAsync();
        }

        public async Task AddWorkingTime(ServiceAdditionViewModel serviceAddition, int serviceId)
        {
            var workingTimes = serviceAddition.WorkingTimes.Select(workingTime => new TimeService
            {
                WorkingTimeId = workingTime,
                ServiceId = serviceId
            });

            _context.TimeServices.AddRange(workingTimes);
        }

        public async Task AddServiceOption(ServiceAdditionViewModel serviceAddition, int serviceId)
        {
            var serviceOptions = serviceAddition.ServiceOptions.Select(option => new ServiceOption
            {
                ServiceId = serviceId,
                PetType = option.PetType,
                Weight = option.Weight,
                Price = option.Price,
                IsDelete = false
            });

            _context.ServiceOptions.AddRange(serviceOptions);
        }

        public async Task AddImageService(ServiceAdditionViewModel serviceAddition, int serviceId)
        {
            int maxImageId = (from i in _context.Images
                              orderby i.ImageId descending
                              select i.ImageId).FirstOrDefault();
            foreach (var imageData in serviceAddition.Images)
            {
                maxImageId++;
                Models.Image image = new Models.Image()
                {
                    ImageId = maxImageId,
                    ServiceId = serviceId
                };
                if (IsBase64String(imageData))
                {
                    ImageUploadResult result = await _cloudinary.UploadImage(imageData, "image_" + maxImageId);
                    image.ImageUrl = result.Url.ToString();
                }
                else
                {
                    image.ImageUrl = imageData;
                }
                _context.Images.Add(image);
            }
        }

        public async Task UpdateService(ServiceAdditionViewModel serviceAddition)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceAddition.ServiceId);
                service.Name = serviceAddition.Name;
                service.Type = serviceAddition.Type;
                service.SupDescription = serviceAddition.Subdescription;
                service.Description = serviceAddition.Description;
                service.IsDelete = serviceAddition.IsDelete;
                await _context.SaveChangesAsync();

                var workingTimes = await _context.TimeServices.Where(t => t.ServiceId == serviceAddition.ServiceId).ToListAsync();
                _context.TimeServices.RemoveRange(workingTimes);
                await _context.SaveChangesAsync();
                await AddWorkingTime(serviceAddition, serviceAddition.ServiceId);
                await _context.SaveChangesAsync();

                var images = await _context.Images.Where(i => i.ServiceId == serviceAddition.ServiceId).ToListAsync();
                _context.Images.RemoveRange(images);
                await _context.SaveChangesAsync();
                await AddImageService(serviceAddition, serviceAddition.ServiceId);
                await _context.SaveChangesAsync();

                await UpdateServiceOptions(serviceAddition);
                await _context.SaveChangesAsync();

                if (serviceAddition.IsDelete == true)
                {
                    DeleteService(serviceAddition.ServiceId);
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
        }

        public async Task UpdateServiceOptions(ServiceAdditionViewModel serviceAddition)
        {
            foreach (var option in serviceAddition.ServiceOptions)
            {
                if (option.ServiceOptionId != 0)
                {
                    var oldOption = await _context.ServiceOptions.FindAsync(option.ServiceOptionId);
                    oldOption.PetType = option.PetType;
                    oldOption.Weight = option.Weight;
                    oldOption.Price = option.Price;
                    oldOption.IsDelete = option.IsDelete;
                }
                else
                {
                    _context.ServiceOptions.Add(option);
                }
            }
        }

        public void DeleteService(int serviceId)
        {
            var service = _context.Services.Find(serviceId);
            service.IsDelete = true;

            var serviceOptions = _context.ServiceOptions.Where(so => so.ServiceId == serviceId).ToList();
            foreach (var option in serviceOptions)
            {
                option.IsDelete = true;
            }

            _context.SaveChanges();
        }

        public List<BookServiceViewModel> GetOrderedServicesByConditions(OrderedServiceViewModel orderServiceVM,
        int pageIndex, int pageSize)
        {
            List<BookServiceViewModel> orderedServices = new List<BookServiceViewModel>();

            // Determine the list of order services that satisfy the filter options
            DateOnly? orderServiceDate = null;
            if (DateTime.TryParseExact(orderServiceVM.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                orderServiceDate = DateOnly.FromDateTime(parsedDate);
            }

            string? customerNameOrPhone = orderServiceVM.NameOrPhone.IsNullOrEmpty() ? null : orderServiceVM.NameOrPhone;
            int? serviceId = orderServiceVM.ServiceId.IsNullOrEmpty() ? null : Int32.Parse(orderServiceVM.ServiceId);

            orderedServices = (from os in _context.OrderServices
                               join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                               join s in _context.Services on so.ServiceId equals s.ServiceId
                               where os.Status == orderServiceVM.Status
                               && (customerNameOrPhone == null || os.Name.Contains(customerNameOrPhone) || os.Phone.Contains(customerNameOrPhone))
                               && (serviceId == null || s.ServiceId == serviceId) //If serviceId is null, then return true and do not evaluate the subsequent condition.
                               && (orderServiceDate == null || os.OrderDate == orderServiceDate)
                               select new BookServiceViewModel
                               {
                                   OrderServiceId = os.OrderServiceId,
                                   Name = os.Name,
                                   Phone = os.Phone,
                                   OrderDate = os.OrderDate.ToString("dd/MM/yyyy"),
                                   OrderTime = os.OrderTime,
                                   ServiceName = s.Name,
                                   PetType = so.PetType,
                                   Status = os.Status,
                               }).ToList();
            // -----------------------------

            // Sort the list according to different criteria depending on the sorting parameter.
            if (orderServiceVM.SortServiceId != null)
            {
                if (orderServiceVM.SortServiceId == "Ascending")
                    orderedServices = orderedServices.OrderBy(o => o.OrderServiceId).ToList();
                else
                    orderedServices = orderedServices.OrderByDescending(o => o.OrderServiceId).ToList();
            }

            if (orderServiceVM.SortCustomerName != null)
            {
                if (orderServiceVM.SortCustomerName == "Ascending")
                    orderedServices = orderedServices.OrderBy(o => o.Name).ToList();
                else
                    orderedServices = orderedServices.OrderByDescending(o => o.Name).ToList();
            }

            if (orderServiceVM.SortOrderDate != null)
            {
                if (orderServiceVM.SortOrderDate == "Ascending")
                    orderedServices = orderedServices.OrderBy(o =>
                    DateTime.ParseExact(o.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToList();
                else
                    orderedServices = orderedServices.OrderByDescending(o =>
                    DateTime.ParseExact(o.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToList();
            }

            if (orderServiceVM.SortOrderTime != null)
            {
                if (orderServiceVM.SortOrderTime == "Ascending")
                    orderedServices = orderedServices.OrderBy(o => o.OrderTime).ToList();
                else
                    orderedServices = orderedServices.OrderByDescending(o => o.OrderTime).ToList();
            }
            //-----------------------------

            //Skip the number of elements depending on the index of the current page and the page size.
            var orderedServicesDisplay = orderedServices.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            //------------------------------

            return orderedServicesDisplay;
        }

        public int GetTotalCountOrderedServicesByConditions(OrderedServiceViewModel orderServiceVM)
        {
            DateOnly? orderServiceDate = null;
            if (DateTime.TryParseExact(orderServiceVM.OrderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                orderServiceDate = DateOnly.FromDateTime(parsedDate);
            }

            string? customerNameOrPhone = orderServiceVM.NameOrPhone.IsNullOrEmpty() ? null : orderServiceVM.NameOrPhone;
            int? serviceId = orderServiceVM.ServiceId.IsNullOrEmpty() ? null : Int32.Parse(orderServiceVM.ServiceId);

            var orderedServices = (from os in _context.OrderServices
                                   join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                                   join s in _context.Services on so.ServiceId equals s.ServiceId
                                   where os.Status == orderServiceVM.Status
                                   && (customerNameOrPhone == null || os.Name.Contains(customerNameOrPhone) || os.Phone.Contains(customerNameOrPhone))
                                   && (serviceId == null || s.ServiceId == serviceId)  //If serviceId is null, then return true and do not evaluate the subsequent condition.
                                   && (orderServiceDate == null || os.OrderDate == orderServiceDate)
                                   select new BookServiceViewModel
                                   {
                                       OrderServiceId = os.OrderServiceId,
                                   }).ToList();

            var totalItem = orderedServices.Count;
            return totalItem;
        }

        public List<String> GetListServiceTypes()
        {
            var listTypes = (from s in _context.Services
                             select s.Type).Distinct().OrderByDescending(s => s).ToList();
            return listTypes;
        }

        public List<ServiceTableViewModel> GetListServiceByConditions(ServiceFilterViewModel serviceFilterVM,
            int pageIndex, int pageSize)
        {
            string? serviceName = serviceFilterVM.Name.IsNullOrEmpty() ? null : serviceFilterVM.Name;
            string? serviceType = serviceFilterVM.ServiceType.IsNullOrEmpty() ? null : serviceFilterVM.ServiceType;
            bool? status = serviceFilterVM.Status.IsNullOrEmpty() ? null : bool.Parse(serviceFilterVM.Status);
            var listService = (from s in _context.Services
                               where (serviceName == null || s.Name.Contains(serviceName))
                               && (serviceType == null || s.Type == serviceType)
                               && (status == null || s.IsDelete == status)
                               select new ServiceTableViewModel
                               {
                                   ServiceId = s.ServiceId,
                                   Name = s.Name,
                                   Type = s.Type,
                                   IsDelete = s.IsDelete,
                               }).ToList();

            if (listService != null)
            {
                foreach (var service in listService)
                {
                    service.ImageUrl = _context.Images.Where(i => i.ServiceId == service.ServiceId).FirstOrDefault().ImageUrl;
                    service.Price = (from s in _context.Services
                                     join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                                     where s.ServiceId == service.ServiceId
                                     orderby so.Price ascending
                                     select so.Price).FirstOrDefault();
                    service.UsedQuantity = (from s in _context.Services
                                            join so in _context.ServiceOptions on s.ServiceId equals so.ServiceId
                                            join os in _context.OrderServices on so.ServiceOptionId equals os.ServiceOptionId
                                            where s.ServiceId == service.ServiceId && os.Status == "Đã thanh toán"
                                            select os.OrderServiceId).Count();
                }

                if (serviceFilterVM.SortServiceName != null)
                {
                    if (serviceFilterVM.SortServiceName == "Ascending")
                        listService = listService.OrderBy(s => s.Name).ToList();
                    else
                        listService = listService.OrderByDescending(s => s.Name).ToList();
                }

                if (serviceFilterVM.SortServiceId != null)
                {
                    if (serviceFilterVM.SortServiceId == "Ascending")
                        listService = listService.OrderBy(s => s.ServiceId).ToList();
                    else
                        listService = listService.OrderByDescending(s => s.ServiceId).ToList();
                }

                if (serviceFilterVM.SortPrice != null)
                {
                    if (serviceFilterVM.SortPrice == "Ascending")
                        listService = listService.OrderBy(s => s.Price).ToList();
                    else
                        listService = listService.OrderByDescending(s => s.Price).ToList();
                }

                if (serviceFilterVM.SortUsedQuantity != null)
                {
                    if (serviceFilterVM.SortUsedQuantity == "Ascending")
                        listService = listService.OrderBy(s => s.UsedQuantity).ToList();
                    else
                        listService = listService.OrderByDescending(s => s.UsedQuantity).ToList();
                }
            }


            var listDisplay = listService.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return listDisplay;
        }

        public int GetTotalCountListServicesByConditions(ServiceFilterViewModel serviceFilterVM)
        {
            string? serviceName = serviceFilterVM.Name.IsNullOrEmpty() ? null : serviceFilterVM.Name;
            string? serviceType = serviceFilterVM.ServiceType.IsNullOrEmpty() ? null : serviceFilterVM.ServiceType;
            bool? status = serviceFilterVM.Status.IsNullOrEmpty() ? null : bool.Parse(serviceFilterVM.Status);
            var listService = (from s in _context.Services
                               where (serviceName == null || s.Name.Contains(serviceName))
                               && (serviceType == null || s.Type == serviceType)
                               && (status == null || s.IsDelete == status)
                               select new ServiceTableViewModel
                               {
                                   ServiceId = s.ServiceId,
                                   Name = s.Name,
                                   Type = s.Type,
                                   IsDelete = s.IsDelete,
                               }).ToList();

            return listService.Count;
        }

        public float GetTotalServiceSale()
        {
            var totalAmount = (from os in _context.OrderServices
                               where os.Status == "Đã thanh toán"
                               select os.Price).Sum();
            return totalAmount ?? 0;
        }

        public List<ServiceTableViewModel> GetTopSellingService(string startDate, string endDate)
        {
            DateOnly? dateStart = null;
            if (DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                dateStart = DateOnly.FromDateTime(parsedDate);
            }

            DateOnly? dateEnd = null;
            if (DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateEnd))
            {
                dateEnd = DateOnly.FromDateTime(parsedDateEnd);
            }

            var listService = (from os in _context.OrderServices
                               join so in _context.ServiceOptions on os.ServiceOptionId equals so.ServiceOptionId
                               join s in _context.Services on so.ServiceId equals s.ServiceId
                               where (dateStart == null || dateStart <= os.OrderDate)
                               && (dateEnd == null || os.OrderDate <= dateEnd)
                               && os.Status == "Đã thanh toán"
                               group os by so.ServiceId into g
                               select new ServiceTableViewModel
                               {
                                   ServiceId = g.Key,
                                   UsedQuantity = g.Count(),
                                   TotalSale = g.Sum(s => s.Price) ?? 0
                               }).OrderByDescending(s => s.UsedQuantity).ToList();

            foreach (var item in listService)
            {
                var service = _context.Services.FirstOrDefault(s => s.ServiceId == item.ServiceId);
                item.Name = service.Name;
                item.Type = service.Type;
                item.ImageUrl = _context.Images.Where(i => i.ServiceId == item.ServiceId).FirstOrDefault()?.ImageUrl;
            }

            return listService;
        }

        public List<float> GetServiceSaleOfMonth(int month, int year)
        {
            List<float> dataService = new List<float>();

            var orders = (from o in _context.OrderServices
                          where o.OrderDate.Month == month && o.OrderDate.Year == year && o.Status == "Đã thanh toán"
                          select o).ToList();

            var numberOfDay = DateTime.DaysInMonth(year, month);

            for (int i = 1; i <= numberOfDay; i++)
            {
                dataService.Add(orders.Where(o => o.OrderDate.Day == i).FirstOrDefault()?.Price ?? 0);
            }

            return dataService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            var orders = (from od in _context.OrderServices
                          where od.OrderDate < today && (od.Status == "Đã xác nhận" || od.Status == "Chưa xác nhận")
                          select od).ToList();

            foreach (var order in orders)
            {
                order.Status = "Đã hủy";
                order.IsDelete = true;
            }

            await _context.SaveChangesAsync();
        }

        private bool IsBase64String(string imageData)
        {
            if (imageData.Contains("cloudinary"))
            {
                return false;
            }
            try
            {
                var base64Data = imageData.Split(',')[1];
                Convert.FromBase64String(base64Data);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
