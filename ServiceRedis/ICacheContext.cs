﻿using Microsoft.Extensions.Logging;
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
    }
}