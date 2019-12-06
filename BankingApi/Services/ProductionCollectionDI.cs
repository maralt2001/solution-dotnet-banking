using ApiAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoService;

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

            services.AddAuthentication().AddCookie("Cookie");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {

                options.TokenValidationParameters = new ApplicationToken(configuration).GetTokenValidationParameterAsync().Result;

            });



            return services;
        }

        public static IApplicationBuilder AddProductionApplictionBuilder(this IApplicationBuilder app)
        {
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
