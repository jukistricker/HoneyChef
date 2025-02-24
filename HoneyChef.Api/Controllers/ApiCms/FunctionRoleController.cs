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
using IOITCore.Services.Common;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Services.Interfaces;
using IOITCore.Exceptions;

namespace IOITCore.Controllers.ApiCms
{
    [Authorize]
    [Route("api/cms/[controller]")]
    [ApiController]
    public class FunctionRoleController : BaseController
    {
        private static readonly ILog log = LogMaster.GetLogger("function-role", "function-role");
        private static string functionCode = "QLPQ";
        private readonly IFunctionRoleService _entityService;

        public FunctionRoleController(IFunctionRoleService entityService)
        {
            _entityService = entityService;
        }

        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetByPage(userClaims, paging));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetById(userClaims, id));
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutFunctionRole(int id, [FromBody] RoleDTO data)
        //{
        //    DefaultResponse def = new DefaultResponse();

        //    //check role
        //    UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.UPDATE).data;
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
        //            return Ok(def);
        //        }

        //        //if (data.UserId != userId)
        //        //{
        //        //    def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
        //        //    return Ok(def);
        //        //}
        //        if (data.LevelRole <= 0 || data.LevelRole >= 256)
        //        {
        //            def.meta = new Meta(212, "Cấp độ quyền chỉ được từ 1 đến 255!");
        //            return Ok(def);
        //        }
        //        if (id != data.Id)
        //        {
        //            def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
        //            return Ok(def);
        //        }

        //        Role current = await _context.Roles.FindAsync(id);
        //        if (current == null)
        //        {
        //            def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_UPDATE_MESSAGE);
        //            return Ok(def);
        //        }

        //        Role checkItemExist = await _context.Roles.Where(f => f.Id != data.Id && f.Code.Trim() == data.Code.Trim() && f.Status != ApiEnums.EntityStatus.DELETED).FirstOrDefaultAsync();
        //        if (checkItemExist != null)
        //        {
        //            def.meta = new Meta(211, "Mã đã tồn tại!");
        //            return Ok(def);
        //        }

        //        using (var transaction = _context.Database.BeginTransaction())
        //        {
        //            current.Code = data.Code.Trim();
        //            current.LevelRole = data.LevelRole;
        //            current.Name = data.Name;
        //            current.Note = data.Note;
        //            current.UpdatedAt = DateTime.Now;
        //            current.UpdatedById = userClaims.userId;
        //            current.UpdatedBy = userClaims.fullName;

        //            try
        //            {
        //                //update list function
        //                foreach (var item in data.listFunction)
        //                {
        //                    var functionNew = _context.FunctionRoles.Where(e => e.TargetId == data.Id
        //                    && e.FunctionId == item.FunctionId
        //                    && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_ROLE
        //                    && e.Status != ApiEnums.EntityStatus.DELETED).ToList();
        //                    //add new
        //                    if (functionNew.Count <= 0)
        //                    {
        //                        FunctionRole functionRole = new FunctionRole();
        //                        functionRole.TargetId = data.Id;
        //                        functionRole.FunctionId = item.FunctionId;
        //                        functionRole.ActiveKey = item.ActiveKey;
        //                        functionRole.Type = (int)ApiEnums.TypeFunction.FUNCTION_ROLE;
        //                        functionRole.CreatedAt = DateTime.Now;
        //                        functionRole.UpdatedAt = DateTime.Now;
        //                        functionRole.CreatedById = userClaims.userId;
        //                        functionRole.UpdatedById = userClaims.userId;
        //                        functionRole.CreatedBy = userClaims.fullName;
        //                        functionRole.UpdatedBy = userClaims.fullName;
        //                        functionRole.Status = ApiEnums.EntityStatus.NORMAL;
        //                        _context.FunctionRoles.Add(functionRole);
        //                    }
        //                    else
        //                    {
        //                        //update
        //                        var functionRoleExit = functionNew.FirstOrDefault();
        //                        functionRoleExit.ActiveKey = item.ActiveKey;
        //                        functionRoleExit.UpdatedAt = DateTime.Now;
        //                        functionRoleExit.UpdatedById = userClaims.userId;
        //                        functionRoleExit.UpdatedBy = userClaims.fullName;
        //                        _context.Entry(functionRoleExit).State = EntityState.Modified;
        //                    }
        //                }

        //                _context.Entry(current).State = EntityState.Modified;
        //                await _context.SaveChangesAsync();

        //                if (current.Id > 0)
        //                {
        //                    transaction.Commit();
        //                }
        //                else
        //                    transaction.Rollback();

        //                def.meta = new Meta(200, ApiConstants.MessageResource.UPDATE_SUCCESS);
        //                def.data = data;
        //                return Ok(def);
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!RoleExists(id))
        //                {
        //                    def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_UPDATE_MESSAGE);
        //                    return Ok(def);
        //                }
        //                else
        //                {
        //                    def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
        //                    return Ok(def);
        //                    throw;
        //                }
        //            }
        //        }
        //        //}
        //    }
        //    catch
        //    {
        //        def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
        //        return Ok(def);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] RoleDTO data)
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
