
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoService;

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

            return services;

        }

        public static IApplicationBuilder AddDevelopmentApplictionBuilder(this IApplicationBuilder app)
        {

            app.UseCors(builder => builder.WithOrigins("http://localhost:50410/bankingaccounts"));
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
            });

            return app;
        }

       
    }

    
}
