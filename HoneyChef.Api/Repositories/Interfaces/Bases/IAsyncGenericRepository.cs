using IOITCore.Entities.Bases;
using IOITCore.Enums;
using IOITCore.Repositories.Bases;

namespace IOITCore.Repositories.Interfaces.Bases
{
    public interface IAsyncGenericRepository<TEntity, TId>
        where TEntity : BaseEntity<TId>
    {
        IQueryable<TEntity> All(bool skipDeleted = true);

        #region Add
        Task AddAsync(TEntity entity);
        void Add(TEntity entity);
        void Add(TEntity entity, ApiEnums.EntityStatus status = ApiEnums.EntityStatus.NORMAL);
        Task AddRangeAsync(List<TEntity> entities);
        void AddRange(List<TEntity> entities);
        #endregion

        #region Update
        //Task<TEntity> UpdateAsync(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        Task<int> ExecuteNonQuery(string storeProcedure, params AppSpParameter[] parameters);
        #endregion

        #region Delete
        void Delete(TEntity entity);
        void DeleteRange(List<TEntity> entities);
        #endregion

        Task<int> CountAsync(ISpecification<TEntity> spec);

        #region Get
        Task<TEntity> GetByKeyAsync(TId keyValue, bool skipDeleted = true);
        Task<IList<TEntity>> GetByKeysAsync(List<TId> keyValues);
        TEntity? GetByKey(TId keyValue, bool skipDeleted = true);
        Task<TEntity> FirstAsync(ISpecification<TEntity> spec);
        Task<TEntity> FindAsync(object[] keyValues);
        Task<IList<TResult>> GetFromSql<TResult>(string storeProcedure, params AppSpParameter[] parameters) where TResult : new();
        Task<IReadOnlyList<TEntity>> ListAllAsync();
        Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec);
        Task<IPagedResult<TEntity>> PaggingAsync(ISpecification<TEntity> spec);
        #endregion

        #region "detectChange"
        void DetectEntity(TEntity entity);
        #endregion
    }
}
