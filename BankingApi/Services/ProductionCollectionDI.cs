using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoService;
using System.Text;

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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["issuer"],
                        ValidAudience = configuration["audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["securityKey"]))
                    };
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
