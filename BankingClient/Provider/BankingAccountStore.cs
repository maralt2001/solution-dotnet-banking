using ApiDataService;
using System;
using System.Collections.Generic;
using System.Linq;
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
        
    }

    
}
