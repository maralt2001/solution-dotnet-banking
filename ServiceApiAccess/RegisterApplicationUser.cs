using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace ApiAccess
{
    public class RegisterApplicationUser : ApplicationUser
    {

        
        public string confirmPassword { get; set; }
        

        public RegisterApplicationUser()
        {

        }

        public RegisterApplicationUser(string email, string password, string confirmPassword) : base(email, password)
        {
            
            this.confirmPassword = HashPassword(confirmPassword).Result;
        }

        public Task<bool> ComparePasswords()
        {
            var result = Task.Run(() => { 
            
                if(password == confirmPassword)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                     
            
            });

            return result;
        }

        
    }
}
