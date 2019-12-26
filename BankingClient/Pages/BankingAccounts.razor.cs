using ApiDataService;
using BankingClient.Data;
using Microsoft.AspNetCore.Components;
using BankingClient.Provider;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace BankingClient.Pages
{
    public class BaseBankingAccounts : ComponentBase
    {
        #region Dependency Injections
        [Inject] public BankingAccountStore _BankingAccountStore { get; set; }
        [Inject] private BankingAccountsService _BankingAccountService { get; set; }
        [Inject] public UserState _UserState { get; set; }
        #endregion

        #region Page Parameter/Variables
        [Parameter] public bool _ResetButtonIsDisabled { get; set; } = true;
        [Parameter] public bool _GetAllButtonIsDisabled { get; set; } = false;
        [Parameter] public bool _GoButtonIsDisabled { get; set; } = true;

        [Parameter] public string _SearchTextValue { get; set; } = "";
        [Parameter] public List<string> _SearchOptions { get; set; } = new List<string>() { "Firstname", "Lastname" };
        [Parameter] public string _SelectedOption { get; set; } = "Firstname";

        [Parameter] public bool _ShowDetails { get; set; } = false;
        [Parameter] public BankingAccount _Account { get; set; }
        #endregion


        // Method for get all Accounts via Api call. The call is providet by Data.BankingAccountService
        public async void GetAllAccounts()
        {
            _BankingAccountStore.Blob = await _BankingAccountService.GetAccountsAsync();
            _ResetButtonIsDisabled = false;
            _GetAllButtonIsDisabled = true;
            StateHasChanged();

        }

        public void SetSelectedOption(ChangeEventArgs e)
        {
            _SelectedOption = e.Value.ToString();
        }

        // Method for search an Account via Api call. The call is providet by Data.BankingAccountService
        public async void SearchAccount()
        {
            
            var result = _BankingAccountService.GetOneAccountAsync(_SelectedOption.ToLower(), _SearchTextValue);
            _BankingAccountStore.SetBankingAccountToBlob(await result);
            _ResetButtonIsDisabled = false;
            _GoButtonIsDisabled = true;
            _SearchTextValue = "";
            
            StateHasChanged();

        }

        // Method for Render Children ViewAccountDetails
        public void ShowAccountDetails(BankingAccount account)
        {
            _ShowDetails = true;
            _Account = account;
        }

        // Method for close Children ViewAccountDetails
        public void CloseAccountDetails(bool Value)
        {
            _ShowDetails = false;
            _Account = new BankingAccount();
            StateHasChanged();
        }

        // Method clear the BankingAccountStore and handle the Button states
        public void ResetTable()
        {
            _BankingAccountStore = new BankingAccountStore();
            _ResetButtonIsDisabled = true;
            _GetAllButtonIsDisabled = false;
            StateHasChanged();
        }

        // Method handle the Button states
        public void EnableGoButton(KeyboardEventArgs args)
        {
            _GoButtonIsDisabled = false;
            _GetAllButtonIsDisabled = true;
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            _BankingAccountStore = new BankingAccountStore();
        }

    }
}
