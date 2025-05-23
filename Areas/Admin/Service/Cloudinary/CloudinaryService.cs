using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace PetStoreProject.Areas.Admin.Service.Cloudinary
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(CloudinaryDotNet.Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> GetBase64Image(string imageUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                byte[] imageData = await httpClient.GetByteArrayAsync(imageUrl);
                var base64 = "data:image/jpg;base64," + Convert.ToBase64String(imageData);
                return base64;
            }
        }

        public async Task<ImageUploadResult> UploadImage(string imageData, string imageId)
        {
            var base64Data = imageData.Split(',')[1];
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("image.jpg", new MemoryStream(imageBytes)),
                PublicId = imageId,
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult;
        }

        public async Task<ImageUploadResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult;
            }
        }
    }
}
