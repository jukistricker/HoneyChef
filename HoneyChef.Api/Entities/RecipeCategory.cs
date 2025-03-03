using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class RecipeCategory:AbstractEntity<long>
    {
        public long? RecipeId { get; set; }
        public long? CategoryId { get; set; }
    }
}
