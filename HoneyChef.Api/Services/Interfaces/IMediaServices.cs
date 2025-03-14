using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace HoneyChef.Api.Services.Interfaces
{
    public interface IMediaServices
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeleteImgAsync(string publicId);
    }
}