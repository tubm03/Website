using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Consultion;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ConsultationController : Controller
    {
        private readonly IConsultationRepository _consultion;
        private readonly PetStoreDBContext _context;
        public ConsultationController(IConsultationRepository consultion, PetStoreDBContext context)
        {
            _consultion = consultion;
            _context = context;
        }
        [HttpGet]
        public IActionResult List(int? page, int? pageSize)
        {
            var consultations = _consultion.GetListConsultation(page, pageSize);
            return View(consultations);
        }

        [HttpPost]
        public JsonResult Detail(int consultionId)
        {
            var c = _consultion.GetDetail(consultionId);
            return Json(c);
        }

        [HttpPost]
        public JsonResult Reply(int consultionId, string messagge)
        {
            _consultion.Reply(consultionId, messagge);
            return Json(new { success = true });
        }
    }
}