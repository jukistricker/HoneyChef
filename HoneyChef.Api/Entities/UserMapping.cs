using IOITCore.Entities.Bases;

namespace IOITCore.Entities
{
    public class UserMapping : AbstractEntity
    {
        public long? UserId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
    }
}
