
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public async Task<bool> SerializeObjectAndSaveStringAsync<T>(T dataObject, ConnectionInfo info, bool contextLoggerEnable = false)
        {
            var result = Task.Run(() => 
            {
                try
                {
                    if (contextLoggerEnable)
                    {
                        ContextLogger.LogInformation("Saving in redis cache");
                    }
                    string connectionId = info.Id;
                    var hashEntries = ToHashEntries(dataObject);
                    Database.HashSet(connectionId, hashEntries);
                    return true;
                }
                catch (Exception)
                {
                    if (contextLoggerEnable)
                    {
                        ContextLogger.LogWarning("Saving in redis went wrong");
                    }
                    return false;
                }
            });

            return await result;
            
        }

        public async Task<T> LoadFromCacheReturnObject<T>(ConnectionInfo info, bool contextLoggerEnable = false)
        {
            var result = Task.Run(() =>
            {
            try
            {
                if (contextLoggerEnable)
                {
                    ContextLogger.LogInformation("Loading from redis cache");
                }
               
                    HashEntry[] loadCache = Database.HashGetAll(info.Id);
                    return GetObjectFromHash<T>(loadCache);

                }
                catch (Exception)
                {
                    if (contextLoggerEnable)
                    {
                        ContextLogger.LogWarning("Loading from redis went wrong");
                    }


                    return (T)Activator.CreateInstance(typeof(T));
                }
            });

           return await result;
            
        }

        private HashEntry[] ToHashEntries(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select
                (
                      property =>
                      {
                          object propertyValue = property.GetValue(obj);
                          string hashValue;

                  // This will detect if given property value is 
                  // enumerable, which is a good reason to serialize it
                  // as JSON!
                  if (propertyValue is IEnumerable<object>)
                          {
                      // So you use JSON.NET to serialize the property
                      // value as JSON
                      hashValue = JsonConvert.SerializeObject(propertyValue);
                          }
                          else
                          {
                              hashValue = propertyValue.ToString();
                          }

                          return new HashEntry(property.Name, hashValue);
                      }
                )
                .ToArray();
        }

        private T GetObjectFromHash<T>(HashEntry[] hashEntries)
        {
            var cacheItemsList = new List<KeyValuePair<string, string>>();
            foreach (var item in hashEntries)
            {
                cacheItemsList.Add(new KeyValuePair<string, string>(item.Name, item.Value));
            }

            T obj = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] objectProperties = obj.GetType().GetProperties();

            foreach (var item in objectProperties)
            {
                obj.GetType().GetProperty(item.Name).SetValue(obj, cacheItemsList.Find(s => s.Key == item.Name).Value);
            }

            return obj;
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
