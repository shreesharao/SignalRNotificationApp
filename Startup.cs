using Terarecon.Eureka.Cardiac.NotificationService.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Terarecon.Eureka.Cardiac.NotificationService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
                            {
                                builder
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .WithOrigins("http://localhost:5000", "http://localhost:4200", "http://15.206.77.238:4200")
                                    .AllowCredentials();
                            }));

            services.AddSignalR();
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFileServer();
            app.UseCors("CorsPolicy");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("api/cardiac/notification");
            });
        }
    }
}
