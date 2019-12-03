﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ApiAccess
{
    public class ApplicationUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [ReadOnly(true)]
        public string _id { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string password { get; set; }
        public int accessFailedCount { get; set; }
        public DateTime createdAt { get; set; }

        public ApplicationUser()
        {

        }

        public ApplicationUser(string email, string password)
        {
            this.email = email;
            this.password = HashPassword(password).Result;
            this.createdAt = DateTime.Now;
            this.accessFailedCount = 0;
        }


        internal async Task<string> HashPassword(string password)
        {
            var result = Task.Run(() =>
            {
                return new PasswordHasher<ApplicationUser>().HashPassword(this, password);

            });

            return await result;

        }

        public async Task<PasswordVerificationResult> PasswordVerification(string hashedPassword, string providedPassword)
        {
            var result = Task.Run(() =>
            {
                return new PasswordHasher<ApplicationUser>().VerifyHashedPassword(this, hashedPassword, providedPassword);

            });

            return await result;
        }

        public async Task<bool> IsValidEmail()
        {
            var result = Task.Run(() =>
            {
                return new EmailAddressAttribute().IsValid(this.email);
            });

            return await result;
        }

        
    }
}
