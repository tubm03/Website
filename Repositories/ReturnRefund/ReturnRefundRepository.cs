using CloudinaryDotNet.Actions;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.ReturnRefund
{
    public class ReturnRefundRepository : IReturnRefundRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly ICloudinaryService _cloudinary;

        public ReturnRefundRepository(PetStoreDBContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinary = cloudinaryService;
        }
        public List<Models.ReturnRefund> GetReturnRefunds()
        {
            return _context.ReturnRefunds.ToList();
        }

        public async Task AddImageToReturnRefund(int returnId, ViewModels.CreateReturnRefund returnRefund)
        {
            // Fetch the max ImageId once before the loop to avoid concurrency issues.
            int maxImageId = (from i in _context.Images
                              orderby i.ImageId descending
                              select i.ImageId).FirstOrDefault();

            foreach (var imageData in returnRefund.images)
            {
                maxImageId++;
                var image = new Models.Image
                {
                    ImageId = maxImageId,
                    ReturnId = returnId,
                    ServiceId = null,
                    NewsId = null,
                    OrderId = null,
                };

                try
                {
                    ImageUploadResult result = await _cloudinary.UploadImage(imageData, "image_" + maxImageId);
                    image.ImageUrl = result.Url.ToString();
                    _context.Images.Add(image);
                }
                catch (Exception ex)
                {
                    // Handle specific exceptions (e.g., upload failures) if necessary.
                    throw new Exception("Error uploading image: " + ex.Message, ex);
                }
            }

            // Save changes after adding all images
            await _context.SaveChangesAsync();
        }

        public async Task CreateNewReturnRefund(ViewModels.CreateReturnRefund returnRefund)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var returnRef = new Models.ReturnRefund
                    {
                        ReasonReturn = returnRefund.reasonReturn,
                        Status = "Yêu cầu trả hàng",
                        ResponseContent = null
                    };

                    _context.ReturnRefunds.Add(returnRef);
                    await _context.SaveChangesAsync();

                    await AddImageToReturnRefund(returnRef.ReturnId, returnRefund);

                    // Commit transaction if all operations succeed
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception or handle it as needed
                    throw new Exception("Error creating return/refund request: " + ex.Message, ex);
                }
            }
        }

        public Models.ReturnRefund GetReturnRefundById(int returnId)
        {
            return _context.ReturnRefunds.Find(returnId);
        }

        public void UpdateReturnRefund(int returnId, string status, string reponseContent)
        {
            var returnRefund = _context.ReturnRefunds.Find(returnId);
            if (returnRefund != null)
            {
                returnRefund.Status = status;
                returnRefund.ResponseContent = reponseContent;
                _context.SaveChanges();
            }
        }
    }
}
