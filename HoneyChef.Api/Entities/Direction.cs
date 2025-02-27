using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Direction : AbstractEntity<Guid>
    {
        public string? Title { get; set; }
    }
}
