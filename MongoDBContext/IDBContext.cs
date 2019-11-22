using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoService
{
    public interface IDBContext
    {
        IMongoDatabase Database { get; set; }
        MongoClient MongoClient { get; set; }

        Task<bool> DeleteRecordAsync<T>(string collectionName, string id);
        Task<bool> InsertRecordAsync<T>(string collectionName, T record);
        Task<T> LoadRecordAsync<T>(string collectionName, string id);
        Task<List<T>> LoadRecordsAsync<T>(string collectionName);
        Task<List<T>> LoadRecordsSkpLimitAsync<T>(string collectionName, int limit, int skip);
        Task<bool> UpdateRecordAsync<T>(string collectionName, string id, T record);
        Task<T> LoadOneRecordRegexAsync<T>(string collectionName, string field, string regexvalue);
        Task<bool> IsConnectionUp(int secondToWait = 1);
    }
}