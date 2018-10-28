using Crash.Fit.Nutrition;
using System;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Crash.Fit.Migration
{
    class Program
    {
        public static string ConnectionString = "";

        static void Main(string[] args)
        {
            //UpdateMealNutrients();
            //UpdateMealRowNutrients();
            EverKinetic.ImportData(ConnectionString);
        }


        static void UpdateMealNutrients()
        {
            IEnumerable<Guid> mealIds;
            using (var conn = CreateConnection())
            {
                mealIds = conn.Query<Guid>("SELECT * FROM Meal WHERE NutrientsJson IS NULL").ToArray();
            }
            foreach (var mealId in mealIds)
            {
                var mealDetails = GetMeal(mealId);

                using (var conn = CreateConnection())
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute("UPDATE Meal SET NutrientsJson=@NutrientsJson WHERE Id=@Id", new
                        {
                            mealDetails.Id,
                            NutrientsJson = JsonConvert.SerializeObject(mealDetails.Nutrients)
                        }, tran);

                        conn.Execute("UPDATE MealRow SET NutrientsJson=@NutrientsJson WHERE Id=@Id", mealDetails.Rows.Select(r => new
                        {
                            r.Id,
                            NutrientsJson = JsonConvert.SerializeObject(r.Nutrients)
                        }), tran);

                        tran.Commit();
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                    }
                }
            }

        }
        static void UpdateMealRowNutrients()
        {
            IEnumerable<MealRow> mealRows;
            using (var conn = CreateConnection())
            {
                mealRows = conn.Query<MealRowRaw>(@"SELECT  MealRow.* FROM MealRow 
JOIN Meal ON Meal.Id=MealRow.MealId
WHERE MealRow.NutrientsJson IS NOT NULL AND Meal.Time > @start", new { start = new DateTimeOffset(2015,8,11,7,31,0,new TimeSpan(3,0,0)) }).Select(r => new MealRow
                {
                    FoodId = r.FoodId,
                    FoodName = r.FoodName,
                    Id = r.Id,
                    Index = r.Index,
                    MealId = r.MealId,
                    Nutrients = JsonConvert.DeserializeObject<Dictionary<int,decimal>>(r.NutrientsJson),
                    PortionId = r.PortionId,
                    PortionName = r.PortionName,
                    Quantity = r.Quantity,
                    Weight = r.Weight
                }).ToArray();
            }

            var count = mealRows.Sum(r => r.Nutrients.Count);

            var sw = Stopwatch.StartNew();
            foreach(var mealGroup in mealRows.GroupBy(r => r.MealId))
            {
                var mealRowNutrients = mealGroup.NutrientsToDataTable();

                using (var conn2 = CreateConnection())
                using (var bulk = new SqlBulkCopy(conn2) { DestinationTableName = "MealRowNutrient" })
                {
                    bulk.WriteToServer(mealRowNutrients);
                }
            }
            sw.Stop();
            ;
        }
        static IEnumerable<Guid> GetAllUserIds()
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<Guid>("SELECT Id FROM [auth].[User]").ToArray();
            }
        }
        static IEnumerable<Meal> GetMeals(Guid userId)
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<Meal>("SELECT * FROM Meal WHERE UserId=@userId",new { userId }).ToArray();
            }
        }
        static MealDetails GetMeal(Guid id)
        {
            var sql = @"SELECT * FROM Meal WHERE Id=@id;
SELECT * FROM MealNutrient WHERE MealId=@id;
SELECT * FROM MealRow WHERE MealId=@id;
SELECT * FROM MealRowNutrient WHERE MealId=@id;";
            using (var conn = CreateConnection())
            using(var multi = conn.QueryMultiple(sql,new { id }))
            {
                var meal = multi.ReadSingle<MealDetails>();
                var mealNutrients = multi.Read<NutrientAmount>().ToArray();
                meal.Nutrients = mealNutrients.ToDictionary(n => n.NutrientId, n => n.Amount);
                meal.Rows = multi.Read<MealRow>().ToArray();
                var rowNutrients = multi.Read<MealRowNutrientRaw>().ToArray();

                foreach(var row in meal.Rows)
                {
                    row.Nutrients = rowNutrients.Where(n => n.MealRowId == row.Id).ToDictionary(n => n.NutrientId, n => n.Amount);      
                }

                return meal;
            }
        }
        static SqlConnection CreateConnection()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }
        static INutritionRepository GetNutritionRepository()
        {
            return new NutritionRepository(ConnectionString);
        }
    }
    class MealRowNutrientRaw : NutrientAmount
    {
        public Guid MealRowId { get; set; }
    }
    class MealRowRaw
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public int Index { get; set; }
        public Guid FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Quantity { get; set; }
        public Guid? PortionId { get; set; }
        public string PortionName { get; set; }
        public decimal Weight { get; set; }
        public string NutrientsJson { get; set; }
    }
}
