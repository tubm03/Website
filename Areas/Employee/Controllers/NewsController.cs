using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Models;
using PetStoreProject.Repositories.News;
using PetStoreProject.ViewModels;
using X.PagedList;

namespace PetStoreProject.Areas.Employee.Controllers
{
	[Area("Employee")]
	public class NewsController : Controller
	{
		private readonly PetStoreDBContext _dbContext;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly INewsRepository _newsRepository;

		public NewsController(PetStoreDBContext petStoreDBContext, ICloudinaryService cloudinaryService, INewsRepository newsRepository)
		{
			_dbContext = petStoreDBContext;
			_newsRepository = newsRepository;
			_cloudinaryService = cloudinaryService;
		}


		public ActionResult Index()
		{
			return View();
		}
		public IActionResult UpdateStatusNews(int newsId)
		{
			bool updateSuccessful = _newsRepository.UpdateNewsStatus(newsId);
            if (updateSuccessful)
            {
                return Ok(new
                {
                    success = true,
                    message = "Cập nhật thành công"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Cập nhật không thành công"
                });
            }
        }


        public async Task<IActionResult> UpdateNews(int newsId, string content, string title, string summary, IFormFile thumbnail, int tag)
		{
			try
			{
				var news = _dbContext.News.FirstOrDefault(n => n.NewsId == newsId);
				var img = _dbContext.Images.FirstOrDefault(n => n.NewsId == newsId);
                var email = HttpContext.Session.GetString("userEmail");
                var eid = _dbContext.Employees.Where(e => e.Email == email).Select(e => e.EmployeeId).FirstOrDefault();

                news.Content = content;
				news.Title = title;
				news.Summary = summary;
				news.TagId = tag;
				news.DatePosted = DateOnly.FromDateTime(DateTime.Now);
                news.EmployeeId = eid;

                if (thumbnail != null)
				{
                    var uploadResult = await _cloudinaryService.UploadImage(thumbnail);
                    var url = uploadResult.Url.ToString();
					img.ImageUrl = url;
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã sảy ra lỗi khi cập nhật bản ghi." });
            }
            return RedirectToAction("ListNews");
        }
		public IActionResult EditNews(int id)
		{
            NewsViewModel model = _newsRepository.GetNewsById(id);
            ViewBag.listTag = _dbContext.TagNews.ToList();
            return View(model);
		}
		public IActionResult ListNews(int? page, int? pageSize)
		{
			if (page == null)
			{
				page = 1;
			}
			if (pageSize == null)
			{
				pageSize = 12;
			}
			var listNews = _newsRepository.GetListNewsForEmployee();
			return View(listNews.ToPagedList((int)page, (int)pageSize));
		}
		public IActionResult CreateNews()
		{
			var listag = _dbContext.TagNews.ToList();
			return View(listag);
		}
		[HttpPost]
		public async Task<IActionResult> SaveContent(string content, string title, string summary, IFormFile thumbnail, int tag)
		{
            try
            {
                var email = HttpContext.Session.GetString("userEmail");
                var eid = _dbContext.Employees.Where(e => e.Email == email).Select(e => e.EmployeeId).FirstOrDefault();
                var article = new News { Content = content, Title = title, Summary = summary, EmployeeId = eid, DatePosted = DateOnly.FromDateTime(DateTime.Now), TagId = tag };
                var uploadResult = await _cloudinaryService.UploadImage(thumbnail);
                var url = uploadResult.Url.ToString();
                int maxImgId = _dbContext.Images.Max(img => img.ImageId);
                var newImage = new Image
                {
                    ImageId = ++maxImgId,
                    ImageUrl = url!,
                    News = article,
                };
                _dbContext.News.Add(article);
                _dbContext.Images.Add(newImage);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã sảy ra lỗi khi cập nhật bản ghi." });
            }

			return RedirectToAction("CreateNews");
		}
		[HttpPost]
		public async Task<IActionResult> UploadImage(IFormFile file)
		{
			var uploadResult = await _cloudinaryService.UploadImage(file);

			if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return BadRequest("Could not upload image to Cloudinary");
			}

			var url = uploadResult.Url.ToString();
			return Json(new { location = url });
		}
	}
}
