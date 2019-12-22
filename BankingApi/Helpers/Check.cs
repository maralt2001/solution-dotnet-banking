using ApiDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BankingApi.Helpers
{
    public static class Check
    {
        public static async Task<List<string>> GetFieldsOfBankingAccount(BankingAccount bankingAccount)
        {

            var checkBankingAccount = Task.Run(() =>
            {
                List<string> list = new List<string>();
                Type type = bankingAccount.GetType();
                
                foreach(PropertyInfo propertyInfo in type.GetProperties()) 
                {
                   
                   list.Add(propertyInfo.Name);
                    
                }
                return list;
            });

            return await checkBankingAccount;

        }

        public static async Task<List<string>> GetPropertyAndValue(BankingAccount bankingAccount)
        {
            var checkPropertyAndValue = Task.Run(() =>
            {
                Type type = bankingAccount.GetType();

                PropertyInfo[] props = type.GetProperties();
                List<string> list = new List<string>();
                foreach (var prop in props)
                {
                    if (prop.GetValue(bankingAccount) != null)
                    {
                        list.Add(prop.Name);
                        list.Add(prop.GetValue(bankingAccount).ToString());
                    }
                }
                return list;

            });

            return await checkPropertyAndValue;

        }
    }
}
