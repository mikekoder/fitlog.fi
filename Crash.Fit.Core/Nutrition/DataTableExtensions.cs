using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Crash.Fit.Nutrition
{
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable(this IEnumerable<MealRow> rows)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("MealId", typeof(Guid));
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("FoodId", typeof(Guid));
            table.Columns.Add("PortionId", typeof(Guid));
            table.Columns.Add("Quantity", typeof(decimal));
            table.Columns.Add("Weight", typeof(decimal));

            var index = 0;
            foreach (var row in rows)
            {
                table.Rows.Add(row.Id, row.MealId, index, row.FoodId, row.PortionId, row.Quantity, row.Weight);
            }

            return table;
        }
        public static DataTable NutrientsToDataTable(this IEnumerable<MealRow> rows)
        {
            var table = new DataTable();
            table.Columns.Add("MealRowId", typeof(Guid));
            table.Columns.Add("MealId", typeof(Guid));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("NutrientId", typeof(int));

            foreach (var row in rows)
            foreach (var nutrient in row.Nutrients)
            {
                table.Rows.Add(row.Id, row.MealId, nutrient.Value, nutrient.Key);
            }

            return table;
        }
    }
}
