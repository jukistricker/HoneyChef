using IOITCore.Entities;
using IOITCore.Constants;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using IOITCore.Models.Common;
using IOITCore.Enums;
using IOITCore.Services.Common;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Services.Interfaces;
using IOITCore.Exceptions;
using IOITCore.Models.ViewModels;

namespace IOITCore.Controllers.ApiCms
{
    [Authorize]
    [Route("api/cms/[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        private static readonly ILog log = LogMaster.GetLogger("role", "role");
        private static string functionCode = "QLPQ";
        private readonly IRoleService _entityService;

        public RoleController(IRoleService entityService)
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
        public async Task<IActionResult> GetRole(int id)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetById(userClaims, id)); ;
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutRole(int id, Role data)
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

        //        if (id != data.Id)
        //        {
        //            def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
        //            return Ok(def);
        //        }

        //        //Role checkItemExist = _context.Roles.Where(f => f.Id != data.Id && f.Code.Trim() == data.Code.Trim() && f.Status != ApiEnums.EntityStatus.DELETED).FirstOrDefault();
        //        //if (checkItemExist != null)
        //        //{
        //        //    def.meta = new Meta(211, "Code Exist!");
        //        //    return Ok(def);
        //        //}
        //        //Role entity = await _roleService.SaveData(userClaims, data);

        //        //def.meta = new Meta(200, "Success");
        //        //def.data = entity;
        //        return Ok(await _roleService.SaveData(userClaims, data));
        //        //using (var transaction = _context.Database.BeginTransaction())
        //        //{
        //        //    Role data = await _context.Roles.FindAsync(id);
        //        //    if (data == null)
        //        //    {
        //        //        def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_UPDATE_MESSAGE);
        //        //        return Ok(def);
        //        //    }
        //        //    data.Code = role.Code;
        //        //    data.Name = role.Name;
        //        //    data.Note = role.Note;
        //        //    data.LevelRole = role.LevelRole;
        //        //    data.UpdatedById = userClaims.userId;
        //        //    data.UpdatedBy = userClaims.fullName;
        //        //    data.UpdatedAt = DateTime.Now;

        //        //    _context.Entry(data).State = EntityState.Modified;
        //        //    try
        //        //    {
        //        //        await _context.SaveChangesAsync();

        //        //        if (data.Id > 0)
        //        //        {
        //        //            transaction.Commit();
        //        //        }
        //        //        else
        //        //            transaction.Rollback();

        //        //        def.meta = new Meta(200, "Success");
        //        //        def.data = data;
        //        //        return Ok(def);
        //        //    }
        //        //    catch (DbUpdateException e)
        //        //    {
        //        //        transaction.Rollback();
        //        //        log.Error("DbUpdateException:" + e);
        //        //        if (RoleExists(data.Id))
        //        //        {
        //        //            def.meta = new Meta(212, "Exist");
        //        //            return Ok(def);
        //        //        }
        //        //        else
        //        //        {
        //        //            def.meta = new Meta(500, "Internal Server Error");
        //        //            return Ok(def);
        //        //        }
        //        //    }
        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        def.meta = new Meta(500, "Internal Server Error");
        //        return Ok(def);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] Role data)
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
