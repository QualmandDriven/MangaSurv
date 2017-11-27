using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MangaSurvWebApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MangaSurvWebApi.Service;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;

namespace MangaSurvWebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            env.ConfigureNLog("nlog.config");

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            ApplicationConfiguration appConfig = ApplicationConfiguration.GetApplicationConfiguration();
            appConfig.PostgresConString = Configuration.GetConnectionString("MangaSurvPostgres");

            // Add framework services.
            
            services.AddDbContext<MangaSurvContext>(opts => 
                opts.UseNpgsql(appConfig.PostgresConString)
            );

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                options.Audience = Configuration["Auth0:ClientId"];
            });

            services.AddMvc();

            // Add functionality to inject IOptions<T>
            services.AddOptions();
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<Auth0Settings> auth0Settings)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();

            //var options = new JwtBearerOptions
            //{
                //Audience = auth0Settings.Value.ClientId,
                //Audience = "YAmDw5AUhffZAJoYD1kdFWTp0vA8coXv",
                //Authority = $"https://{auth0Settings.Value.Domain}/",
                //TokenValidationParameters =
                //{
                //    ValidIssuer = $"https://{auth0Settings.Value.Domain}/",
                //    ValidAudience = auth0Settings.Value.ClientId,
                //    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(auth0Settings.Value.SecretKey))
                //},

            //    Events = new JwtBearerEvents
            //    {
            //        OnChallenge = context =>
            //        {

            //            return Task.FromResult(0);
            //        },
            //        OnMessageReceived = context =>
            //        {
            //            return Task.FromResult(0);
            //        },
            //        OnTokenValidated = context =>
            //        {
            //            // If you need the user's information for any reason at this point, you can get it by looking at the Claims property
            //            // of context.Ticket.Principal.Identity
            //            var claimsIdentity = context.Ticket.Principal.Identity as ClaimsIdentity;
            //            if (claimsIdentity != null)
            //            {
            //                // Get the user's ID
            //                string userId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            //                // Get the name
            //                string name = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            //            }

            //            return Task.FromResult(0);
            //        }
            //    }
            //};

            app.UseAuthentication();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            });
            app.UseMvc();
        }
    }
}
