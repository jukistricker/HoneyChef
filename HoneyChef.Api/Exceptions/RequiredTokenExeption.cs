using IOITCore.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOITCore.Exceptions
{
    public class RequiredTokenExeption : Exception
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }

        public RequiredTokenExeption()
            : base()
        {
            Message = string.Empty;
            StatusCode = ApiConstants.StatusCode.Error401;
            ErrorCode = ApiConstants.ErrorCode.ERROR_AUTHORIZED;
        }

        public RequiredTokenExeption(string message = "")
            : base(message)
        {
            Message = message;
            StatusCode = ApiConstants.StatusCode.Error401;
            ErrorCode = ApiConstants.ErrorCode.ERROR_AUTHORIZED;
        }

        public RequiredTokenExeption(string message = "", int errorStatus = ApiConstants.StatusCode.Error401)
            : base(message)
        {
            Message = message;
            StatusCode = errorStatus;
            ErrorCode = ApiConstants.ErrorCode.ERROR_AUTHORIZED;
        }

        public RequiredTokenExeption(string message = "", int errorStatus = ApiConstants.StatusCode.Error401, string errorCode = ApiConstants.ErrorCode.ERROR_AUTHORIZED)
            : base(message)
        {
            Message = message;
            StatusCode = errorStatus;
            ErrorCode = errorCode;
        }

        public RequiredTokenExeption(string message = "", int errorStatus = ApiConstants.StatusCode.Error401, string errorCode = ApiConstants.ErrorCode.ERROR_AUTHORIZED, Exception innerException = null)
            : base(message, innerException)
        {
            Message = message;
            StatusCode = errorStatus;
            ErrorCode = errorCode;
        }
    }
}
