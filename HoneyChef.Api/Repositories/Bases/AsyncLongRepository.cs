using IOITCore.Entities.Bases;
using IOITCore.Repositories.Bases;
using IOITCore.Repositories.Interfaces.Bases;
using Microsoft.EntityFrameworkCore;

namespace IOIT.Shared.Common.Bases.Repositories
{
    public class AsyncLongRepository<TEntity> : AsyncGenericRepository<TEntity, long>, IAsyncLongRepository<TEntity>
        where TEntity : BaseEntity<long>
    {
        public AsyncLongRepository(DbContext context) : base(context)
        {

        }
    }
}
