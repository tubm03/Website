using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.News
{
    public interface INewsRepository
    {
        public NewsViewModel GetNewsById(int id);

        public List<NewsViewModel> GetListNews();

        public List<NewsViewModel> GetListNewsForEmployee();

        public List<NewsViewModel> GetListNewsForHomePage();
        public bool UpdateNewsStatus(int newsId);
    }
}
