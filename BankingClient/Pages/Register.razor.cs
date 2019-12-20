using ApiAccess;
using BankingClient.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankingClient.Pages
{
    public class RegisterBase : ComponentBase
    {
        [Inject] ApiUserService UserService { get; set; }
        [Inject] public ApplicationUser ApplicationUser { get; set; }
        [Inject] public RegisterApplicationUser RegisterApplicationUser { get; set; }

        [Parameter] public bool Init { get; set; } = true;
        [Parameter] public bool RegisterResult { get; set; }
        [Parameter] public string Username { get; set; }
        [Parameter] public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Parameter] public string ConfirmPassword { get; set; }


        public async void RegisterUser()
        {
            RegisterApplicationUser.email = Username;
            RegisterApplicationUser.password = Password;
            RegisterApplicationUser.confirmPassword = ConfirmPassword;

            if(await RegisterApplicationUser.ValidatePasswords())
            {

                ApplicationUser.email = Username; ApplicationUser.password = Password;
                var result = await UserService.RegisterUser(ApplicationUser);

                if(result.IsRegistered)
                {
                    RegisterResult = true;
                    Init = false;
                    Username = "";
                    Password = "";
                    ConfirmPassword = "";
                }
                else
                {
                    RegisterResult = false;
                    Init = false;
                    Username = "";
                    Password = "";
                    ConfirmPassword = "";
                }


                StateHasChanged();

            }
        }
    }
}
