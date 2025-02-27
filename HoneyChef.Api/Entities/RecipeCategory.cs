using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class RecipeCategory : AbstractEntity<int>
    {
        public int IdRecipe { get; set; }
        public int IdCategory { get; set; }
    }
}
