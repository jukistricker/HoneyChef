using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class DetailDirection : AbstractEntity<Guid>
    {
        public string? Description { get; set; }
    }
}
