using IOITCore.Entities;
using IOITCore.Repositories.Interfaces.Bases;

namespace IOITCore.Repositories.Interfaces
{
    public interface IUserRoleRepository : IAsyncGenericRepository<UserRole, int>
    {
        Task<List<UserRole>> GetListDataByCondition(string condition);
    }
}
