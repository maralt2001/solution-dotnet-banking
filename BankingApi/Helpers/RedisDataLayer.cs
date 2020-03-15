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
		public static ILogger _logger { get; set; }
        public static Task<bool> CachingStrings(IDatabase cache, KeyValuePair<RedisKey,RedisValue>[] keyValues)
        {
			var result = Task.Run(() => 
			{
				try
				{
					cache.StringSet(keyValues);
					_logger.LogInformation("caching strings in redis");
					return true;
				}
				catch (Exception)
				{
					_logger.LogWarning("caching string in redis went wrong");
					return false;
				}
			});
			
			return result;
			
        }

		public static Task<RedisValue[]> GetStringsFromCache(IDatabase cache, RedisKey[] redisKeys)
		{
			var result = Task.Run(() => 
			{
				try
				{
					_logger.LogInformation("reading values from redis cache");
					return cache.StringGet(redisKeys);
				}
				catch (Exception)
				{
					_logger.LogWarning("reading values from redis cache went wrong");
					return Array.Empty<RedisValue>();
				}
			});

			return result;
			
		}
    }
}
