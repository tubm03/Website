using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Consultion;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
    public class ConsultationController : Controller
    {
        private readonly IConsultationRepository _consultionRepository;
        private readonly PetStoreDBContext _context;

        public ConsultationController(IConsultationRepository consultionRepository, PetStoreDBContext context)
        {
            _consultionRepository = consultionRepository;
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ConsultationCreateRequestViewModel consultion)
        {
            if (ModelState.IsValid)
            {
                var consultionId = _consultionRepository.CreateConsultation(consultion);
                ViewData["Result"] = "Success";
            }
            return View();
        }
    }
}
