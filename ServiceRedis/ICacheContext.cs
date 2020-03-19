using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceRedis
{
    public interface ICacheContext
    {
        IDatabase Database { get; set; }
        
        Task<RedisValue[]> LoadStringsAsync(RedisKey[] redisKeys, bool contextLoggerEnable = false);
        Task<bool> SaveStringsAsync(KeyValuePair<RedisKey, RedisValue>[] keyValues, bool contextLoggerEnable = false);
        Task<bool> SaveHashAsync<T>(T dataObject, ConnectionInfo info, bool contextLoggerEnable = false);
        Task<T> LoadHashAsync<T>(ConnectionInfo info, bool contextLoggerEnable = false);
        


    }
}