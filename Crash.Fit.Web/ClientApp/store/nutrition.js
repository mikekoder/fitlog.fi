import api from '../api'
import constants from './constants'
import moment from 'moment'

export default {
    state: {

        mealsStart: null,
        mealsEnd: null,
        mealDays: [],
        meals: [],
        mealsDisplayStart: null,
        mealsDisplayEnd: null,

        nutrientGroups: [
            { id: 'MACROCMP', name: 'Makrot' },
            { id: 'VITAM', name: 'Vitamiinit' },
            { id: 'MINERAL', name: 'Mineraalit' },
            { id: 'CARBOCMP', name: 'Hiilihydraatit' },
            { id: 'FAT', name: 'Rasvat' }],

        nutrientsLoaded: false,
        nutrients: [],
        nutrientsGrouped: {},
        nutritionGoalsLoaded: false,
        nutritionGoals: [],

        mealDefinitionsLoaded: false,
        mealDefinitions: [],

        latestFoods: [],
        mostUsedFoods: []
    },
    actions: {

        // Meals
        [constants.SELECT_MEAL_DATE_RANGE]({ commit, state }, { start, end, success, failure }) {
            commit(constants.SELECT_MEAL_DATE_RANGE_SUCCESS, { start, end });
            if (success) {
                success();
            }
        },
        [constants.FETCH_MEALS]({ commit, state }, { start, end, success, failure }) {
            if (state.mealsStart && state.mealsEnd) {
                if (moment(start).isBefore(state.mealsStart) || moment(end).isAfter(state.mealsEnd)) {
                    start = moment.min(moment(start), moment(state.mealsEnd));
                    end = moment.max(moment(end), moment(state.mealsStart));
                }
                else {
                    // within already loaded period
                    if (success) {
                        success();
                    }
                    return;
                }
            }

            api.listMeals(start, end).then(function (meals) {
                commit(constants.FETCH_MEALS_SUCCESS, { start, end, meals })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_MEAL]({ commit, state }, { id, success, failure }) {
            api.getMeal(id).then(function (meal) {
                meal.time = new Date(meal.time);
                commit(constants.FETCH_MEAL_SUCCESS, { meal });
                if (success) {
                    success(meal);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_MEAL]({ commit, state }, { meal, success, failure }) {
            api.saveMeal(meal).then(function (savedMeal) {
                savedMeal.time = new Date(savedMeal.time);
                commit(constants.SAVE_MEAL_SUCCESS, { id: meal.id, meal: savedMeal })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.DELETE_MEAL]({ commit, state }, { meal, success, failure }) {
            api.deleteMeal(meal.id).then(function () {
                commit(constants.DELETE_MEAL_SUCCESS, { meal })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.RESTORE_MEAL]({ commit, state }, { id, success, failure }) {
            api.restoreMeal(id).then(function (meal) {
                commit(constants.RESTORE_MEAL_SUCCESS, { meal })
                if (success) {
                    success(meal);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_MEAL_ROW]({ commit, state }, { row, success, failure }) {
            api.saveMealRow(row).then(function (savedRow) {
                var meal = state.meals.find(m => m.id == savedRow.mealId);
                if (meal) {
                    commit(constants.SAVE_MEAL_ROW_SUCCESS, { row: savedRow });
                }
                else {
                    api.getMeal(savedRow.mealId).then(function (createdMeal) {
                        createdMeal.time = new Date(createdMeal.time);
                        commit(constants.FETCH_MEAL_SUCCESS, { meal: createdMeal });

                    });
                }

            }).fail(function () {
                if (failure) {
                    failure();
                }
            });

        },
        // Foods
        [constants.FETCH_MY_FOODS]({ commit, state }, { success, failure }) {
            api.listFoods().then(function (foods) {
                if (success) {
                    success(foods);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_FOODS]({ commit, state }, { ids, success, failure }) {
            var apiCalls = [];
            for (var i in ids) {
                if (ids[i]) {
                    apiCalls.push(api.getFood(ids[i]));
                }
            }
            if (apiCalls.length == 0) {
                if (success) {
                    success([]);
                }
                return;
            }
            Promise.all(apiCalls).then(function (foods) {
                if (success) {
                    success(foods);
                }
            }).catch(function (reason) {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_LATEST_FOODS]({ commit, state }, { id, success, failure }) {
            api.getLatestFoods().then(function (foods) {
                commit(constants.FETCH_LATEST_FOODS_SUCCESS, { foods });
                if (success) {
                    success(foods);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_MOST_USED_FOODS]({ commit, state }, { id, success, failure }) {
            api.getMostUsedFoods().then(function (foods) {
                commit(constants.FETCH_MOST_USED_FOODS_SUCCESS, { foods });
                if (success) {
                    success(foods);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_FOOD]({ commit, state }, { id, success, failure }) {
            api.getFood(id).then(function (food) {
                if (success) {
                    success(food);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_FOOD]({ commit, state }, { food, success, failure }) {
            api.saveFood(food).then(function (savedFood) {
                if (success) {
                    success(savedFood);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.DELETE_FOOD]({ commit, state }, { food, success, failure }) {
            api.deleteFood(food.id).then(function () {
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        // Recipes
        [constants.FETCH_RECIPES]({ commit, state }, { success, failure }) {
            api.listRecipes().then(function (recipes) {
                if (success) {
                    success(recipes);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_RECIPE]({ commit, state }, { id, success, failure }) {
            api.getRecipe(id).then(function (recipe) {
                if (success) {
                    success(recipe);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_RECIPE]({ commit, state }, { recipe, success, failure }) {
            api.saveRecipe(recipe).then(function (savedRecipe) {
                if (success) {
                    success(savedRecipe);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.DELETE_RECIPE]({ commit, state }, { recipe, success, failure }) {
            api.deleteRecipe(recipe.id).then(function () {
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        // Nutrients
        [constants.FETCH_NUTRIENTS]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.nutrientsLoaded && !forceRefresh) {
                if (success) {
                    success(state.nutrients);
                }
                return;
            }
            api.listNutrients().then(function (nutrients) {
                commit(constants.FETCH_NUTRIENTS_SUCCESS, { nutrients })
                if (success) {
                    success(nutrients);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_NUTRIENT_SETTINGS]({ commit, state }, { settings, success, failure }) {
            api.saveNutrientSettings(settings).then(function (nutrients) {
                commit(constants.FETCH_NUTRIENTS_SUCCESS, { nutrients })
                if (success) {
                    success(nutrients);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        // Nutrition goals
        [constants.FETCH_NUTRITION_GOALS]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.nutritionGoalsLoaded && !forceRefresh) {
                if (success) {
                    success(state.nutritionGoals);
                }
                return;
            }
            api.getNutritionGoals().then(function (goals) {
                commit(constants.FETCH_NUTRITION_GOALS_SUCCESS, { goals })
                if (success) {
                    success(goals);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_NUTRITION_GOALS]({ commit, state }, { goals, success, failure }) {
            api.saveNutritionGoals(goals).then(function (savedGoals) {
                commit(constants.FETCH_NUTRITION_GOALS_SUCCESS, { goals: savedGoals })
                if (success) {
                    success(savedGoals);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },

        // Meal rhythm
        [constants.FETCH_MEAL_DEFINITIONS]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.mealDefinitionsLoaded && !forceRefresh) {
                if (success) {
                    success(state.mealDefinitions);
                }
                return;
            }
            api.getMealDefinitions().then(function (definitions) {
                commit(constants.FETCH_MEAL_DEFINITIONS_SUCCESS, { definitions })
                if (success) {
                    success(definitions);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_MEAL_DEFINITIONS]({ commit, state }, { definitions, success, failure }) {
            api.saveMealDefinitions(definitions).then(function (savedDefinitions) {
                commit(constants.SAVE_MEAL_DEFINITIONS_SUCCESS, { definitions: savedDefinitions })
                if (success) {
                    success(savedDefinitions);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        }
    },
    mutations: {
        [constants.SELECT_MEAL_DATE_RANGE_SUCCESS](state, { start, end }) {
            state.mealsDisplayStart = start;
            state.mealsDisplayEnd = end;
        },
        [constants.FETCH_NUTRIENTS_SUCCESS](state, { nutrients }) {
            state.nutrients = nutrients;
            var grouped = {};
            for (var i in nutrients) {
                var nutrient = nutrients[i];
                if (grouped[nutrient.fineliGroup]) {
                    grouped[nutrient.fineliGroup].push(nutrient);
                }
                else {
                    grouped[nutrient.fineliGroup] = [nutrient];
                }

            }
            state.nutrientsGrouped = grouped;
            state.nutrientsLoaded = true;
        },

        [constants.FETCH_NUTRITION_GOALS_SUCCESS](state, { goals }) {
            state.nutritionGoals = goals;
            state.nutritionGoalsLoaded = true;
        },
        [constants.FETCH_MEALS_STARTED](state) {
            state.mealsLoading = true;
        },
        [constants.FETCH_MEALS_SUCCESS](state, { start, end, meals }) {
            state.mealsLoading = false;
            var days = state.mealDays;
            for (var i in meals) {
                var meal = meals[i];
                meal.time = new Date(meal.time);
                meal.definition = state.mealDefinitions.find(d => d.id == meal.definitionId);
                if (!state.mealsStart || !state.mealsEnd || moment(meal.time).isBefore(state.mealsStart) || moment(meal.time).isAfter(state.mealsEnd)) {
                    addMeal(meal, state);
                }
            }
            if (!state.mealsStart || moment(start).isBefore(state.mealsStart)) {
                state.mealsStart = start;
            }
            if (!state.mealsEnd || moment(end).isAfter(state.mealsEnd)) {
                state.mealsEnd = end;
            }
            state.mealDays = days.sort(function (a, b) {
                return a.date.getTime() < b.date.getTime() ? 1 : -1;
            });
        },
        [constants.FETCH_MEALS_FAILURE](state) {
            state.mealsLoading = false;
        },
        [constants.FETCH_MEAL_SUCCESS](state, { meal }) {
            addMeal(meal, state);
        },
        [constants.SAVE_MEAL_STARTED](state) {

        },
        [constants.SAVE_MEAL_SUCCESS](state, { id, meal }) {
            if (id) {
                var old = state.meals.find(m => m.id == id);
                if (old) {
                    removeMeal(old, state)
                }
            }
            addMeal(meal, state);
        },
        [constants.SAVE_MEAL_FAILURE](state) {

        },
        [constants.DELETE_MEAL_STARTED](state) {

        },
        [constants.DELETE_MEAL_SUCCESS](state, { meal }) {
            removeMeal(meal, state)
        },
        [constants.RESTORE_MEAL_SUCCESS](state, { meal }) {
            meal.time = new Date(meal.time);
            if (state.mealsStart && state.mealsEnd && moment(meal.time).isAfter(state.mealsStart) && moment(meal.time).isBefore(state.mealsEnd)) {
                addMeal(meal, state);
            }
        },
        [constants.DELETE_MEAL_FAILURE](state) {

        },
        [constants.LOGOUT_SUCCESS](state) {
            // TODO: clear state
        },
        [constants.FETCH_MEAL_DEFINITIONS_SUCCESS](state, { definitions }) {
            state.mealDefinitions = definitions.sort((a, b) => (a.startHour || 99) - (b.startHour || 99));
            state.mealDefinitionsLoaded = true;
        },
        [constants.SAVE_MEAL_DEFINITIONS_SUCCESS](state, { definitions }) {
            state.mealDefinitions = definitions;
        },
        [constants.SAVE_MEAL_ROW_SUCCESS](state, { row }) {

            updateMealRow(row, state);
        },
        [constants.SAVE_HOME_NUTRIENTS_SUCCESS](state, { nutrients }) {
            state.nutrients.forEach(n => {
                var index = nutrients.findIndex(id => n.id == id);
                if (index >= 0) {
                    n.homeOrder = index;
                }
                else {
                    n.HomeOrder = undefined;
                }
            });
        },
        [constants.FETCH_LATEST_FOODS_SUCCESS](state, { foods }) {
            state.latestFoods = foods;
        },
        [constants.FETCH_MOST_USED_FOODS_SUCCESS](state, { foods }) {
            state.mostUsedFoods = foods;
        }
    }
}

    function removeMeal(meal, state){
        var date = moment(meal.time).startOf('day');
        var day = state.mealDays.find(d => moment(d.date).isSame(date, 'day'));
        if(day){
            for (var nutrientId in meal.nutrients) {
                if (day.nutrients[nutrientId]) {
                    day.nutrients[nutrientId] -= meal.nutrients[nutrientId];
                }
            }
            day.meals.splice(day.meals.findIndex(m => m.id == meal.id), 1);
            if(day.meals.length == 0){
                state.mealDays.splice(state.mealDays.findIndex(d => d == day), 1);
            }
        }
        state.meals.splice(state.meals.findIndex(m => m.id == meal.id), 1);
    }
    function addMeal(meal, state){
        var date = moment(meal.time).startOf('day');
        var day = state.mealDays.find(d => moment(d.date).isSame(date, 'day'));
        if (!day) {
            day = { date: date.toDate(), meals: [], nutrients: {}};
            state.mealDays.push(day);
            state.mealDays.sort(function (a, b) {
                if (a.date.getTime() < b.date.getTime())
                    return 1;
                if (a.date.getTime() > b.date.getTime())
                    return -1;
                return 0;
            });
        }
        day.meals.push(meal);
        day.meals.sort(function (a, b) {
            if (a.time.getTime() < b.time.getTime())
                return 1;
            if (a.time.getTime() > b.time.getTime())
                return -1;
            return 0;
        });
        for (var nutrientId in meal.nutrients) {
            if (!day.nutrients[nutrientId]) {
                day.nutrients[nutrientId] = 0;
            }
            day.nutrients[nutrientId] += meal.nutrients[nutrientId];
        }
        state.meals.push(meal);
    }
    function updateMealRow(row, state) {
        var meal = state.meals.find(m => m.id == row.mealId);
        var rowIndex = meal.rows.findIndex(r => r.id == row.id);
        if (rowIndex >= 0) {
            var oldRow = meal.rows[rowIndex];
            for (var i in oldRow.nutrients) {
                meal.nutrients[i] -= oldRow.nutrients[i];
            }
            meal.rows.splice(rowIndex, 1, row);
        }
        else {            
            meal.rows.push(row);
        }
        for (var i in row.nutrients) {
            meal.nutrients[i] += row.nutrients[i];
        }
    }