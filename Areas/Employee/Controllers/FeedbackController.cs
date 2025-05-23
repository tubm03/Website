
using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.FeedBack;
using X.PagedList;

namespace PetStoreProject.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackRepository _feedback;
        private readonly PetStoreDBContext _dbContext;
        public FeedbackController(IFeedbackRepository feedbackRepository, PetStoreDBContext petStoreDBContext)
        {
            _feedback = feedbackRepository;
            _dbContext = petStoreDBContext;
        }
        public IActionResult List(int? status, DateTime from, DateTime to, int? page, int? pageSize)
        {
            var list = _feedback.GetListFeedBack();
            if (status != null)
            {
                if (status == 1)
                {
                    list = list.Where(x => x.Status == false).ToList();
                }
                else if (status == 2)
                {
                    list = list.Where(x => x.Status == true).ToList();
                }
            }

            if (from == DateTime.MinValue && to == DateTime.MinValue)
            {
                DateTime today = DateTime.Today;
                DateTime firstDayOfWeek = DateTimeHelpers.GetFirstDayOfWeek(today);
                DateTime lastDayOfWeek = DateTimeHelpers.GetLastDayOfWeek(today);
                list = _feedback.GetListFeedBack().Where(x => x.CreatedDate > firstDayOfWeek && x.CreatedDate < lastDayOfWeek).ToList();
                ViewBag.FirstDayOfWeek = firstDayOfWeek.ToString("yyyy-MM-dd");
                ViewBag.LastDayOfWeek = lastDayOfWeek.ToString("yyyy-MM-dd");
            }
            else
            {
                ViewBag.FirstDayOfWeek = from.ToString("yyyy-MM-dd");
                ViewBag.LastDayOfWeek = to.ToString("yyyy-MM-dd");
                list = list.Where(x => x.CreatedDate > from && x.CreatedDate < to).ToList();
            }


            ViewBag.Status = status;
            ViewBag.Length = list.Count();
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            return View(list.ToPagedList((int)page, (int)pageSize));
        }

        [HttpPost]
        public IActionResult Submit()
        {
            try
            {
                int fbId = Int32.Parse(Request.Form["FeedBackId"]);

                var feedBack = _dbContext.Feedbacks.SingleOrDefault(fb => fb.FeedbackId == fbId);

                if (feedBack != null)
                    feedBack.Status = true;
                var email = HttpContext.Session.GetString("userEmail");
                // check have employee responed
                if (_dbContext.ResponseFeedbacks.Any(x => x.FeedbackId == fbId))
                {
                    var respFb = _dbContext.ResponseFeedbacks.Where(x => x.FeedbackId == fbId).FirstOrDefault();
                    if (respFb != null)
                    {
                        respFb.EmployeeId = _dbContext.Employees.Where(e => e.Email == email).Select(e => e.EmployeeId).FirstOrDefault();
                        respFb.Content = Request.Form["response"];
                        respFb.DateCreated = DateTime.Now;
                        _dbContext.SaveChanges();
                    }
                }
                else
                {
                    ResponseFeedback responseFeedback = new ResponseFeedback();
                    responseFeedback.FeedbackId = fbId;
                    responseFeedback.Content = Request.Form["response"];
                    responseFeedback.DateCreated = DateTime.Now;
                    responseFeedback.EmployeeId = _dbContext.Employees.Where(e => e.Email == email).Select(e => e.EmployeeId).FirstOrDefault();
                    _dbContext.ResponseFeedbacks.Add(responseFeedback);
                    _dbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Nội dung phản hồi không hợp lệ.");
            }
            return Ok("Phản hồi đã được gửi thành công.");
        }
    }
}
