using IOITCore.Entities.Bases;

namespace IOITCore.Entities
{
    public class FunctionRole : AbstractEntity
    {
        public long TargetId { get; set; }
        public int FunctionId { get; set; }
        public string ActiveKey { get; set; }
        public byte? Type { get; set; }
    }
}
