using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoneyChef.Api.Models.ViewModels;
using HoneyChef.Api.Services.Interfaces;
using IOITCore.Controllers;
using IOITCore.Enums;
using IOITCore.Exceptions;
using IOITCore.Models.Common;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Services.Common;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IOITCore.Constants;

namespace HoneyChef.Api.Controllers.ApiCms
{
    [Authorize]
    [Route("api/cms/[controller]")]
    public class CountryController : BaseController
    {
        private static readonly ILog log = LogMaster.GetLogger("country","country");
        private static string functionCode = "QLQG";
        private readonly ICountryServices _entityService;

        public CountryController(ICountryServices countryServices)
        {
            _entityService = countryServices;
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetData(long id)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetById(userClaims, id));
        }

       
        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetByPage(userClaims, paging));
        }

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] CountryDTO data){
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.CREATE).data;
            if(!ModelState.IsValid){
             throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }
            return Ok(await _entityService.SaveData(userClaims, data));
        
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.DELETED).data;
            return Ok(await _entityService.DeleteData(userClaims, id));
        }

    }
}