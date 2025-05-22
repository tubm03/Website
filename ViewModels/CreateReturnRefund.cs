using System.Drawing;

namespace PetStoreProject.ViewModels
{
    public class CreateReturnRefund
    {
        public long OrderId { get; set; }
        public string reasonReturn { get; set; }

        public List<string> images { get; set; }
    }
}
