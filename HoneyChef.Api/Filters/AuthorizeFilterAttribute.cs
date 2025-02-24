using IOITCore.Constants;
using IOITCore.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace IOITCore.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenHeader = context.HttpContext.Request.Headers["Authorization"];

            if (tokenHeader.IsNullOrEmpty())
            {
                context.Result = new OkObjectResult(new DefaultResponse().Error("Authorization is null", ApiConstants.StatusCode.Error401))
                {
                    StatusCode = ApiConstants.StatusCode.Error401
                };
            }
        }
    }
}
