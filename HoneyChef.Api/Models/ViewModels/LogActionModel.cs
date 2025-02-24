using IOITCore.Entities;

namespace IOITCore.Models.ViewModels
{
    public class LogActionModel : LogAction
    {
        public LogActionModel(string actionName, Enums.ApiEnums.Action actionType, int actionStatus, string ipAdress, long? createdById)
        {
            ActionName = actionName;
            ActionType = actionType;
            ActionStatus = actionStatus;
            IpAddress = ipAdress;
            CreatedAt = DateTime.Now;
            CreatedById = createdById;
        }
    }
}
