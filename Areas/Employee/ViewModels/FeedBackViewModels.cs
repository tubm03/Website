namespace PetStoreProject.Areas.Employee.ViewModels
{
    public class FeedBackViewModels
    {
        public int FeedBackId { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public double Rating { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }
        public string? ContentResponse { get; set; }
    }
}
