using AutoMapper;
using Fitlog.Api.Models.Activities;
using Fitlog.Api.Models.Feedback;
using Fitlog.Api.Models.Measurements;
using Fitlog.Api.Models.Nutrition;
using Fitlog.Api.Models.Profile;
using Fitlog.Api.Models.Training;
using Fitlog.Activities;
using Fitlog.Feedback;
using Fitlog.Logging;
using Fitlog.Measurements;
using Fitlog.Nutrition;
using Fitlog.Profile;
using Fitlog.Settings;
using Fitlog.Training;
using System.Collections.Generic;
using Fitlog.Web;
using System;
using System.Linq;

namespace Fitlog.Web
{
    public class Mappings : AutoMapper.Profile
    {
        public Mappings()
        {
            // Nutrients
            CreateMap<Nutrient, NutrientResponse>().AfterMap((model, response) =>
            {
                response.HideSummary = model.DefaultHideSummary;
                response.HideDetails = model.DefaultHideDetails;
            });
            /*
            CreateMap<UserNutrient, NutrientResponse>().AfterMap((model, response) => 
            {
                response.HideSummary = model.UserHideSummary ?? model.DefaultHideSummary;
                response.HideDetails = model.UserHideDetails ?? model.DefaultHideDetails;
            });
            */
            CreateMap<NutrientSetting, NutrientSettingResponse>();
            CreateMap<NutrientSettingRequest, NutrientSetting>().AfterMap((request, model) =>
            {
                model.HideDetails = request.UserHideDetails;
                model.HideSummary = request.UserHideSummary;
            });
            CreateMap<IEnumerable<NutrientAmount>, Dictionary<int, decimal>>().ConvertUsing(na => na.ToDictionary(n => n.NutrientId, n => n.Amount));
            CreateMap<NutrientAmount, NutrientAmountModel>();
            CreateMap<NutrientAmountModel, NutrientAmount>();

            CreateMap<NutritionGoalDetails, NutritionGoalResponse>();
            CreateMap<NutritionGoalPeriod, NutritionGoalPeriodResponse>();
            CreateMap<NutritionGoalValue, NutritionGoalPeriodResponse.NutrientValue>();

            CreateMap<NutritionGoalRequest, NutritionGoalDetails>();
            CreateMap<NutritionGoalPeriodRequest, NutritionGoalPeriod>();
            CreateMap<NutritionGoalPeriodRequest.NutrientValue, NutritionGoalValue>().AfterMap((request, model) =>
            {
                if (model.Min.HasValue && model.Max.HasValue && model.Min.Value > model.Max.Value)
                {
                    var temp = model.Min;
                    model.Min = model.Max;
                    model.Max = temp;
                }
            });

            CreateMap<DayNutrient, NutrientHistoryResponse>();

            // Foods
            CreateMap<FoodSearchResult, FoodSearchResultResponse>();
            CreateMap<FoodSearchNutrientResult, FoodSearchNutrientResultResponse>();
            CreateMap<FoodSummary, FoodSummaryResponse>();
            CreateMap<FoodDetails, FoodDetailsResponse>();
            CreateMap<FoodNutrientAmount, FoodNutrientAmountResponse>();
            CreateMap<NutrientAmountModel, FoodNutrientAmount>();
            CreateMap<FoodRequest, FoodDetails>();

            // Meals
            CreateMap<MealDetails, MealDetailsResponse>();
            CreateMap<MealRow, MealRowModel>();
            CreateMap<MealRequest, MealDetails>().ForMember(d => d.Time, x => { x.Ignore(); }).AfterMap((source, target) =>
            {
                int hour = 0;
                int minute = 0;
                var timeParts = (source.Time ?? "").Replace('.', ':').Split(':');
                if (timeParts.Length > 0)
                {
                    int.TryParse(timeParts[0], out hour);
                }
                if (timeParts.Length > 1)
                {
                    int.TryParse(timeParts[1], out minute);
                }
                var date = DateTimeUtils.ToLocal(source.Date);
                var time = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);

                target.Time = new DateTimeOffset(time, DateTimeUtils.GetTimeZoneOffset(time));
            });
            CreateMap<MealRowModel, MealRow>();

            // Meal rhythm
            CreateMap<MealDefinition, MealDefinitionResponse>().AfterMap((model, response) =>
            {
                if (model.Start.HasValue)
                {
                    response.StartHour = model.Start.Value.Hours;
                    response.StartMinute = model.Start.Value.Minutes;
                }
                if (model.End.HasValue)
                {
                    response.EndHour = model.End.Value.Hours;
                    response.EndMinute = model.End.Value.Minutes;
                }
            });
            // Recipes
            CreateMap<FoodSummary, RecipeSummaryResponse>();
            CreateMap<FoodDetails, RecipeDetailsResponse>().AfterMap((source, target) => 
            {
                if (source.IsRecipe && target.Portions != null)
                {
                    var recipeWeight = source.CookedWeight ?? target.Ingredients.Sum(i => i.Weight);
                    foreach (var portion in target.Portions)
                    {
                        portion.Amount = Math.Round(recipeWeight / portion.Weight);
                    }
                }
            });
            CreateMap<RecipeIngredient, RecipeIngredientModel>();
            CreateMap<RecipeRequest, FoodDetails>();
            CreateMap<RecipeIngredientModel, RecipeIngredient>();

            // Portions
            CreateMap<Portion, PortionResponse>();
            CreateMap<PortionRequest, Portion>();

            // Workouts
            CreateMap<WorkoutSummary, WorkoutSummaryResponse>().AfterMap((model, response) =>
            {
                if (model.Duration.HasValue)
                {
                    response.Hours = model.Duration.Value.Hours;
                    response.Minutes = model.Duration.Value.Minutes;
                }
            });
            CreateMap<WorkoutDetails, WorkoutDetailsResponse>().AfterMap((model, response) =>
            {
                if (model.Duration.HasValue)
                {
                    response.Hours = model.Duration.Value.Hours;
                    response.Minutes = model.Duration.Value.Minutes;
                }
            });
            CreateMap<WorkoutSet, WorkoutSetResponse>();
            CreateMap<WorkoutRequest, WorkoutDetails>().AfterMap((request, model) =>
            {
                model.Time = DateTimeUtils.ToLocal(model.Time);
                if (request.Hours.HasValue || request.Minutes.HasValue)
                {
                    model.Duration = TimeSpan.FromMinutes((request.Hours ?? 0) * 60 + (request.Minutes ?? 0));
                }
            });
            CreateMap<WorkoutSetRequest, WorkoutSet>();

            // Exercises
            CreateMap<Exercise, ExerciseResponse>();
            CreateMap<ExerciseDetails, ExerciseDetailsResponse>();
            CreateMap<ExerciseSummary, ExerciseSummaryResponse>();
            CreateMap<ExerciseRequest, ExerciseDetails>();
            CreateMap<OneRepMax, ExerciseHistoryResponse>();
            CreateMap<ExerciseImage, ExerciseImageResponse>();
            CreateMap<Equipment, EquipmentResponse>();

            // Routines
            CreateMap<RoutineSummary, RoutineResponse>();
            CreateMap<RoutineDetails, RoutineDetailsResponse>();
            CreateMap<RoutineWorkout, RoutineWorkoutResponse>();
            CreateMap<RoutineExercise, RoutineExerciseResponse>();
            CreateMap<RoutineRequest, RoutineDetails>();
            CreateMap<RoutineWorkoutRequest, RoutineWorkout>();
            CreateMap<RoutineExerciseRequest, RoutineExercise>();

            // MuscleGroups
            CreateMap<MuscleGroup, MuscleGroupResponse>();

            // Training goals
            CreateMap<TrainingGoalDetails, TrainingGoalResponse>();
            CreateMap<TrainingGoalExercise, TrainingGoalExerciseResponse>();
            CreateMap<TrainingGoalRequest, TrainingGoalDetails>();
            CreateMap<TrainingGoalExerciseRequest, TrainingGoalExercise>();

            // Measurements
            CreateMap<MeasureSummary, MeasureSummaryResponse>();
            CreateMap<Measurement, MeasurementResponse>();

            // Profile
            CreateMap<Profile.Profile, ProfileResponse>();

            // Feedback
            CreateMap<FeedbackSummary, FeedbackSummaryResponse>();
            CreateMap<FeedbackDetails, FeedbackDetailsResponse>();
            CreateMap<FeedbackComment, FeedbackDetailsResponse.Comment>();
            CreateMap<FeedbackRequest, FeedbackDetails>();

            // Activities
            CreateMap<Activity, ActivityResponse>();
            CreateMap<ActivityRequest, Activity>();
            CreateMap<EnergyExpenditure, EnergyExpenditureResponse>().AfterMap((source, target) =>
            {
                if (source.Duration.HasValue)
                {
                    target.Hours = source.Duration.Value.Hours;
                    target.Minutes = source.Duration.Value.Minutes;
                }

            });
            CreateMap<EnergyExpenditureRequest, EnergyExpenditure>().AfterMap((request, model) =>
            {
                model.Time = DateTimeUtils.ToLocal(model.Time);
                if (request.Hours.HasValue || request.Minutes.HasValue)
                {
                    model.Duration = TimeSpan.FromMinutes((request.Hours ?? 0) * 60 + (request.Minutes ?? 0));
                }
            });

            CreateMap<ActivityPreset, ActivityPresetResponse>().AfterMap((model, response) =>
            {
                var inactivityHours = 24 - response.Sleep - response.LightActivity - response.ModerateActivity - response.HeavyActivity;
                response.Factor = (Constants.Activities.SleepFactor * response.Sleep + Constants.Activities.InactivityFactor * inactivityHours + Constants.Activities.LightActivityFactor * response.LightActivity + Constants.Activities.ModerateActivityFactor * response.ModerateActivity + Constants.Activities.HeavyActivityFactor * response.HeavyActivity) / 24;
            });
            CreateMap<ActivityPresetRequest, ActivityPreset>();
        }
    }
}
