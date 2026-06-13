using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace CSharer.Core.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(string cloudName, string apiKey, string apiSecret)
        {
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadVideo(string videoPath)
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(videoPath),
                Folder = "csharer_uploads"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }
    }
}