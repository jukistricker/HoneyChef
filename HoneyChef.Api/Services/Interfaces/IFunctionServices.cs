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
    public interface IFunctionService
    {
        public Task<DefaultResponse> GetByPage(UserClaims userClaims, FilteredPagination paging);
        public Task<DefaultResponse> GetById(UserClaims userClaims, int id);
        public Task<DefaultResponse> SaveData(UserClaims userClaims, Function entity);
        public Task<DefaultResponse> DeleteData(UserClaims user, int id);
        public List<SmallFunctionDTO> ListFunction(List<SmallFunctionDTO> dt, int functionId, int level,
            int roleMax, int currentId, string keySearch, bool disabled);
    }
}
