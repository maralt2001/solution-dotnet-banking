using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace ApiAccess
{
    public class ApplicationToken
    {
        private readonly SymmetricSecurityKey symmetricSecurityKey;
        private readonly SigningCredentials signingCredentials;
        private readonly string issuer;
        private readonly string audience;

        public ApplicationToken(IConfiguration configuration)
        {
            symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["securityKey"]));
            issuer = configuration["issuer"];
            audience = configuration["audience"];
            signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        }
        //Create JWT Security Token
        public async Task<string> GetJwtSecurityTokenAsync()
        {
            Task<JwtSecurityToken> result = Task.Run(() =>
            {
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
        // Set Token Validation Parameter
        public async Task<TokenValidationParameters> GetTokenValidationParameterAsync()
        {
            var result = Task.Run(() => {

                var tokenParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = symmetricSecurityKey
                };

                return tokenParameters;

            });

            return await result;
        }

    }
}
