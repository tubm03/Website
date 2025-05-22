namespace PetStoreProject.ViewModels
{
    public class NewsViewModel
    {
        public int NewsId { get; set; }

        public string Title { get; set;}

        public string Description { get; set;}

        public string Content { get; set;}

        public DateOnly DateOnly { get; set;}

        public string url_thumnail { get; set;} 

        public int tagId { get; set;}
        public string tagName { get; set;}

        public string employeeName { get; set;}

        public bool status { get; set;}

    }
}
