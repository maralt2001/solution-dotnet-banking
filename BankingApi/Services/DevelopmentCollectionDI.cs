
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoService;
using ApiAccess;
using ServiceRedis;
using BankingApi.Attributes;
using Microsoft.Extensions.Logging;
using System;

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

            services.AddSingleton<ICacheContext>(sp => new RedisClient(configuration.GetSection("RedisConnectionPath").Value));
            CacheContext.HashExpire = Int32.Parse( configuration.GetSection("RedisHashExpire").Value);
            return services;

        }

        public static IApplicationBuilder AddDevelopmentApplictionBuilder(this IApplicationBuilder app)
        {

            app.UseCors(builder => builder.WithOrigins("http://localhost:50410/bankingaccounts"));
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
