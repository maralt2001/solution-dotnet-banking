using ApiAccess;
using BankingClient.Data;
using BankingClient.Provider;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingClient.Pages
{
    public class LogoutBase : ComponentBase
    {
        [Inject] ApiUserService UserService { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] protected UserState UserState { get; set; }


        public async Task LogoutUser()
        {
            if(UserState.IsLoggedIn)
            {

                var result = await UserService.LogoutUser();
                UserState.IsLoggedIn = result.IsLoggedin;
                Navigation.NavigateTo("login");
            }
        }

    }
}
