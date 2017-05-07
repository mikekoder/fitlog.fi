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
                options.User.AllowedUserNameCharacters += "åäöÅÄÖ";
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
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
            services.AddTransient<ILogRepository>(s =>
            {
                return new LogRepository(SqlClientFactory.Instance, Configuration.GetConnectionString("Crash.Fit"));
            });

            AutoMapper.Mapper.Initialize(m => {

                // Nutrients
                m.CreateMap<Nutrient, Models.Nutrition.NutrientResponse>();
                m.CreateMap<IEnumerable<NutrientAmount>, Dictionary<Guid, decimal>>().ConvertUsing(na => na.ToDictionary(n=> n.NutrientId,n => n.Amount));
                m.CreateMap<NutrientAmount, Models.Nutrition.NutrientAmountResponse>();
                // Foods
                m.CreateMap<Food, Models.Nutrition.FoodSummaryResponse>();
                m.CreateMap<FoodSummary, Models.Nutrition.FoodSummaryResponse>();
                m.CreateMap<FoodDetails, Models.Nutrition.FoodDetailsResponse>();      
                m.CreateMap<Models.Nutrition.FoodRequest, FoodDetails>();

                // Meals
                m.CreateMap<MealDetails, Models.Nutrition.MealDetailsResponse>();
                m.CreateMap<MealRow, Models.Nutrition.MealRow>();
                m.CreateMap<Models.Nutrition.MealRequest, MealDetails>();
                m.CreateMap<Models.Nutrition.MealRow, MealRow>();

                // Recipes
                m.CreateMap<FoodSummary, Models.Nutrition.RecipeSummaryResponse>();
                m.CreateMap<FoodDetails, Models.Nutrition.RecipeDetailsResponse>();
                m.CreateMap<RecipeIngredient, Models.Nutrition.RecipeIngredient>();
                m.CreateMap<Models.Nutrition.RecipeRequest, FoodDetails>();
                m.CreateMap<Models.Nutrition.RecipeIngredient, RecipeIngredient>();

                // Portions
                m.CreateMap<Portion, Models.Nutrition.Portion>();
                m.CreateMap<Models.Nutrition.Portion, Portion>();

                // Workouts
                m.CreateMap<WorkoutSummary, Models.Training.WorkoutSummaryResponse>();
                m.CreateMap<WorkoutDetails, Models.Training.WorkoutResponse>();
                m.CreateMap<WorkoutSet, Models.Training.WorkoutSetResponse>();
                m.CreateMap<Models.Training.WorkoutRequest, WorkoutDetails>();
                m.CreateMap<Models.Training.WorkoutSetRequest, WorkoutSet>();

                // Exercises
                m.CreateMap<Exercise, Models.Training.ExerciseResponse>();
                m.CreateMap<ExerciseDetails, Models.Training.ExerciseDetailsResponse>();
                m.CreateMap<ExerciseSummary, Models.Training.ExerciseSummaryResponse>();
                m.CreateMap<Models.Training.ExerciseRequest, ExerciseDetails>();

                // Routines
                m.CreateMap<RoutineSummary, Models.Training.RoutineSummaryResponse>();
                m.CreateMap<RoutineDetails, Models.Training.RoutineDetailsResponse>();
                m.CreateMap<RoutineWorkout, Models.Training.RoutineWorkoutResponse>();
                m.CreateMap<RoutineExercise, Models.Training.RoutineExerciseResponse>();
                m.CreateMap<Models.Training.RoutineRequest, RoutineDetails>();
                m.CreateMap<Models.Training.RoutineWorkoutRequest, RoutineWorkout>();
                m.CreateMap<Models.Training.RoutineExerciseRequest, RoutineExercise>();

                // MuscleGroups
                m.CreateMap<MuscleGroup, Models.Training.MuscleGroupResponse>();
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
