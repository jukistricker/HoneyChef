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
using System.Linq.Dynamic.Core;
using System.Web;
//using static DevExpress.XtraEditors.Mask.MaskSettings;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Function = IOITCore.Entities.Function;

namespace IOITCore.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFunctionRepository _entityRepo;
        private readonly ILogActionRepository _logActionRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public FunctionService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IFunctionRepository entityRepo,
            ILogActionRepository logActionRepository,
            IHttpContextAccessor contextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _entityRepo = entityRepo;
            _logActionRepository = logActionRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<DefaultResponse> GetByPage(UserClaims userClaims, FilteredPagination paging)
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
                        def.data = await data.Select(e => new
                        {
                            e.Id,
                            e.Code,
                            e.Name,
                            e.Note,
                            e.Status,
                            e.Url,
                            e.Icon,
                            e.FunctionParentId,
                            e.Location,
                            e.IsParamRoute,
                            functionParent = _entityRepo.All(true).Where(f => f.Id == e.FunctionParentId).Select(f => new
                            {
                                FunctionId = f.Id,
                                f.Name,
                                f.Code,
                            }).FirstOrDefault(),
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

            LogActionModel log = new LogActionModel("Xem danh sách chức năng", ApiEnums.Action.VIEW, def.meta.error_code, GetClientIpAddress(), userClaims.userId);
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
                var entity = await _entityRepo.GetByKeyAsync(id);
                if (entity == null)
                    def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_VIEW_MESSAGE);
                else
                    def.data = entity;
            }
            catch (Exception ex)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            }

            LogActionModel log = new LogActionModel("Xem chi tiết chức năng", ApiEnums.Action.VIEW, def.meta.error_code, GetClientIpAddress(), userClaims.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();

            return def;
        }

        public async Task<DefaultResponse> SaveData(UserClaims userClaims, Function entity)
        {
            DefaultResponse def = new DefaultResponse();

            try
            {
                entity.FunctionParentId = entity.FunctionParentId != null ? entity.FunctionParentId : 0;
                if (entity.Id <= 0)
                {
                    if (userClaims.userId != entity.CreatedById)
                    {
                        throw new CommonException(ApiConstants.MessageResource.BAD_REQUEST_MESSAGE, 400);
                    }
                    var checkItemExist = _entityRepo.All().Where(f => f.Code == entity.Code && f.Status != ApiEnums.EntityStatus.DELETED).FirstOrDefault();
                    if (checkItemExist != null)
                    {
                        throw new CommonException("Mã đã tồn tại!", 211);
                    }
                    entity.Status = ApiEnums.EntityStatus.NORMAL;
                    entity.CreatedById = userClaims.userId;
                    entity.CreatedBy = userClaims.fullName;
                    entity.CreatedById = userClaims.userId;
                    await _entityRepo.AddAsync(entity);
                    def.meta = new Meta(200, ApiConstants.MessageResource.ADD_SUCCESS);
                }
                else
                {
                    if (userClaims.userId != entity.UpdatedById)
                    {
                        throw new CommonException(ApiConstants.MessageResource.BAD_REQUEST_MESSAGE, 400);
                    }
                    if (entity.Id == entity.FunctionParentId)
                    {
                        throw new CommonException("Chức năng cha không hợp lệ!", 215);
                    }
                    var checkExist = _entityRepo.All().Where(f => f.Code == entity.Code && f.Status != ApiEnums.EntityStatus.DELETED && f.Id != entity.Id).FirstOrDefault();
                    if (checkExist != null)
                    {
                        throw new CommonException("Mã đã tồn tại!", 211);
                    }
                    var curEntity = await _entityRepo.GetByKeyAsync(entity.Id);
                    if (curEntity == null)
                    {
                        throw new CommonException(ApiConstants.MessageResource.NOT_FOUND_UPDATE_MESSAGE, 404);
                    }
                    entity.Status = curEntity.Status;
                    entity.UpdatedById = userClaims.userId;
                    entity.UpdatedBy = userClaims.fullName;
                    entity.UpdatedById = userClaims.userId;
                    _entityRepo.Update(entity);
                    def.meta = new Meta(200, ApiConstants.MessageResource.UPDATE_SUCCESS);
                }
                await _unitOfWork.CommitChangesAsync();

                def.data = entity;
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            }

            LogActionModel log = new LogActionModel(entity.Id < 0 ? "Thêm mới chức năng" : "Sửa chức năng", entity.Id < 0 ? ApiEnums.Action.CREATE : ApiEnums.Action.UPDATE, def.meta.error_code, GetClientIpAddress(), userClaims.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();
            return def;
        }

        public async Task<DefaultResponse> DeleteData(UserClaims user, int id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                var entity = await _entityRepo.GetByKeyAsync(id);
                if (entity == null)
                {
                    throw new CommonException(ApiConstants.MessageResource.NOT_FOUND_DELETE_MESSAGE, 404);
                }
                //xóa những cái khác liên quan
                entity.UpdatedBy = user.fullName;
                entity.UpdatedById = user.userId;
                entity.UpdatedAt = DateTime.Now;
                entity.Status = ApiEnums.EntityStatus.DELETED;
                _entityRepo.Update(entity);
                await _unitOfWork.CommitChangesAsync();
            }
            catch (Exception e)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            }
            LogActionModel log = new LogActionModel("Xóa chức năng", ApiEnums.Action.DELETED, def.meta.error_code, "", user.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();
            return def;
        }

        public List<SmallFunctionDTO> ListFunction(List<SmallFunctionDTO> dt, int functionId, int level,
            int roleMax, int currentId, string keySearch, bool disabled)
        {
            var index = level + 1;
            try
            {
                IEnumerable<Function> data;
                //if (roleMax == 1)
                //{
                //    data = _context.Functions.Where(e => e.FunctionParentId == functionId && e.Status != ApiEnums.EntityStatus.DELETED).ToList();
                //}
                //else
                //{
                //    data = (from fr in _context.FunctionRoles
                //            join f in _context.Functions on fr.FunctionId equals f.Id
                //            where fr.TargetId == roleMax && fr.Type == (int)ApiEnums.TypeFunction.FUNCTION_ROLE
                //            && fr.Status != ApiEnums.EntityStatus.DELETED && fr.Status != ApiEnums.EntityStatus.DELETED
                //            && f.FunctionParentId == functionId
                //            select f).ToList();
                //}
                data = _entityRepo.All().Where(e => e.FunctionParentId == functionId && e.Status != ApiEnums.EntityStatus.DELETED).OrderBy(x => x.Location).ToList();

                if (data != null)
                {
                    if (data.Count() > 0)
                    {
                        foreach (var item in data)
                        {
                            SmallFunctionDTO function = new SmallFunctionDTO();
                            function.FunctionId = item.Id;
                            function.Code = item.Code;
                            function.Name = item.Name;
                            function.IsParamRoute = item.IsParamRoute;
                            function.KeySearch = item.FunctionParentId == 0 ? item.Name : keySearch + " " + item.Name;
                            function.Level = level;
                            function.disabled = item.Id == currentId ? true : disabled;
                            //function.children = listFunction(item.FunctionId);
                            dt.Add(function);
                            try
                            {
                                ListFunction(dt, item.Id, index, roleMax, currentId, function.KeySearch, function.disabled);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }

            return dt;
        }

        private string GetClientIpAddress()
        {
            return _contextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
