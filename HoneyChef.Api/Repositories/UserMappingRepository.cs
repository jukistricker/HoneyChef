using IOITCore.Entities;
using IOITCore.Persistence;
using IOITCore.Repositories.Interfaces;
using IOITCore.Repositories.Bases;

namespace IOITCore.Repositories
{
    public class UserMappingRepository : AsyncGenericRepository<UserMapping, int>, IUserMappingRepository
    {
        private readonly IOITDbContext _context;
        public UserMappingRepository(IOITDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
