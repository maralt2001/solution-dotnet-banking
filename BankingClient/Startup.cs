using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BankingClient.Data;
using BankingClient.Provider;
using System.Net;
using ApiAccess;
using ServiceHttp;
using Microsoft.JSInterop;
using System.Net.Http;

namespace BankingClient
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
        }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages().AddMvcOptions(options => options.EnableEndpointRouting = false); 
            services.AddServerSideBlazor();
            services.AddScoped<IBankingAccountsService,BankingAccountsService>();
            services.AddScoped<ApiUserService>();
            services.AddScoped<UserState>();
            services.AddScoped<BankingAccountStore>();
            services.AddScoped<CookieContainer>();
            services.AddScoped<ApplicationUser>();
            services.AddScoped<RegisterApplicationUser>();
            services.AddHttpClient();
            
           

        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                
                endpoints.MapFallbackToPage("/_Host");
              
            });
        }
    }
}
