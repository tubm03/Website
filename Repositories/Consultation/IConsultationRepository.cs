using PetStoreProject.Areas.Admin.ViewModels;
using PetStoreProject.Areas.Employee.ViewModels;
using PetStoreProject.ViewModels;
using X.PagedList;

namespace PetStoreProject.Repositories.Consultion
{
    public interface IConsultationRepository
    {
        public IPagedList<ConsultationViewForAdmin> GetListConsultation(int? page, int? pageSize);
        int CreateConsultation(ConsultationCreateRequestViewModel consultion);
        public ConsultationViewForAdmin GetDetail(int id);
        public void Reply(int id, string message);
    }
}
