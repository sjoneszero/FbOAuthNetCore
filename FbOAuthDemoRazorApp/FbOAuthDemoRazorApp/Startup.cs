using FbOAuthDemoRazorApp.Middleware;
using FbOAuthDemoRazorApp.Models;
using FbOAuthDemoRazorApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace FbOAuthDemoRazorApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;

            var configBuilder = new ConfigurationBuilder();
            configBuilder
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = configBuilder.Build();
        }        

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            var appSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(appSettings);
            services.AddSingleton<IAppSettings>(appSettings);

            var facebookAuthSettings = new FacebookAuthSettings();
            Configuration.GetSection("FacebookAuthSettings").Bind(facebookAuthSettings);
            services.AddSingleton<IFacebookAuthSettings>(facebookAuthSettings);
            services.AddHttpContextAccessor();
            services.AddMvc();
            services.AddControllers();
            services.AddHttpClient("DefaultHttpClient");

            services.AddTransient<IFacebookAuthService, FacebookAuthService>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
                var settings = sp.GetRequiredService<IFacebookAuthSettings>();

                return new FacebookAuthService(settings, httpClient);
            });

            services.AddTransient<IFacebookDataService, FacebookDataService>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
                var settings = sp.GetRequiredService<IFacebookAuthSettings>();

                return new FacebookDataService(httpClient); 
            });
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
                app.UseHsts();
            }

            app.UseWhen(
                context => context.Request.Path.StartsWithSegments(new PathString("/api/auth")) == false,
                appBuilder =>
                {
                    appBuilder.UseAuthentication().UseMiddleware<JwtMiddleware>();
                }
            );

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
