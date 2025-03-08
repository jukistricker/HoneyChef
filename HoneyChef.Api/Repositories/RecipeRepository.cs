using HoneyChef.Api.Entities;
using HoneyChef.Api.Repositories.Interfaces;
using IOITCore.Persistence;
using IOITCore.Repositories.Bases;

namespace HoneyChef.Api.Repositories
{
    public class RecipeRepository : AsyncGenericRepository<Recipe, Guid> , IRecipeRepository
    {
        private readonly IOITDbContext _context;

        public RecipeRepository(IOITDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
