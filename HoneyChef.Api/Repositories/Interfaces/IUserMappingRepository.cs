using IOITCore.Entities;
using IOITCore.Repositories.Interfaces.Bases;

namespace IOITCore.Repositories.Interfaces
{
    public interface IUserMappingRepository : IAsyncGenericRepository<UserMapping, int>
    {
    }
}
