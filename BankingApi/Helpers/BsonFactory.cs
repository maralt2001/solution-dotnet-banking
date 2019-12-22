using ApiDataService;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApi.Helpers
{
    public static class BsonFactory
    {
        public async static Task<BsonDocument> GetSetDocumentAsync(BankingAccount bankingAccount)
        {
            var result = Task.Run(() =>

            {
                var serializerSettings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                };
                BsonDocument bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(bankingAccount, serializerSettings)) } };
                return bson;

            });

            return await result;


        }
        
    }
}
