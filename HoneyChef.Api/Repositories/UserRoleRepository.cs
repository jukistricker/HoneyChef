using IOITCore.Entities;
using IOITCore.Persistence;
using IOITCore.Repositories.Interfaces;
using IOITCore.Repositories.Bases;

namespace IOITCore.Repositories
{
    public class UserRoleRepository : AsyncGenericRepository<UserRole, int>, IUserRoleRepository
    {
        private readonly IOITDbContext _context;
        public UserRoleRepository(IOITDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<UserRole>> GetListDataByCondition(string condition)
        {
            throw new System.NotImplementedException();
        }
    }
}
