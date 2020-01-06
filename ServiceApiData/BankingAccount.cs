using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "The Firstname is required")]
        public string firstname { get; set; }
        [Required(ErrorMessage = "The Lastname is requiered")]
        public string lastname { get; set; }
        public bool? isActive { get; set; } = null;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? createdAt { get; set; } = null;

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

        public PatchBankingAccount AsPatchBankingAccount()
        {
            return (PatchBankingAccount)this;
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

    public class PatchBankingAccount : BankingAccount
    {
        public PatchBankingAccount(BankingAccount bankingAccount, DateTime dateTime, string changedBy)
        {
            firstname = bankingAccount.firstname;
            lastname = bankingAccount.lastname;
            isActive = bankingAccount.isActive;

            changed = new AccountChanged { changedAt = dateTime, changedBy = changedBy };
        }
    }
}
