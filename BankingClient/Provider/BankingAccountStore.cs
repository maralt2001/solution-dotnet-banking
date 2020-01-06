using ApiDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace BankingClient.Provider
{
    public class BankingAccountStore
    {
        public BankingAccount[] Blob = new BankingAccount[0];

        public void SetBankingAccountToBlob (BankingAccount bankingAccount)
        {
            BankingAccount[] array = { bankingAccount };
            this.Blob = array;
            
        }

        public void SetBankingAccountsToBlob (BankingAccount[] bankingAccounts)
        {
            BankingAccount[] array = bankingAccounts;
            this.Blob = array;
        }

        internal BankingAccount[] SortDescending(string byKey)
        {
            if(Blob.Length != 0)
            {
                
                PropertyInfo propertyInfo = typeof(BankingAccount).GetProperty(byKey);
                return Blob.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToArray<BankingAccount>();
               

            }
            else
            {
                return Blob;
            }
            
        }

        internal BankingAccount[] SortAscending(string byKey)
        {
            if (Blob.Length != 0)
            {

                PropertyInfo propertyInfo = typeof(BankingAccount).GetProperty(byKey);
                return Blob.OrderBy(x => propertyInfo.GetValue(x, null)).ToArray<BankingAccount>();
                

            }
            else
            {
                return Blob;
            }
        }
        
    }

    
}
