using IOITCore.Models.ViewModels;
using IOITCore.Constants;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IOITCore.Models.Common;
using IOITCore.Enums;
using IOITCore.Services.Common;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Services.Interfaces;
using IOITCore.Exceptions;
using Function = IOITCore.Entities.Function;

namespace IOITCore.Controllers.ApiCms
{
    [Authorize]
    [Route("api/cms/[controller]")]
    [ApiController]
    public class FunctionController : BaseController
    {
        private static readonly ILog log = LogMaster.GetLogger("function", "function");
        private static string functionCode = "QLCN";
        private readonly IFunctionService _entityService;

        public FunctionController(IFunctionService entityService)
        {
            _entityService = entityService;
        }

        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetByPage(userClaims, paging));
        }

        [HttpGet("listFunction")]
        public IActionResult listFunction([FromQuery] int id = 0)
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;

            try
            {
                List<SmallFunctionDTO> functions = new List<SmallFunctionDTO>();
                def.data = _entityService.ListFunction(functions, 0, 0, userClaims.roleMax, id, "", false);
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
                return Ok(def);
            }
        }

        [HttpGet("listFunctionRole")]
        public IActionResult listFunctionRole()
        {
            DefaultResponse def = new DefaultResponse();
            //check role
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;

            try
            {
                List<FunctionDT> functions = new List<FunctionDT>();
                def.data = listFunction(0);
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                return Ok(def);
            }
            catch (Exception e)
            {
                log.Error("Exception" + e);
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
                return Ok(def);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetById(userClaims, id));
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(Function data)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.CREATE).data;
            if (!ModelState.IsValid)
            {
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
