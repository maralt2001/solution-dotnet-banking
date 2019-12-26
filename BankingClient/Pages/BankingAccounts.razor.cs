using ApiDataService;
using BankingClient.Data;
using Microsoft.AspNetCore.Components;
using BankingClient.Provider;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async void SetSelectedOption(ChangeEventArgs e)
        {
            var result = Task.Run(() =>
            {
                _SelectedOption = e.Value.ToString();
            });

            await result;
        }

        // Method for search an Account via Api call. The call is providet by Data.BankingAccountService
        public async void SearchAccount()
        {
            Task<BankingAccount[]> result = _BankingAccountService.GetAccountsRegexAsync(_SelectedOption.ToLower(), _SearchTextValue);
            _BankingAccountStore.SetBankingAccountsToBlob(await result);
            _ResetButtonIsDisabled = false;
            _GoButtonIsDisabled = true;
            _SearchTextValue = "";
            
            StateHasChanged();

        }

        // Method for Render Children ViewAccountDetails
        public async void ShowAccountDetails(BankingAccount account)
        {
            var result = Task.Run(() => 
            {
                _ShowDetails = true;
                _Account = account;
            });

            await result;
        }

        // Method for close Children ViewAccountDetails
        public async void CloseAccountDetails(bool Value)
        {
            var result = Task.Run(() =>
            {
                _ShowDetails = false;
                _Account = new BankingAccount();
            });

            await result;
            StateHasChanged();
        }

        // Method clear the BankingAccountStore and handle the Button states
        public async void ResetTable()
        {
            var result = Task.Run(() => 
            {
                _BankingAccountStore = new BankingAccountStore();
                _ResetButtonIsDisabled = true;
                _GetAllButtonIsDisabled = false;
                
            });

            await result;
            StateHasChanged();

        }

        // Method handle the Button states
        public void EnableGoButton(KeyboardEventArgs args)
        {
            _GoButtonIsDisabled = false;
            _GetAllButtonIsDisabled = true;
            StateHasChanged();
        }

        protected override async void OnInitialized()
        {
            var result = Task.Run(() => 
            {
                _BankingAccountStore = new BankingAccountStore();
            });

            await result;
            
        }

    }
}
