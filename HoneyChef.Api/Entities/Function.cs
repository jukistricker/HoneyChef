using IOITCore.Entities.Bases;

namespace IOITCore.Entities
{
    public class Function : AbstractEntity
    {
        public string Name { get; set; }
        public string? Code { get; set; }
        public int FunctionParentId { get; set; }
        public string? Url { get; set; }
        public string? Note { get; set; }
        public int? Location { get; set; }
        public string? Icon { get; set; }
        public bool? IsParamRoute { get; set; }
    }
}
