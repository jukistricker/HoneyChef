using IOITCore.Entities;
using IOITCore.Persistence;
using IOITCore.Repositories.Interfaces;
using IOITCore.Repositories.Bases;

namespace IOITCore.Repositories
{
    public class FunctionRoleRepository : AsyncGenericRepository<FunctionRole, int>, IFunctionRoleRepository
    {
        private readonly IOITDbContext _context;
        public FunctionRoleRepository(IOITDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
