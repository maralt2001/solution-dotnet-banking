using ApiAccess;
using BankingApi.Attributes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoService;
using StackExchange.Redis;

namespace BankingApi.Services
{
    public static class ProductionCollectionDI
    {

        public static IServiceCollection AddProductionServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddSingleton<IDBContext>(sp => new MongoWithCredential(
                configuration.GetSection("DBConnection").GetSection("DB").Value,
                configuration.GetSection("DbConnection").GetSection("Path").Value,
                "web",
                "db"));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {

                options.TokenValidationParameters = new ApplicationToken(configuration).GetTokenValidationParameterAsync().Result;

            });

            services.AddSingleton<IDatabase>(sp =>
                ConnectionMultiplexer.Connect(
                    new ConfigurationOptions
                    {
                        EndPoints = { configuration.GetSection("Redis").GetSection("ConnectionPath").Value },
                        ConnectRetry = 3
                    }).GetDatabase()
            );


            return services;
        }

        public static IApplicationBuilder AddProductionApplictionBuilder(this IApplicationBuilder app)
        {
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
