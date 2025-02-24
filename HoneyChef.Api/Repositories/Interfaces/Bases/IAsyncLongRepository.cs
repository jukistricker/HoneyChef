using IOITCore.Entities.Bases;

namespace IOITCore.Repositories.Interfaces.Bases
{
    public interface IAsyncLongRepository<TEntity> : IAsyncGenericRepository<TEntity, long>
        where TEntity : BaseEntity<long>
    {

    }
}
