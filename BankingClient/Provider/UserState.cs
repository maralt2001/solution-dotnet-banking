using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingClient.Provider
{
    public class UserState
    {
        public string Username { get; set; } = "";
        public bool IsLoggedIn { get; set; } = false;
    }
}
