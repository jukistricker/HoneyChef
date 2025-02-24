using IOIT.Shared.Common.Bases.Persistence;
using IOITCore.Entities.Bases;
using IOITCore.Enums;
using IOITCore.Extensions;
using IOITCore.Repositories.Interfaces.Bases;
using Microsoft.EntityFrameworkCore;

namespace IOITCore.Repositories.Bases
{
    public class AsyncGenericRepository<TEntity, TId> : IAsyncGenericRepository<TEntity, TId>
        where TEntity : BaseEntity<TId>
    {
        //private readonly IOptions<DbConnections> _options;
        public DbContext DbContext
        {
            get;
        }
        public IQueryable<TEntity> DbSet
        {
            get;
        }

        public AsyncGenericRepository(DbContext context)
        {
            DbContext = context;
            DbSet = context.Set<TEntity>();
            //_options = options;
        }

        //public IQueryable<TEntity> All()
        //{
        //    return DbSet;
        //}
        public IQueryable<TEntity> All(bool skipDeleted = true)
        {
            var query = DbSet.AsNoTracking();
            if (skipDeleted)
                query = query.Where(p => p.Status != ApiEnums.EntityStatus.DELETED);
            return query;
        }

        public void Add(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
        }
        public void Add(TEntity entity, ApiEnums.EntityStatus status = ApiEnums.EntityStatus.NORMAL)
        {
            entity.Status = status;
            DbContext.Set<TEntity>().Add(entity);
        }
        public async Task AddAsync(TEntity entity)
        {
            await DbContext.Set<TEntity>().AddAsync(entity);
        }

        public void AddRange(List<TEntity> entities)
        {
            DbContext.Set<TEntity>().AddRange(entities);
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            await DbContext.Set<TEntity>().AddRangeAsync(entities);
        }

        public async Task<int> CountAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public void Delete(TEntity entity)
        {
            //DbContext.Set<TEntity>().Remove(entity);

            entity.Status = ApiEnums.EntityStatus.DELETED;
            Update(entity);
        }

        public void DeleteRange(List<TEntity> entities)
        {
            //DbContext.Set<TEntity>().RemoveRange(entities);

            foreach (TEntity entity in entities)
            {
                entity.Status = ApiEnums.EntityStatus.DELETED;
            }
            UpdateRange(entities);
        }

        public async Task<TEntity> GetByKeyAsync(TId keyValue, bool skipDeleted = true)
        {
            if (skipDeleted)
                return await DbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Id.Equals(keyValue) && x.Status != ApiEnums.EntityStatus.DELETED);
            else
                return await DbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Id.Equals(keyValue));

        }

        //public TEntity GetByKey(TId keyValue)
        //{
        //    return DbSet.AsNoTracking().SingleOrDefault(x => x.Id.Equals(keyValue));
        //}
        public TEntity? GetByKey(TId keyValue, bool skipDeleted = true)
        {
            var query = DbSet.AsNoTracking().Where(p => p.Id.Equals(keyValue));
            if (skipDeleted)
                return query.Where(p => p.Status != ApiEnums.EntityStatus.DELETED).SingleOrDefault();
            else
                return query.SingleOrDefault();
        }
        public async Task<TEntity> FirstAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<TEntity> FindAsync(object[] keyValues)
        {
            return await DbContext.Set<TEntity>().FindAsync(keyValues);
        }
        public async Task<IReadOnlyList<TEntity>> ListAllAsync()
        {
            return await DbSet.Where(s => s.Status != ApiEnums.EntityStatus.DELETED).ToListAsync();
        }

        public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }
        public async Task<IPagedResult<TEntity>> PaggingAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecificationPaggingAsync(spec);
        }
        public void Update(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange(List<TEntity> entities)
        {
            DbContext.UpdateRange(entities);
        }

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
        {
            return SpecificationEvaluator<TEntity, TId>.GetQuery(DbSet.AsQueryable(), spec);
        }
        private async Task<IPagedResult<TEntity>> ApplySpecificationPaggingAsync(ISpecification<TEntity> spec)
        {
            return await SpecificationEvaluator<TEntity, TId>.PaggingAsync(DbSet.AsQueryable(), spec);
        }

        public async Task<IList<TResult>> GetFromSql<TResult>(string storeProcedure, params AppSpParameter[] parameters) where TResult : new()
        {
            return await DbContext.GetFromSqlAsync<TResult>(storeProcedure, parameters);
        }

        public async Task<IList<TEntity>> GetByKeysAsync(List<TId> keyValues)
        {
            return await DbSet.Where(c => keyValues.Contains(c.Id)).ToListAsync();
        }

        public async Task<int> ExecuteNonQuery(string storeProcedure, params AppSpParameter[] parameters)
        {
            return await DbContext.ExecuteNonQuery(storeProcedure, parameters);
        }

        public void DetectEntity(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Detached;
        }
    }
}
