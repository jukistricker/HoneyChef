using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Category : AbstractEntity<int>
    {
        public string Title { get; set; }
    }
}
