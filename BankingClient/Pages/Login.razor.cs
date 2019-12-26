using ApiAccess;
using ApiDataService;
using BankingClient.Data;
using BankingClient.Provider;
using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace BankingClient.Pages
{
    public class LoginBase : ComponentBase
    {
        [Inject] ApiUserService UserService { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] protected UserState UserState { get; set; }
        [Inject] public ApplicationUser ApplicationUser { get; set; }

        [Parameter] public bool Init { get; set; } = true;
        [Parameter] public bool LoginResult { get; set; }
       
     

        public async void LoginUser()
        {
            LoginResult result = await UserService.LoginUser(ApplicationUser);

            if(result.IsLoggedin)
            {
                UserState.IsLoggedIn = true;
                UserState.Username = ApplicationUser.email;
                Init = false;
                ApplicationUser = new ApplicationUser();
                Navigation.NavigateTo("bankingaccounts");
            }
            else
            {
                UserState.IsLoggedIn = false;
                UserState.Username = "";
                Init = false;
                ApplicationUser = new ApplicationUser();
            }

            StateHasChanged();

        }

        protected override void OnInitialized()
        {
            ApplicationUser = new ApplicationUser();
            StateHasChanged();

        }
    }
}
