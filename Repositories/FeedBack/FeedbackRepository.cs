using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.FeedBack
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly PetStoreDBContext _dbContext;

        public FeedbackRepository(PetStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<FeedBackViewModels> GetListFeedBack()
        {
            var list = (from fb in _dbContext.Feedbacks
                        join p in _dbContext.Products on fb.ProductId equals p.ProductId into gj1
                        from fbP in gj1.DefaultIfEmpty()
                        join s in _dbContext.Services on fb.ServiceId equals s.ServiceId into gj2
                        from fbS in gj2.DefaultIfEmpty()
                        join resp in _dbContext.ResponseFeedbacks on fb.FeedbackId equals resp.FeedbackId into gj3
                        from subResp in gj3.DefaultIfEmpty()
                        select new FeedBackViewModels
                        {
                            FeedBackId = fb.FeedbackId,
                            ProductName = fbP != null ? fbP.Name : fbS.Name,
                            CustomerName = fb.Name,
                            Rating = fb.Rating,
                            Content = fb.Content,
                            CreatedDate = fb.DateCreated,
                            Status = fb.Status,
                            ContentResponse = subResp != null ? subResp.Content : null
                        }).OrderByDescending(x => x.CreatedDate).ToList();
            return list;
        }

        public List<FeedbackViewModels> GetListFeedBackForService(int serviceId)
        {
            var listFeedback = (from fb in _dbContext.Feedbacks
                                where fb.ServiceId == serviceId
                                join respfb in _dbContext.ResponseFeedbacks on fb.FeedbackId equals respfb.FeedbackId into feedbackResponses
                                from resp in feedbackResponses.DefaultIfEmpty()
                                join e in _dbContext.Employees on resp.EmployeeId equals e.EmployeeId into employeeResponses
                                from emp in employeeResponses.DefaultIfEmpty()
                                select new FeedbackViewModels
                                {
                                    CustomerName = fb.Name,
                                    Rating = fb.Rating,
                                    Content = fb.Content,
                                    EmployeeName = emp != null ? emp.FullName : null,
                                    ContentResponse = resp != null ? resp.Content : null,
                                    DateCreated = fb.DateCreated,
                                    DateResp = (DateTime)(resp != null ? resp.DateCreated : (DateTime?)null)
                                }).ToList();


            return listFeedback;
        }

    }
}
