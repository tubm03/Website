using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;
using X.PagedList;

namespace PetStoreProject.Repositories.Consultion
{
    public class ConsultationRepository : IConsultationRepository
    {
        private readonly PetStoreDBContext _dbContext;
        private readonly EmailService _emailService;

        public ConsultationRepository(PetStoreDBContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public int CreateConsultation(ConsultationCreateRequestViewModel consultion)
        {
            var newConsultation = new Consultation
            {
                Name = consultion.CustomerName,
                Email = consultion.Email,
                PhoneNumber = consultion.Phone,
                Title = consultion.Title,
                Content = consultion.Content,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Status = false,
            };
            _dbContext.Consultations.Add(newConsultation);
            _dbContext.SaveChanges();
            return newConsultation.ConsultationId;
        }

        public ConsultationViewForAdmin GetDetail(int id)
        {
            var consultion = _dbContext.Consultations.Find(id);
            if (consultion == null)
            {
                return null;
            }
            return new ConsultationViewForAdmin
            {
                Id = consultion.ConsultationId,
                Name = consultion.Name,
                Email = consultion.Email,
                Phone = consultion.PhoneNumber,
                Title = consultion.Title,
                Date = consultion.Date,
                Status = consultion.Status,
                Content = consultion.Content,
                Response = consultion.Response
            };
        }

        public IPagedList<ConsultationViewForAdmin> GetListConsultation(int? page, int? pageSize)
        {
            var listConsultation = (from c in _dbContext.Consultations
                                    select new ConsultationViewForAdmin
                                    {
                                        Id = c.ConsultationId,
                                        Name = c.Name,
                                        Email = c.Email ?? "No email provided",
                                        Phone = c.PhoneNumber ?? "No phone number",
                                        Title = c.Title ?? "No title",
                                        Date = c.Date,
                                        Status = c.Status
                                    }).ToList();
            page = page == null ? 1 : page;
            pageSize = pageSize == null ? 10 : pageSize;
            return listConsultation.ToPagedList((int)page, (int)pageSize);
        }

        public void Reply(int id, string message)
        {
            var consultation = _dbContext.Consultations.Find(id);
            var email = consultation.Email;
            var subject = "Trả lời yêu cầu";
            var body = "Nội dung tư vấn: " + consultation.Content + "<br><br>Trả lời:" + message;
            consultation.Status = true;
            consultation.Response = message;
            _dbContext.SaveChanges();
            _emailService.SendEmail(email, subject, body);
        }
    }
}
