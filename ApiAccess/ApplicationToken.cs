using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace ApiAccess
{
    public class ApplicationToken
    {
        private readonly IConfiguration configuration;

        public ApplicationToken(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<string> GetJwtSecurityTokenAsync()
        {
            Task<JwtSecurityToken> result = Task.Run(() =>

            {

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["securityKey"]));

                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(

                    issuer: configuration["issuer"],
                    audience: configuration["audience"],
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials

                    );
                return token;

            });

            return new JwtSecurityTokenHandler().WriteToken(await result);
        }

    }
}
