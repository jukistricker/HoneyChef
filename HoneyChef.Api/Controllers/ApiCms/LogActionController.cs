using IOITCore.Constants;
using IOITCore.Exceptions;
using IOITCore.Models.Common;
using IOITCore.Repositories.Interfaces;
using IOITCore.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Web;

namespace IOITCore.Controllers.ApiCms
{
    [Route("api/cms/[controller]")]
    [ApiController]
    public class LogActionController : ControllerBase
    {
        private readonly ILogActionRepository _logActionRepository;
        private readonly IUserRepository _userRepository;

        public LogActionController(ILogActionRepository logActionRepository, IUserRepository userRepository)
        {
            _logActionRepository = logActionRepository;
            _userRepository = userRepository;
        }

        [HttpGet("GetByPage")]
        public IActionResult GetBypate([FromQuery] FilteredPagination paging)
        {
            DefaultResponse def = new DefaultResponse();
            try
            {
                if (paging != null)
                {
                    def.meta = new Meta(200, ApiConstants.MessageResource.ACCTION_SUCCESS);

                    var data = _logActionRepository.All();

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
                        var users = _userRepository.All();
                        def.data = data.Select(e => new
                        {
                            ActionName = e.ActionName,
                            ActionType = e.ActionType,
                            ActionStatus = e.ActionStatus,
                            IpAddress = e.IpAddress,
                            UserName = users.Where(p => p.Id == e.CreatedById).First().UserName,
                            CreatedAt = e.CreatedAt,
                        }).OrderBy(p => p.CreatedAt).ToList();
                    }
                    return Ok(def);
                }
                else
                {
                    throw new CommonException(ApiConstants.MessageResource.BAD_REQUEST_MESSAGE, 400);
                }
            }
            catch (Exception ex)
            {
                throw new CommonException(ApiConstants.MessageResource.ERROR_500_MESSAGE, 500);
            }
        }
    }
}
