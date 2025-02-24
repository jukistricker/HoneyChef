using IOITCore.Entities.Bases;

namespace IOITCore.Repositories.Interfaces.Bases
{
    public interface IAsyncIntRepository<TEntity> : IAsyncGenericRepository<TEntity, int>
        where TEntity : BaseEntity<int>
    {

    }
}
