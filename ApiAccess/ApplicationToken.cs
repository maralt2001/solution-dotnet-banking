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
        private string securityKey;
        private string issuer;
        private string audience;
        

        public ApplicationToken(string securityKey, string issuer, string audience)
        {
            this.securityKey = securityKey;
            this.issuer = issuer;
            this.audience = audience;
        }
        public async Task<string> GetJwtSecurityTokenAsync()
        {
            Task<JwtSecurityToken> result = Task.Run(() =>

            {

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(

                    issuer: issuer,
                    audience: audience,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials

                    );
                return token;

            });

            return new JwtSecurityTokenHandler().WriteToken(await result);
        }

    }
}
