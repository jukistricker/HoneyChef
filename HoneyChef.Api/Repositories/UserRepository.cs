using IOITCore.Entities;
using IOITCore.Persistence;
using IOITCore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using IOITCore.Repositories.Bases;

namespace IOITCore.Repositories
{
    public class UserRepository : AsyncGenericRepository<User, long>, IUserRepository
    {
        private readonly IOITDbContext _context;
        public UserRepository(IOITDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByUserName(string username)
        {
            var entity = await DbSet.Where(s => s.UserName.Contains(username)).FirstOrDefaultAsync();

            return entity;
        }
    }
}
