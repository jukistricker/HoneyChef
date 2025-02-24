using IOITCore.Entities;
using IOITCore.Models.Common;
using IOITCore.Models.ViewModels;
using IOITCore.Models.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITCore.Services.Interfaces
{
    public interface IUserRoleService
    {
        public Task<DefaultResponse> GetByPage(UserClaims userClaims, FilteredPagination paging);
        public Task<DefaultResponse> GetByPageNotRole(UserClaims userClaims, FilteredPagination paging);
        public Task<DefaultResponse> GetById(UserClaims userClaims, long id);
        public Task<DefaultResponse> SaveData(UserClaims userClaims, UserRoleDT entity);
        public Task<DefaultResponse> DeleteData(UserClaims userClaims, long id);
    }
}
