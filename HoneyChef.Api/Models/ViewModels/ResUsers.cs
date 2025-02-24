using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITCore.Models.ViewModels
{
    public class ResUsers
    {
    }

    public class LoginModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string keycode { get; set; }
        public string? keycheck { get; set; }
    }

    public partial class UserLogin
    {
        public long userId { get; set; }
        public long? userMapId { get; set; }
        public int? languageId { get; set; }
        public int? departmentId { get; set; }
        public string? departmentName { get; set; }
        public int? positionId { get; set; }
        public int? type { get; set; }
        public byte? sex { get; set; }
        public string? code { get; set; }
        public int? countLogin { get; set; }
        public string? fullName { get; set; }
        public string? avata { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string? address { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public DateTime? birthday { get; set; }
        public string? cardId { get; set; }
        public int? badgeNotification { get; set; }
        public int? badgeWaring { get; set; }
        public int? status { get; set; }
        public int? roleMax { get; set; }
        public byte? roleLevel { get; set; }
        public bool isRoleGroup { get; set; }
        public bool? isPhoneConfirm { get; set; }
        public string access_token { get; set; }
        public string access_key { get; set; }
        public string? registerCode { get; set; }
        public string? baseApi { get; set; }
        public string? baseUrlImg { get; set; }
        public string? baseUrlImgThumb { get; set; }
        public string? baseUrlFile { get; set; }
        public List<MenuDTO>? listMenus { get; set; }
    }

    public partial class MenuDTO
    {
        public int MenuId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int MenuParent { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string ActiveKey { get; set; }
        public int? Status { get; set; }
        public bool? IsParamRoute { get; set; }
        public List<MenuDTO>? listMenus { get; set; }
    }

    public partial class UserChangePass
    {
        public long UserId { get; set; }
        public string? PasswordOld { get; set; }
        public string? PasswordNew { get; set; }
        public string? UserName { get; set; }
    }

    public partial class GetUserByUserName
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public string Avata { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public long? UserCreatedId { get; set; }
        public long? UserUpdatedId { get; set; }
    }

    public partial class UserInfo
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? Code { get; set; }
        public string? Avata { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int? Status { get; set; }
    }
    public partial class UserRegister
    {
        public string? PhoneMain { get; set; }
        public byte? LanguageId { get; set; }
    }

    public partial class UserChangeLanguage
    {
        public long UserId { get; set; }
        public int LanguageId { get; set; }
    }

    public partial class UserChangePass
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }

    public partial class UserChangeInfo
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string? Avata { get; set; }
        public byte? Sex { get; set; }
        public string? Birthday { get; set; }
        public string? CardId { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public partial class UserRequildPass
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }



}
