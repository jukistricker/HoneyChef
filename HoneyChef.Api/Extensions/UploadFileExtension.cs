using IOITCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace HoneyChef.Api.Extensions
{
    public class UploadFileExtension
    {
        public class FileDetail
        {
            public string name { get; set; }
            public string url { get; set; }
        }

        public class VerifyImageRequest
        {
            public string img_front { get; set; }
            public string? img_face { get; set; }
        }

        private readonly IConfiguration _config;

        public UploadFileExtension(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<Object> UpLoadFiles (List<IFormFile> files)
        {
            if (files != null && files.Count() > 0)
            {
                long size = files.Sum(f => f.Length);
                List<FileDetail> filePaths = new List<FileDetail>();
                int i = 0;
                string url = _config.GetSection("config")["Url"];
                string uploadFolder = "";
                uploadFolder = _config.GetSection("config")["Directory"] + "\\loyalty";
                foreach (var formFile in files)
                {
                    i += 1;
                    if (formFile.Length > 0)
                    {
                        try
                        {
                            //name folder
                            if (!Directory.Exists(uploadFolder))
                            {
                                Directory.CreateDirectory(uploadFolder);
                            }
                            string file_name = StringExtension.RemoveDiacriticUrls(formFile.FileName).ToUpper();

                            //name file
                            var uniqueFileName = Path.GetFileNameWithoutExtension(file_name).Replace(" ", "") + "_" + DateTime.Now.ToString("ssmmhhddMMyyyy") + i.ToString() + Path.GetExtension(formFile.FileName);
                            //path
                            var filePath = Path.Combine(uploadFolder, uniqueFileName);
                            if (!String.IsNullOrEmpty("f"))
                            {
                                var x = new FileDetail();
                                x.name = uniqueFileName;
                                x.url = url + "loyalty" + "/" + uniqueFileName;
                                filePaths.Add(x);
                            }
                            else
                            {
                                var x = new FileDetail();
                                x.name = uniqueFileName;
                                x.url = url + uniqueFileName;
                                filePaths.Add(x);
                            }

                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            return new { status = 400 , code = ex.Message};
                        }

                    }
                }
                return new {status = 200, data = filePaths };
            }
            else
            {
                return new { status = 400, code = "EMPTY_FILE" };
            }
        }
    }
}
