﻿using Fitlog.Api.Models.Nutrition;
using Fitlog.Api.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitlog.Api
{
    public interface IApiClient
    {
        ApiResult<ProfileResponse> GetProfile();
        /*
        // Nutrients
        ApiResult<NutrientResponse[]> GetNutrients();
        ApiResult<NutrientResponse[]> GetUserNutrients();
        ApiResult<NutrientTargetsResponse[]> GetNutrientTargets();
        ApiResult<NutrientTargetsResponse[]> SaveNutrientTargets(NutrientTargetsRequest[] targets);

        // Foods
        ApiResult<FoodSearchResultResponse[]> SearchFoods(string text);
        ApiResult<FoodSummaryResponse[]> GetUserFoods();
        ApiResult<FoodSummaryResponse[]> GetRecipes();
        ApiResult<FoodDetailsResponse[]> GetFood(Guid id);
        ApiResult<FoodDetailsResponse> CreateFood(FoodRequest food);
        ApiResult<FoodDetailsResponse> UpdateFood(Guid id, FoodRequest food);
        ApiResult DeleteFood(Guid id);

        // Meals
        ApiResult<MealDetailsResponse[]> GetMeals(DateTimeOffset? start, DateTimeOffset? end);
        ApiResult<MealDetailsResponse> GetMeal(Guid id);
        ApiResult<MealDetailsResponse> CreateMeal(MealRequest meal);
        ApiResult<MealDetailsResponse> UpdateMeal(Guid id, MealRequest meal);
        ApiResult DeleteMeal(Guid id);
        */
    }
}
