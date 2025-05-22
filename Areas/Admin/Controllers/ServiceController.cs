using Microsoft.AspNetCore.Mvc;
using PetStoreProject.ViewModels;
using PetStoreProject.Repositories.Service;
using PetStoreProject.Models;
using PetStoreProject.Areas.Admin.ViewModels;

namespace PetStoreProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _service;

        public ServiceController(IServiceRepository service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult List()
        {
            var serviceFilterVM = new ServiceFilterViewModel();
            ViewData["ListServiceType"] = _service.GetListServiceTypes();
            ViewData["ListService"] = _service.GetListServiceByConditions(serviceFilterVM, 1, 7);
            var totalService = _service.GetTotalCountListServicesByConditions(serviceFilterVM);
            ViewData["numberOfPage"] = (int)Math.Ceiling((double)totalService / 7);
            return View();
        }

        [HttpPost]
        public Object List(ServiceFilterViewModel serviceFilterVM, int pageIndex, int pageSize)
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

        [HttpGet("/Admin/Service/Detail/{serviceId}")]
        public IActionResult Detail(int serviceId)
        {
            ViewData["ListServiceId"] = _service.GetAllServiceId();
            ViewData["ServiceDetail"] = _service.GetServiceDetail(serviceId);
            ViewData["FirstServiceOption"] = _service.GetFistServiceOptionForAdmin(serviceId);
            ViewData["ListServiceOption"] = _service.GetServiceOptions(serviceId);
            return View();
        }

        [HttpPost]
        public ServiceOptionViewModel GetOptionViewModel(int serviceId, string petType)
        {
            return _service.GetFirstServiceAndListWeightOfPetTypeForAdmin(serviceId, petType);
        }

        [HttpPost]
        public ServiceOptionViewModel GetServiceOptionPrice(int serviceId, string petType, string weight)
        {
            return _service.GetNewServiceOptionBySelectWeight(serviceId, petType, weight);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ServiceTypes"] = _service.GetListServiceTypes();
            ViewData["AllWorkingTime"] = _service.GetAllWorkingTime();
            ViewData["AllPetWeight"] = _service.GetAllWeightOfPet();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceAdditionViewModel serviceAddition)
        {
            await _service.AddNewService(serviceAddition);
            var lastestServiceId = _service.GetAllServiceId().OrderByDescending(s => s).FirstOrDefault();
            return Json(new { serviceId = lastestServiceId });
        }

        [HttpGet("/Admin/Service/Update/{serviceId}")]
        public IActionResult Update(int serviceId)
        {
            ViewData["ServiceTypes"] = _service.GetListServiceTypes();
            ViewData["AllWorkingTime"] = _service.GetAllWorkingTime();
            ViewData["AllPetWeight"] = _service.GetAllWeightOfPet();
            ViewData["ListServiceOption"] = _service.GetServiceOptions(serviceId);
            var service = _service.GetServiceDetail(serviceId);
            ViewData["ServiceDetail"] = new ServiceAdditionViewModel
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Type = service.Type,
                Subdescription = service.SubDescription,
                Description = service.Description,
                IsDelete = (bool)service.IsDelete,
                Images = service.Images.Select(i => i.ImageUrl).ToList(),
                WorkingTimes = _service.GetWorkingTimeId(serviceId)
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(ServiceAdditionViewModel serviceAddition) {
            await _service.UpdateService(serviceAddition);
            return Json(new { serviceId = serviceAddition.ServiceId });
        }

        [HttpPost]
        public void Delete(int serviceId) { 
            _service.DeleteService(serviceId);
        }
    }
}
