using PetStoreProject.Models;

namespace PetStoreProject.Areas.Admin.ViewModels
{
    public class ServiceAdditionViewModel
    {
        public int ServiceId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public List<int> WorkingTimes { get; set; }

        public string Subdescription { get; set; }

        public string Description { get; set; }

        public bool IsDelete { get; set; }

        public List<string> Images { get; set; }

        public List<ServiceOption>? ServiceOptions { get; set; }
    }
}
