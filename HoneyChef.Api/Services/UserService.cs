using AutoMapper;
using IOITCore.Models.ViewModels;
using IOITCore.Repositories.Interfaces;
using IOITCore.Services.Interfaces;
using IOITCore.Exceptions;
using IOITCore.Constants;
using static IOITCore.Enums.ApiEnums;
using IOITCore.Services.Common;
using IOITCore.Enums;
using IOITCore.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IOITCore.Models.Common;
using Microsoft.EntityFrameworkCore;
using IOITCore.Models.ViewModels.Bases;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using DrawingColor = SixLabors.ImageSharp.Color;
using ImageSharpPointF = SixLabors.ImageSharp.PointF;
using SystemFontsAlias = SixLabors.Fonts.SystemFonts;
using Azure.Core;


namespace IOITCore.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IFunctionRoleRepository _funcRoleRepo;
        private readonly IFunctionRepository _funcRepo;
        private readonly IRoleRepository _roleRepo;

        public UserService(
            IConfiguration configuration,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserRepository userRepo,
            IUserRoleRepository userRoleRepo,
            IFunctionRoleRepository funcRoleRepo,
            IFunctionRepository funcRepo,
            IRoleRepository roleRepo)
        {
            _configuration = configuration;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _funcRoleRepo = funcRoleRepo;
            _funcRepo = funcRepo;
            _roleRepo = roleRepo;
        }

        public async Task<DefaultResponse> GetCapChar()
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                int keylen = 6;
                string caplen = _configuration["AppSettings:CaptChaLen"];
                if (caplen != null)
                    keylen = UtilsService.ConvertInt(caplen);
                if (keylen == 0)
                    keylen = 6;
                string key = UtilsService.randomString(keylen);
                string keyencode = UtilsService.MD5Hash(key);

                // Tạo ảnh sử dụng ImageSharp
                int width = 150;
                int height = 56;
                using (var img = new Image<Rgba32>(width, height))
                {
                    img.Mutate(ctx => ctx.Fill(DrawingColor.White));

                    var font = SystemFontsAlias.CreateFont("Arial", 24);
                    img.Mutate(ctx => ctx.DrawText(key, font, DrawingColor.Black, new ImageSharpPointF(25, 20)));

                    using (var ms = new MemoryStream())
                    {
                        img.SaveAsPng(ms);
                        string st = Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.InsertLineBreaks);
                       
                        def.metadata = keyencode;
                        def.data = st;
                        def.meta = new Meta(200, "OK");
                        return def;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CommonException(ApiConstants.MessageResource.ERROR_500_MESSAGE, 500);
            }
        }

        public bool Validate(string key, string cookies)
        {
            string keyencode = UtilsService.MD5Hash(key.Trim().ToUpper());
            bool res = false;
            if (keyencode == cookies)
            {
                res = true;
            }
            return res;
        }

        public async Task<UserLogin> Login(LoginModel loginModel)
        {
            DefaultResponse def = new DefaultResponse();
            if (loginModel != null)
            {
                //bool validatecap = Validate(loginModel.keycode, loginModel.keycheck);
                //if (!validatecap)
                //{
                //    throw new CommonException("Mã xác thực không đúng!", 213, ApiConstants.ErrorCode.ERROR_AUTHORIZED);
                //}
                string username = loginModel.email;

                var user = _userRepo.All().Where(e => e.UserName == username && e.Status != EntityStatus.DELETED).ToList();
                if (user.Count > 0)
                {
                    string password = user.FirstOrDefault().KeyLock.Trim() + user.FirstOrDefault().RegEmail.Trim() + user.FirstOrDefault().Id + loginModel.password.Trim();
                    password = UtilsService.GetMD5Hash(password);
                    var userLogin = _userRepo.All().Where(e => e.UserName == username && e.Password == password && e.Status != EntityStatus.DELETED).Select(e => new UserLogin()
                    {
                        userId = e.Id,
                        userName = e.UserName,
                        email = e.Email,
                        fullName = e.FullName,
                        avata = e.Avata,
                        address = e.Address,
                        departmentId = e.DepartmentId,
                        positionId = e.PositionId,
                        departmentName = "",
                        password = e.Password,
                        phone = e.Phone,
                        roleMax = e.RoleMax,
                        roleLevel = e.RoleLevel,
                        isRoleGroup = e.IsRoleGroup != null ? (bool)e.IsRoleGroup : true,
                        status = (int)e.Status,
                        baseApi = _configuration["AppSettings:baseApi"],
                        baseUrlImg = _configuration["AppSettings:baseUrlImg"],
                        baseUrlImgThumb = _configuration["AppSettings:baseUrlImgThumb"],
                        baseUrlFile = _configuration["AppSettings:baseUrlFile"],
                    }).FirstOrDefault();

                    if (userLogin != null)
                    {
                        //check if user lock
                        if (userLogin.status == (int)EntityStatus.LOCK)
                        {
                            throw new CommonException("Tài khoản đã bị khóa!", 223, ApiConstants.ErrorCode.USER_LOCKED);
                        }

                        var role = _roleRepo.All().Where(r => r.Id == userLogin.roleMax && r.Status != EntityStatus.DELETED).FirstOrDefault();
                        userLogin.code = role != null ? role.Code : "";

                        var userId = userLogin.userId;
                        List<MenuDTO> listFunctionRole = new List<MenuDTO>();
                        //lấy danh sách quyền theo chức năng, nếu danh sách quyền theo chức năng null thì lấy
                        //danh sách quyền theo nhóm quyền

                        if (!userLogin.isRoleGroup)
                        {
                            var listFR = (from fr in _funcRoleRepo.All()
                                          join f in _funcRepo.All() on fr.FunctionId equals f.Id
                                          where fr.TargetId == userLogin.userMapId
                                             && fr.Type == (int)ApiEnums.TypeFunction.FUNCTION_USER
                                             && fr.Status == ApiEnums.EntityStatus.NORMAL
                                             && f.Status != ApiEnums.EntityStatus.DELETED
                                          select new
                                          {
                                              fr.Id,
                                              fr.FunctionId,
                                              fr.Type,
                                              fr.Status,
                                              fr.ActiveKey,
                                              f.Location,
                                              f.Code,
                                              f.Name,
                                              f.Url,
                                              f.Icon,
                                              f.FunctionParentId,
                                              f.IsParamRoute
                                          }).OrderBy(e => e.Location).ToList();
                            foreach (var itemFR in listFR)
                            {
                                //check exits
                                var fr = listFunctionRole.Where(e => e.MenuId == itemFR.FunctionId).ToList();
                                if (fr.Count > 0)
                                {
                                    string key1 = fr.FirstOrDefault().ActiveKey;
                                    if (fr.FirstOrDefault().ActiveKey != itemFR.ActiveKey)
                                    {
                                        key1 = CheckRoleService.PlusActiveKey(fr.FirstOrDefault().ActiveKey, itemFR.ActiveKey);
                                    }
                                    fr.FirstOrDefault().ActiveKey = key1;
                                }
                                else
                                {
                                    Function function = _funcRepo.All().Where(e => e.Id == itemFR.FunctionId).FirstOrDefault();
                                    if (function != null)
                                    {
                                        MenuDTO menu = new MenuDTO();
                                        menu.MenuId = itemFR.FunctionId;
                                        menu.Code = function.Code;
                                        menu.Name = function.Name;
                                        menu.Url = function.Url;
                                        menu.Icon = function.Icon;
                                        menu.MenuParent = (int)function.FunctionParentId;
                                        menu.ActiveKey = itemFR.ActiveKey;
                                        menu.IsParamRoute = function.IsParamRoute;
                                        listFunctionRole.Add(menu);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //get list user role
                            var userRole = _userRoleRepo.All().Where(e => e.UserId == userId && e.Status == EntityStatus.NORMAL).ToList();
                            //get list function role
                            foreach (var item in userRole)
                            {
                                var listFRR = (from fr in _funcRoleRepo.All()
                                               join f in _funcRepo.All() on fr.FunctionId equals f.Id
                                               where fr.TargetId == item.RoleId
                                                  && fr.Type == (int)ApiEnums.TypeFunction.FUNCTION_ROLE
                                                  && fr.Status == ApiEnums.EntityStatus.NORMAL
                                                  && f.Status == ApiEnums.EntityStatus.NORMAL
                                               select new
                                               {
                                                   fr.Id,
                                                   fr.FunctionId,
                                                   fr.Type,
                                                   fr.Status,
                                                   fr.ActiveKey,
                                                   f.Location,
                                                   f.Code,
                                                   f.Name,
                                                   f.Url,
                                                   f.Icon,
                                                   f.FunctionParentId,
                                                   f.IsParamRoute
                                               }).OrderBy(e => e.Location).ToList();
                                foreach (var itemFR in listFRR)
                                {
                                    //check exits
                                    var fr = listFunctionRole.Where(e => e.MenuId == itemFR.FunctionId).ToList();
                                    if (fr.Count > 0)
                                    {
                                        string key1 = fr.FirstOrDefault().ActiveKey;
                                        if (fr.FirstOrDefault().ActiveKey != itemFR.ActiveKey)
                                        {
                                            key1 = CheckRoleService.PlusActiveKey(fr.FirstOrDefault().ActiveKey, itemFR.ActiveKey);
                                        }
                                        fr.FirstOrDefault().ActiveKey = key1;
                                    }
                                    else
                                    {
                                        Function function = _funcRepo.All().Where(e => e.Id == itemFR.FunctionId).FirstOrDefault();
                                        if (function != null)
                                        {
                                            MenuDTO menu = new MenuDTO();
                                            menu.MenuId = itemFR.FunctionId;
                                            menu.Code = function.Code;
                                            menu.Name = function.Name;
                                            menu.Url = function.Url;
                                            menu.Icon = function.Icon;
                                            menu.MenuParent = (int)function.FunctionParentId;
                                            menu.ActiveKey = itemFR.ActiveKey;
                                            menu.IsParamRoute = function.IsParamRoute;
                                            listFunctionRole.Add(menu);
                                        }
                                    }
                                }
                            }
                        }

                        string access_key = "";
                        int count = listFunctionRole.Count;
                        if (count > 0)
                        {
                            for (int i = 0; i < count - 1; i++)
                            {
                                if (listFunctionRole[i].ActiveKey != "000000000")
                                {
                                    access_key += listFunctionRole[i].Code + ":" + listFunctionRole[i].ActiveKey + "-";
                                }
                            }

                            access_key = access_key + listFunctionRole[count - 1].Code + ":" + listFunctionRole[count - 1].ActiveKey;
                        }

                        userLogin.access_key = access_key;
                        userLogin.listMenus = CheckRoleService.CreateMenu(listFunctionRole, 0);

                        var claims = new List<Claim>
                                {
                                    new Claim(JwtRegisteredClaimNames.Email, userLogin.email),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(ClaimTypes.NameIdentifier, userLogin.userId.ToString()),
                                    new Claim(ClaimTypes.Name, userLogin.fullName),
                                        new Claim("UserId", userLogin.userId != null ? userLogin.userId.ToString() : ""),
                                        new Claim("Name", userLogin.fullName != null ? userLogin.fullName.ToString() : ""),
                                        new Claim("RoleMax", userLogin.roleMax != null ? userLogin.roleMax.ToString() : ""),
                                        new Claim("RoleLevel", userLogin.roleLevel != null ? userLogin.roleLevel.ToString() : ""),
                                        new Claim("AccessKey", access_key != null ? access_key : ""),
                                };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:JwtKey"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["AppSettings:JwtExpireDays"]));

                        var token = new JwtSecurityToken(
                            _configuration["AppSettings:JwtIssuer"],
                            _configuration["AppSettings:JwtIssuer"],
                            claims,
                            expires: expires,
                            signingCredentials: creds
                        );

                        userLogin.access_token = new JwtSecurityTokenHandler().WriteToken(token);
                        def.data = userLogin;
                        def.meta = new Meta(200, "success");
                        return userLogin;
                    }
                    else
                    {
                        //check if email exist
                        var existed = _userRepo.All().Where(e => e.UserName == username && e.Status != EntityStatus.DELETED).FirstOrDefault();
                        if (existed != null)
                        {
                            throw new CommonException("Tài khoản hoặc mật khẩu không chính xác!", 213, ApiConstants.ErrorCode.USER_PASSWORD_INCORRECT);
                        }
                        else
                        {
                            throw new CommonException("Tài khoản hoặc mật khẩu không chính xác!", 404, ApiConstants.ErrorCode.USER_PASSWORD_INCORRECT);
                        }
                    }
                }
                else
                {
                    throw new BadRequestException("Tài khoản không tồn tại trong hệ thống", 400, ApiConstants.ErrorCode.USERNAME_INVALID);
                }
            }
            else
            {
                throw new ValidationException("Invalid model", ApiConstants.ErrorCode.ERROR_VALIDATION);
            }
        }

        public async Task<DefaultResponse> ChangePass(UserClaims userClaims, long id, UserChangePass data)
        {
            DefaultResponse def = new DefaultResponse();
            //var identity = (ClaimsIdentity)User.Identity;
            //long userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            //string name = identity.Claims.Where(c => c.Type == "Name").Select(c => c.Value).SingleOrDefault();

            //string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)ApiEnums.Action.UPDATE))
            //{
            //    def.meta = new Meta(222, "No permission");
            //    return Ok(def);
            //}
            try
            {

                var user = await _userRepo.All().Where(e => e.Id == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CommonException("Không tìm thấy tài khoản trên hệ thống!", 404);
                }

                //check password old
                string password = user.KeyLock.Trim() + user.RegEmail.Trim() + user.Id + data.PasswordOld.Trim();

                password = UtilsService.GetMD5Hash(password);

                if (user.Password.Trim() != password)
                {
                    throw new CommonException("Mật khẩu cũ không chính xác!", 213);
                }

                //using (var transaction = _context.Database.BeginTransaction())
                //{
                //update user
                string passwordNew = user.KeyLock.Trim() + user.RegEmail.Trim() + user.Id + data.PasswordNew.Trim();
                passwordNew = UtilsService.GetMD5Hash(passwordNew);

                user.Password = passwordNew;
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = userClaims.fullName;
                user.UpdatedById = userClaims.userId;
                _userRepo.Update(user);
                def.meta = new Meta(200, ApiConstants.MessageResource.UPDATE_SUCCESS);
                await _unitOfWork.CommitChangesAsync();

                def.data = user;
                return def;
                
            }
            catch (Exception e)
            {
                throw new CommonException(ApiConstants.MessageResource.ERROR_500_MESSAGE, 500);
                //def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
                //return Ok(def);
            }
        }

        public async Task<DefaultResponse> AdminChangePass(UserClaims userClaims, long id, UserChangePass data)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            ////var identity = (ClaimsIdentity)User.Identity;
            ////long userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            ////string access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault();
            ////int companyId = int.Parse(identity.Claims.Where(c => c.Type == "CompanyId").Select(c => c.Value).SingleOrDefault());
            ////string fullname = identity.Claims.Where(c => c.Type == "FullName").Select(c => c.Value).SingleOrDefault();
            //if (!CheckRole.CheckRoleByCode(access_key, functionCode, (int)ApiEnums.Action.UPDATE))
            //{
            //    def.meta = new Meta(222, ApiConstants.MessageResource.NOPERMISION_UPDATE_MESSAGE);
            //    return Ok(def);
            //}
            try
            {
                //if (!ModelState.IsValid || companyId != idc)
                //{
                //    def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
                //    return Ok(def);
                //}

                //if (id != data.UserId)
                //{
                //    def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
                //    return Ok(def);
                //}

                var user = await _userRepo.All().Where(e => e.Id == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CommonException("Không tìm thấy tài khoản trên hệ thống!", 404);
                }

                //using (var transaction = _context.Database.BeginTransaction())
                //{
                //update user
                string passwordNew = user.KeyLock.Trim() + user.RegEmail.Trim() + user.Id + data.PasswordNew.Trim();
                passwordNew = UtilsService.GetMD5Hash(passwordNew);

                user.Password = passwordNew;
                user.UpdatedAt = DateTime.Now;
                user.UpdatedById = userClaims.userId;
                user.UpdatedBy = userClaims.fullName;
                _userRepo.Update(user);
                def.meta = new Meta(200, ApiConstants.MessageResource.UPDATE_SUCCESS);
                await _unitOfWork.CommitChangesAsync();

                def.data = user;
                return def;
                //_context.Entry(user).State = EntityState.Modified;

                //try
                //{
                //    await _context.SaveChangesAsync();
                //    if (user.Id > 0)
                //    {
                //        transaction.Commit();

                //try
                //{
                //    //Gửi mail
                //    if (user.Email != null)
                //    {
                //        var messageMail = new DtoNotificationSendEmailQueue(
                //            user.FullName,
                //            new String[] { user.Email },
                //            "Đổi mật khẩu - QLDA",
                //            "Tài khoản của bạn đã được đổi mật khẩu trên hệ thống quản lý dự án",
                //            EmailTypes.DEFAULT,
                //            string.Empty
                //        );

                //        //await _identityProducer.SendEmail(messageMail);
                //    }
                //    //Ghi log hệ thống
                //    //var log_system = new List<DtoNotificationFirebaseCreateQueue>();
                //    //log_system.Add(new DtoNotificationFirebaseCreateQueue(
                //    //    "Admin đổi mật khẩu",
                //    //    "User",
                //    //    user.Id.ToString(),
                //    //    $"Tài khoản {name} vừa đổi mật khẩu cho người dùng {user.FullName}",
                //    //    (int)ApiEnums.Action.UPDATE,
                //    //    -1,
                //    //    JsonConvert.SerializeObject(user),
                //    //    IpAddress(),
                //    //    0,
                //    //    TypeAction.ACTION,
                //    //    -1,
                //    //    userId,
                //    //    name
                //    //));
                //    //await _identityProducer.SendNotifyToFirebase(log_system);
                //}
                //catch { }

                ////}
                ////else
                ////    transaction.Rollback();
                ////def.meta = new Meta(200, "Đổi mật khẩu cho tài khoản " + data.UserName + " thành công!");
                ////return Ok(def);
                //}
                //catch (DbUpdateConcurrencyException e)
                //{
                //    transaction.Rollback();
                //    log.Error("DbUpdateConcurrencyException:" + e);
                //    if (!UserExists(id))
                //    {
                //        def.meta = new Meta(404, "Không tìm thấy tài khoản trên hệ thống!");
                //        return Ok(def);
                //    }
                //    else
                //    {
                //        def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
                //        return Ok(def);
                //        throw;
                //    }
                //}
                //}
                //}
            }
            catch (Exception e)
            {
                throw new CommonException(ApiConstants.MessageResource.ERROR_500_MESSAGE, 500); ;
            }
        }

        public async Task<DefaultResponse> LockUser(UserClaims userClaims, long id, byte k)
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                var user = await _userRepo.All().Where(e => e.Id == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CommonException("Không tìm thấy tài khoản trên hệ thống!", 404);
                }
                if (user.Id != id)
                {
                    throw new BadRequestException(ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
                }

                user.UpdatedAt = DateTime.Now;
                user.UpdatedById = userClaims.userId;
                user.UpdatedBy = userClaims.fullName;
                user.Status = (ApiEnums.EntityStatus)k;
                _userRepo.Update(user);
                def.meta = new Meta(200, ApiConstants.MessageResource.UPDATE_SUCCESS);
                await _unitOfWork.CommitChangesAsync();

                def.data = user;
                return def;


            }
            catch (Exception e)
            {
                throw new CommonException(ApiConstants.MessageResource.ERROR_500_MESSAGE, 500);
            }
        }

        public async Task<DefaultResponse> InfoUser(long id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {

                var user = await _userRepo.All().Where(e => e.Id == id && e.Status != ApiEnums.EntityStatus.DELETED).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CommonException("Không tìm thấy tài khoản trên hệ thống!", 404);
                }

                def.data = user;
                def.meta = new Meta(200, ApiConstants.MessageResource.UPDATE_SUCCESS);
                return def;
            }
            catch (Exception ex)
            {
                throw new CommonException(ApiConstants.MessageResource.ERROR_500_MESSAGE, 500);
            }
        }

        public async Task<DefaultResponse> ChangeInfoUser(UserClaims userClaims, UserInfo data)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
                //    return Ok(def);
                //}

                //if (data.UserId != userId)
                //{
                //    def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
                //    return Ok(def);
                //}

                var user = await _userRepo.All().Where(e => e.Id == data.UserId && e.Status != ApiEnums.EntityStatus.DELETED).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CommonException("Không tìm thấy tài khoản trên hệ thống!", 404);
                }

                user.FullName = data.FullName;
                user.Email = data.Email;
                user.Code = data.Code;
                //user.Avata = data.Avata != null && data.Avata != "" ? data.Avata : user.Avata;
                user.Avata = data.Avata;
                user.Address = data.Address;
                user.Phone = data.Phone;
                user.UpdatedAt = DateTime.Now;
                user.UpdatedById = userClaims.userId;
                user.UpdatedBy = userClaims.fullName;
                _userRepo.Update(user);
                def.meta = new Meta(200, ApiConstants.MessageResource.UPDATE_SUCCESS);
                await _unitOfWork.CommitChangesAsync();

                def.data = user;
                return def;
            }
            catch (Exception e)
            {
                throw new CommonException(ApiConstants.MessageResource.ERROR_500_MESSAGE, 500);
            }
        }


    }
}
