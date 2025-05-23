namespace PetStoreProject.ViewModels
{
    public class HomeViewModel
    {
        public int NumberOfDogFoods { get; set; }
        public int NumberOfDogAccessories { get; set; }
        public int NumberOfCatFoods { get; set; }
        public int NumberOfCatAccessories { get; set; }

        public List<HomeProductViewModel> DogFoodsDisplayed { get; set; }
        public List<HomeProductViewModel> CatFoodsDisplayed { get; set; }
        public List<HomeProductViewModel> DogAccessoriesDisplayed { get; set; }
        public List<HomeProductViewModel> CatAccessoriesDisplayed { get; set; }
        public List<ServiceViewModel> ServicesDisplayed { get; set; }
        public List<NewsViewModel> NewsDisplayed { get; set; }
    }
}
