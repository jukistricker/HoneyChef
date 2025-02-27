using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Ingredient : AbstractEntity<Guid>
    {
        public Guid IdRecipe { get; set; }
        public string Title { get; set; } = string.Empty;
        public string IdReferenceToMongo { get; set; } = string.Empty;

    }
}
