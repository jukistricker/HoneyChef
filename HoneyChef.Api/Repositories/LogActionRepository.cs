using IOITCore.Entities;
using IOITCore.Persistence;
using IOITCore.Repositories.Interfaces;
using IOITCore.Repositories.Bases;

namespace IOITCore.Repositories
{
    public class LogActionRepository : AsyncGenericRepository<LogAction, long>, ILogActionRepository
    {
        private readonly IOITDbContext _context;
        public LogActionRepository(IOITDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
