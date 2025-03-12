using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using HoneyChef.Api.Entities;
using HoneyChef.Api.Repositories.Interfaces;
using HoneyChef.Api.Services.Interfaces;
using IOITCore.Constants;
using IOITCore.Entities;
using IOITCore.Enums;
using IOITCore.Exceptions;
using IOITCore.Models.Common;
using IOITCore.Models.ViewModels;
using IOITCore.Models.ViewModels.Bases;
using IOITCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using HoneyChef.Api.Services.Common;
using HoneyChef.Api.Models.ViewModels;
using FluentValidation;
using HoneyChef.Api.Models.Validators;
using System.Data;

namespace HoneyChef.Api.Services
{
    public class CountryServices : ICountryServices
    {
        private readonly ICountryRepository _entityRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogActionRepository _logActionRepository;
        private readonly IValidator<CountryDTO> _validator;

        public CountryServices(
            ICountryRepository entityRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor contextAccessor,
            ILogActionRepository logActionRepository,
            IValidator<CountryDTO> validator)
        {
            _entityRepo = entityRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _logActionRepository = logActionRepository;
            _validator = validator;
        }

        public async Task<DefaultResponse> DeleteData(UserClaims userClaims, long id)
        {
            DefaultResponse def = new DefaultResponse();
            try{
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);

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
 
                await _unitOfWork.CommitChangesAsync();
            }
            catch(Exception ex){
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
                def.data = ex;
            }
            LogActionModel log = new LogActionModel("Xóa quốc gia", ApiEnums.Action.DELETED, def.meta.error_code, IpService.GetClientIpAddress(_contextAccessor), userClaims.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();
            return def;
        }

        public async Task<DefaultResponse> GetById(UserClaims userClaims, long id)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                var entity = await _entityRepo.All().Where(e => e.Id == id).Select(c => new
                {
                    c.Id,
                    c.CountryName,
                    c.CreatedAt,
                    c.UpdatedAt,
                    c.CreatedBy,
                    c.UpdatedBy
                }).FirstOrDefaultAsync();
                if (entity == null)
                {
                    def.meta = new Meta(404, ApiConstants.MessageResource.NOT_FOUND_VIEW_MESSAGE);
                }
                else
                {
                    def.data = entity;
                }

            }
            catch (Exception ex)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            }
            LogActionModel log = new LogActionModel("Xem chi tiết",
                ApiEnums.Action.VIEW,
                def.meta.error_code,
                IpService.GetClientIpAddress(_contextAccessor),
                userClaims.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();

            return def;
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
                            e.CountryName,

                        }).ToListAsync();
                    }

                }
                else
                    throw new CommonException(ApiConstants.MessageResource.BAD_REQUEST_MESSAGE, 400);

            }
            catch (Exception ex)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
            }

            LogActionModel log = new LogActionModel("Xem danh sách quốc gia",
                                        ApiEnums.Action.VIEW,
                                        def.meta.error_code,
                                        IpService.GetClientIpAddress(_contextAccessor),
                                        userClaims.userId);
            LogAction logAction = _mapper.Map<LogAction>(log);
            _logActionRepository.Add(logAction);
            await _unitOfWork.CommitChangesAsync();

            return def;
        }

        public Task<DefaultResponse> GetByPageNotRole(UserClaims userClaims, FilteredPagination paging)
        {
            throw new NotImplementedException();
        }

        public async Task<DefaultResponse> SaveData(UserClaims userClaims, CountryDTO entity)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);
                var validationRes = await _validator.ValidateAsync(entity);
                if (!validationRes.IsValid)
                {
                    throw new CommonException(string.Join("; ", validationRes.Errors.Select(e => e.ErrorMessage)), 400);

                }
                _unitOfWork.BeginTransaction(IsolationLevel.Serializable);
                //add
                if (entity.Id <= 0)
                {
                    var checkItemExist = await _entityRepo.All().Where(f => f.CountryName.Trim() == entity.CountryName.Trim()).FirstOrDefaultAsync();
                    if (checkItemExist != null)
                    {
                        throw new CommonException("Quốc gia đã tồn tại!", 211);
                    }
                    entity.CreatedBy = userClaims.fullName;
                    entity.UpdatedBy = userClaims.fullName;
                    entity.CreatedAt = DateTime.Now;
                    entity.UpdatedAt = DateTime.Now;
                    entity.CreatedById=userClaims.userId;
                    entity.UpdatedById=userClaims.userId;
                    entity.Status = ApiEnums.EntityStatus.NORMAL;
                    await _entityRepo.AddAsync(entity);
                    await _unitOfWork.CommitChangesAsync();
                }
                //update
                else{
                    var curEntity = await _entityRepo.GetByKeyAsync(entity.Id);
                    if (curEntity == null){
                        throw new CommonException(ApiConstants.MessageResource.NOT_FOUND_UPDATE_MESSAGE, 404);
                    }
                    var duplicateName = await _entityRepo.All().Where(f=>f.CountryName.Trim() == entity.CountryName.Trim()
                        &&f.Id!=entity.Id).FirstOrDefaultAsync();
                    if(duplicateName != null){
                        throw new CommonException("Tên quốc gia đã tồn tại",211);
                    }
                    curEntity.CountryName=entity.CountryName;
                    curEntity.UpdatedAt=DateTime.Now;
                    curEntity.UpdatedBy = userClaims.fullName;
                    curEntity.UpdatedById=userClaims.userId;

                    _entityRepo.Update(curEntity);
                }
                LogActionModel log = new LogActionModel(entity.Id < 0 ? "Thêm mới quốc gia" : "Sửa quốc gia", entity.Id < 0 ? ApiEnums.Action.CREATE : ApiEnums.Action.UPDATE, def.meta.error_code, IpService.GetClientIpAddress(_contextAccessor), userClaims.userId);
                LogAction logAction = _mapper.Map<LogAction>(log);
                _logActionRepository.Add(logAction);

                await _unitOfWork.CommitChangesAsync();
                _unitOfWork.CommitTransaction();
                def.data = entity;

            }
            catch (Exception ex)
            {
                def.meta = new Meta(500, ApiConstants.MessageResource.ERROR_500_MESSAGE);
                def.data= ex;
                _unitOfWork.RollbackTransaction();
            }
            return def;
        }

    }
}