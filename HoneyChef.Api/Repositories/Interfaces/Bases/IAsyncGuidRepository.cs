using IOITCore.Entities.Bases;

namespace IOITCore.Repositories.Interfaces.Bases
{
    public interface IAsyncGuidRepository<TEntity> : IAsyncGenericRepository<TEntity, Guid>
        where TEntity : BaseEntity<Guid>
    {
    }
}
