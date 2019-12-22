using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ApiDataService
{
    public class BankingAccount : IBankingAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [ReadOnly(true)]
        public string _id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public Nullable<bool> isActive { get; set; } = null;

        [BsonRepresentation(BsonType.DateTime)]
        public Nullable<DateTime> createdAt { get; set; } = null;

        public AccountChanged changed { get; set; }

        public void Deconstruct(out string id, out string firstname, out string lastname, out bool? isActive, out DateTime? createdAt, out AccountChanged changed)
        {
            id = this._id;
            firstname = this.firstname;
            lastname = this.lastname;
            isActive = this.isActive;
            createdAt = this.createdAt;
            changed = this.changed;
        }

        public BankingAccount AddChanged(in DateTime dateTime, in string changedby)
        {
            
            changed = new AccountChanged
            {
                changedAt = dateTime,
                changedBy = changedby
            };
            return this;
        }

        public BankingAccount AddCreateAt(in DateTime dateTime)
        {
            createdAt = dateTime;
            return this;
        }

        public KeyValuePair<string, string> GetKeyValueOf(string propertyName)
        {
            Type type = this.GetType();
            PropertyInfo[] props = type.GetProperties();
            var result = props.Where(s => s.Name == propertyName).FirstOrDefault();
            KeyValuePair<string, string> valuePair = new KeyValuePair<string, string>(result.Name, result.GetValue(this).ToString());

            return valuePair;   
            
        }

    }

    public class AccountChanged
    {

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime changedAt { get; set; }
        public string changedBy { get; set; }

        internal void Deconstruct(out DateTime changedAt, out string changedBy)
        {
            changedAt = this.changedAt;
            changedBy = this.changedBy;
        }
        
    }
}
