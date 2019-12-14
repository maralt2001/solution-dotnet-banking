using ApiAccess;
using BankingApi.Models;
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

        [Parameter] public bool Init { get; set; } = true;
        [Parameter] public bool LoginResult { get; set; }
        [Parameter] public string Username { get; set; }
        [Parameter] public string Password { get; set; }

        public async void LoginUser()
        {
            var user = new ApplicationUser
            {
                email = Username,
                password = Password
            };

            var result = await UserService.LoginUser(user);
            

            if(result.IsLoggedin)
            {
                UserState.IsLoggedIn = true;
                UserState.Username = user.email;
                Init = false;
                Username = "";
                Password = "";
                Navigation.NavigateTo("bankingaccounts");
            }
            else
            {
                UserState.IsLoggedIn = false;
                UserState.Username = "";
                Init = false;
                Password = "";
                Username = "";
            }
            

            StateHasChanged();
            


        }
    }
}
