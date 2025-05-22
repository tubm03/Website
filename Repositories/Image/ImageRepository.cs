using CloudinaryDotNet.Actions;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Models;

namespace PetStoreProject.Repositories.Image
{
    public class ImageRepository : IImageRepository
    {
        private readonly PetStoreDBContext _context;
        private readonly ICloudinaryService _cloudinary;

        public ImageRepository(PetStoreDBContext context, ICloudinaryService cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public async Task<string> CreateImage(string imageData)
        {
            int maxId = _context.Images.Max(i => i.ImageId);
            int imageId = maxId + 1;
            ImageUploadResult result = await _cloudinary.UploadImage(imageData, "image_" + imageId);

            if (result.Error != null)
            {
                return result.Error.ToString();
            }
            var url = result.Url.ToString();
            var image = new PetStoreProject.Models.Image
            {
                ImageId = imageId,
                ImageUrl = url
            };
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
            return imageId.ToString();
        }

        public List<String> GetImagesByReturnRefundId(int returnRefundId)
        {
            List<String> images = new List<String>();
            var imageList = _context.Images.Where(i => i.ReturnId == returnRefundId).ToList();
            foreach (var image in imageList)
            {
                images.Add(image.ImageUrl);
            }
            return images;
        }
    }
}
