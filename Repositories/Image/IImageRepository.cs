namespace PetStoreProject.Repositories.Image
{
    public interface IImageRepository
    {
        public Task<string> CreateImage(string imageData);
        public List<String> GetImagesByReturnRefundId(int returnRefundId);
    }
}
