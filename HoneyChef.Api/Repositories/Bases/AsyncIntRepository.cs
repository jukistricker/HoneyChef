using IOITCore.Entities.Bases;
using IOITCore.Repositories.Bases;
using IOITCore.Repositories.Interfaces.Bases;
using Microsoft.EntityFrameworkCore;

namespace IOIT.Shared.Common.Bases.Repositories
{
    public class AsyncIntRepository<TEntity> : AsyncGenericRepository<TEntity, int>, IAsyncIntRepository<TEntity>
        where TEntity : BaseEntity<int>
    {
        public AsyncIntRepository(DbContext context) : base(context)
        {

        }
    }
}
