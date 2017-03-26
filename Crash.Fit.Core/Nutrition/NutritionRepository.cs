using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Crash.Fit.Nutrition
{
    public class NutritionRepository : RepositoryBase, INutritionRepository
    {
        public NutritionRepository(DbProviderFactory dbFactory, string connectionString) : base(dbFactory,connectionString)
        {
        }

        public IEnumerable<Nutrient> GetNutrients()
        {
            var sql = @"SELECT * FROM Nutrient";
            using (var conn = CreateConnection())
            {
                return conn.Query<Nutrient>(sql);
            }
        }
        public bool CreateNutrient(Nutrient nutrient)
        {
            nutrient.Id = Guid.NewGuid();
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Nutrient(Id,Name,ShortName,Unit,FineliId,FineliClass,FineliGroup,UIOrder,UIVisible) VALUES(@Id,@Name,@ShortName,@Unit,@FineliId,@FineliClass,@FineliGroup,@UIOrder,@UIVisible)", nutrient, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    nutrient.Id = Guid.Empty;
                    return false;
                }
            }
        }
        public bool UpdateNutrient(Nutrient nutrient)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Nutrient SET Name=@Name, ShortName=@ShortName, Unit=@Unit, FineliId=@FineliId, FineliClass=@FineliClass, FineliGroup=@FineliGroup, UIOrder=@UIOrder, UIVisible=@UIVisible WHERE Id=@Id", nutrient, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public IEnumerable<DailyIntake> GetDailyIntakes(Gender gender, TimeSpan age)
        {
            var sql = @"
SELECT * FROM DailyIntake 
WHERE Gender = @gender AND StartAge <= @age AND EndAge >= @age";
            var parameters = new { gender = gender.ToString(), age = age.TotalDays/365 };
            using (var conn = CreateConnection())
            {
                return conn.Query<DailyIntake>(sql, parameters);
            }
        }
        public FoodDetails GetFood(Guid id)
        {
            return GetFoods(new[] { id }).FirstOrDefault();
        }
        public IEnumerable<FoodDetails> GetFoods(IEnumerable<Guid> ids)
        {
            var sql = @"
SELECT * FROM Food WHERE Id IN @ids;
SELECT * FROM FoodPortion WHERE FoodId IN @ids;
SELECT * FROM FoodNutrient WHERE FoodId IN @ids;
SELECT * FROM RecipeIngredient WHERE RecipeId IN @ids ORDER BY [Index]";
            var parameters = new { ids };
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var foods = multi.Read<FoodDetails>().ToArray();
                var portions = multi.Read<PortionRaw>().ToArray();
                var nutrients = multi.Read<FoodNutrientAmount>().ToArray();
                var ingredients = multi.Read<RecipeIngredientRaw>().ToArray();
                foreach(var food in foods)
                {
                    food.Portions = portions.Where(p => p.FoodId == food.Id).ToArray();
                    food.Nutrients = nutrients.Where(p => p.FoodId == food.Id).ToArray();
                    food.Ingredients = ingredients.Where(p => p.RecipeId == food.Id).ToArray();
                }
                return foods;
            }
        }
        public IEnumerable<FoodMinimal> SearchFoods(string[] nameTokens, Guid? userId = null)
        {
            var parameters = new DynamicParameters();
            var sql = "";
            if (nameTokens != null)
            {
                for (int i = 0; i < nameTokens.Length; i++)
                {
                    parameters.Add("p" + i, nameTokens[i]);
                    sql += " AND Name LIKE CONCAT('%',@p" + i + ",'%')";
                }
            }
            if (userId.HasValue)
            {
                sql += " AND (UserId IS NULL OR UserId=@UserId)";
                parameters.Add("UserId", userId.Value);
            }
            else
            {
                sql += " AND UserId IS NULL";
            }
            sql = "SELECT * FROM Food WHERE " + sql.Substring(5) + " ORDER BY Name";
            using (var conn = CreateConnection())
            {
                return conn.Query<FoodMinimal>(sql, parameters);
            }
        }
        public IEnumerable<FoodMinimal> SearchUserFoods(Guid userId)
        {
            var sql = @"
SELECT * FROM Food WHERE UserId=@userId AND IsRecipe=0 AND Deleted IS NULL;";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId }))
            {
                var recipes = multi.Read<FoodMinimal>().ToList();
                return recipes;
            }
        }
        public IEnumerable<FoodMinimal> SearchRecipes(Guid userId)
        {
            var sql = @"
SELECT * FROM Food WHERE UserId=@userId AND IsRecipe=1 AND Deleted IS NULL;";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql,new { userId }))
            {
                var recipes = multi.Read<FoodMinimal>().ToList();
                return recipes;
            }
        }
        public bool CreateFood(FoodDetails food)
        {
            food.Id = Guid.NewGuid();
            food.Created = DateTimeOffset.Now;
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Food(Id,UserId,Name,IsRecipe,FineliId,Created) VALUES(@Id,@UserId,@Name,@IsRecipe,@FineliId,@Created)", food, tran);
                    conn.Execute("INSERT INTO FoodNutrient(FoodId,NutrientId,Amount) VALUES(@FoodId,@NutrientId,@Amount)",
                        food.Nutrients.Select(n => new { FoodId = food.Id, n.NutrientId, n.Amount }), tran);
                    conn.Execute("INSERT INTO FoodPortion(Id,FoodId,Name,Weight) VALUES(@Id,@FoodId,@Name,@Weight)", food.Portions.Select(p => new { Id = Guid.NewGuid(), FoodId = food.Id, p.Name, p.Weight }), tran);
                    if(food.Ingredients != null)
                    {
                        conn.Execute("INSERT INTO RecipeIngredient(RecipeId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(@RecipeId,@Index,@FoodId,@Quantity,@PortionId,@Weight)", food.Ingredients.Select((i,index) => new
                        {
                            RecipeId = food.Id,
                            Index = index,
                            i.FoodId,
                            i.Quantity,
                            i.PortionId,
                            i.Weight
                        }), tran);
                    }
                    tran.Commit();
                    return true;
                }
                catch
                {
                    food.Id = Guid.Empty;
                    tran.Rollback();
                    return false;
                }
            }
        }
        public bool UpdateFood(FoodDetails food)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM FoodNutrient WHERE FoodId=@Id", new { Id = food.Id }, tran);
                    conn.Execute("DELETE FROM RecipeIngredient WHERE RecipeId=@Id", new { Id = food.Id }, tran);

                    conn.Execute("UPDATE Food SET Name=@Name WHERE Id=@Id", food, tran);
                    conn.Execute("INSERT INTO FoodNutrient(FoodId,NutrientId,Amount) VALUES(@FoodId,@NutrientId,@Amount)",
                        food.Nutrients.Select(n => new { FoodId = food.Id, n.NutrientId, n.Amount }), tran);

                    if(food.Portions.Any(p => p.Id != Guid.Empty))
                    {
                        conn.Execute("UPDATE FoodPortion SET Name=@Name, Weight=@Weight WHERE Id=@Id", food.Portions.Where(p => p.Id != Guid.Empty)
                            .Select(p => new { p.Id, p.Name, p.Weight }), tran);
                    }
                    if (food.Portions.Any(p => p.Id == Guid.Empty))
                    {
                        conn.Execute("INSERT INTO FoodPortion(Id,FoodId,Name,Weight) VALUES(@Id,@FoodId,@Name,@Weight)", food.Portions.Where(p => p.Id == Guid.Empty)
                            .Select(p => new { Id = Guid.NewGuid(), FoodId = food.Id, p.Name, p.Weight }), tran);
                    }
                    if (food.Ingredients != null)
                    {
                        conn.Execute("INSERT INTO RecipeIngredient(RecipeId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(@RecipeId,@Index,@FoodId,@Quantity,@PortionId,@Weight)", food.Ingredients.Select((i,index) => new
                        {
                            RecipeId = food.Id,
                            Index = index,
                            i.FoodId,
                            i.Quantity,
                            i.PortionId,
                            i.Weight
                        }), tran);
                    }
                    tran.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    return false;
                }
            }
        }
        public bool DeleteFood(FoodMinimal food)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Food SET Deleted=@Deleted WHERE Id=@Id", new { Id = food.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
        }
        public bool RestoreFood(Guid id, out FoodDetails food)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Food SET Deleted=NULL WHERE Id=@Id", new { Id = id }, tran);
                    tran.Commit();
                    food = GetFood(id);
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    food = null;
                    return false;
                }
            }
        }

        public MealDetails GetMeal(Guid id)
        {
            var sql = @"
SELECT * FROM Meal WHERE Id=@id;
SELECT * FROM MealNutrient WHERE MealId=@id;
SELECT * FROM MealRow WHERE MealId=@id ORDER BY [Index];";
            var parameters = new { id };
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var meal = multi.Read<MealDetails>().SingleOrDefault();
                if (meal != null)
                {
                    meal.Nutrients = multi.Read<NutrientAmount>().ToArray();
                    meal.Rows = multi.Read<MealRow>().ToArray();
                }
                return meal;
            }
        }
        public IEnumerable<MealDetails> SearchMeals(Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            var filter = "UserId=@userId AND Time >= @start AND Time <= @end AND Deleted IS NULL";
            var sql = $@"
SELECT * FROM Meal WHERE {filter};
SELECT * FROM MealNutrient WHERE MealId IN (SELECT Id FROM Meal WHERE {filter});";
            var parameters = new { userId, start, end };
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var meals = multi.Read<MealDetails>().ToList();
                var nutrients = multi.Read<MealNutrient>().ToList();
                foreach (var meal in meals)
                {
                    meal.Nutrients = nutrients.Where(n => n.MealId == meal.Id).Cast<NutrientAmount>().ToArray();
                }
                return meals;
            }
        }
        public bool CreateMeal(MealDetails meal)
        {
            meal.Id = Guid.NewGuid();
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Meal(Id,UserId,Time,Name) VALUES(@Id,@UserId,@Time,@Name)", meal, tran);
                    conn.Execute("INSERT INTO MealNutrient(MealId,NutrientId,Amount) VALUES(@MealId,@NutrientId,@Amount)",
                        meal.Nutrients.Select(n => new { MealId = meal.Id, n.NutrientId, n.Amount }), tran);
                    conn.Execute("INSERT INTO MealRow(MealId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(@MealId,@Index,@FoodId,@Quantity,@PortionId,@Weight)", meal.Rows.Select((r, i) => new
                    {
                        MealId = meal.Id,
                        Index = i,
                        r.FoodId,
                        r.Quantity,
                        r.PortionId,
                        r.Weight }), tran);
                    tran.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    meal.Id = Guid.Empty;
                    tran.Rollback();
                    return false;
                }
            }
        }
        public bool UpdateMeal(MealDetails meal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM MealNutrient WHERE MealId=@Id", new { Id = meal.Id }, tran);
                    conn.Execute("DELETE FROM MealRow WHERE MealId=@Id", new { Id = meal.Id }, tran);

                    conn.Execute("UPDATE Meal SET Time=@Time, Name=@Name WHERE Id=@Id", meal, tran);
                    conn.Execute("INSERT INTO MealNutrient(MealId,NutrientId,Amount) VALUES(@MealId,@NutrientId,@Amount)",
                                            meal.Nutrients.Select(n => new { MealId = meal.Id, n.NutrientId, n.Amount }), tran);
                    conn.Execute("INSERT INTO MealRow(MealId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(@MealId,@Index,@FoodId,@Quantity,@PortionId,@Weight)", meal.Rows.Select((r,i) => new
                    {
                        MealId = meal.Id,
                        Index = i,
                        r.FoodId,
                        r.Quantity,
                        r.PortionId,
                        r.Weight }), tran);
                    tran.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    return false;
                }
            }
        }
        public bool DeleteMeal(MealMinimal meal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Meal SET Deleted=@Deleted WHERE Id=@Id", new { Id = meal.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
        }
        public bool RestoreMeal(Guid id, out MealDetails meal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Meal SET Deleted=NULL WHERE Id=@Id", new { Id = id }, tran);
                    tran.Commit();
                    meal = GetMeal(id);
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    meal = null;
                    return false;
                }
            }
        }
 
        private class PortionRaw : Portion
        {
            public Guid FoodId { get; set; }
        }
        private class FoodNutrientAmount : NutrientAmount
        {
            public Guid FoodId { get; set; }
        }
        private class RecipeIngredientRaw : RecipeIngredient
        {
            public Guid RecipeId { get; set; }
        }
    }
}
