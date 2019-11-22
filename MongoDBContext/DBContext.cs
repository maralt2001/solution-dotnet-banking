using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoService.Helpers;

namespace MongoService
{
    public abstract class DBContext : IDBContext
    {
        public MongoClient MongoClient { get; set; }
        public IMongoDatabase Database { get; set; }

        public async Task<bool> InsertRecordAsync<T>(string collectionName, T record)
        {
            try
            {
                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
                await collection.InsertOneAsync(record);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<List<T>> LoadRecordsAsync<T>(string collectionName)
        {
            IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
            return await collection.FindAsync(new BsonDocument()).Result.ToListAsync();

        }

        public async Task<bool> UpdateRecordAsync<T>(string collectionName, string id, T record)
        {
            BsonDocument bson = await BsonFactory.GetSetDocumentAsync(record);

            var filter = Builders<T>.Filter.Eq("_id", id);
            try
            {
                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
                var result = await collection.UpdateOneAsync(filter, bson);
                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<T> LoadRecordAsync<T>(string collectionName, string id)
        {
            try
            {

                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", id);
                return await collection.FindAsync(filter).Result.FirstOrDefaultAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> DeleteRecordAsync<T>(string collectionName, string id)
        {
            try
            {
                IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
                var filter = Builders<T>.Filter.Eq("_id", id);
                var result = await collection.DeleteOneAsync(filter);
                if (result.DeletedCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<T>> LoadRecordsSkpLimitAsync<T>(string collectionName, int limit, int skip)
        {
            IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);

            return await collection.Find(new BsonDocument()).Skip(skip).Limit(limit).ToListAsync();

        }

        public async Task<T> LoadOneRecordRegexAsync<T>(string collectionName, string field, string regexvalue)
        {
            IMongoCollection<T> collection = Database.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Regex(field, new BsonRegularExpression($"{regexvalue}", "i"));
            return await collection.FindAsync(filter).Result.FirstOrDefaultAsync();
        }

        public async Task<bool> IsConnectionUp (int secondToWait = 1)
        {
            if (secondToWait <= 0)
            {
                throw new ArgumentOutOfRangeException("secondToWait", secondToWait, "Must be at least 1 second");
            }
            else
            {
                Task<bool> result = Task.Run(() => { return Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(secondToWait * 1000);});
                return await result;
               
            }
                
        }


    }

    public class MongoWithCredential : DBContext
    {
        public MongoWithCredential(string databaseName, string databaseUrl, string user, string password)
        {
            MongoCredential credential = MongoCredential.CreateCredential(databaseName, user, password);
            MongoClientSettings settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(databaseUrl)
            };
            MongoClient = new MongoClient(settings);
            Database = MongoClient.GetDatabase(databaseName);
            
            
        }
        
    }

    public class MongoWithoutCredential : DBContext
    {
        public MongoWithoutCredential(string databaseName, string databaseUrl)
        {
            MongoClient = new MongoClient(databaseUrl);
            Database = MongoClient.GetDatabase(databaseName);
        }
    }
}
