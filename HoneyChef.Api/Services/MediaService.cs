using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HoneyChef.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HoneyChef.Api.Services
{
    public class MediaService : IMediaServices
    {
        private readonly Cloudinary _cloudinary;

        public MediaService(IOptions<CloudinarySettings> cloudinaryConfig)
        {
            var account = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File không hợp lệ.");
            }

            string uniqueFileName = GenerateUniqueFileName(file.FileName);

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(uniqueFileName, stream),
                PublicId = $"shopweb/{Path.GetFileNameWithoutExtension(uniqueFileName)}", // Lưu file với tên mới (không có đuôi)
                Transformation = new Transformation().Crop("limit").Width(800).Height(800),
                Folder = "HoneyChef" // Tạo thư mục trên Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Lỗi upload ảnh: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl.ToString();
        }


        public Task<DeletionResult> DeleteImgAsync(string publicId)
        {
            throw new NotImplementedException();
        }


        private string GenerateUniqueFileName(string originalFileName, int length = 16)
        {
            string extension = Path.GetExtension(originalFileName); // Lấy đuôi file (jpg, png, ...)
            string guid = Guid.NewGuid().ToString("N"); // Tạo GUID không có dấu "-"

            return $"{guid.Substring(0, length)}{extension}";
        }

    }

    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

}