using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class NutritionRepository : RepositoryBase
    {
        public NutritionRepository(DbProviderFactory dbFactory, string connectionString) : base(dbFactory,connectionString)
        {
        }

        public IEnumerable<Nutrient> GetNutrients()
        {
            throw new NotImplementedException();
        }
        public FoodDetails GetFood(Guid id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<FoodDetails> SearchFoods(Guid userId, params string[] nameParts)
        {
            throw new NotImplementedException();
        }
        public void CreateFood(FoodDetails food)
        {

        }
        public void UpdateFood(FoodDetails food)
        {

        }
        public void DeleteFood(FoodDetails food)
        {

        }

        public MealDetails GetMeal(Guid id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<MealDetails> GetMeals(Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            throw new NotImplementedException();
        }
        public void CreateMeal(MealDetails Meal)
        {

        }
        public void UpdateMeal(MealDetails Meal)
        {

        }
        public void DeleteMeal(MealDetails Meal)
        {

        }
    }
}
