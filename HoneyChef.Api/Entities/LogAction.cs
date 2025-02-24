using IOITCore.Entities.Bases;

namespace IOITCore.Entities
{
    public class LogAction : AbstractEntity<long>
    {
        public string ActionName { get; set; }
        public Enums.ApiEnums.Action ActionType { get; set; }
        public int ActionStatus { get; set; }
        public string IpAddress { get; set; }
    }
}
