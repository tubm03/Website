namespace PetStoreProject.ViewModels
{
    public class SearchResultViewModel
    {
        public List<SearchViewModel> Results { get; set; }
        public int TotalResults { get; set; }

        public SearchResultViewModel()
        {
            Results = new List<SearchViewModel>();
        }
    }
}
