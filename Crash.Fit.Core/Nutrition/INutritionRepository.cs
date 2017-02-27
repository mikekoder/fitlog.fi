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
        bool CreateNutrient(Nutrient nutrient);
        bool UpdateNutrient(Nutrient nutrient);
        IEnumerable<DailyIntake> GetDailyIntakes(Gender gender, TimeSpan age);


        IEnumerable<FoodMinimal> SearchFoods(string[] nameTokens, Guid? userId = null);
        FoodDetails GetFood(Guid id);
        IEnumerable<FoodDetails> GetFoods(IEnumerable<Guid> ids);
        //IEnumerable<FoodDetails> GetFoods(IEnumerable<Guid> ids);
        bool CreateFood(FoodDetails food);
        bool UpdateFood(FoodDetails food);
        bool DeleteFood(FoodMinimal food);
        bool RestoreFood(Guid id, out FoodDetails food);

        //IEnumerable<Portion> GetPortions(IEnumerable<Guid> ids);

        IEnumerable<MealDetails> SearchMeals(Guid userId, DateTimeOffset start, DateTimeOffset end);
        MealDetails GetMeal(Guid id);
        bool CreateMeal(MealDetails meal);
        bool UpdateMeal(MealDetails meal);
        bool DeleteMeal(MealMinimal meal);
        bool RestoreMeal(Guid id, out MealDetails meal);
    }
}
