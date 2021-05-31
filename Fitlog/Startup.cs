using AutoMapper;
using Fitlog.Activities;
using Fitlog.Feedback;
using Fitlog.Logging;
using Fitlog.Measurements;
using Fitlog.Nutrition;
using Fitlog.Profile;
using Fitlog.Settings;
using Fitlog.Training;
using Fitlog.Web;
using Fitlog.Web.Models.Auth;
using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Fitlog
{
    public class Startup
    {
        private const string ConnectionStringKey = "ConnectionStrings:Fitlog";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddFacebook(o =>
            {
                o.AppId = Configuration["Authentication:Facebook:AppId"];
                o.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            }).AddGoogle(o =>
            {
                o.ClientId = Configuration["Authentication:Google:ClientId"];
                o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                o.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                o.ClaimActions.Clear();
                o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                o.ClaimActions.MapJsonKey("urn:google:profile", "link");
                o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            }).AddJwtBearer(o =>
            {

                o.Audience = Configuration.GetSection("Authentication:Jwt:SiteUrl").Value;
                //o.Authority = Configuration.GetSection("Authentication:Jwt:SiteUrl").Value;
                o.ClaimsIssuer = Configuration.GetSection("Authentication:Jwt:SiteUrl").Value;
                o.RequireHttpsMetadata = false;
                o.IncludeErrorDetails = true;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    //ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Authentication:Jwt:Key").Value)),

                    //ValidateIssuer = true,
                    ValidIssuer = Configuration.GetSection("Authentication:Jwt:SiteUrl").Value,

                    //ValidateAudience = true,
                    ValidAudience = Configuration.GetSection("Authentication:Jwt:SiteUrl").Value,

                    //ValidateLifetime = true,
                    //ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(Configuration.GetSection(ConnectionStringKey).Value);
            });

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<UserContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters += "Â‰ˆ≈ƒ÷";
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                //options.Cookies.ApplicationCookie.CookieHttpOnly = false;
            });

            services.AddAuthorization();

            /*
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add((new CorsAuthorizationFilterFactory("ApiCorsPolicy"));
            });
            */

            var corsPolicy = new CorsPolicyBuilder()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .Build();

            services.AddCors(options =>
            {
                options.AddPolicy("ApiCorsPolicy", corsPolicy);
            });

            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
            services.AddMvc();

            

            services.AddTransient<INutritionRepository>(s =>
            {
                return new NutritionRepository(Configuration.GetSection(ConnectionStringKey).Value);
            });
            services.AddTransient<ITrainingRepository>(s =>
            {
                return new TrainingRepository(Configuration.GetSection(ConnectionStringKey).Value);
            });
            services.AddTransient<IMeasurementRepository>(s =>
            {
                return new MeasurementRepository(Configuration.GetSection(ConnectionStringKey).Value);
            });
            services.AddTransient<IProfileRepository>(s =>
            {
                return new ProfileRepository(Configuration.GetSection(ConnectionStringKey).Value);
            });
            services.AddTransient<ILogRepository>(s =>
            {
                return new LogRepository(Configuration.GetSection(ConnectionStringKey).Value);
            });
            services.AddTransient<IFeedbackRepository>(s =>
            {
                return new FeedbackRepository(Configuration.GetSection(ConnectionStringKey).Value);
            });
            services.AddTransient<IActivityRepository>(s =>
            {
                return new ActivityRepository(Configuration.GetSection(ConnectionStringKey).Value);
            });
            services.AddTransient<ISettingsRepository>(s =>
            {
                return new SettingsRepository(Configuration.GetSection(ConnectionStringKey).Value);
            });

            var sender = new MailgunSender(Configuration.GetSection("MailGun:Domain").Value, Configuration.GetSection("MailGun:ApiKey").Value, MailGunRegion.EU);
            services.AddSingleton<ISender>(sender);

            services.AddAutoMapper(typeof(Mappings));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseCors("ApiCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                // Do work that doesn't write to the Response.
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");
            });
        }
    }
}
