using IOITCore.Models.Common;
using IOITCore.Models.ViewModels;
using IOITCore.Models.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITCore.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserLogin> Login(LoginModel loginModel);
        Task<DefaultResponse> GetCapChar();
        Task<DefaultResponse> ChangePass(UserClaims userClaims, long id, UserChangePass data);
        Task<DefaultResponse> AdminChangePass(UserClaims userClaims, long id, UserChangePass data);
        Task<DefaultResponse> LockUser(UserClaims userClaims, long id, byte k);
        Task<DefaultResponse> InfoUser(long id);
        Task<DefaultResponse> ChangeInfoUser(UserClaims userClaims, UserInfo data);
    }
}
