using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using HoneyChef.Api.Entities;
using HoneyChef.Api.Models.ViewModels;
using IOITCore.Models.Common;
using IOITCore.Models.ViewModels.Bases;

namespace HoneyChef.Api.Services.Interfaces
{
    public interface ICountryServices
    {
        public Task<DefaultResponse> GetByPage(UserClaims userClaims, FilteredPagination paging);
        public Task<DefaultResponse> GetByPageNotRole(UserClaims userClaims, FilteredPagination paging);
        public Task<DefaultResponse> GetById(UserClaims userClaims, long id);
        public Task<DefaultResponse> SaveData(UserClaims userClaims, CountryDTO entity);
        public Task<DefaultResponse> DeleteData(UserClaims userClaims, long id);

    }
}