using IOITCore.Services.Interfaces;

namespace IOITCore.Services.Common
{
    public class CacheService : ICacheService
    {
        public T GetCache<T>(string key)
        {
            throw new NotImplementedException();
        }

        public object GetCache(string key, Type type)
        {
            throw new NotImplementedException();
        }

        public T GetCache<T>(string key, Func<T> acquire)
        {
            throw new NotImplementedException();
        }

        public T GetCache<T>(string key, int cacheTime, Func<T> acquire)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetCacheAsync<T>(string key, Func<Task<T>> acquire)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetCacheAsync<T>(string key, int cacheTime, Func<Task<T>> acquire)
        {
            throw new NotImplementedException();
        }

        public void RemoveCache(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveCacheAsync(string key)
        {
            throw new NotImplementedException();
        }

        public void SaveCache<T>(string key, int cacheTime, Func<T> acquire)
        {
            throw new NotImplementedException();
        }

        public void SaveCache<T>(string key, int cacheTime, T result)
        {
            throw new NotImplementedException();
        }

        public void SaveCache<T>(string key, Func<T> acquire)
        {
            throw new NotImplementedException();
        }

        public void SaveCache<T>(string key, T result)
        {
            throw new NotImplementedException();
        }

        public Task SaveCacheAsync<T>(string key, Func<Task<T>> acquire)
        {
            throw new NotImplementedException();
        }

        public Task SaveCacheAsync<T>(string key, int cacheTime, Func<Task<T>> acquire)
        {
            throw new NotImplementedException();
        }
    }
}
