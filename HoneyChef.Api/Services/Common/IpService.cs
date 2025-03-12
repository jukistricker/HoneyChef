using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyChef.Api.Services.Common
{
    public class IpService
    {
        public static string GetClientIpAddress(IHttpContextAccessor httpContextAccessor)
        {
            var ipAddress = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress;
            return ipAddress?.MapToIPv4().ToString() ?? "Unknown";
        }
    }
}


