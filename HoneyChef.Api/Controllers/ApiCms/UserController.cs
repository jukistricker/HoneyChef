using IOITCore.Entities;
using IOITCore.Models.ViewModels;
using IOITCore.Persistence;
using IOITCore.Constants;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Web;
using IOITCore.Models.Common;
using IOITCore.Enums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using IOITCore.Services.Common;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Services.Interfaces;
using IOITCore.Exceptions;
using IOITCore.Repositories.Interfaces;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace IOITCore.Controllers.ApiCms
{
    [Route("api/cms/[controller]")]
    public class UserController : BaseController
    {
        //private readonly IUserService _userService;

        private static readonly ILog log = LogMaster.GetLogger("user", "user");
        private static string functionCode = "QLND";
        private readonly IConfiguration _configuration;
        private readonly IUserService _entityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOfficeService _officeService;
        private readonly IUserRepository _userRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UserController(IConfiguration configuration, IUserService entityService,
            IHttpContextAccessor httpContextAccessor, IOfficeService officeService, IUserRepository userRepository, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _entityService = entityService;
            _httpContextAccessor = httpContextAccessor;
            _officeService = officeService;
            _userRepository = userRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("getCapChar")]
        public async Task<IActionResult> GetCapChar()
        {
            DefaultResponse def = new DefaultResponse();
            def = await _entityService.GetCapChar();
            //Response.Cookies.Append(
            //                "keyencode1", def.metadata + "",
            //                new CookieOptions
            //                {
            //                    Expires = DateTimeOffset.UtcNow.AddYears(1),
            //                    IsEssential = true,
            //                    Path = "/",
            //                    HttpOnly = false,
            //                }
            //            );
            //string test1= Request.Cookies["keyencode1"];
            return Ok(def);
        }

        private string GetKeyCode()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies["keyencode"];
            //return Request.Cookies["keyencode1"];
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            loginModel.keycheck = GetKeyCode();
            return Ok(await _entityService.Login(loginModel));
        }

        [Authorize]
        [HttpPut("changePass/{id}")]
        public async Task<ActionResult> ChangePass(long id, [FromBody] UserChangePass data)
        {
            //DefaultResponse def = new DefaultResponse();
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.UPDATE).data;
            if (!ModelState.IsValid)
            {
                throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }
            if (id != data.UserId)
            {
                throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }
            return Ok(await _entityService.ChangePass(userClaims, id, data));
            //var identity = (ClaimsIdentity)User.Identity;
            //long userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            //string name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).SingleOrDefault();

            ////string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            ////if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)ApiEnums.Action.UPDATE))
            ////{
            ////    def.meta = new Meta(222, "No permission");
            ////    return Ok(def);
            ////}
            //try
            //{
            //    if (!ModelState.IsValid)
            //    {
            //        def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
            //        return Ok(def);
            //    }

            //    if (id != data.UserId)
            //    {
            //        def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
            //        return Ok(def);
            //    }

            //    User user = await _context.Users.FindAsync(id);
            //    if (user == null)
            //    {
            //        def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
            //        return Ok(def);
            //    }

            //    //check password old
            //    string password = user.KeyLock.Trim() + user.RegEmail.Trim() + user.Id + data.PasswordOld.Trim();

            //    password = UtilsService.GetMD5Hash(password);

            //    if (user.Password.Trim() != password)
            //    {
            //        def.meta = new Meta(213, "Mật khẩu cũ không chính xác!");
            //        return Ok(def);
            //    }

            //    using (var transaction = _context.Database.BeginTransaction())
            //    {
            //        //update user
            //        string passwordNew = user.KeyLock.Trim() + user.RegEmail.Trim() + user.Id + data.PasswordNew.Trim();
            //        passwordNew = UtilsService.GetMD5Hash(passwordNew);

            //        user.Password = passwordNew;
            //        user.UpdatedAt = DateTime.Now;
            //        user.UpdatedById = userId;
            //        _context.Entry(user).State = EntityState.Modified;

            //        try
            //        {
            //            await _context.SaveChangesAsync();
            //            if (user.Id > 0)
            //            {
            //                transaction.Commit();
            //            }
            //            else
            //                transaction.Rollback();
            //            def.meta = new Meta(200, "Đổi mật khẩu thành công!");
            //            return Ok(def);
            //        }
            //        catch (DbUpdateConcurrencyException e)
            //        {
            //            transaction.Rollback();
            //            log.Error("DbUpdateConcurrencyException:" + e);
            //            if (!UserExists(id))
            //            {
            //                def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
            //                return Ok(def);
            //            }
            //            else
            //            {
            //                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            //                return Ok(def);
            //                throw;
            //            }
            //        }
            //    }
            //    //}
            //}
            //catch (Exception e)
            //{
            //    def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            //    return Ok(def);
            //}
        }

        [Authorize]
        [HttpPut("adminChangePass/{idc}/{id}")]
        public async Task<ActionResult> adminChangePass(int idc, long id, UserChangePass data)
        {
            //DefaultResponse def = new DefaultResponse();
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.UPDATE).data;
            if (!ModelState.IsValid)
            {
                throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }
            if (id != data.UserId)
            {
                throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }
            return Ok(await _entityService.AdminChangePass(userClaims, id, data));
            ////check role
            //var identity = (ClaimsIdentity)User.Identity;
            //long userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            //string fullname = identity.Claims.Where(c => c.Type == "FullName").Select(c => c.Value).SingleOrDefault();
            ////if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)ApiEnums.Action.UPDATE))
            ////{
            ////    def.meta = new Meta(222, ApiConstants.MessageResource.NOPERMISION_UPDATE_MESSAGE);
            ////    return Ok(def);
            ////}
            //try
            //{
            //    if (!ModelState.IsValid || companyId != idc)
            //    {
            //        def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
            //        return Ok(def);
            //    }

            //    if (id != data.UserId)
            //    {
            //        def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
            //        return Ok(def);
            //    }

            //    User user = await _context.Users.FindAsync(id);
            //    if (user == null)
            //    {
            //        def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
            //        return Ok(def);
            //    }

            //    using (var transaction = _context.Database.BeginTransaction())
            //    {
            //        //update user
            //        string passwordNew = user.KeyLock.Trim() + user.RegEmail.Trim() + user.Id + data.PasswordNew.Trim();
            //        passwordNew = UtilsService.GetMD5Hash(passwordNew);

            //        user.Password = passwordNew;
            //        user.UpdatedAt = DateTime.Now;
            //        user.UpdatedById = userId;
            //        user.UpdatedBy = fullname;
            //        _context.Entry(user).State = EntityState.Modified;

            //        try
            //        {
            //            await _context.SaveChangesAsync();
            //            if (user.Id > 0)
            //            {
            //                transaction.Commit();

            //                //try
            //                //{
            //                //    //Gửi mail
            //                //    if (user.Email != null)
            //                //    {
            //                //        var messageMail = new DtoNotificationSendEmailQueue(
            //                //            user.FullName,
            //                //            new String[] { user.Email },
            //                //            "Đổi mật khẩu - QLDA",
            //                //            "Tài khoản của bạn đã được đổi mật khẩu trên hệ thống quản lý dự án",
            //                //            EmailTypes.DEFAULT,
            //                //            string.Empty
            //                //        );

            //                //        //await _identityProducer.SendEmail(messageMail);
            //                //    }
            //                //    //Ghi log hệ thống
            //                //    //var log_system = new List<DtoNotificationFirebaseCreateQueue>();
            //                //    //log_system.Add(new DtoNotificationFirebaseCreateQueue(
            //                //    //    "Admin đổi mật khẩu",
            //                //    //    "User",
            //                //    //    user.Id.ToString(),
            //                //    //    $"Tài khoản {name} vừa đổi mật khẩu cho người dùng {user.FullName}",
            //                //    //    (int)ApiEnums.Action.UPDATE,
            //                //    //    -1,
            //                //    //    JsonConvert.SerializeObject(user),
            //                //    //    IpAddress(),
            //                //    //    0,
            //                //    //    TypeAction.ACTION,
            //                //    //    -1,
            //                //    //    userId,
            //                //    //    name
            //                //    //));
            //                //    //await _identityProducer.SendNotifyToFirebase(log_system);
            //                //}
            //                //catch { }

            //            }
            //            else
            //                transaction.Rollback();
            //            def.meta = new Meta(200, "Đổi mật khẩu cho tài khoản " + data.UserName + " thành công!");
            //            return Ok(def);
            //        }
            //        catch (DbUpdateConcurrencyException e)
            //        {
            //            transaction.Rollback();
            //            log.Error("DbUpdateConcurrencyException:" + e);
            //            if (!UserExists(id))
            //            {
            //                def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
            //                return Ok(def);
            //            }
            //            else
            //            {
            //                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            //                return Ok(def);
            //                throw;
            //            }
            //        }
            //    }
            //    //}
            //}
            //catch (Exception e)
            //{
            //    def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            //    return Ok(def);
            //}
        }

        [Authorize]
        [Route("lockUser/{idc}/{id}/{k}")]
        [HttpPut]
        public async Task<ActionResult> LockUser(int idc, long id, byte k)
        {
            //DefaultResponse def = new DefaultResponse();
            //var identity = (ClaimsIdentity)User.Identity;
            //long userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            //string fullname = identity.Claims.Where(c => c.Type == "FullName").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)ApiEnums.Action.UPDATE))
            //{
            //    def.meta = new Meta(222, ApiConstants.MessageResource.NOPERMISION_UPDATE_MESSAGE);
            //    return Ok(def);
            //}
            //try
            //{
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.UPDATE).data;
            ////if (!ModelState.IsValid)
            ////{
            ////    def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
            ////    return Ok(def);
            ////}

            ////User user = await _context.Users.FindAsync(id);
            ////if (user == null)
            ////{
            ////    def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
            ////    return Ok(def);
            ////}
            ////if (user.Id != id)
            ////{
            ////    def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
            ////    return Ok(def);
            ////}
            if (!ModelState.IsValid)
            {
                throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }

            return Ok(await _entityService.LockUser(userClaims, id, k));

            //using (var transaction = _context.Database.BeginTransaction())
            //{
            //    user.UpdatedAt = DateTime.Now;
            //    user.UpdatedById = userId;
            //    user.UpdatedBy = fullname;
            //    user.Status = (ApiEnums.EntityStatus)k;
            //    _context.Entry(user).State = EntityState.Modified;

            //    try
            //    {
            //        //Lock a map id
            //        await _context.SaveChangesAsync();
            //        if (user.Id > 0)
            //        {
            //            transaction.Commit();
            //        }
            //        else
            //            transaction.Rollback();
            //        def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
            //        return Ok(def);
            //    }
            //    catch (DbUpdateConcurrencyException e)
            //    {
            //        transaction.Rollback();
            //        log.Error("DbUpdateConcurrencyException:" + e);
            //        if (!UserExists(id))
            //        {
            //            def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
            //            return Ok(def);
            //        }
            //        else
            //        {
            //            def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            //            return Ok(def);
            //            throw;
            //        }
            //    }
            //}
            //}
            //////}
            //////catch (Exception e)
            //////{
            //////    def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            //////    return Ok(def);
            //////}
        }

        [Authorize]
        [HttpGet("infoUser/{id}")]
        public async Task<ActionResult> InfoUser(long id)
        {
            //DefaultResponse def = new DefaultResponse();
            //var identity = (ClaimsIdentity)User.Identity;
            //long userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            //string name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).SingleOrDefault();
            //try
            //{
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.UPDATE).data;
            return Ok(await _entityService.InfoUser(id));
            //    var user = await _context.Users.Where(e => e.Id == id && e.Status != ApiEnums.EntityStatus.DELETED).FirstOrDefaultAsync();

            //    if (user == null)
            //    {
            //        def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
            //        return Ok(def);
            //    }

            //    def.data = user;
            //    def.meta = new Meta(200, "Success");
            //    return Ok(def);
            //}
            //catch (Exception ex)
            //{
            //    log.Error("Exception:" + ex);
            //    def.meta = new Meta(500, "Internal Server Error");
            //    return Ok(def);
            //}
        }

        [Authorize]
        [HttpPut("changeInfoUser")]
        public async Task<ActionResult> ChangeInfoUser(UserInfo data)
        {
            //DefaultResponse def = new DefaultResponse();
            //var identity = (ClaimsIdentity)User.Identity;
            //long userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            //string name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).SingleOrDefault();

            //try
            //{
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.UPDATE).data;

            if (!ModelState.IsValid)
            {
                throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }
            if (data.UserId != userClaims.userId)
            {
                throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }
            return Ok(await _entityService.ChangeInfoUser(userClaims, data));
            //if (data.UserId != userId)
            //{
            //    def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
            //    return Ok(def);
            //}

            //    User user = await _context.Users.FindAsync(data.UserId);
            //    if (user == null)
            //    {
            //        def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
            //        return Ok(def);
            //    }

            //    using (var transaction = _context.Database.BeginTransaction())
            //    {
            //        user.FullName = data.FullName;
            //        user.Email = data.Email;
            //        user.Code = data.Code;
            //        //user.Avata = data.Avata != null && data.Avata != "" ? data.Avata : user.Avata;
            //        user.Avata = data.Avata;
            //        user.Address = data.Address;
            //        user.Phone = data.Phone;
            //        user.UpdatedAt = DateTime.Now;
            //        user.UpdatedById = userId;
            //        _context.Update(user);

            //        try
            //        {
            //            await _context.SaveChangesAsync();
            //            if (user.Id > 0)
            //            {
            //                transaction.Commit();
            //            }
            //            else
            //                transaction.Rollback();
            //            def.meta = new Meta(200, "Thay đổi thông tin tài khoản thành công!");
            //            return Ok(def);
            //        }
            //        catch (DbUpdateConcurrencyException e)
            //        {
            //            transaction.Rollback();
            //            log.Error("DbUpdateConcurrencyException:" + e);
            //            if (!UserExists(user.Id))
            //            {
            //                def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
            //                return Ok(def);
            //            }
            //            else
            //            {
            //                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            //                return Ok(def);
            //                throw;
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            //    return Ok(def);
            //}
        }

        //private string IpAddress()
        //{
        //    if (Request.Headers.ContainsKey("X-Forwarded-For"))
        //        return Request.Headers["X-Forwarded-For"];
        //    else
        //        return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        //}

        //private bool UserExists(long id)
        //{
        //    return _context.Users.Count(e => e.Id == id) > 0;
        //}

        //[HttpPost("ExportWord")]
        //public async Task<ActionResult> ExportWord()
        //{
        //    var data = new List<WordDTO>();

        //    data.Add(new WordDTO("name", "Thien"));
        //    data.Add(new WordDTO("age", "24"));
        //    data.Add(new WordDTO("job", "IT"));

        //    DefaultResponse def = new DefaultResponse();
        //    DevExpress.Drawing.Internal.DXDrawingEngine.ForceSkia();
        //    try
        //    {
        //        string filename = "result" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".docx";

        //        string template = @"Template/test.docx";
        //        string webRootPath = _hostingEnvironment.WebRootPath;
        //        string templatePath = Path.Combine(webRootPath, template);

        //        var path = _officeService.ExportWord(templatePath, data);

        //        if (path != null)
        //        {
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
        //                string fileName = "bien_ban-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".docx";

        //                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
        //                {
        //                    file.CopyTo(ms);
        //                }

        //                try
        //                {
        //                    System.IO.File.Delete(path);
        //                }
        //                catch { }

        //                return File(ms.ToArray(), "application/octet-stream", fileName);
        //            }
        //        }
        //        else
        //        {
        //            def.meta = new Meta(215, "Data file null!");
        //            return Ok(def);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("GetExportWordExample:" + ex.Message);
        //        throw;
        //    }

        //}

        [HttpPost("ImportExcel")]
        public async Task<ActionResult> ImportExcel()
        {
            var def = new DefaultResponse();


            string template = @"Template/test.xlsx";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string templatePath = Path.Combine(webRootPath, template);

            var data = _officeService.ImportExcel(templatePath, 0, 1, 5);

            def.data = data;

            return Ok(def);
        }

        [HttpPost("ExportExcelTemplate")]
        public async Task<ActionResult> ExportExcelTemplate()
        {
            var def = new DefaultResponse();

            string template = @"Template/TemplateTest.xlsx";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string templatePath = Path.Combine(webRootPath, template);

            var User = _userRepository.All().Where(p => p.Status != ApiEnums.EntityStatus.DELETED).ToList();

            var ListFields = new List<string>();
            ListFields.Add("FullName");
            ListFields.Add("UserName");
            ListFields.Add("Email");
            ListFields.Add("Password");

            MemoryStream ms = _officeService.ExportExcelTemplate(templatePath, 0, 5, 1, User, ListFields);
            byte[] byteArrayContent = ms.ToArray();
            return File(ms.ToArray(), "application/vnd.ms-excel", $"User_{DateTime.Now:MM_yyyy_HHmmss}");
        }

        [HttpPost("NoTemplate")]
        public async Task<IActionResult> NoTemplate()
        {
            var def = new DefaultResponse();

            var User = _userRepository.All().Where(p => p.Status != ApiEnums.EntityStatus.DELETED).ToList();

            var ListHeader = new List<string>();
            ListHeader.Add("FullName");
            ListHeader.Add("UserName");
            ListHeader.Add("Email");
            ListHeader.Add("Password");

            var ListFields = new List<string>();
            ListFields.Add("FullName");
            ListFields.Add("UserName");
            ListFields.Add("Email");
            ListFields.Add("Password");


            MemoryStream ms = _officeService.ExportExcelNoTemplate(ListHeader, 0, 5, 1, User, ListFields);
            byte[] byteArrayContent = ms.ToArray();
            return File(ms.ToArray(), "application/vnd.ms-excel", $"User_{DateTime.Now:MM_yyyy_HHmmss}");
        }

    }
}
