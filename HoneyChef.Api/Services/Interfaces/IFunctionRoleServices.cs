using IOITCore.Models.Common;
using IOITCore.Models.ViewModels;
using IOITCore.Models.ViewModels.Bases;

namespace IOITCore.Services.Interfaces
{
    public interface IFunctionRoleService
    {
        public Task<DefaultResponse> GetByPage(UserClaims userClaims, FilteredPagination paging);
        public Task<DefaultResponse> GetById(UserClaims userClaims, int id);
        public Task<DefaultResponse> SaveData(UserClaims userClaims, RoleDTO entity);
        public Task<DefaultResponse> DeleteData(UserClaims user, int id);
    }
}
