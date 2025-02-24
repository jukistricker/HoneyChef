using IOITCore.Entities.Bases;
using IOITCore.Repositories.Bases;
using IOITCore.Repositories.Interfaces.Bases;
using Microsoft.EntityFrameworkCore;

namespace IOIT.Shared.Common.Bases.Repositories
{
    public class AsyncGuidRepository<TEntity> : AsyncGenericRepository<TEntity, Guid>, IAsyncGuidRepository<TEntity>
        where TEntity : BaseEntity<Guid>
    {
        public AsyncGuidRepository(DbContext context) : base(context)
        {

        }
    }
}
