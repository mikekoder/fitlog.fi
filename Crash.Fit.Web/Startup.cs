using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Crash.Fit.Nutrition;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Crash.Fit.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Crash.Fit.Training;
using Microsoft.AspNetCore.Http.Internal;
using Crash.Fit.Logging;
using Crash.Fit.Measurements;
using Crash.Fit.Profile;
using Crash.Fit.Api.Models.Feedback;
using Crash.Fit.Feedback;
using Crash.Fit.Api.Models.Training;
using Crash.Fit.Api.Models.Nutrition;
using Crash.Fit.Api.Models.Profile;
using Crash.Fit.Api.Models.Measurements;

namespace Crash.Fit.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Crash.Fit"));
            });

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<UserContext, Guid>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters += "������";
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Cookies.ApplicationCookie.CookieHttpOnly = false;
            });

            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
            services.AddMvc();
            


            services.AddTransient<INutritionRepository>(s => 
            {
                return new NutritionRepository(SqlClientFactory.Instance, Configuration.GetConnectionString("Crash.Fit"));
            });
            services.AddTransient<ITrainingRepository>(s =>
            {
                return new TrainingRepository(SqlClientFactory.Instance, Configuration.GetConnectionString("Crash.Fit"));
            });
            services.AddTransient<IMeasurementRepository>(s =>
            {
                return new MeasurementRepository(SqlClientFactory.Instance, Configuration.GetConnectionString("Crash.Fit"));
            });
            services.AddTransient<IProfileRepository>(s =>
            {
                return new ProfileRepository(SqlClientFactory.Instance, Configuration.GetConnectionString("Crash.Fit"));
            });
            services.AddTransient<ILogRepository>(s =>
            {
                return new LogRepository(SqlClientFactory.Instance, Configuration.GetConnectionString("Crash.Fit"));
            });
            services.AddTransient<IFeedbackRepository>(s =>
            {
                return new FeedbackRepository(SqlClientFactory.Instance, Configuration.GetConnectionString("Crash.Fit"));
            });
            AutoMapper.Mapper.Initialize(m => {

                // Nutrients
                m.CreateMap<Nutrient, NutrientResponse>().AfterMap((source, target) =>
                {
                    target.HideSummary = source.DefaultHideSummary;
                    target.HideDetails = source.DefaultHideDetails;
                });
                m.CreateMap<UserNutrient, NutrientResponse>().AfterMap((source, target) => 
                {
                    target.HideSummary = source.UserHideSummary ?? source.DefaultHideSummary;
                    target.HideDetails = source.UserHideDetails ?? source.DefaultHideDetails;
                });
                m.CreateMap<NutrientSettingRequest, NutrientSetting>().AfterMap((source, target) => 
                {
                    target.HideDetails = source.UserHideDetails;
                    target.HideSummary = source.UserHideSummary;
                });
                m.CreateMap<IEnumerable<NutrientAmount>, Dictionary<Guid, decimal>>().ConvertUsing(na => na.ToDictionary(n => n.NutrientId, n => n.Amount));
                m.CreateMap<NutrientAmount, NutrientAmountModel>();
                m.CreateMap<NutrientAmountModel, NutrientAmount>();
                // Foods
                m.CreateMap<FoodSearchResult, FoodSearchResultResponse>();
                m.CreateMap<FoodSummary, FoodSummaryResponse>();
                m.CreateMap<FoodDetails, FoodDetailsResponse>();
                m.CreateMap<FoodNutrientAmount, FoodNutrientAmountResponse>();
                m.CreateMap<NutrientAmountModel, FoodNutrientAmount>();
                m.CreateMap<FoodRequest, FoodDetails>();

                // Meals
                m.CreateMap<MealDetails, MealDetailsResponse>();
                m.CreateMap<MealRow, MealRowModel>();
                m.CreateMap<MealRequest, MealDetails>();
                m.CreateMap<MealRowModel, MealRow>();

                // Recipes
                m.CreateMap<FoodSummary, RecipeSummaryResponse>();
                m.CreateMap<FoodDetails, RecipeDetailsResponse>();
                m.CreateMap<RecipeIngredient, RecipeIngredientModel>();
                m.CreateMap<RecipeRequest, FoodDetails>();
                m.CreateMap<RecipeIngredientModel, RecipeIngredient>();

                // Portions
                m.CreateMap<Portion, PortionResponse>();
                m.CreateMap<PortionRequest, Portion>();

                // Workouts
                m.CreateMap<WorkoutSummary, WorkoutSummaryResponse>();
                m.CreateMap<WorkoutDetails, WorkoutDetailsResponse>();
                m.CreateMap<WorkoutSet, WorkoutSetResponse>();
                m.CreateMap<WorkoutRequest, WorkoutDetails>();
                m.CreateMap<WorkoutSetRequest, WorkoutSet>();

                // Exercises
                m.CreateMap<Exercise, ExerciseResponse>();
                m.CreateMap<ExerciseDetails, ExerciseDetailsResponse>();
                m.CreateMap<ExerciseSummary, ExerciseSummaryResponse>();
                m.CreateMap<ExerciseRequest, ExerciseDetails>();

                // Routines
                m.CreateMap<RoutineSummary, RoutineResponse>();
                m.CreateMap<RoutineDetails, RoutineDetailsResponse>();
                m.CreateMap<RoutineWorkout, RoutineWorkoutResponse>();
                m.CreateMap<RoutineExercise, RoutineExerciseResponse>();
                m.CreateMap<RoutineRequest, RoutineDetails>();
                m.CreateMap<RoutineWorkoutRequest, RoutineWorkout>();
                m.CreateMap<RoutineExerciseRequest, RoutineExercise>();

                // MuscleGroups
                m.CreateMap<MuscleGroup, MuscleGroupResponse>();

                // Measurements
                m.CreateMap<MeasureSummary, MeasureSummaryResponse>();

                // Profile
                m.CreateMap<Profile.Profile, ProfileResponse>();

                // Feedback
                m.CreateMap<FeedbackSummary, FeedbackSummaryResponse>();
                m.CreateMap<FeedbackDetails, FeedbackDetailsResponse>();
                m.CreateMap<FeedbackComment, FeedbackDetailsResponse.Comment>();
                m.CreateMap<FeedbackRequest, FeedbackDetails>();
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();
            app.UseGoogleAuthentication(new GoogleOptions()
            {
                ClientId = Configuration["Authentication:Google:ClientId"],
                ClientSecret = Configuration["Authentication:Google:ClientSecret"]
            });
            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = Configuration["Authentication:Facebook:AppId"],
                AppSecret = Configuration["Authentication:Facebook:AppSecret"]
            });
            
            app.Use(async (context, next) =>
            {
                context.Request.EnableRewind();
                // Do work that doesn't write to the Response.
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
            
        }
    }
}
