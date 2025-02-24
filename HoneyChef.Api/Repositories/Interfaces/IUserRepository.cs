using IOITCore.Entities;
using IOITCore.Repositories.Interfaces.Bases;

namespace IOITCore.Repositories.Interfaces
{
    public interface IUserRepository : IAsyncLongRepository<User>
    {
        Task<User> GetByUserName(string username);
    }
}
