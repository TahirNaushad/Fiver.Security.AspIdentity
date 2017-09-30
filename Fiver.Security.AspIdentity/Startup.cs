using Fiver.Security.AspIdentity.Models.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiver.Security.AspIdentity
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(configuration["DB_CONN"]));

            services.AddIdentity<AppIdentityUser, AppIdentityRole>()
                    .AddEntityFrameworkStores<AppIdentityDbContext>()
                    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // password settings

                // lockout settings

                // user settings

            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Security/Login";
                options.LogoutPath = "/Security/Logout";
                options.AccessDeniedPath = "/Security/Access";
                options.SlidingExpiration = true;
                options.Cookie = new CookieBuilder
                {
                    //Domain = "",
                    HttpOnly = true,
                    Name = ".Fiver.Security.Cookie",
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
            });
            
            services.AddMvc();
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
