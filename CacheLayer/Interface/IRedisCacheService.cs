namespace CacheLayer.Interface
{
    public interface IRedisCacheService
    {
        Task<T?> GetCachedData<T>(string key);
        Task SetCachedData<T>(string key, T value, TimeSpan expiration);
        Task<bool> RemoveCachedData(string key);
    }
}
