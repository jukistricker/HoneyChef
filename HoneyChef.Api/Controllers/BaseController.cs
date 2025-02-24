using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.Net.Http.Headers;
using IOITCore.Exceptions;
using static IOITCore.Constants.ApiConstants;
using IOITCore.Enums;
using IOITCore.Constants;
using IOITCore.Models.Common;
using IOITCore.Services.Common;
using IOITCore.Models.ViewModels.Bases;

namespace IOITCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        private IMediator _media;
        protected IMediator _mediator => _media ??= HttpContext.RequestServices.GetService<IMediator>();
        protected IMapper _mapper => HttpContext.RequestServices.GetService<IMapper>();

        protected DefaultResponse GetUserClaims()
        {
            DefaultResponse responseApi = new DefaultResponse();
            UserClaims userClaims = new UserClaims();
            if (User == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            var identity = User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            userClaims = new UserClaims
            {
                User = User,
                Identity = identity,
                access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault() ?? string.Empty,
                fullName = identity.Claims.Where(c => c.Type == "FullName").Select(c => c.Value).SingleOrDefault() ?? string.Empty,
                userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault() ?? "0"),
                roleMax = int.Parse(identity.Claims.Where(c => c.Type == "RoleMax").Select(c => c.Value).SingleOrDefault() ?? "9999"),
                Token = Request.Headers[HeaderNames.Authorization]
            };
            if (userClaims == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            if (userClaims.access_key == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            responseApi.data = userClaims;
            return responseApi;
        }

        protected DefaultResponse GetUserClaims(string functionCode, int type)
        {
            DefaultResponse responseApi = new DefaultResponse();
            UserClaims userClaims = new UserClaims();
            if (User == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            var identity = User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            userClaims = new UserClaims
            {
                User = User,
                Identity = identity,
                access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault() ?? string.Empty,
                fullName = identity.Claims.Where(c => c.Type == "FullName").Select(c => c.Value).SingleOrDefault() ?? string.Empty,
                userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault() ?? "0"),
                userMapId = int.Parse(identity.Claims.Where(c => c.Type == "UserMapId").Select(c => c.Value).SingleOrDefault() ?? "0"),
                roleMax = int.Parse(identity.Claims.Where(c => c.Type == "RoleMax").Select(c => c.Value).SingleOrDefault() ?? "9999"),
                type = int.Parse(identity.Claims.Where(c => c.Type == "Type").Select(c => c.Value).SingleOrDefault() ?? "9999"),
                Token = Request.Headers[HeaderNames.Authorization]
            };
            if(userClaims == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            if (userClaims.access_key == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            responseApi.data = userClaims;
            if (userClaims.userId != 8 && !string.IsNullOrEmpty(functionCode) && !CheckRoleService.CheckRoleByCode(userClaims.access_key, functionCode, type))
            {
                if(type == (int)ApiEnums.Action.VIEW)
                    throw new CommonException(MessageResource.NOPERMISION_VIEW_MESSAGE, ApiConstants.StatusCode.NoPermision);
                else if (type == (int)ApiEnums.Action.CREATE)
                    throw new CommonException(MessageResource.NOPERMISION_CREATE_MESSAGE, 222);
                else if (type == (int)ApiEnums.Action.UPDATE)
                    throw new CommonException(MessageResource.NOPERMISION_UPDATE_MESSAGE, 222);
                else if (type == (int)ApiEnums.Action.DELETED)
                    throw new CommonException(MessageResource.NOPERMISION_DELETE_MESSAGE, 222);
                else if (type == (int)ApiEnums.Action.IMPORT)
                    throw new CommonException(MessageResource.NOPERMISION_ACTION_MESSAGE, 222);
                else if (type == (int)ApiEnums.Action.EXPORT)
                    throw new CommonException(MessageResource.NOPERMISION_ACTION_MESSAGE, 222);
                else if (type == (int)ApiEnums.Action.PRINT)
                    throw new CommonException(MessageResource.NOPERMISION_ACTION_MESSAGE, 222);
                else if (type == (int)ApiEnums.Action.EDIT_ANOTHER_USER)
                    throw new CommonException(MessageResource.NOPERMISION_ACTION_MESSAGE, 222);
            }
            return responseApi;
        }

        protected DefaultResponse GetUserClaimsMultiCole(List<string> listFunctionCodes, int type)
        {
            DefaultResponse responseApi = new DefaultResponse();
            UserClaims userClaims = new UserClaims();
            if (User == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            var identity = User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            userClaims = new UserClaims
            {
                User = User,
                Identity = identity,
                access_key = identity.Claims.Where(c => c.Type == "AccessKey").Select(c => c.Value).SingleOrDefault() ?? string.Empty,
                fullName = identity.Claims.Where(c => c.Type == "FullName").Select(c => c.Value).SingleOrDefault() ?? string.Empty,
                userId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault() ?? "0"),
                userMapId = int.Parse(identity.Claims.Where(c => c.Type == "UserMapId").Select(c => c.Value).SingleOrDefault() ?? "0")
            };
            if (userClaims == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            if (userClaims.access_key == null)
            {
                throw new BadRequestException(MessageResource.INVALID);
            }
            responseApi.data = userClaims;
            bool checkRole = false;
            foreach (var functionCode in listFunctionCodes)
            {
                if (!string.IsNullOrEmpty(functionCode) && CheckRoleService.CheckRoleByCode(userClaims.access_key, functionCode, type))
                {
                    checkRole = true;
                    break;
                }
            }
            if (!checkRole)
            {
                if (type == (int)ApiEnums.Action.VIEW)
                    throw new CommonException(MessageResource.NOPERMISION_VIEW_MESSAGE, ApiConstants.StatusCode.NoPermision);
                else if (type == (int)ApiEnums.Action.CREATE)
                    throw new CommonException(MessageResource.NOPERMISION_CREATE_MESSAGE, ApiConstants.StatusCode.NoPermision);
                else if (type == (int)ApiEnums.Action.UPDATE)
                    throw new CommonException(MessageResource.NOPERMISION_UPDATE_MESSAGE, ApiConstants.StatusCode.NoPermision);
                else if (type == (int)ApiEnums.Action.DELETED)
                    throw new CommonException(MessageResource.NOPERMISION_DELETE_MESSAGE, ApiConstants.StatusCode.NoPermision);
                else if (type == (int)ApiEnums.Action.IMPORT)
                    throw new CommonException(MessageResource.NOPERMISION_ACTION_MESSAGE, ApiConstants.StatusCode.NoPermision);
                else if (type == (int)ApiEnums.Action.EXPORT)
                    throw new CommonException(MessageResource.NOPERMISION_ACTION_MESSAGE, ApiConstants.StatusCode.NoPermision);
                else if (type == (int)ApiEnums.Action.PRINT)
                    throw new CommonException(MessageResource.NOPERMISION_ACTION_MESSAGE, ApiConstants.StatusCode.NoPermision);
                else if (type == (int)ApiEnums.Action.EDIT_ANOTHER_USER)
                    throw new CommonException(MessageResource.NOPERMISION_ACTION_MESSAGE, ApiConstants.StatusCode.NoPermision);
            }
            return responseApi;
        }

        protected ActionResult Res(DefaultResponse res)
        {
            return Content(res?.ToJson(), "application/json");
        }

        protected Output<T> Res<T>(T data, int? metadata = 0, string msg = MessageResource.ACCTION_SUCCESS, int code = ApiConstants.StatusCode.Success200)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            return new Output<T>
            {
                data = data,
                meta = new Meta
                {
                    error_code = code,
                    error_message = msg
                },
                metadata = metadata
            };
        }


    }
}
