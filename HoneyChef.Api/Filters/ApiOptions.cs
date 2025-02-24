using static IOITCore.Enums.ApiEnums;

namespace IOITCore.Filters
{
    public class ApiOptions
    {
        public ShowLogLevel ShowLogLevel { get; set; }
        public string SiteURL { get; set; }
        public string EmailConfirmationExpiryTime { get; set; }
    }
}
