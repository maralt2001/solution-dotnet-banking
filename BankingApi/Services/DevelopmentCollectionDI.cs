
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoService;
using ApiAccess;

using StackExchange.Redis;
using BankingApi.Attributes;

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
            services.AddSingleton<IDatabase>(sp => 
                    ConnectionMultiplexer.Connect(
                    new ConfigurationOptions { 
                        EndPoints = { configuration.GetSection("RedisConnectionPath").Value } ,
                        ConnectRetry = 3 
                    }).GetDatabase()
            );


            return services;

        }

        public static IApplicationBuilder AddDevelopmentApplictionBuilder(this IApplicationBuilder app)
        {

            app.UseCors(builder => builder.WithOrigins("http://localhost:50410/bankingaccounts"));
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            CheckEncryptCache.CacheProvider = app.ApplicationServices;

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
            });

            return app;
        }

       
    }

    
}
