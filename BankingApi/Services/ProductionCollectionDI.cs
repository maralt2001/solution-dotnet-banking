using ApiAccess;
using BankingApi.Attributes;
using BankingApi.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoService;
using ServiceRedis;
using System;

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
            // Read Redis Settings from appsettings.json in section Redis
            services.AddSingleton<ICacheContext>(sp => new RedisClient(configuration.GetSection("Redis").GetSection("ConnectionPath").Value));
            CacheContext.HashExpire = Int32.Parse(configuration.GetSection("Redis").GetSection("RedisHashExpire").Value);
            return services;
        }

        public static IApplicationBuilder AddProductionApplictionBuilder(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //Set static fields CacheContext and ContextLogger of Classes CheckEncryptCache and CacheContext
            CheckEncryptCache.CacheContext = (ICacheContext)app.ApplicationServices.GetService(typeof(ICacheContext));
            CacheContext.ContextLogger = LoggerFactory.Create(builder => { builder.AddConsole(); }).CreateLogger("RedisCacheContext");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            return app;
        }
    }

    

   
}
