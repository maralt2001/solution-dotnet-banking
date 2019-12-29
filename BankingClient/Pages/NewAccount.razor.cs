using ApiDataService;
using BankingClient.Data;
using BankingClient.Provider;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingClient.Pages
{
    public class NewAccountBase : ComponentBase
    {
        #region Dependency Injection
        [Inject] private BankingAccountsService _BankingAccountService { get; set; }
        [Inject] public UserState _UserState { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        #endregion

        [Parameter] public BankingAccount _BankingAccount { get; set; } = new BankingAccount();
        [Parameter] public string _firstname { get; set; } = "";
        [Parameter] public string _lastname { get; set; } = "";
        [Parameter] public bool _isActive { get; set; } = false;

        [Parameter] public bool _isAccountSaved { get; set; } = false;

        public async void  SaveAccount()
        {

          bool saveresult = await _BankingAccountService.SaveAccountAsync(new BankingAccount {firstname = _firstname, lastname = _lastname, isActive = _isActive });
          if (saveresult)
          {
             _isAccountSaved = true;
                _firstname = "";
                _lastname = "";
                StateHasChanged();
            }
          else
          {
            _isAccountSaved = false;
                _firstname = "";
                _lastname = "";
                StateHasChanged();
            }

            
           
        }
    }
}
