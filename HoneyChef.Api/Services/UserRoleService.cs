using AutoMapper;
using IOITCore.Models.ViewModels;
using IOITCore.Repositories.Interfaces;
using IOITCore.Services.Interfaces;
using IOITCore.Exceptions;
using IOITCore.Constants;
using IOITCore.Enums;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Entities;
using IOITCore.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Web;
using Polly;
using System.Linq.Dynamic.Core;
using IOITCore.Services.Common;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;
using log4net;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;

namespace IOITCore.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _entityRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IFunctionRoleRepository _functionRoleRepo;

        public UserRoleService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserRepository entityRepo,
            IRoleRepository roleRepo,
            IUserRoleRepository userRoleRepo,
            IFunctionRoleRepository functionRoleRepo)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _entityRepo = entityRepo;
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
            _functionRoleRepo = functionRoleRepo;
        }

        public async Task<DefaultResponse> GetByPage(UserClaims userClaims, FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                IQueryable<User> data = Enumerable.Empty<User>().AsQueryable();
                data = _entityRepo.All();
                paging.query = paging.query ?? "1=1";
                if (userClaims.roleMax != 1)
                {
                    paging.query += " RoleLevel > " + userClaims.roleLevel + " AND UserCreateId = " + userClaims.userId;
                }
                if (paging.query != null)
                {
                    paging.query = HttpUtility.UrlDecode(paging.query);
                }

                data = data.Where(paging.query ?? "1=1");
                def.metadata = data.Count();

                if (paging.page_size > 0)
                {
                    if (paging.order_by != null)
                    {
                        data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                    }
                    else
                    {
                        data = data.OrderBy("CreatedAt desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                    }
                }
                else
                {
                    if (paging.order_by != null)
                    {
                        data = data.OrderBy(paging.order_by);
                    }
                    else
                    {
                        data = data.OrderBy("CreatedAt desc");
                    }
                }

                if (paging.select != null && paging.select != "")
                {
                    paging.select = "new(" + paging.select + ")";
                    paging.select = HttpUtility.UrlDecode(paging.select);
                    def.data = await data.Select(paging.select).ToDynamicListAsync();
                }
                else
                {
                    def.data = await data.Select(c => new
                    {
                        c.Id,
                        c.UserMapId,
                        c.Code,
                        c.FullName,
                        c.UserName,
                        c.Avata,
                        c.Address,
                        c.Email,
                        c.Phone,
                        c.Birthday,
                        c.CardId,
                        c.Status,
                        c.BranchId,
                        c.DepartmentId,
                        c.Type,
                        c.PositionId,
                        c.RoleMax,
                        c.RoleLevel,
                        c.IsRoleGroup,
                        c.IsPhoneConfirm,
                        c.IsEmailConfirm,
                        c.IsAppartment,
                        c.RegisterCode,
                        c.LastLoginAt,
                        c.CountLogin,
                        c.LanguageId,
                        listRole = (from ur in _userRoleRepo.All(true)
                                    join role in _roleRepo.All(true) on ur.RoleId equals role.Id
                                    where ur.UserId == c.Id
                                    select new
                                    {
                                        ur.RoleId,
                                        RoleName = role.Name,
                                    }).ToList(),
                        listFunction = _functionRoleRepo.All(true).Where(e => e.TargetId == c.Id && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_USER).Select(e => new
                        {
                            e.FunctionId,
                            e.ActiveKey
                        }).ToList(),
                        c.CreatedAt,
                        c.UpdatedAt,
                        c.CreatedBy,
                        c.UpdatedBy,
                        c.CreatedById,
                        c.UpdatedById,
                        c.CompanyId
                    }).ToListAsync();
                }
                return def;
            }
            else
            {
                def.meta = new Meta(400, ApiConstants.MessageResource.ERROR_400_MESSAGE);
                return def;
            }
        }

        public async Task<DefaultResponse> GetByPageNotRole(UserClaims userClaims, FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                IQueryable<User> data = from c in _entityRepo.All().Where(e => e.Status == ApiEnums.EntityStatus.NORMAL)
                                        where !_userRoleRepo.All(true).Any(m => m.UserId == c.Id && m.Status != ApiEnums.EntityStatus.DELETED)
                                        select c;
                if (paging.query != null)
                {
                    paging.query = HttpUtility.UrlDecode(paging.query);
                }

                data = data.Where(paging.query);
                def.metadata = data.Count();

                if (paging.page_size > 0)
                {
                    if (paging.order_by != null)
                    {
                        data = data.OrderBy(paging.order_by).Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                    }
                    else
                    {
                        data = data.OrderBy("CreatedAt desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
                    }
                }
                else
                {
                    if (paging.order_by != null)
                    {
                        data = data.OrderBy(paging.order_by);
                    }
                    else
                    {
                        data = data.OrderBy("CreatedAt desc");
                    }
                }

                if (paging.select != null && paging.select != "")
                {
                    paging.select = "new(" + paging.select + ")";
                    paging.select = HttpUtility.UrlDecode(paging.select);
                    def.data = data.Select(paging.select);
                }
                else
                {
                    def.data = data;
                }

                return def;
            }
            else
            {
                def.meta = new Meta(400, ApiConstants.MessageResource.ERROR_400_MESSAGE);
                return def;
            }
        }

        public async Task<DefaultResponse> GetById(UserClaims userClaims, long id)
        {
            DefaultResponse def = new DefaultResponse();
            def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
            var data = await _entityRepo.All().Where(c => c.Id == id).Select(c => new
            {
                c.Id,
                c.UserMapId,
                c.Code,
                c.FullName,
                c.UserName,
                c.Avata,
                c.Address,
                c.Email,
                c.Phone,
                c.Birthday,
                c.CardId,
                c.Status,
                c.BranchId,
                c.DepartmentId,
                c.Type,
                c.PositionId,
                c.RoleMax,
                c.RoleLevel,
                c.IsRoleGroup,
                c.IsPhoneConfirm,
                c.IsEmailConfirm,
                c.IsAppartment,
                c.RegisterCode,
                c.LastLoginAt,
                c.CountLogin,
                c.LanguageId,
                listRole = (from ur in _userRoleRepo.All(true)
                            join role in _roleRepo.All(true) on ur.RoleId equals role.Id
                            where ur.UserId == c.Id
                            select new
                            {
                                ur.RoleId,
                                RoleName = role.Name,
                            }).ToList(),
                listFunction = _functionRoleRepo.All(true).Where(e => e.TargetId == c.Id && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_USER).Select(e => new
                {
                    e.FunctionId,
                    e.ActiveKey
                }).ToList(),
                c.CreatedAt,
                c.UpdatedAt,
                c.CreatedBy,
                c.UpdatedBy,
                c.CreatedById,
                c.UpdatedById,
                c.CompanyId
            }).FirstOrDefaultAsync();
            if (data == null)
                def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_VIEW_MESSAGE);
            else
                def.data = data;
            return def;
        }

        public async Task<DefaultResponse> SaveData(UserClaims userClaims, UserRoleDT entity)
        {
            DefaultResponse def = new DefaultResponse();
            def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
            _unitOfWork.BeginTransaction(IsolationLevel.Serializable);
            if (entity.Id <= 0)
            {
                var checkUserNameExist = _entityRepo.All().Where(f => f.UserName == entity.UserName).FirstOrDefault();
                if (checkUserNameExist != null)
                {
                    throw new CommonException("Tài khoản đã tồn tại trên hệ thống!", 211);
                }

                User user = new User();
                user.Address = entity.Address;
                user.FullName = entity.FullName;
                user.UserName = entity.UserName;
                user.Code = entity.Code;
                user.Email = entity.Email != null ? entity.Email.Trim().ToLower() : "";
                user.Avata = entity.Avata;
                user.Password = UtilsService.GetMD5Hash(entity.Password);
                user.Phone = entity.Phone;
                user.BranchId = entity.BranchId;
                user.DepartmentId = entity.DepartmentId;
                user.Type = entity.Type;
                user.PositionId = entity.PositionId;
                user.KeyLock = UtilsService.RandomString(8);
                user.RegEmail = UtilsService.RandomString(8);
                user.RoleMax = 9999;
                user.RoleLevel = 99;
                user.IsRoleGroup = entity.IsRoleGroup != null ? entity.IsRoleGroup : true;
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                user.Status = ApiEnums.EntityStatus.NORMAL;
                user.CreatedById = userClaims.userId;
                user.UpdatedById = userClaims.userId;
                user.CreatedBy = userClaims.fullName;
                user.UpdatedBy = userClaims.fullName;
                await _entityRepo.AddAsync(user);
                await _unitOfWork.CommitChangesAsync();
                entity.UserId = user.Id;

                //update pass
                string pass = user.KeyLock.Trim() + user.RegEmail.Trim() + user.Id + user.Password.Trim();
                user.Password = UtilsService.GetMD5Hash(pass);

                // role
                var checkRole = false;
                byte level = 99;
                int max = 9999;
                //add role 
                if (entity.listRole != null)
                {
                    foreach (var item in entity.listRole)
                    {
                        var role = await _roleRepo.All().Where(e => e.Id == item.RoleId).FirstOrDefaultAsync();
                        if (role != null)
                        {
                            UserRole userRole = new UserRole();
                            userRole.RoleId = item.RoleId;
                            userRole.UserId = user.Id;
                            userRole.CreatedAt = DateTime.Now;
                            userRole.UpdatedAt = DateTime.Now;
                            userRole.CreatedById = userClaims.userId;
                            userRole.UpdatedById = userClaims.userId;
                            userRole.CreatedBy = userClaims.fullName;
                            userRole.UpdatedBy = userClaims.fullName;
                            userRole.Status = ApiEnums.EntityStatus.NORMAL;
                            await _userRoleRepo.AddAsync(userRole);
                            //check role
                            if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
                                checkRole = true;
                            //
                            if (role.LevelRole < level)
                            {
                                level = (byte)role.LevelRole;
                                max = role.Id;
                            }
                        }
                    }
                }
                //update cấp độ user và quyền cao nhất của user đó
                user.RoleLevel = level;
                user.RoleMax = max;
                _entityRepo.Update(user);

                //add function
                if (entity.listFunction != null)
                {
                    foreach (var item in entity.listFunction)
                    {
                        FunctionRole functionRole = new FunctionRole();
                        functionRole.TargetId = entity.UserId;
                        functionRole.FunctionId = item.FunctionId;
                        functionRole.ActiveKey = item.ActiveKey;
                        functionRole.Type = (int)ApiEnums.TypeFunction.FUNCTION_USER;
                        functionRole.CreatedAt = DateTime.Now;
                        functionRole.UpdatedAt = DateTime.Now;
                        functionRole.CreatedById = userClaims.userId;
                        functionRole.UpdatedById = userClaims.userId;
                        functionRole.CreatedBy = userClaims.fullName;
                        functionRole.UpdatedBy = userClaims.fullName;
                        functionRole.Status = ApiEnums.EntityStatus.NORMAL;
                        await _functionRoleRepo.AddAsync(functionRole);
                    }
                }

                await _unitOfWork.CommitChangesAsync();
            }
            else
            {
                var current = await _entityRepo.GetByKeyAsync(entity.Id);
                if (current == null)
                {
                    throw new NotFoundException(ApiConstants.MessageResource.NOT_FOUND_UPDATE_MESSAGE);
                }

                current.FullName = entity.FullName;
                current.Code = entity.Code;
                current.Phone = entity.Phone;
                current.Email = entity.Email != null ? entity.Email.Trim().ToLower() : "";
                current.Address = entity.Address;
                current.Avata = entity.Avata;
                current.BranchId = entity.BranchId;
                current.DepartmentId = entity.DepartmentId;
                current.Type = entity.Type;
                current.PositionId = entity.PositionId;
                current.IsRoleGroup = entity.IsRoleGroup != null ? entity.IsRoleGroup : true;
                current.UpdatedAt = DateTime.Now;
                current.UpdatedById = userClaims.userId;
                current.UpdatedBy = userClaims.fullName;
                _entityRepo.Update(current);

                //role old
                byte levelOld = (byte)current.RoleLevel;
                // role
                var checkRole = false;
                byte level = 99;
                int max = 9999;
                //update list role
                //add new
                if (entity.listRole != null)
                {
                    foreach (var item in entity.listRole)
                    {
                        var role = await _roleRepo.GetByKeyAsync(item.RoleId);
                        if (role != null)
                        {
                            var userRoleNew = _userRoleRepo.All().Where(e => e.UserId == entity.Id && e.RoleId == item.RoleId).ToList();
                            if (userRoleNew.Count <= 0)
                            {
                                UserRole userRole = new UserRole();
                                userRole.RoleId = item.RoleId;
                                userRole.UserId = entity.Id;
                                userRole.Status = ApiEnums.EntityStatus.NORMAL;
                                await _userRoleRepo.AddAsync(userRole);
                            }
                            //check role
                            if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
                                checkRole = true;
                            //
                            if (role.LevelRole < level)
                            {
                                level = (byte)role.LevelRole;
                                max = role.Id;
                            }
                        }
                    }
                }
                //delete old
                var listUserRole = _userRoleRepo.All().Where(e => e.UserId == entity.Id).ToList();
                foreach (var item in listUserRole)
                {
                    var listNew = entity.listRole.Where(e => e.RoleId == item.RoleId).ToList();
                    if (listNew.Count() <= 0)
                    {
                        UserRole userRoleExit = await _userRoleRepo.GetByKeyAsync(item.Id);
                        userRoleExit.Status = ApiEnums.EntityStatus.DELETED;
                        _userRoleRepo.Update(userRoleExit);
                    }
                    else
                    {
                        var role = await _roleRepo.GetByKeyAsync(item.RoleId);
                        if (role != null)
                        {
                            //check role
                            if (role.Code.Trim() == "ADMIN" || role.Code.Trim() == "MANAGER" || role.Code.Trim() == "USER" || role.Code.Trim() == "MANAGER_FULL")
                                checkRole = true;
                        }
                    }
                }

                //update quyền cao nhất và cấp cao nhất của user
                current.RoleLevel = level;
                current.RoleMax = max;
                _entityRepo.Update(current);

                //update list function
                if (entity.listFunction != null)
                {
                    foreach (var item in entity.listFunction)
                    {
                        var functionNew = _functionRoleRepo.All().Where(e => e.TargetId == entity.Id
                        && e.FunctionId == item.FunctionId
                        && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_USER
                        && e.Status != ApiEnums.EntityStatus.DELETED).ToList();
                        //add new
                        if (functionNew.Count <= 0)
                        {
                            FunctionRole functionRole = new FunctionRole();
                            functionRole.TargetId = entity.Id;
                            functionRole.FunctionId = item.FunctionId;
                            functionRole.ActiveKey = item.ActiveKey ?? "111111111";
                            functionRole.Type = (int)ApiEnums.TypeFunction.FUNCTION_USER;
                            functionRole.CreatedAt = DateTime.Now;
                            functionRole.UpdatedAt = DateTime.Now;
                            functionRole.CreatedById = userClaims.userId;
                            functionRole.UpdatedById = userClaims.userId;
                            functionRole.CreatedBy = userClaims.fullName;
                            functionRole.UpdatedBy = userClaims.fullName;
                            functionRole.Status = ApiEnums.EntityStatus.NORMAL;
                            await _functionRoleRepo.AddAsync(functionRole);
                        }
                        else
                        {
                            //update
                            var functionRoleExit = functionNew.FirstOrDefault();
                            if (functionRoleExit != null)
                            {
                                functionRoleExit.ActiveKey = item.ActiveKey ?? "111111111";
                                functionRoleExit.UpdatedAt = DateTime.Now;
                                functionRoleExit.UpdatedById = userClaims.userId;
                                functionRoleExit.UpdatedBy = userClaims.fullName;
                                _functionRoleRepo.Update(functionRoleExit);
                            }
                        }
                    }
                }
            }
            _unitOfWork.CommitTransaction();
            def.data = entity;
            return def;
        }

        public async Task<DefaultResponse> DeleteData(UserClaims userClaims, long id)
        {
            DefaultResponse def = new DefaultResponse();
            var data = await _entityRepo.GetByKeyAsync(id);
            if (data == null)
            {
                throw new NotFoundException(ApiConstants.MessageResource.NOT_FOUND_DELETE_MESSAGE);
            }

            if (id == 1)
            {
                throw new CommonException("Không thể xóa tài khoản root!", 210);
            }

            //using (var transaction = _context.Database.BeginTransaction())
            //{
            //delete user
            data.UpdatedById = userClaims.userId;
            data.UpdatedBy = userClaims.fullName;
            data.UpdatedAt = DateTime.Now;
            data.Status = ApiEnums.EntityStatus.DELETED;
            _entityRepo.Update(data);

            //delete user role
            var userRoles = await _userRoleRepo.All().Where(e => e.UserId == id).ToListAsync();
            foreach (var item in userRoles)
            {
                item.UpdatedAt = DateTime.Now;
                item.UpdatedById = userClaims.userId;
                item.UpdatedBy = userClaims.fullName;
                item.Status = ApiEnums.EntityStatus.DELETED;
            }
            _userRoleRepo.UpdateRange(userRoles);
            //delete function role
            var functionRoles = await _functionRoleRepo.All().Where(e => e.TargetId == id
                && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_USER
                && e.Status != ApiEnums.EntityStatus.DELETED).ToListAsync();
            foreach (var item in functionRoles)
            {
                item.UpdatedAt = DateTime.Now;
                item.UpdatedById = userClaims.userId;
                item.UpdatedBy = userClaims.fullName;
                item.Status = ApiEnums.EntityStatus.DELETED;
            }
            _functionRoleRepo.UpdateRange(functionRoles);
            await _unitOfWork.CommitChangesAsync();
            return def;
            //}
            //var entity = await _entityRepo.GetByKeyAsync(id);
            //if (entity == null)
            //    return true;

            ////if (_roleRepo.All().Any(p => p.Id == id))
            ////    return false;
            //entity.Status = ApiEnums.EntityStatus.DELETED;
            //entity.UpdatedBy = user.fullName;
            //entity.UpdatedById = user.userId;
            //_entityRepo.Update(entity);
            //await _unitOfWork.CommitChangesAsync();
            //return true;
        }


    }
}
