namespace PetStoreProject.ViewModels
{
    public class FeedbackViewModels
    {
        public string CustomerName { get; set; }
        public double Rating { get; set; }
        public string Content { get; set; }
        public string? EmployeeName { get; set; }
        public string? ContentResponse { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateResp { get; set; }
    }
}
