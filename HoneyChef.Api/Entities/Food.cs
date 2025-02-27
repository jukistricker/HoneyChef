using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Food : AbstractEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public int Code { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; } = 1;


    }
}
