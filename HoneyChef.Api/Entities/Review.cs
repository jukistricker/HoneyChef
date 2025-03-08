using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Review:AbstractEntity<long>
    {
        public long? RecipeId { get; set; }
        public int? rating { get; set; }
        public string? review { get; set; }
    }
}
