using HoneyChef.Api.Entities;
using IOITCore.Enums;
using IOITCore.Repositories.Bases;
using IOITCore.Repositories.Interfaces.Bases;

namespace HoneyChef.Api.Repositories.Interfaces
{
    public interface IRecipeRepository : IAsyncGenericRepository<Recipe, Guid>
    {
        
    }
}
