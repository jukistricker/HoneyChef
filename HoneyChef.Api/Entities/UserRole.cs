using IOITCore.Entities.Bases;

namespace IOITCore.Entities
{
    public class UserRole : AbstractEntity
    {
        public long UserId { get; set; }
        public int RoleId { get; set; }
    }
}
