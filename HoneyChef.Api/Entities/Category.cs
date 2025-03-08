using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Category:AbstractEntity<long>
    {
        public string? CategoryName { get; set; }
    }
}
