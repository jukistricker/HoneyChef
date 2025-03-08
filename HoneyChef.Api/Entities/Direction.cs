using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Direction:AbstractEntity<long>
    {
        public long? RecipeId { get; set; }
        public string? Title { get; set; }
        public Guid? reference_mongo_id { get; set; } 
    }
}
