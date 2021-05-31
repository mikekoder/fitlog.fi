using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitlog.Nutrition
{
    public interface INutritionRepository
    {
        IEnumerable<Nutrient> GetNutrients();
        IEnumerable<NutrientSetting> GetNutrientSettings(Guid userId);
        void CreateNutrient(Nutrient nutrient);
        void UpdateNutrient(Nutrient nutrient);
        IEnumerable<DailyIntake> GetDailyIntakes(string gender, TimeSpan age);


        IEnumerable<FoodSearchResult> SearchFoods(string[] nameTokens, Guid? userId = null);
        IEnumerable<FoodSummary> SearchUserFoods(Guid userId);
        IEnumerable<FoodSummary> SearchRecipes(Guid userId);
        FoodDetails GetFood(Guid id, Guid? userId = null);
        IEnumerable<FoodDetails> GetFoods(IEnumerable<Guid> ids, Guid? userId = null);
        void CreateFood(FoodDetails food);
        void UpdateFood(FoodDetails food);
        void DeleteFood(Food food);
        void RestoreFood(Guid id, out FoodDetails food);

        IEnumerable<MealDetails> SearchMeals(Guid userId, DateTimeOffset start, DateTimeOffset end);
        MealDetails GetMeal(Guid id);
        void CreateMeal(MealDetails meal);
        void UpdateMeal(MealDetails meal);
        void DeleteMeal(Meal meal);
        void RestoreMeal(Guid id, out MealDetails meal);
        void CreateMealRow(MealRow row, int index);
        void UpdateMealRow(MealRow row);
        void DeleteMealRow(MealRow row);

        void SaveNutrientSettings(IEnumerable<NutrientSetting> settings);
        IEnumerable<NutritionGoalDetails> GetNutritionGoals(Guid userId);
        NutritionGoalDetails GetNutritionGoal(Guid id);
        void CreateNutritionGoal(NutritionGoalDetails goal);
        void UpdateNutritionGoal(NutritionGoalDetails goal);
        void ActivateNutritionGoal(NutritionGoal goal);
        void DeleteNutritionGoal(NutritionGoal goal);
        void SaveMealDefinitions(IEnumerable<MealDefinition> definitions);
        IEnumerable<FoodSearchResult> SearchLatestFoods(Guid userId, int count);
        IEnumerable<FoodSearchResult> SearchFoodsByEan(string ean, Guid? userId = null);
        IEnumerable<FoodSearchNutrientResult> SearchFoodsTopNutrients(int nutrientId, Guid userId, int count, bool descending = true);
        IEnumerable<MealDefinition> GetMealDefinitions(Guid userId);
        void SaveHomeNutrients(Guid userId, int[] nutrientIds);
        IEnumerable<FoodSearchResult> SearchMostUsedFoods(Guid userId, int count);
        IEnumerable<DayNutrient> GetDailyNutrients(Guid userId, DateTimeOffset start, DateTimeOffset end);
        void CreateFavouriteMeal(FavouriteMeal favourite);
        IEnumerable<FavouriteMeal> GetFavouriteMeals(Guid userId);
        IEnumerable<MealDetails> GetMeals(IEnumerable<Guid> ids);
        FavouriteMeal GetFavouriteMeal(Guid id);
        void DeleteFavouriteMeal(Guid id);
    }
}
