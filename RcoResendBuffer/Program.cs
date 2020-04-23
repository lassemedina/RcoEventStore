using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RcoResendBuffer.Context;
using RcoResendBuffer.Service;

namespace RcoResendBuffer
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildSystemServiceHost().Run();
            Console.WriteLine("Hello World!");
        }
        public static IHost BuildSystemServiceHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient();
                    services.AddTransient<AuthContext>();
                    services.AddSingleton<QueueWorkerService>();
                    services.AddHostedService<QueueWorkerService>(provider =>  provider.GetService<QueueWorkerService>());

                })
                .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseKestrel()
                                   
                    .UseStartup<Startup>();
                })
            .Build();
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
