using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public interface INutritionRepository
    {
        IEnumerable<Nutrient> GetNutrients();
        IEnumerable<UserNutrient> GetUserNutrients(Guid userId);
        bool CreateNutrient(Nutrient nutrient);
        bool UpdateNutrient(Nutrient nutrient);
        IEnumerable<DailyIntake> GetDailyIntakes(string gender, TimeSpan age);


        IEnumerable<FoodSearchResult> SearchFoods(string[] nameTokens, Guid? userId = null);
        IEnumerable<FoodSummary> SearchUserFoods(Guid userId);
        IEnumerable<FoodSummary> SearchRecipes(Guid userId);
        FoodDetails GetFood(Guid id);
        IEnumerable<FoodDetails> GetFoods(IEnumerable<Guid> ids);
        //IEnumerable<FoodDetails> GetFoods(IEnumerable<Guid> ids);
        bool CreateFood(FoodDetails food);
        bool UpdateFood(FoodDetails food);
        bool DeleteFood(Food food);
        bool RestoreFood(Guid id, out FoodDetails food);

        //IEnumerable<Portion> GetPortions(IEnumerable<Guid> ids);

        IEnumerable<MealDetails> SearchMeals(Guid userId, DateTimeOffset start, DateTimeOffset end);
        MealDetails GetMeal(Guid id);
        bool CreateMeal(MealDetails meal);
        bool UpdateMeal(MealDetails meal);
        bool DeleteMeal(Meal meal);
        bool RestoreMeal(Guid id, out MealDetails meal);

        bool SaveNutrientSettings(Guid userId, IEnumerable<NutrientSetting> settings);
        IEnumerable<NutrientTarget> GetNutrientTargets(Guid userId);
        bool SaveNutrientTargets(Guid userId, IEnumerable<NutrientTarget> targets);
    }
}
