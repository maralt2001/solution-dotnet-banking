
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceRedis
{
    public abstract class CacheContext : ICacheContext
    {

        public IDatabase Database { get; set; }
        public ConnectionMultiplexer Connection { get; set; }
        public string ConnectionPath { get; set; }
        public static ILogger ContextLogger { get; set; }
        public static int HashExpire { get; set; }
        public static bool StartupFail { get; set; } = false;

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

        public async Task<bool> SaveHashAsync<T>(T dataObject, ConnectionInfo info, bool contextLoggerEnable = false)
        {
            return await Task.Run(() => 
            {
                try
                {
                    if (contextLoggerEnable)
                    {
                        ContextLogger.LogInformation("Saving in redis cache");
                    }
                    
                    Database.HashSet(info.Id, ToHashEntries(dataObject));
                    Database.KeyExpire(info.Id, TimeSpan.FromMinutes(HashExpire));
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

            
        }

        public async Task<T> LoadHashAsync<T>(RedisKey key, bool contextLoggerEnable = false)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (contextLoggerEnable)
                    {
                        ContextLogger.LogInformation("Loading from redis cache");
                    }
               
                    HashEntry[] loadCache = Database.HashGetAll(key);
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
            
        }

        public async Task<List<RedisKey>> LoadKeysOfCache()
        {
            return await Task.Run(() =>
            {
                try
                {
                    return Connection.GetServer(ConnectionPath).Keys().ToList();
                }
                catch (NullReferenceException)
                {

                    return new List<RedisKey>();
                }
                

            });
            
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
                ).ToArray();
        }

        private T GetObjectFromHash<T>(HashEntry[] hashEntries)
        {
            
            var cacheItemsList = new List<KeyValuePair<string, string>>();

            //iterate each item in hashEntries Array an set it as key value pair in cachItemList
            foreach (var item in hashEntries)
            {
                cacheItemsList.Add(new KeyValuePair<string, string>(item.Name, item.Value));
            }

            //create Oject of type T from Method call
            T obj = (T)Activator.CreateInstance(typeof(T));

            //get properties all properties of obj
            PropertyInfo[] objectProperties = obj.GetType().GetProperties();

            // iterate of each Property Info Array set for every Property of obj the value from the cachItemsList
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
            try
            {
                this.Connection = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { connectionPath },
                    ConnectRetry = 1,
                    ReconnectRetryPolicy = new LinearRetry(5000)

                }
                );
                this.Database = Connection.GetDatabase();
                this.ConnectionPath = connectionPath;
            }
            catch (RedisConnectionException)
            {
                StartupFail = true;
                HandleRedisConnectionError(connectionPath);
                return;
            }


        }

        private void HandleRedisConnectionError(string path)
        {
            // Thread run at startup when Redis Client failed to connect. Retry interval every 10s
            Thread startUpRedisConnection = new Thread(() =>
            {
                while (StartupFail)
                {
                    try
                    {
                        this.Connection = ConnectionMultiplexer.Connect(
                        new ConfigurationOptions
                        {
                            EndPoints = { path },
                            ConnectRetry = 1,
                            ReconnectRetryPolicy = new LinearRetry(5000)

                        });
                        this.Database = Connection.GetDatabase();
                        this.ConnectionPath = path;
                        StartupFail = false;
                        ContextLogger.LogInformation("Connection to redis established");
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                        StartupFail = true;
                        ContextLogger.LogError("Connection to redis fails");
                    }
                }
            });
            startUpRedisConnection.Start();
           
            
        }

    }
}
