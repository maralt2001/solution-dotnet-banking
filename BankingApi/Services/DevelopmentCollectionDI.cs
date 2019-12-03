
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MongoService;
using System;
using System.Text;

namespace BankingApi.Services
{
    public static class DevelopmentCollectionDI 
    {

        public static IServiceCollection AddDevelopmentServices(this IServiceCollection services, IConfiguration configuration)
        {
            string securityKey = "mystrongsecretkey";
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));


            services.AddControllers();
            services.AddCors();

            services.AddSingleton<IDBContext>(sp => new MongoWithoutCredential(
                configuration.GetSection("DBName").Value,
                configuration.GetSection("DbConnectionPath").Value));

            services.AddAuthentication().AddCookie("Cookie");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "mar.in",
                        ValidAudience = "readers",
                        IssuerSigningKey = symmetricSecurityKey
                    };
                });
           

            return services;

        }

        public static IApplicationBuilder AddDevelopmentApplictionBuilder(this IApplicationBuilder app)
        {

            app.UseCors(builder => builder.WithOrigins("http://localhost:50410/bankingaccounts"));
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
            });

            return app;
        }

       
    }

    
}
