using IOITCore.Exceptions;
using IOITCore.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Serilog;
using IOITCore.Models.Common;
using IOITCore.Services.Interfaces;

namespace IOITCore.Filters
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IOptionsSnapshot<ApiOptions> _settings;
        private readonly ICacheService _cacheService;
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        private static ILogger<GlobalExceptionFilterAttribute> _logger;

        public GlobalExceptionFilterAttribute(ICacheService cacheService
            , IOptionsSnapshot<ApiOptions> settings
            , ILogger<GlobalExceptionFilterAttribute> logger)
        {
            _settings = settings;
            _cacheService = cacheService;
            _logger = logger;
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(System.ComponentModel.DataAnnotations.ValidationException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(BadRequestException), HandleBadRequestException},
                { typeof(RequiredTokenExeption), HandleRequiredTokenException},
                { typeof(CommonException), HandleCommmonException},
                { typeof(UnpermissionException), HandlePermissionException}
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            var type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            //if (context.ModelState.IsValid)
            //{
            //    HandleInvalidModelStateException(context);
            //    return;
            //}

            HandleUnknownException(context);
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            context.Result = new ObjectResult(new DefaultResponse().Error(context.Exception.Message, ApiConstants.StatusCode.Error500))
            {
                StatusCode = ApiConstants.StatusCode.Error500
            };

            Log.ForContext("ERROR_CODE", ApiConstants.ErrorCode.ERROR_SERVER).Error($@"HandleUnknownException() =>Error:{context.Exception.Message}");

            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as System.ComponentModel.DataAnnotations.ValidationException;

            context.Result = new OkObjectResult(new DefaultResponse().Error(exception.Message, ApiConstants.StatusCode.Error422))
            {
                StatusCode = ApiConstants.StatusCode.Error422
            };

            Log.ForContext("ERROR_CODE", ApiConstants.ErrorCode.ERROR_VALIDATION).Error($@"HandleValidationException() =>Error:{context.Exception.Message}");

            context.ExceptionHandled = true;
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            context.Result = new OkObjectResult(new DefaultResponse().Error(context.Exception.Message, ApiConstants.StatusCode.Error422))
            {
                StatusCode = ApiConstants.StatusCode.Error422
            };

            Log.ForContext("ERROR_CODE", ApiConstants.ErrorCode.ERROR_SERVER).Error($@"HandleInvalidModelStateException() =>Error:{context.Exception.Message}");

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundException;

            context.Result = new OkObjectResult(new DefaultResponse().Error(exception != null ? exception.Message : context.Exception.Message, ApiConstants.StatusCode.Error400))
            {
                StatusCode = ApiConstants.StatusCode.Error400
            };

            Log.ForContext("ERROR_CODE", exception.ErrorCode ?? ApiConstants.ErrorCode.ERROR_NOTFOUND).Error($@"HandleNotFoundException() =>Error:{context.Exception.Message}");

            context.ExceptionHandled = true;
        }

        private void HandleBadRequestException(ExceptionContext context)
        {
            var exception = context.Exception as BadRequestException;

            context.Result = new OkObjectResult(new DefaultResponse().Error(context.Exception.Message, ApiConstants.StatusCode.Error400));

            Log.ForContext("ERROR_CODE", exception.ErrorCode ?? ApiConstants.ErrorCode.ERROR_BADREQUEST).Error($@"HandleBadRequestException() =>Error:{context.Exception.Message}");

            context.ExceptionHandled = true;
        }

        private void HandleRequiredTokenException(ExceptionContext context)
        {
            var exception = context.Exception as RequiredTokenExeption;

            context.Result = new OkObjectResult(new DefaultResponse().Error(exception != null ? exception.ErrorCode : string.Empty, ApiConstants.StatusCode.Error401))
            {
                StatusCode = ApiConstants.StatusCode.Error401
            };

            Log.ForContext("ERROR_CODE", exception.ErrorCode ?? ApiConstants.ErrorCode.ERROR_AUTHORIZED).Error($@"HandleRequiredTokenException() =>Error:{context.Exception.Message}");

            context.ExceptionHandled = true;
        }

        private void HandleCommmonException(ExceptionContext context)
        {
            var exception = context.Exception as CommonException;

            context.Result = new OkObjectResult(new DefaultResponse().Error(
                   exception != null ? exception.Message : string.Empty,
                   exception.StatusCode
               ))
            {
                StatusCode = ApiConstants.StatusCode.Success200
            };

            Log.ForContext("ERROR_CODE", exception.ErrorCode ?? ApiConstants.ErrorCode.ERROR_BADREQUEST).Error($@"HandleCommmonException() =>Error:{context.Exception.Message}");

            context.ExceptionHandled = true;
        }

        private void HandlePermissionException(ExceptionContext context)
        {
            var exception = context.Exception as UnpermissionException;

            context.Result = new OkObjectResult(new DefaultResponse().Error(
                   exception.Message,
                   exception.StatusCode
               ))
            {
                StatusCode = ApiConstants.StatusCode.Success200
            };

            Log.ForContext("ERROR_CODE", exception.ErrorCode ?? ApiConstants.ErrorCode.ERROR_PERMISSSION).Error($@"HandlePermissionException() =>Error:{context.Exception.Message}");

            context.ExceptionHandled = true;
        }
    }
}
