using IOITCore.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOITCore.Exceptions
{
    public class GlobalServerGprcException : Exception
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }

        public GlobalServerGprcException()
            : base()
        {
            Message = string.Empty;
            ErrorCode = ApiConstants.StatusCode.Error400;
        }

        public GlobalServerGprcException(string message = "")
            : base(message)
        {
            Message = message;
            ErrorCode = ApiConstants.StatusCode.Error400;
        }

        public GlobalServerGprcException(string message = "", int errorCode = ApiConstants.StatusCode.Error400)
            : base(message)
        {
            Message = message;
            ErrorCode = errorCode;
        }

        public GlobalServerGprcException(string message = "", int errorCode = ApiConstants.StatusCode.Error400, Exception innerException = null)
            : base(message, innerException)
        {
            Message = message;
            ErrorCode = errorCode;
        }
    }
}
