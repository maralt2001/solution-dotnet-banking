
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceRedis
{
    public abstract class CacheContext : ICacheContext
    {

        public IDatabase Database { get; set; }
        public static ILogger ContextLogger { get; set; }

        public async Task<bool> SaveStringsAsync(KeyValuePair<RedisKey, RedisValue>[] keyValues, bool contextLoggerEnable = false)
        {
            var result = Task.Run(() =>
            {
                try
                {
                    if(contextLoggerEnable)
                    {
                        ContextLogger.LogInformation("Saving in redis cache");
                    }
                    Database.StringSet(keyValues);
                    return true;
                }
                catch (Exception)
                {
                    if(contextLoggerEnable)
                    {
                        ContextLogger.LogWarning("Saving in redis went wrong");
                    }
                    return false;
                }
            });

            return await result;

        }
        public async Task<RedisValue[]> LoadStringsAsync(RedisKey[] redisKeys, bool contextLoggerEnable = false)
        {
            var result = Task.Run(() =>
            {
                try
                {
                    if (contextLoggerEnable)
                    {
                        ContextLogger.LogInformation("Loading from redis cache");
                    }
                    return Database.StringGet(redisKeys);
                }
                catch (Exception)
                {
                    if (contextLoggerEnable)
                    {
                        ContextLogger.LogWarning("Loading from redis went wrong");
                    }

                    return Array.Empty<RedisValue>();
                }
            });

            return await result;
        }
    }

    public class RedisClient : CacheContext
    {
        public RedisClient(string connectionPath)
        {
           this.Database = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { connectionPath },
                    ConnectRetry = 3
                }
                ).GetDatabase();

        }

        
    }
}
