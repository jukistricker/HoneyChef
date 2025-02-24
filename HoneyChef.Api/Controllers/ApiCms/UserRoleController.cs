using AutoMapper;
using IOITCore.Entities;
using IOITCore.Models.ViewModels;
using IOITCore.Persistence;
using IOITCore.Constants;
using IOITCore.Enums;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Web;
using IOITCore.Models.Common;
using IOITCore.Services.Common;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Services.Interfaces;
using IOITCore.Exceptions;

namespace IOITCore.Controllers.ApiCms
{
    [Authorize]
    [Route("api/cms/[controller]")]
    [ApiController]
    public class UserRoleController : BaseController
    {
        private static readonly ILog log = LogMaster.GetLogger("user-role", "user-role");
        private static string functionCode = "QLND";
        private static string functionCodePQGS = "PQGSHD";
        //private readonly IOITDbContext _context;
        //private readonly IMapper _mapper;
        private readonly IUserRoleService _entityService;

        public UserRoleController(IUserRoleService entityService)
        {
            _entityService = entityService;
        }

        [HttpGet("GetByPage")]
        public async Task<IActionResult> GetByPage([FromQuery] FilteredPagination paging)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetByPage(userClaims, paging));
        }

        [HttpGet]
        [Route("GetByPageNotRole")]
        public async Task<IActionResult> GetByPageNotRoleAsync([FromQuery] FilteredPagination paging)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetByPageNotRole(userClaims, paging));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.VIEW).data;
            return Ok(await _entityService.GetById(userClaims, id));
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUserRole(long id, [FromBody] UserRoleDT data)
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

        //        if (id != data.UserId)
        //        {
        //            def.meta = new Meta(400, ApiConstants.MessageResource.BAD_REQUEST_MESSAGE);
        //            return Ok(def);
        //        }

        //        User current = await _context.Users.FindAsync(id);
        //        if (current == null)
        //        {
        //            def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_UPDATE_MESSAGE);
        //            return Ok(def);
        //        }

        //        User checkUserNameExist = _context.Users.Where(f => f.Id != data.UserId && f.UserName == data.UserName && f.Status != ApiEnums.EntityStatus.DELETED).FirstOrDefault();
        //        if (checkUserNameExist != null)
        //        {
        //            def.meta = new Meta(211, "Tài khoản đã tồn tại!");
        //            return Ok(def);
        //        }

        //        using (var transaction = _context.Database.BeginTransaction())
        //        {
        //            //update user
        //            current.FullName = data.FullName;
        //            current.Code = data.Code;
        //            current.Phone = data.Phone;
        //            current.Email = data.Email != null ? data.Email.Trim().ToLower() : "";
        //            current.Address = data.Address;
        //            current.Avata = data.Avata;
        //            current.BranchId = data.BranchId;
        //            current.DepartmentId = data.DepartmentId;
        //            current.Type = data.Type;
        //            current.PositionId = data.PositionId;
        //            current.IsRoleGroup = data.IsRoleGroup != null ? data.IsRoleGroup : true;
        //            current.UpdatedAt = DateTime.Now;
        //            current.UpdatedById = userClaims.userId;
        //            current.UpdatedBy = userClaims.fullName;
        //            _context.Entry(current).State = EntityState.Modified;

        //            try
        //            {
        //                //role old
        //                byte levelOld = (byte)current.RoleLevel;
        //                // role
        //                var checkRole = false;
        //                byte level = 99;
        //                int max = 9999;
        //                //update list role
        //                //add new
        //                if (data.listRole != null)
        //                {
        //                    foreach (var item in data.listRole)
        //                    {
        //                        var role = _context.Roles.Find(item.RoleId);
        //                        if (role != null)
        //                        {
        //                            var userRoleNew = _context.UserRoles.Where(e => e.UserId == data.UserId && e.RoleId == item.RoleId && e.Status != ApiEnums.EntityStatus.DELETED).ToList();
        //                            if (userRoleNew.Count <= 0)
        //                            {
        //                                UserRole userRole = new UserRole();
        //                                userRole.RoleId = item.RoleId;
        //                                userRole.UserId = data.UserId;
        //                                userRole.Status = ApiEnums.EntityStatus.NORMAL;
        //                                _context.UserRoles.Add(userRole);
        //                            }
        //                            //check role
        //                            if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
        //                                checkRole = true;
        //                            //
        //                            if (role.LevelRole < level)
        //                            {
        //                                level = (byte)role.LevelRole;
        //                                max = role.Id;
        //                            }
        //                        }
        //                    }
        //                }
        //                //delete old
        //                var listUserRole = _context.UserRoles.Where(e => e.UserId == data.UserId && e.Status != ApiEnums.EntityStatus.DELETED).ToList();
        //                foreach (var item in listUserRole)
        //                {
        //                    var listNew = data.listRole.Where(e => e.RoleId == item.RoleId).ToList();
        //                    if (listNew.Count() <= 0)
        //                    {
        //                        UserRole userRoleExit = await _context.UserRoles.FindAsync(item.Id);
        //                        userRoleExit.Status = ApiEnums.EntityStatus.DELETED;
        //                        _context.Entry(userRoleExit).State = EntityState.Modified;
        //                    }
        //                    else
        //                    {
        //                        //Check xem có phải quyền giám sát ko
        //                        var role = _context.Roles.Find(item.RoleId);
        //                        if (role != null)
        //                        {
        //                            //check role
        //                            if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
        //                                checkRole = true;
        //                        }
        //                    }
        //                }

        //                //update quyền cao nhất và cấp cao nhất của user
        //                current.RoleLevel = level;
        //                current.RoleMax = max;
        //                _context.Entry(current).State = EntityState.Modified;

        //                //update list function
        //                if (data.listFunction != null)
        //                {
        //                    foreach (var item in data.listFunction)
        //                    {
        //                        var functionNew = _context.FunctionRoles.Where(e => e.TargetId == data.UserId
        //                        && e.FunctionId == item.FunctionId
        //                        && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_USER
        //                        && e.Status != ApiEnums.EntityStatus.DELETED).ToList();
        //                        //add new
        //                        if (functionNew.Count <= 0)
        //                        {
        //                            FunctionRole functionRole = new FunctionRole();
        //                            functionRole.TargetId = data.UserId;
        //                            functionRole.FunctionId = item.FunctionId;
        //                            functionRole.ActiveKey = item.ActiveKey;
        //                            functionRole.Type = (int)ApiEnums.TypeFunction.FUNCTION_USER;
        //                            functionRole.CreatedAt = DateTime.Now;
        //                            functionRole.UpdatedAt = DateTime.Now;
        //                            functionRole.CreatedById = data.UserCreateId;
        //                            functionRole.UpdatedById = data.UserCreateId;
        //                            functionRole.Status = ApiEnums.EntityStatus.NORMAL;
        //                            _context.FunctionRoles.Add(functionRole);
        //                        }
        //                        else
        //                        {
        //                            //update
        //                            var functionRoleExit = functionNew.FirstOrDefault();
        //                            functionRoleExit.ActiveKey = item.ActiveKey;
        //                            functionRoleExit.UpdatedAt = DateTime.Now;
        //                            functionRoleExit.UpdatedById = data.UserCreateId;
        //                            _context.Entry(functionRoleExit).State = EntityState.Modified;
        //                        }
        //                    }
        //                }

        //                await _context.SaveChangesAsync();

        //                transaction.Commit();

        //                def.meta = new Meta(200, ApiConstants.MessageResource.UPDATE_SUCCESS);
        //                return Ok(def);
        //            }
        //            catch (DbUpdateConcurrencyException e)
        //            {
        //                transaction.Rollback();
        //                log.Error("DbUpdateConcurrencyException:" + e);
        //                if (!UserRoleExists(id))
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
        //    catch (Exception e)
        //    {
        //        log.Error("Exception: " + e);
        //        def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
        //        return Ok(def);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] UserRoleDT data)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.CREATE).data;
            if (!ModelState.IsValid)
            {
                throw new CommonException(ApiConstants.MessageResource.INVALID, 400);
            }
            return Ok(await _entityService.SaveData(userClaims, data));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteData(long id)
        {
            UserClaims userClaims = (UserClaims)GetUserClaims(functionCode, (int)ApiEnums.Action.DELETED).data;
                return Ok(await _entityService.DeleteData(userClaims, id));

        }
    }
}
