
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoService;
using ApiAccess;

namespace BankingApi.Services
{
    public static class DevelopmentCollectionDI 
    {

        public static IServiceCollection AddDevelopmentServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers();
            services.AddCors();

            services.AddSingleton<IDBContext>(sp => new MongoWithoutCredential(
                configuration.GetSection("DBName").Value,
                configuration.GetSection("DbConnectionPath").Value));

            services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {

                options.TokenValidationParameters = new ApplicationToken(configuration).GetTokenValidationParameterAsync().Result;

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
