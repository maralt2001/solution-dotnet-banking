using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApi.Helpers
{
    public abstract class RedisDataLayer
    {
        public static Task<bool> CachingStrings(IDatabase cache, KeyValuePair<RedisKey,RedisValue>[] keyValues)
        {
			var result = Task.Run(() => 
			{
				try
				{
					cache.StringSet(keyValues);
					return true;
				}
				catch (Exception)
				{

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
					return cache.StringGet(redisKeys);
				}
				catch (Exception)
				{

					return Array.Empty<RedisValue>();
				}
			});

			return result;
			
		}
    }
}
