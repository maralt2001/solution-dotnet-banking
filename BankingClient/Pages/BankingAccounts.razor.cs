using BankingApi.Models;
using BankingClient.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingClient.Pages
{
    public class BaseBankingAccounts : ComponentBase
    {
        [Inject] BankingAccountsService AccountService { get; set; }

        [Parameter] public BankingAccount[] Accounts { get; set; }
        [Parameter] public BankingAccount[] Cache { get; set; }
        [Parameter] public int Count { get; set; }


        [Parameter] public string Value { get; set; }
        [Parameter] public Dictionary<string, Object> ButtonResetDictionary { get; set; } = new Dictionary<string, object>();

        [Parameter] public bool IsTableRowClicked { get; set; }
        [Parameter] public BankingAccount TableRowAccount { get; set; }

        // are called when the search button is clicked
        public void SearchValue()
        {
            var result = Accounts.Where(s => s.firstname == Value || s.lastname == Value).ToArray();
            Count = result.Length;
            Value = "";
            ButtonResetDictionary.Remove("disabled");
            Accounts = result;
        }

        //are called when the reset button is clicked the table will be refreshed
        public void ResetTable()
        {
            Accounts = Cache;
            Count = Cache.Length;
            ButtonResetDictionary.Add("disabled", true);

        }

        // are called when a row in the table is clicked
        public void ViewAccountDetails(BankingAccount oneAccount)
        {
            IsTableRowClicked = true;
            TableRowAccount = oneAccount;

        }

        //are called from ViewAccountDetails Component if ModalClosed -> Child Return Action
        public void ShowAccountDetails(bool value)
        {
            IsTableRowClicked = false;
            StateHasChanged();
        }

        //are called when a component is first initialised and each time new or updated parameters are received from the parent in the render tree.
        protected override async Task OnParametersSetAsync()
        {
            var setInitStates = Task.Run(() => ButtonResetDictionary.Add("disabled", true));
            await setInitStates;
        }

        //Once the component has received its initial parameters from its parent in the render tree
        protected override async Task OnInitializedAsync()
        {
            Accounts = await AccountService.GetAccountsAsync();

            Cache = Accounts;
            Count = Accounts.Length;

        }
    }
}
