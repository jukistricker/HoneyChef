using System.Security.Claims;

namespace IOITCore.Models.ViewModels.Bases
{
    public class UserClaims
    {
        public ClaimsPrincipal? User { get; set; }
        public ClaimsIdentity? Identity { get; set; }
        public string access_key { get; set; } = string.Empty;
        public long userId { get; set; } = 0;
        public long userMapId { get; set; } = 0;
        public string fullName { get; set; } = string.Empty;
        public int roleMax { get; set; } = 9999;
        public int roleLevel { get; set; } = 9999;
        public int type { get; set; } = 0;
        public string Token { get; set; }
    }
}
