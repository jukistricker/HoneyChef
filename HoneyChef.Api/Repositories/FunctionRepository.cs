using IOITCore.Entities;
using IOITCore.Persistence;
using IOITCore.Repositories.Interfaces;
using IOITCore.Repositories.Bases;

namespace IOITCore.Repositories
{
    public class FunctionRepository : AsyncGenericRepository<Function, int>, IFunctionRepository
    {
        private readonly IOITDbContext _context;
        public FunctionRepository(IOITDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
