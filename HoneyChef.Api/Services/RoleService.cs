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
using System.Web;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace IOITCore.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _entityRepo;
        private readonly IFunctionRoleRepository _functionRoleRepo;

        public RoleService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IRoleRepository entityRepo,
            IFunctionRoleRepository functionRoleRepo)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _entityRepo = entityRepo;
            _functionRoleRepo = functionRoleRepo;
        }

        public async Task<DefaultResponse> GetByPage(UserClaims userClaims, FilteredPagination paging)
        {
            Console.WriteLine("heheeeeeeee");
            DefaultResponse def = new DefaultResponse();
            if (paging != null)
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                var data = _entityRepo.All();
                if (userClaims.roleMax != 1)
                {
                    paging.query = "LevelRole > " + userClaims.roleLevel;
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
                    var listFunctionRoles= _functionRoleRepo.All().Where(fr => fr.TargetId == 1).ToList();
                    def.data = await (from e in data
                  join fr in _functionRoleRepo.All() on e.Id equals fr.TargetId into functionRoles
                  select new
                  {
                      e.Id,
                      e.Name,
                      e.Code,
                      e.Note,
                      e.LevelRole,
                      e.Status,
                      listFunction = functionRoles.Select(fr => new
                      {
                          fr.FunctionId,
                          fr.ActiveKey
                      }).ToList()
                  }).ToListAsync();

                }
                return def;
            }
            else
            {
                throw new CommonException(ApiConstants.MessageResource.BAD_REQUEST_MESSAGE, 400);
            }
        }

        public async Task<DefaultResponse> GetById(UserClaims userClaims, int id)
        {
            DefaultResponse def = new DefaultResponse();
            def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
            var entity = await _entityRepo.GetByKeyAsync(id);
            if (entity == null)
                def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_VIEW_MESSAGE);
            else
                def.data = entity;
            return def;
        }

        public async Task<DefaultResponse> SaveData(UserClaims userClaims, Role entity)
        {
            DefaultResponse def = new DefaultResponse();

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
            return def;
        }

        public async Task<DefaultResponse> DeleteData(UserClaims user, int id)
        {
            DefaultResponse def = new DefaultResponse();
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
            return def;
        }


    }
}
