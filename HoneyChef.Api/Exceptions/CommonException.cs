using IOITCore.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOITCore.Exceptions
{
    public class CommonException : Exception
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }

        public CommonException()
            : base()
        {
            Message = string.Empty;
            StatusCode = ApiConstants.StatusCode.Error404;
            ErrorCode = ApiConstants.ErrorCode.ERROR_BADREQUEST;
        }

        public CommonException(string message = "")
            : base(message)
        {
            Message = message;
            StatusCode = ApiConstants.StatusCode.Error404;
            ErrorCode = ApiConstants.ErrorCode.ERROR_BADREQUEST;
        }

        public CommonException(string message = "", int errorStatus = ApiConstants.StatusCode.Error404)
            : base(message)
        {
            Message = message;
            StatusCode = errorStatus;
            ErrorCode = ApiConstants.ErrorCode.ERROR_BADREQUEST;
        }

        public CommonException(string message = "", int errorStatus = ApiConstants.StatusCode.Error404, string errorCode = ApiConstants.ErrorCode.ERROR_BADREQUEST)
            : base(message)
        {
            Message = message;
            StatusCode = errorStatus;
            ErrorCode = errorCode;
        }
    }
}
