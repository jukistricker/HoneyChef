using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class DirectionDetail : AbstractEntity<Guid>
    {
        public string? Description { get; set; }
    }
}
