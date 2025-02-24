using IOITCore.Entities;
using IOITCore.Persistence;
using IOITCore.Repositories.Interfaces;
using IOITCore.Repositories.Bases;

namespace IOITCore.Repositories
{
    public class RoleRepository : AsyncGenericRepository<Role, int>, IRoleRepository
    {
        private readonly IOITDbContext _context;
        public RoleRepository(IOITDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
