using Microsoft.EntityFrameworkCore.Migrations;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.News
{
    public class NewsRepository : INewsRepository
    {
        private readonly PetStoreDBContext _dbContext;

        public NewsRepository(PetStoreDBContext petStoreDBContext)
        {
            _dbContext = petStoreDBContext;
        }

        public List<NewsViewModel> GetListNews()
        {
            var listNews = (from img in _dbContext.Images
                            join n in _dbContext.News on img.NewsId equals n.NewsId
                            where n.IsDelete == false
                            select new NewsViewModel
                            {
                                url_thumnail = img.ImageUrl,
                                NewsId = n.NewsId,
                                Title = n.Title,
                                Description = n.Summary,
                                DateOnly = n.DatePosted,
                            }).OrderByDescending(n => n.DateOnly).ToList();
            return listNews;
        }
        public List<NewsViewModel> GetListNewsForHomePage()
        {
            var listNews = (from img in _dbContext.Images
                            join n in _dbContext.News on img.NewsId equals n.NewsId
                            where n.IsDelete == false
                            select new NewsViewModel
                            {
                                url_thumnail = img.ImageUrl,
                                NewsId = n.NewsId,
                                Title = n.Title,
                                Description = n.Summary,
                                DateOnly = n.DatePosted,
                            }).OrderByDescending(n => n.DateOnly).Take(3).ToList();
            return listNews;
        }

        public List<NewsViewModel> GetListNewsForEmployee()
        {
            var listNews = (from img in _dbContext.Images
                            join n in _dbContext.News on img.NewsId equals n.NewsId
                            join e in _dbContext.Employees on n.EmployeeId equals e.EmployeeId
                            join t in _dbContext.TagNews on n.TagId equals t.TagId
                            select new NewsViewModel
                            {
                                employeeName = e.FullName,
                                tagName = t.TagName,
                                url_thumnail = img.ImageUrl,
                                NewsId = n.NewsId,
                                Title = n.Title,
                                status = (bool)n.IsDelete,
                                Description = n.Summary,
                                DateOnly = n.DatePosted,
                            }).OrderByDescending(n => n.DateOnly).ToList();
            return listNews;
        }

        public NewsViewModel GetNewsById(int id)
        {
            var news = (from img in _dbContext.Images
                        join n in _dbContext.News on img.NewsId equals n.NewsId
                        join t in _dbContext.TagNews on n.TagId equals t.TagId
                        where n.NewsId == id
                        select new NewsViewModel
                        {
                            tagId = (int)n.TagId,
                            Description = n.Summary,
                            tagName = t.TagName,
                            url_thumnail = img.ImageUrl,
                            NewsId = id,
                            Title = n.Title,
                            Content = n.Content,
                            DateOnly = n.DatePosted,
                        }).FirstOrDefault();
            return news;
        }

        public bool UpdateNewsStatus(int newsId)
        {
            try
            {
                var news = _dbContext.News.FirstOrDefault(n => n.NewsId == newsId);
                news.IsDelete = !news.IsDelete;

                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status: {ex.Message}");
                return false;
            }
        }
    }
}
