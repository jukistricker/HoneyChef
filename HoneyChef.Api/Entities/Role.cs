using IOITCore.Entities.Bases;

namespace IOITCore.Entities
{
    public class Role : AbstractEntity
    {
        public string? Code { get; set; }
        public string Name { get; set; }
        public byte? LevelRole { get; set; }
        public string? Note { get; set; }
    }
}
