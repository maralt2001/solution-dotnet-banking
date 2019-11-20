using System;
using BankingApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BankingApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            switch (Configuration.GetSection("ASPNETCORE_ENVIRONMENT").Value)
            {
                case "Development":
                    services.AddDevelopmentServices(Configuration);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The Application is running in Development mode");
                    break;

                case "Production":
                    services.AddProductionServices(Configuration);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The Application is running in Production mode");
                    break;
            }
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            switch(env.IsDevelopment())
            {
                case true:
                    app.UseDeveloperExceptionPage();
                    app.AddDevelopmentApplictionBuilder();
                    break;
                case false:
                    app.AddProductionApplictionBuilder();
                    break;
            }

        }
    }
        
}
