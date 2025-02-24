using IOITCore.Entities;
using IOITCore.Repositories.Interfaces.Bases;

namespace IOITCore.Repositories.Interfaces
{
    public interface IRoleRepository : IAsyncGenericRepository<Role, int>
    {
    }
}
