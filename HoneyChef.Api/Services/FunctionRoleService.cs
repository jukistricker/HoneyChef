using AutoMapper;
using IOITCore.Constants;
using IOITCore.Entities;
using IOITCore.Enums;
using IOITCore.Exceptions;
using IOITCore.Models.Common;
using IOITCore.Models.ViewModels;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Repositories.Interfaces;
using IOITCore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Web;
//using static DevExpress.XtraEditors.Mask.MaskSettings;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IOITCore.Services
{
    public class FunctionRoleService : IFunctionRoleService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _entityRepo;
        private readonly IFunctionRoleRepository _functionRoleRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly ILogActionRepository _logActionRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public FunctionRoleService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IRoleRepository entityRepo,
            IFunctionRoleRepository functionRoleRepo,
            IUserRoleRepository userRoleRepo,
            ILogActionRepository logActionRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _entityRepo = entityRepo;
            _functionRoleRepo = functionRoleRepo;
            _userRoleRepo = userRoleRepo;
            _logActionRepository = logActionRepository;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<DefaultResponse> GetByPage(UserClaims user, FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                if (paging != null)
                {
                    def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                    var data = _entityRepo.All();
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
                            data = data.OrderBy("Id desc").Skip((paging.page - 1) * paging.page_size).Take(paging.page_size);
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
                            data = data.OrderBy("Id desc");
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
                        def.data = await data.Select(c => new
                        {
                            c.Id,
                            c.Code,
                            c.Name,
                            c.Note,
                            c.LevelRole,
                            c.Status,
                            listFunction = _functionRoleRepo.All(true).Where(e => e.TargetId == c.Id
                            && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_ROLE).Select(e => new
                            {
                                e.FunctionId,
                                e.ActiveKey
                            }).ToList(),
                            c.CreatedAt,
                            c.UpdatedAt,
                            c.CreatedBy,
                            c.UpdatedBy
                        }).ToListAsync();
                    }
                }
                else
                {
                    throw new CommonException(ApiConstants.MessageResource.BAD_REQUEST_MESSAGE, 400);
                }
            }
            catch (Exception ex)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            }

            LogActionModel log = new LogActionModel("Xem danh sách FunctionRole", ApiEnums.Action.VIEW, def.meta.error_code, GetClientIpAddress(), user.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();

            return def;
        }

        public async Task<DefaultResponse> GetById(UserClaims userClaims, int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                var entity = await _entityRepo.All().Where(e => e.Id == id).Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.Name,
                    c.Note,
                    c.LevelRole,
                    c.Status,
                    listFunction = _functionRoleRepo.All(true).Where(e => e.TargetId == c.Id
                    && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_ROLE
                    ).Select(e => new
                    {
                        e.FunctionId,
                        e.ActiveKey
                    }).ToList(),
                    c.CreatedAt,
                    c.UpdatedAt,
                    c.CreatedBy,
                    c.UpdatedBy
                }).FirstOrDefaultAsync();
                if (entity == null)
                    def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_VIEW_MESSAGE);
                else
                    def.data = entity;
            }
            catch (Exception ex)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            }

            LogActionModel log = new LogActionModel("Xem chi tiết ", ApiEnums.Action.VIEW, def.meta.error_code, GetClientIpAddress(), userClaims.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();

            return def;
        }

        public async Task<DefaultResponse> SaveData(UserClaims userClaims, RoleDTO entity)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                if (entity.LevelRole <= 0 || entity.LevelRole >= 256)
                {
                    throw new CommonException("Cấp độ quyền chỉ được từ 1 đến 255!", 212);
                }
                _unitOfWork.BeginTransaction(IsolationLevel.Serializable);
                if (entity.Id <= 0)
                {
                    var checkItemExist = await _entityRepo.All().Where(f => f.Code.Trim() == entity.Code.Trim()).FirstOrDefaultAsync();
                    if (checkItemExist != null)
                    {
                        throw new CommonException("Mã đã tồn tại!", 211);
                    }

                    Role role = new Role();
                    role.Code = entity.Code.Trim();
                    role.Name = entity.Name;
                    role.Note = entity.Note;
                    role.LevelRole = entity.LevelRole;
                    role.CreatedAt = DateTime.Now;
                    role.UpdatedAt = DateTime.Now;
                    role.CreatedById = userClaims.userId;
                    role.UpdatedById = userClaims.userId;
                    role.CreatedBy = userClaims.fullName;
                    role.UpdatedBy = userClaims.fullName;
                    role.Status = ApiEnums.EntityStatus.NORMAL;
                    await _entityRepo.AddAsync(role);
                    await _unitOfWork.CommitChangesAsync();

                    entity.Id = role.Id;

                    //add function
                    if (entity.listFunction != null)
                    {
                        foreach (var item in entity.listFunction)
                        {
                            FunctionRole functionRole = new FunctionRole();
                            functionRole.TargetId = entity.Id;
                            functionRole.FunctionId = item.FunctionId;
                            functionRole.ActiveKey = item.ActiveKey ?? "000000000";
                            functionRole.Type = (int)ApiEnums.TypeFunction.FUNCTION_ROLE;
                            functionRole.CreatedAt = DateTime.Now;
                            functionRole.UpdatedAt = DateTime.Now;
                            functionRole.CreatedById = userClaims.userId;
                            functionRole.UpdatedById = userClaims.userId;
                            functionRole.CreatedBy = userClaims.fullName;
                            functionRole.UpdatedBy = userClaims.fullName;
                            functionRole.Status = ApiEnums.EntityStatus.NORMAL;
                            await _functionRoleRepo.AddAsync(functionRole);
                        }
                        await _unitOfWork.CommitChangesAsync();
                    }

                }
                else
                {
                    var checkExist = _entityRepo.All().Where(f => f.Code.Trim() == entity.Code.Trim()
                    && f.Id != entity.Id).FirstOrDefault();
                    if (checkExist != null)
                    {
                        throw new CommonException("Mã đã tồn tại!", 211);
                    }
                    var curEntity = await _entityRepo.GetByKeyAsync(entity.Id);
                    if (curEntity == null)
                    {
                        throw new CommonException(ApiConstants.MessageResource.NOT_FOUND_UPDATE_MESSAGE, 404);
                    }
                    curEntity.Code = entity.Code.Trim();
                    curEntity.LevelRole = entity.LevelRole;
                    curEntity.Name = entity.Name;
                    curEntity.Note = entity.Note;
                    curEntity.UpdatedAt = DateTime.Now;
                    curEntity.UpdatedById = userClaims.userId;
                    curEntity.UpdatedBy = userClaims.fullName;

                    //update list function
                    foreach (var item in entity.listFunction)
                    {
                        var functionRoleExit = _functionRoleRepo.All().Where(e => e.TargetId == entity.Id
                        && e.FunctionId == item.FunctionId
                        && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_ROLE).FirstOrDefault();
                        //add new
                        if (functionRoleExit == null)
                        {
                            FunctionRole functionRole = new FunctionRole();
                            functionRole.TargetId = entity.Id;
                            functionRole.FunctionId = item.FunctionId;
                            functionRole.ActiveKey = item.ActiveKey ?? "000000000";
                            functionRole.Type = (int)ApiEnums.TypeFunction.FUNCTION_ROLE;
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
                            functionRoleExit.ActiveKey = item.ActiveKey ?? "000000000";
                            functionRoleExit.UpdatedAt = DateTime.Now;
                            functionRoleExit.UpdatedById = userClaims.userId;
                            functionRoleExit.UpdatedBy = userClaims.fullName;
                            _functionRoleRepo.Update(functionRoleExit);
                        }
                    }
                    _entityRepo.Update(curEntity);
                }

                LogActionModel log = new LogActionModel(entity.Id < 0 ? "Thêm mới chức năng" : "Sửa chức năng", entity.Id < 0 ? ApiEnums.Action.CREATE : ApiEnums.Action.UPDATE, def.meta.error_code, GetClientIpAddress(), userClaims.userId);
                LogAction logAction = _mapper.Map<LogAction>(log);
                _logActionRepository.Add(logAction);
                await _unitOfWork.CommitChangesAsync();

                _unitOfWork.CommitTransaction();
                def.data = entity;
            }
            catch (Exception ex)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
                _unitOfWork.RollbackTransaction();
            }
            return def;
        }

        public async Task<DefaultResponse> DeleteData(UserClaims userClaims, int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                var checkUr = await _userRoleRepo.All().Where(e => e.RoleId == id).FirstOrDefaultAsync();
                if (checkUr != null)
                {
                    throw new CommonException("Nhóm quyền đã được gán cho tài khoản không được xóa!", 213);
                }

                var entity = await _entityRepo.GetByKeyAsync(id);
                if (entity == null)
                {
                    throw new CommonException(ApiConstants.MessageResource.NOT_FOUND_DELETE_MESSAGE, 404);
                }
                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedById = userClaims.userId;
                entity.UpdatedBy = userClaims.fullName;
                entity.Status = ApiEnums.EntityStatus.DELETED;
                _entityRepo.Update(entity);

                //
                var fr = _functionRoleRepo.All().Where(e => e.TargetId == entity.Id && e.Type == (int)ApiEnums.TypeFunction.FUNCTION_ROLE).ToList();
                _functionRoleRepo.DeleteRange(fr);

                await _unitOfWork.CommitChangesAsync();
            }
            catch(Exception ex)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            }
            //Check xem đã đc cấp cho tk nào chưa

            LogActionModel log = new LogActionModel("Xóa chức năng", ApiEnums.Action.DELETED, def.meta.error_code, "", userClaims.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();

            return def;

        }

        private string GetClientIpAddress()
        {
            return _contextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
