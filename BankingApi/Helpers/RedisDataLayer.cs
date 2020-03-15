using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApi.Helpers
{
    public abstract class RedisDataLayer
    {
		
        public static Task<bool> CachingStrings(IDatabase cache, ILogger logger, KeyValuePair<RedisKey,RedisValue>[] keyValues)
        {
			var result = Task.Run(() => 
			{
				try
				{
					cache.StringSet(keyValues);
					logger.LogInformation("caching strings in redis");
					return true;
				}
				catch (Exception)
				{
					logger.LogWarning("caching string in redis went wrong");
					return false;
				}
			});
			
			return result;
			
        }

		public static Task<RedisValue[]> GetStringsFromCache(IDatabase cache, ILogger logger, RedisKey[] redisKeys)
		{
			var result = Task.Run(() => 
			{
				try
				{
					logger.LogInformation("reading values from redis cache");
					return cache.StringGet(redisKeys);
				}
				catch (Exception)
				{
					logger.LogWarning("reading values from redis cache went wrong");
					return Array.Empty<RedisValue>();
				}
			});

			return result;
			
		}
    }
}
