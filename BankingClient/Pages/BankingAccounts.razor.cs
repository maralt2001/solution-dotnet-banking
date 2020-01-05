using ApiDataService;
using BankingClient.Data;
using Microsoft.AspNetCore.Components;
using BankingClient.Provider;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Linq;
using System;

namespace BankingClient.Pages
{
    public class BaseBankingAccounts : ComponentBase
    {
        #region Dependency Injections
        [Inject] public BankingAccountStore _BankingAccountStore { get; set; }
        [Inject] private IBankingAccountsService _BankingAccountService { get; set; }
        [Inject] public UserState _UserState { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] public IJSRuntime _JSRuntime { get; set; }
        #endregion

        #region Page Parameter/Variables
        
        [Parameter] public ElementReference ResetButton { get; set; }
        [Parameter] public ElementReference GoButton { get; set; }
        [Parameter] public ElementReference AllButton { get; set; }

        [Parameter] public string _SearchTextValue { get; set; } = string.Empty;
        [Parameter] public List<string> _SearchOptions { get; set; } = new List<string>() { "Firstname", "Lastname" };
        [Parameter] public string _SelectedOption { get; set; } = "Firstname";

        [Parameter] public bool _ShowDetails { get; set; } = false;
        [Parameter] public BankingAccount _Account { get; set; }

        [Parameter] public Dictionary<string, object> _ButtonAttributes { get; set; } = new Dictionary<string, object>()
        {
            {"style","margin-left:3px"},
            {"type", "button" }
        };

        [Parameter] public string SortOption { get; set; } = "ascending";

        [Parameter] public bool _EditButtonClicked { get; set; } = false;
        [Parameter] public BankingAccount _UpdateAccount { get; set; }
        #endregion


        // Method for get all Accounts via Api call. The call is providet by Data.BankingAccountService
        public async void GetAllAccounts()
        {
            _BankingAccountStore.Blob = await _BankingAccountService.GetAccountsAsync();
            await _JSRuntime.InvokeVoidAsync("enableButton", ResetButton);
            await _JSRuntime.InvokeVoidAsync("disableButton", AllButton);
            StateHasChanged();
        }

        

        public void SetSelectedOption(ChangeEventArgs e)
        {
           _SelectedOption = e.Value.ToString();
            StateHasChanged();
           
        }

        // Method for search an Account via Api call. The call is providet by Data.BankingAccountService
        public async void SearchAccount()
        {
            
            Task<BankingAccount[]> result = _BankingAccountService.GetAccountsRegexAsync(_SelectedOption.ToLower(), _SearchTextValue);
            _BankingAccountStore.SetBankingAccountsToBlob(await result);
            await _JSRuntime.InvokeVoidAsync("enableButton", ResetButton);
            await _JSRuntime.InvokeVoidAsync("disableButton", GoButton);
            _SearchTextValue = string.Empty;
            
            StateHasChanged();

        }

        // Method for Render Children ViewAccountDetails
        public void ShowAccountDetails(BankingAccount account)
        {
            _ShowDetails = true;
            _Account = account;
            StateHasChanged();
            
        }

        // Method for close Children ViewAccountDetails
        public void CloseAccountDetails(bool Value)
        {
            _ShowDetails = false;
            StateHasChanged();
        }

        // Method clear the BankingAccountStore and handle the Button states
        public async void ResetTable()
        {
            _BankingAccountStore = new BankingAccountStore();
            await _JSRuntime.InvokeVoidAsync("disableButton", ResetButton);
            await _JSRuntime.InvokeVoidAsync("enableButton", AllButton);
            StateHasChanged();

        }

        // Method handle the Button states
        public async void EnableGoButton(KeyboardEventArgs args)
        {
            await _JSRuntime.InvokeVoidAsync("enableButton", GoButton);
            await _JSRuntime.InvokeVoidAsync("disableButton", AllButton);
            StateHasChanged();
        }

        public void GotoNewAccount()
        {
            Navigation.NavigateTo("NewAccount");
        }

        [JSInvokable]

        public async void OnSortClick(string id)
        {
            var result = Task.Run(() => 
            {
                if(SortOption == "ascending")
                {
                    var sortItems = _BankingAccountStore.Blob.AsEnumerable<BankingAccount>()
                    .ToList()
                    .OrderByDescending(o => o._id)
                    .ToList<BankingAccount>()
                    .ToArray();
                    _BankingAccountStore.Blob = sortItems;
                    SortOption = "descending";
                }
                else
                {
                    var sortItems = _BankingAccountStore.Blob.AsEnumerable<BankingAccount>()
                    .ToList()
                    .OrderBy(o => o._id)
                    .ToList<BankingAccount>()
                    .ToArray();
                    _BankingAccountStore.Blob = sortItems;
                    SortOption = "ascending";
                }

                


            });

            await result;
            await _JSRuntime.InvokeVoidAsync("onSortClick", id);
            StateHasChanged();
            
        }

        public void EditAccount(BankingAccount account)
        {

            _EditButtonClicked = true;
            _UpdateAccount = account;

        }

        public async void ManagerEdit(Tuple<bool, BankingAccount, BankingAccount> updater)
        {
            if (updater.Item1 != true)
            {
                _EditButtonClicked = false;
                _BankingAccountStore.Blob = await _BankingAccountService.GetAccountsAsync();
                StateHasChanged();
            }
            else
            {
                // update Account in DB
                var result = await _BankingAccountService.UpdateAccountAsync(updater.Item2);
                _BankingAccountStore.Blob = await _BankingAccountService.GetAccountsAsync();
                _EditButtonClicked = false;
                StateHasChanged();

            }
        }

        protected override async void OnInitialized()
        {
            var result = Task.Run(() => 
            {
                _BankingAccountStore = new BankingAccountStore();

            });
            await result;

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _JSRuntime.InvokeVoidAsync("disableButton", ResetButton);
                await _JSRuntime.InvokeVoidAsync("disableButton", GoButton);
            }
        }



    }
}
