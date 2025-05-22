namespace PetStoreProject.Repositories.ReturnRefund
{
    public interface IReturnRefundRepository
    {
        public Task CreateNewReturnRefund(ViewModels.CreateReturnRefund returnRefund);

        public List<Models.ReturnRefund> GetReturnRefunds();
        public Task AddImageToReturnRefund(int returnId, ViewModels.CreateReturnRefund returnRefund);

        public Models.ReturnRefund GetReturnRefundById(int returnId);
        public void UpdateReturnRefund(int returnId, string status, string reponseContent);
    }
}
