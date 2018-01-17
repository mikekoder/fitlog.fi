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
        public NutritionRepository(DbProviderFactory dbFactory, string connectionString) : base(dbFactory, connectionString)
        {
        }

        public IEnumerable<Nutrient> GetNutrients()
        {
            var sql = @"SELECT * FROM Nutrient WHERE DELETED IS NULL";
            using (var conn = CreateConnection())
            {
                return conn.Query<Nutrient>(sql);
            }
        }
        public IEnumerable<NutrientSetting> GetNutrientSettings(Guid userId)
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<NutrientSetting>("SELECT * FROM NutrientSettings WHERE UserId=@userId", new { userId });
            }
        }
        public void CreateNutrient(Nutrient nutrient)
        {
            nutrient.Id = Guid.NewGuid();
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Nutrient(Id,Name,ShortName,Unit,FineliId,FineliClass,FineliGroup,DefaultOrder,DefaultHideSummary,DefaultHideDetails) VALUES(@Id,@Name,@ShortName,@Unit,@FineliId,@FineliClass,@FineliGroup,@DefaultOrder,@DefaultHideSummary,@DefaultHideDetails)", nutrient, tran);
                    tran.Commit();
                }
                catch
                {
                    throw;
                }
            }
        }
        public void UpdateNutrient(Nutrient nutrient)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Nutrient SET Name=@Name, ShortName=@ShortName, Unit=@Unit, FineliId=@FineliId, FineliClass=@FineliClass, FineliGroup=@FineliGroup, DefaultOrder=@DefaultOrder,DefaultHideSummary=@DefaultHideSummary,DefaultHideDetails=@DefaultHideDetails WHERE Id=@Id", nutrient, tran);
                    tran.Commit();
                }
                catch
                {
                    throw;
                }
            }
        }
        public IEnumerable<DailyIntake> GetDailyIntakes(string gender, TimeSpan age)
        {
            var sql = @"
SELECT * FROM DailyIntake 
WHERE Gender = @gender AND StartAge <= @age AND EndAge >= @age";
            var parameters = new { gender = gender.ToString(), age = age.TotalDays / 365 };
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
SELECT RI.*, F.Name AS FoodName, FP.Name AS PortionName FROM RecipeIngredient RI 
    JOIN Food F ON F.Id=RI.FoodId
    LEFT JOIN FoodPortion FP ON FP.Id=RI.PortionId
    WHERE RI.RecipeId IN @ids ORDER BY [Index]";
            var parameters = new { ids };
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var foods = multi.Read<FoodDetails>().ToArray();
                var portions = multi.Read<PortionRaw>().ToArray();
                var nutrients = multi.Read<FoodNutrientAmountRaw>().ToArray();
                var ingredients = multi.Read<RecipeIngredientRaw>().ToArray();
                foreach (var food in foods)
                {
                    food.Portions = portions.Where(p => p.FoodId == food.Id).ToArray();
                    food.Nutrients = nutrients.Where(p => p.FoodId == food.Id).ToArray();
                    food.Ingredients = ingredients.Where(p => p.RecipeId == food.Id).ToArray();
                }
                return foods;
            }
        }
        public IEnumerable<FoodSearchResult> SearchFoods(string[] nameTokens, Guid? userId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId.Value);

            var sql = "";
            if (nameTokens != null)
            {
                for (int i = 0; i < nameTokens.Length; i++)
                {
                    parameters.Add("p" + i, nameTokens[i]);
                    sql += " AND Food.Name LIKE CONCAT('%',@p" + i + ",'%')";
                }
            }
            if (userId.HasValue)
            {
                sql += " AND (Food.UserId IS NULL OR Food.UserId=@UserId)";
            }
            else
            {
                sql += " AND Food.UserId IS NULL";
            }
            sql += " AND Food.Deleted IS NULL";
            sql = @"SELECT Food.*, FoodUsage.UsageCount, FoodUsage.LatestUse
FROM Food 
LEFT JOIN FoodUsage ON FoodUsage.FoodId=Food.Id AND FoodUsage.UserId=@UserId
WHERE " + sql.Substring(5) + " ORDER BY Name";
            using (var conn = CreateConnection())
            {
                return conn.Query<FoodSearchResult>(sql, parameters);
            }
        }
        public IEnumerable<FoodSummary> SearchUserFoods(Guid userId)
        {
            var sql = @"
SELECT *, (SELECT COUNT(*) FROM FoodNutrient WHERE FoodId=Food.Id) AS NutrientCount FROM Food WHERE UserId=@userId AND IsRecipe=0 AND Deleted IS NULL;
SELECT R.FoodId, COUNT(*) AS [Count] FROM MealRow R JOIN Meal M ON M.Id=R.MealId
WHERE M.UserId=@userId AND M.Deleted IS NULL
GROUP BY R.FoodId;";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId }))
            {
                var foods = multi.Read<FoodSummary>().ToList();
                var counts = multi.Read<FoodUsageRaw>().ToList();
                foreach (var food in foods)
                {
                    food.UsageCount = counts.FirstOrDefault(c => c.FoodId == food.Id)?.Count ?? 0;
                }
                return foods;
            }
        }
        public IEnumerable<FoodSummary> SearchRecipes(Guid userId)
        {
            var sql = @"
SELECT * FROM Food WHERE UserId=@userId AND IsRecipe=1 AND Deleted IS NULL;
SELECT R.FoodId, COUNT(*) AS [Count] FROM MealRow R JOIN Meal M ON M.Id=R.MealId
WHERE M.UserId=@userId AND M.Deleted IS NULL
GROUP BY R.FoodId;";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId }))
            {
                var foods = multi.Read<FoodSummary>().ToList();
                var counts = multi.Read<FoodUsageRaw>().ToList();
                foreach (var food in foods)
                {
                    food.UsageCount = counts.FirstOrDefault(c => c.FoodId == food.Id)?.Count ?? 0;
                }
                return foods;
            }
        }
        public void CreateFood(FoodDetails food)
        {
            food.Id = Guid.NewGuid();
            food.Created = DateTimeOffset.Now;
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Food(Id,UserId,Name,Manufacturer,IsRecipe,FineliId,Created,CookedWeight,NutrientPortionId) VALUES(@Id,@UserId,@Name,@Manufacturer,@IsRecipe,@FineliId,@Created,@CookedWeight,@NutrientPortionId)", food, tran);
                    conn.Execute("INSERT INTO FoodNutrient(FoodId,NutrientId,Amount,PortionAmount) VALUES(@FoodId,@NutrientId,@Amount,@PortionAmount)",
                        food.Nutrients.Select(n => new { FoodId = food.Id, n.NutrientId, n.Amount, n.PortionAmount }), tran);
                    conn.Execute("INSERT INTO FoodPortion(Id,FoodId,Name,Amount,Weight) VALUES(@Id,@FoodId,@Name,@Amount,@Weight)", food.Portions.Select(p => new
                    {
                        Id = p.Id == Guid.Empty ? Guid.NewGuid() : p.Id,
                        FoodId = food.Id,
                        p.Name,
                        p.Amount,
                        p.Weight
                    }), tran);
                    if (food.Ingredients != null)
                    {
                        conn.Execute("INSERT INTO RecipeIngredient(Id,RecipeId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(newid(),@RecipeId,@Index,@FoodId,@Quantity,@PortionId,@Weight)", food.Ingredients.Select((i, index) => new
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
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void UpdateFood(FoodDetails food)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM FoodNutrient WHERE FoodId=@Id", new { food.Id }, tran);
                    conn.Execute("DELETE FROM RecipeIngredient WHERE RecipeId=@Id", new { food.Id }, tran);
                    conn.Execute("DELETE FROM FoodPortion WHERE FoodId=@Id AND Id NOT IN @ids", new { food.Id, ids = food.Portions.Select(p => p.Id) }, tran);

                    conn.Execute("UPDATE Food SET Name=@Name,Manufacturer=@Manufacturer,CookedWeight=@CookedWeight,NutrientPortionId=@NutrientPortionId WHERE Id=@Id", food, tran);
                    conn.Execute("INSERT INTO FoodNutrient(FoodId,NutrientId,Amount,PortionAmount) VALUES(@FoodId,@NutrientId,@Amount,@PortionAmount)", food.Nutrients.Select(n => new
                    {
                        FoodId = food.Id,
                        n.NutrientId,
                        n.Amount,
                        n.PortionAmount
                    }), tran);
                    conn.Execute("UPDATE FoodPortion SET Name=@Name,Weight=@Weight,Amount=@Amount WHERE Id=@Id AND FoodId=@FoodId", food.Portions.Where(p => p.Id != Guid.Empty).Select(p => new
                    {
                        p.Id,
                        FoodId = food.Id,
                        p.Name,
                        p.Weight,
                        p.Amount
                    }), tran);
                    conn.Execute("INSERT INTO FoodPortion(Id,FoodId,Name,Weight,Amount) VALUES(@Id,@FoodId,@Name,@Weight,@Amount)", food.Portions.Where(p => p.Id == Guid.Empty).Select(p => new
                    {
                        Id = Guid.NewGuid() ,
                        FoodId = food.Id,
                        p.Name,
                        p.Weight,
                        p.Amount
                    }), tran);

                    if (food.Ingredients != null)
                    {
                        conn.Execute("INSERT INTO RecipeIngredient(Id,RecipeId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(newid(),@RecipeId,@Index,@FoodId,@Quantity,@PortionId,@Weight)", food.Ingredients.Select((i, index) => new
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
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void DeleteFood(Food food)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Food SET Deleted=@Deleted WHERE Id=@Id", new { Id = food.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void RestoreFood(Guid id, out FoodDetails food)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Food SET Deleted=NULL WHERE Id=@Id", new { Id = id }, tran);
                    tran.Commit();
                    food = GetFood(id);
                }
                catch
                {
                    tran.Rollback();
                    food = null;
                    throw;
                }
            }
        }

        public MealDetails GetMeal(Guid id)
        {
            var sql = @"
SELECT * FROM Meal WHERE Id=@id;
SELECT * FROM MealNutrient WHERE MealId=@id;
SELECT MR.*, F.Name AS FoodName, FP.Name AS PortionName FROM MealRow MR 
    JOIN Food F ON F.Id=MR.FoodId 
    LEFT JOIN FoodPortion FP ON FP.Id=MR.PortionId
    WHERE MR.MealId=@id ORDER BY [Index];
SELECT * FROM MealRowNutrient WHERE MealRowId IN(SELECT Id FROM MealRow WHERE MealId=@id);";
            var parameters = new { id };
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var meal = multi.Read<MealDetails>().SingleOrDefault();
                if (meal != null)
                {
                    meal.Nutrients = multi.Read<NutrientAmount>().ToArray();
                    meal.Rows = multi.Read<MealRow>().ToArray();
                    var rowNutrients = multi.Read<MealRowNutrientRaw>().ToArray();
                    foreach(var row in meal.Rows)
                    {
                        row.Nutrients = rowNutrients.Where(r => r.MealRowId == row.Id).ToArray();
                    }
                }
                return meal;
            }
        }
        public IEnumerable<MealDetails> SearchMeals(Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            var filter = "UserId=@userId AND Time >= @start AND Time <= @end AND Deleted IS NULL";
            var sql = $@"
SELECT * FROM Meal WHERE {filter};
SELECT * FROM MealNutrient WHERE MealId IN (SELECT Id FROM Meal WHERE {filter});
SELECT MR.*, F.Name AS FoodName, FP.Name AS PortionName FROM MealRow MR 
    JOIN Food F ON F.Id=MR.FoodId 
    LEFT JOIN FoodPortion FP ON FP.Id=MR.PortionId
    WHERE MR.MealId IN(SELECT Id FROM Meal WHERE {filter}) ORDER BY [Index];
SELECT * FROM MealRowNutrient WHERE MealRowId IN(SELECT Id FROM MealRow WHERE MealId IN (SELECT Id FROM Meal WHERE {filter}))";
            var parameters = new { userId, start, end };
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var meals = multi.Read<MealDetails>().ToList();
                var nutrients = multi.Read<MealNutrient>().ToList();
                var rows = multi.Read<MealRow>().ToList();
                var rowNutrients = multi.Read<MealRowNutrientRaw>();
                foreach (var meal in meals)
                {
                    meal.Nutrients = nutrients.Where(n => n.MealId == meal.Id).ToArray();
                    meal.Rows = rows.Where(r => r.MealId == meal.Id).ToArray();
                    foreach(var row in meal.Rows)
                    {
                        row.Nutrients = rowNutrients.Where(r => r.MealRowId == row.Id).ToArray();
                    }
                }
                return meals;
            }
        }
        public void CreateMeal(MealDetails meal)
        {
            meal.Id = Guid.NewGuid();
            foreach(var row in meal.Rows)
            {
                row.Id = Guid.NewGuid();
                row.MealId = meal.Id;
            }
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Meal(Id,UserId,Time,DefinitionId,Created) VALUES(@Id,@UserId,@Time,@DefinitionId,@Created)", meal, tran);
                    conn.Execute("INSERT INTO MealNutrient(MealId,NutrientId,Amount) VALUES(@MealId,@NutrientId,@Amount)",
                        meal.Nutrients.Select(n => new { MealId = meal.Id, n.NutrientId, n.Amount }), tran);
                    conn.Execute("INSERT INTO MealRow(Id,MealId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(@Id,@MealId,@Index,@FoodId,@Quantity,@PortionId,@Weight)", meal.Rows.Select((r, i) => new
                    {
                        Id = r.Id,
                        MealId = meal.Id,
                        Index = i,
                        r.FoodId,
                        r.Quantity,
                        r.PortionId,
                        r.Weight }), tran);
                    conn.Execute("INSERT INTO MealRowNutrient(MealRowId,NutrientId,Amount) VALUES(@Id,@NutrientId,@Amount)", meal.Rows.SelectMany(r => r.Nutrients.Select(n => new
                    {
                        r.Id,
                        n.NutrientId,
                        n.Amount
                    })), tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void UpdateMeal(MealDetails meal)
        {
            foreach(var row in meal.Rows.Where(r => r.Id == Guid.Empty))
            {
                row.MealId = meal.Id;
                row.Id = Guid.NewGuid();
            }
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM MealNutrient WHERE MealId=@Id", new { meal.Id }, tran);
                    conn.Execute("DELETE FROM MealRow WHERE MealId=@Id", new { meal.Id }, tran);
                    conn.Execute("DELETE FROM MealRowNutrient WHERE MealRowId IN(SELECT Id FROM MealRow WHERE MealId=@Id)", new { meal.Id }, tran);

                    conn.Execute("UPDATE Meal SET Time=@Time,DefinitionId=@DefinitionId WHERE Id=@Id", meal, tran);
                    conn.Execute("INSERT INTO MealNutrient(MealId,NutrientId,Amount) VALUES(@MealId,@NutrientId,@Amount)",
                                            meal.Nutrients.Select(n => new { MealId = meal.Id, n.NutrientId, n.Amount }), tran);
                    conn.Execute("INSERT INTO MealRow(Id,MealId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(@Id,@MealId,@Index,@FoodId,@Quantity,@PortionId,@Weight)", meal.Rows.Select((r, i) => new
                    {
                        Id = r.Id,
                        MealId = meal.Id,
                        Index = i,
                        r.FoodId,
                        r.Quantity,
                        r.PortionId,
                        r.Weight }), tran);
                    conn.Execute("INSERT INTO MealRowNutrient(MealRowId,NutrientId,Amount) VALUES(@Id,@NutrientId,@Amount)", meal.Rows.SelectMany(r => r.Nutrients.Select(n => new
                    {
                        r.Id,
                        n.NutrientId,
                        n.Amount
                    })), tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void DeleteMeal(Meal meal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Meal SET Deleted=@Deleted WHERE Id=@Id", new { Id = meal.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void RestoreMeal(Guid id, out MealDetails meal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Meal SET Deleted=NULL WHERE Id=@Id", new { Id = id }, tran);
                    tran.Commit();
                    meal = GetMeal(id);
                }
                catch
                {
                    tran.Rollback();
                    meal = null;
                    throw;
                }
            }
        }
        public void SaveNutrientSettings(IEnumerable<NutrientSetting> settings)
        {
            var sql = @"MERGE INTO NutrientSettings
USING(select @NutrientId AS NutrientId, @UserId AS UserId) AS Source
ON(NutrientSettings.UserId = Source.UserId AND NutrientSettings.NutrientId=Source.NutrientId)
WHEN MATCHED THEN
    UPDATE SET [Order] = @Order, HideSummary=@HideSummary, HideDetails=@HideDetails
WHEN NOT MATCHED THEN
    INSERT(Id,UserId, NutrientId,[Order],HideSummary,HideDetails) VALUES(newid(),@UserId, @NutrientId,@Order,@HideSummary,@HideDetails);";
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(sql, settings, tran);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void SaveHomeNutrients(Guid userId, Guid[] nutrients)
        {
            var sql = @"MERGE INTO NutrientSettings
USING(select @NutrientId AS NutrientId, @UserId AS UserId) AS Source
ON(NutrientSettings.UserId = Source.UserId AND NutrientSettings.NutrientId=Source.NutrientId)
WHEN MATCHED THEN
    UPDATE SET HomeOrder=@HomeOrder
WHEN NOT MATCHED THEN
    INSERT(Id,UserId, NutrientId,HomeOrder) VALUES(newid(),@UserId, @NutrientId,@HomeOrder);";

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE NutrientSettings SET HomeOrder=NULL WHERE UserId=@userId", new { userId }, tran);
                    conn.Execute(sql, nutrients.Select((nutrientId,index) => new
                    {
                        UserId = userId,
                        NutrientId = nutrientId,
                        HomeOrder = index
                    }), tran);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public IEnumerable<NutritionGoalDetails> GetNutritionGoals(Guid userId)
        {
            return GetNutritionGoals("UserId=@userId AND Deleted IS NULL", new { userId });
        }
        public NutritionGoalDetails GetNutritionGoal(Guid id)
        {
            return GetNutritionGoals("Id=@id", new { id }).FirstOrDefault();
        }
        private IEnumerable<NutritionGoalDetails> GetNutritionGoals(string filter, object parameters)
        {
            var sql = $@"
SELECT * FROM NutritionGoal WHERE {filter};
SELECT * FROM NutritionGoalPeriod WHERE NutritionGoalId IN (SELECT Id FROM NutritionGoal WHERE {filter}) ORDER By [Index];
SELECT * FROM NutritionGoalMeal WHERE NutritionGoalPeriodId IN (SELECT Id FROM NutritionGoalPeriod WHERE NutritionGoalId IN (SELECT Id FROM NutritionGoal WHERE {filter}));
SELECT * FROM NutritionGoalValue WHERE NutritionGoalPeriodId IN (SELECT Id FROM NutritionGoalPeriod WHERE NutritionGoalId IN (SELECT Id FROM NutritionGoal WHERE {filter}))";

            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var goals = multi.Read<NutritionGoalDetails>().ToList();
                var periods = multi.Read<NutritionGoalPeriodRaw>().ToList();
                var meals = multi.Read<NutritionGoalMealRaw>().ToList();
                var nutrients = multi.Read<NutritionGoalValueRaw>().ToList();
                foreach (var goal in goals)
                {
                    goal.Periods = periods.Where(p => p.NutritionGoalId == goal.Id).ToArray();
                    foreach (var period in goal.Periods)
                    {
                        period.MealDefinitions = meals.Where(m => m.NutritionGoalPeriodId == period.Id).Select(m => m.MealDefinitionId).ToArray();
                        period.Nutrients = nutrients.Where(n => n.NutritionGoalPeriodId == period.Id).ToArray();
                    }
                }
                return goals;
            }
        }
        public void CreateNutritionGoal(NutritionGoalDetails goal)
        {
            goal.Id = Guid.NewGuid();
            foreach(var period in goal.Periods)
            {
                period.Id = Guid.NewGuid();
            }

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO NutritionGoal(Id,UserId,Name,Active,Created) VALUES(@Id,@UserId,@Name,@Active,@Created)", goal, tran);
                    conn.Execute("INSERT NutritionGoalPeriod(Id,NutritionGoalId,[Index],Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,ExerciseDay,RestDay,WholeDay) VALUES (@Id,@NutritionGoalId,@index,@Monday,@Tuesday,@Wednesday,@Thursday,@Friday,@Saturday,@Sunday,@ExerciseDay,@RestDay,@WholeDay)", goal.Periods.Select((p,index) => new
                    {
                        p.Id,
                        NutritionGoalId = goal.Id,
                        index,
                        p.Monday,
                        p.Tuesday,
                        p.Wednesday,
                        p.Thursday,
                        p.Friday,
                        p.Saturday,
                        p.Sunday,
                        p.ExerciseDay,
                        p.RestDay,
                        p.WholeDay
                    }), tran);
                    conn.Execute("INSERT INTO NutritionGoalMeal(NutritionGoalPeriodId,MealDefinitionId) VALUES(@Id,@MealDefinitionId)", goal.Periods.SelectMany(p => p.MealDefinitions.Select(m => new
                    {
                        p.Id,
                        MealDefinitionId = m
                    })), tran);
                    conn.Execute("INSERT INTO NutritionGoalValue(NutritionGoalPeriodId,NutrientId, Min,Max) VALUES(@Id,@NutrientId,@Min,@Max)", goal.Periods.SelectMany(p => p.Nutrients.Select(n => new
                    {
                        p.Id,
                        n.NutrientId,
                        n.Min,
                        n.Max
                    })), tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void UpdateNutritionGoal(NutritionGoalDetails goal)
        {
            foreach (var period in goal.Periods.Where(p => p.Id == Guid.Empty))
            {
                period.Id = Guid.NewGuid();
            }
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    var periodFilter = "SELECT Id FROM NutritionGoalPeriod WHERE NutritionGoalId=@Id";
                    conn.Execute($"DELETE FROM NutritionGoalValue WHERE NutritionGoalPeriodId IN ({periodFilter})", new { goal.Id }, tran);
                    conn.Execute($"DELETE FROM NutritionGoalMeal WHERE NutritionGoalPeriodId IN ({periodFilter})", new { goal.Id }, tran);
                    conn.Execute("DELETE FROM NutritionGoalPeriod WHERE NutritionGoalId=@Id", new { goal.Id }, tran);

                    conn.Execute("UPDATE NutritionGoal SET Name=@Name WHERE Id=@Id", goal, tran);
                    conn.Execute("INSERT NutritionGoalPeriod(Id,NutritionGoalId,[Index],Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,ExerciseDay,RestDay,WholeDay) VALUES (@Id,@NutritionGoalId,@index,@Monday,@Tuesday,@Wednesday,@Thursday,@Friday,@Saturday,@Sunday,@ExerciseDay,@RestDay,@WholeDay)", goal.Periods.Select((p, index) => new
                    {
                        p.Id,
                        NutritionGoalId = goal.Id,
                        index,
                        p.Monday,
                        p.Tuesday,
                        p.Wednesday,
                        p.Thursday,
                        p.Friday,
                        p.Saturday,
                        p.Sunday,
                        p.ExerciseDay,
                        p.RestDay,
                        p.WholeDay
                    }), tran);
                    conn.Execute("INSERT INTO NutritionGoalMeal(NutritionGoalPeriodId,MealDefinitionId) VALUES(@Id,@MealDefinitionId)", goal.Periods.SelectMany(p => p.MealDefinitions.Select(m => new
                    {
                        p.Id,
                        MealDefinitionId = m
                    })), tran);
                    conn.Execute("INSERT INTO NutritionGoalValue(NutritionGoalPeriodId,NutrientId, Min,Max) VALUES(@Id,@NutrientId,@Min,@Max)", goal.Periods.SelectMany(p => p.Nutrients.Select(n => new
                    {
                        p.Id,
                        n.NutrientId,
                        n.Min,
                        n.Max
                    })), tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }

        }
        public void ActivateNutritionGoal(NutritionGoal goal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE NutritionGoal SET Active=0 WHERE UserId=@UserId", new { goal.UserId }, tran);
                    conn.Execute("UPDATE NutritionGoal SET Active=1 WHERE Id=@Id", new {  goal.Id }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void DeleteNutritionGoal(NutritionGoal goal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE NutritionGoal SET Deleted=@Deleted WHERE Id=@Id", new { goal.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void SaveMealDefinitions(IEnumerable<MealDefinition> definitions)
        {
            foreach (var definition in definitions)
            {
                if (definition.Id == Guid.Empty)
                {
                    definition.Id = Guid.NewGuid();
                }
            }
            var sql = @"MERGE INTO MealDefinition
USING(select @Id AS Id) AS Source
ON(MealDefinition.Id = Source.Id)
WHEN MATCHED THEN
    UPDATE SET Name = @Name, Start=@Start, [End]=@End
WHEN NOT MATCHED THEN
    INSERT(Id,UserId, Name, Start,[End], Created) VALUES(@Id,@UserId, @Name, @Start,@End,GETDATE());";

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(@"UPDATE MealDefinition SET Deleted=@Deleted WHERE UserId IN @userIds AND Id NOT IN @ids", new { Deleted = DateTimeOffset.Now, userIds = definitions.Select(d => d.UserId), ids = definitions.Select(d => d.Id) }, tran);
                    conn.Execute(sql, definitions, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public IEnumerable<MealDefinition> GetMealDefinitions(Guid userId)
        {
            var sql = "SELECT * FROM MealDefinition WHERE UserId=@userId AND Deleted IS NULL";
            using (var conn = CreateConnection())
            {
                try
                {
                    return conn.Query<MealDefinition>(sql, new { userId });
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public MealRow GetMealRow(Guid id)
        {
            var sql = @"
SELECT MR.*, F.Name AS FoodName, FP.Name AS PortionName FROM MealRow MR 
    JOIN Food F ON F.Id=MR.FoodId 
    LEFT JOIN FoodPortion FP ON FP.Id=MR.PortionId
    WHERE MR.Id=@id ORDER BY [Index];
SELECT * FROM MealRowNutrient WHERE MRowId=@id";
            var parameters = new { id };
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var row = multi.Read<MealRow>().SingleOrDefault();
                if (row != null)
                {
                    row.Nutrients = multi.Read<NutrientAmount>().ToArray();
                }
                return row;
            }
        }
        public void AddMealRow(MealRow row)
        {
            row.Id = Guid.NewGuid();
             
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE MealNutrient SET Amount=Amount+@Amount WHERE MealId=@MealId AND NutrientId=@NutrientId", row.Nutrients.Select(n => new
                    {
                        MealId = row.MealId,
                        n.NutrientId,
                        n.Amount
                    }), tran);
                    conn.Execute("INSERT INTO MealRow(Id,MealId,[Index],FoodId,Quantity,PortionId,Weight) VALUES(@Id,@MealId,(SELECT MAX([Index])+1 FROM MealRow WHERE MealId=@MealId),@FoodId,@Quantity,@PortionId,@Weight)", row, tran);
                    conn.Execute("INSERT INTO MealRowNutrient(MealId,RowId,NutrientId,Amount) VALUES(@MealId,@RowId,@NutrientId,@Amount)", row.Nutrients.Select(n => new
                    {
                        row.MealId,
                        RowId = row.Id,
                        n.NutrientId,
                        n.Amount
                    }), tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<FoodSearchResult> SearchLatestFoods(Guid userId, int count)
        {
            var sql = @"SELECT TOP (@count) Food.*, FoodUsage.UsageCount, FoodUsage.LatestUse
FROM Food 
LEFT JOIN FoodUsage ON FoodUsage.FoodId=Food.Id
WHERE FoodUsage.UserId=@userId
ORDER BY FoodUsage.LatestUse DESC";
            using (var conn = CreateConnection())
            {
                return conn.Query<FoodSearchResult>(sql, new { userId, count});
            }
        }

        public IEnumerable<FoodSearchResult> SearchMostUsedFoods(Guid userId, int count)
        {
            var sql = @"SELECT TOP (@count) Food.*, FoodUsage.UsageCount, FoodUsage.LatestUse
FROM Food 
LEFT JOIN FoodUsage ON FoodUsage.FoodId=Food.Id
WHERE FoodUsage.UserId=@userId
ORDER BY FoodUsage.UsageCount DESC";
            using (var conn = CreateConnection())
            {
                return conn.Query<FoodSearchResult>(sql, new { userId, count });
            }
        }

        private class PortionRaw : Portion
        {
            public Guid FoodId { get; set; }
        }
        private class FoodNutrientAmountRaw : FoodNutrientAmount
        {
            public Guid FoodId { get; set; }
        }
        private class RecipeIngredientRaw : RecipeIngredient
        {
            public Guid RecipeId { get; set; }
        }
        private class FoodUsageRaw
        {
            public Guid FoodId { get; set; }
            public int Count { get; set; }
        }
        private class MealRowNutrientRaw : NutrientAmount
        {
            public Guid MealRowId { get; set; }
        }
        private class NutritionGoalPeriodRaw : NutritionGoalPeriod
        {
            public Guid NutritionGoalId { get; set; }
        }
        private class NutritionGoalMealRaw
        {
            public Guid NutritionGoalPeriodId { get; set; }
            public Guid MealDefinitionId { get; set; }
        }
        private class NutritionGoalValueRaw : NutritionGoalValue
        {
            public Guid NutritionGoalPeriodId { get; set; }
        }
    }
}
