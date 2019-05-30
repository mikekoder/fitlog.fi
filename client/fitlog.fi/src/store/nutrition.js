import api from '../api'
import constants from './constants'
import moment from 'moment'
import utils from '../utils';

export default {
    state: {

        diaryDate: new Date(),

        mealsStart: undefined,
        mealsEnd: undefined,
        mealDays: [],
        meals: [],
        mealsDisplayStart: undefined,
        mealsDisplayEnd: undefined,
        mealDraft: undefined,
        rowDraft: undefined,

        nutrientGroups: [
            { id: 'MACROCMP', name: 'Makrot' },
            { id: 'VITAM', name: 'Vitamiinit' },
            { id: 'MINERAL', name: 'Mineraalit' },
            { id: 'CARBOCMP', name: 'Hiilihydraatit' },
            { id: 'FAT', name: 'Rasvat' }],

        nutrientsLoaded: false,
        nutrients: [],
        nutrientsGrouped: {},
        activeNutritionGoalLoaded: false,
        activeNutritionGoal: {},
        nutritionGoalsLoaded: false,
        nutritionGoals: {},

        mealDefinitionsLoaded: false,
        mealDefinitions: [],

        latestFoods: [],
        mostUsedFoods: [],
        ownFoods: []
    },
    actions: {
        [constants.SELECT_MEAL_DIARY_DATE]({ commit, state }, { date }) {
            commit(constants.SELECT_MEAL_DIARY_DATE_SUCCESS, { date });
        },
        [constants.SAVE_MEAL_DIARY_SETTINGS]({ commit, state }, { settings}) {
            return api.saveHomeSettings(settings).then(response => {
                commit(constants.SAVE_MEAL_DIARY_NUTRIENTS_SUCCESS, { nutrients: settings.nutrients })
                return settings;
            });
        },
        // Meals
        [constants.SELECT_MEAL_DATE_RANGE]({ commit, state }, { start, end }) {
            commit(constants.SELECT_MEAL_DATE_RANGE_SUCCESS, { start, end });
            return Promise.resolve();
        },
        [constants.FETCH_MEALS]({ commit, state }, { start, end, force }) {
            if (state.mealsStart && state.mealsEnd) {
                if (moment(start).isBefore(state.mealsStart) || moment(end).isAfter(state.mealsEnd) || force) {
                    start = moment.min(moment(start), moment(state.mealsEnd));
                    end = moment.max(moment(end), moment(state.mealsStart));
                }
                else {
                    return Promise.resolve();
                }
            }

            return api.listMeals(start, end).then(response => {
                commit(constants.FETCH_MEALS_SUCCESS, { start, end, meals: response.data })
                return response.data;
            });
        },
        [constants.FETCH_MEAL]({ commit, state }, { id }) {
            return api.getMeal(id).then(response => {
                var meal = response.data;
                meal.time = new Date(meal.time);
                commit(constants.FETCH_MEAL_SUCCESS, { meal });
                return meal;
            });
        },
        [constants.SAVE_MEAL]({ commit, state }, { meal }) {
            return api.saveMeal(meal).then(response => {
                var savedMeal = response.data;
                savedMeal.time = new Date(savedMeal.time);
                commit(constants.SAVE_MEAL_SUCCESS, { id: meal.id, meal: savedMeal })
                return savedMeal;
            });
        },
        [constants.SAVE_MEAL_DRAFT]({ commit, state }, { meal }) {
            state.mealDraft = meal;
        },
        [constants.DELETE_MEAL]({ commit, state }, { meal}) {
            return api.deleteMeal(meal.id).then(response => {
                commit(constants.DELETE_MEAL_SUCCESS, { meal });
            });
        },
        [constants.RESTORE_MEAL]({ commit, state }, { id }) {
            return api.restoreMeal(id).then(response => {
                commit(constants.RESTORE_MEAL_SUCCESS, { meal: response.data })
                return response.data;
            })
        },
        [constants.SAVE_MEAL_ROW]({ commit, state }, { row }) {
            return api.saveMealRow(row).then(response => {
                var savedRow = response.data;
                var meal = state.meals.find(m => m.id == savedRow.mealId);
                if (meal) {
                    commit(constants.SAVE_MEAL_ROW_SUCCESS, { row: savedRow });
                    return savedRow;
                }
                else {
                    api.getMeal(savedRow.mealId).then(response2 => {
                        var createdMeal = response2.data;
                        createdMeal.time = new Date(createdMeal.time);
                        commit(constants.FETCH_MEAL_SUCCESS, { meal: createdMeal });
                        return createdMeal;
                    });
                }
            });
        },
        [constants.DELETE_MEAL_ROW]({ commit, state }, { row }) {
            return api.deleteMealRow(row).then(response => {
                commit(constants.DELETE_MEAL_ROW_SUCCESS, { row });
            });
        },
        [constants.SAVE_MEAL_ROW_DRAFT]({ commit, state }, { row }) {
            state.rowDraft = row;
        },
        // Foods
        [constants.FETCH_MY_FOODS]({ commit, state }) {
            return api.listFoods().then(response => {
                commit(constants.FETCH_MY_FOODS_SUCCESS, { foods: response.data });
                return response.data;
            });
        },
        [constants.FETCH_FOODS]({ commit, state }, { ids }) {
            var apiCalls = [];
            for (var i in ids) {
                if (ids[i]) {
                    apiCalls.push(api.getFood(ids[i]));
                }
            }
            if (apiCalls.length == 0) {
                return;
            }
            return Promise.all(apiCalls).then(response => {
                return response.map(r => r.data);
            });
        },
        [constants.FETCH_LATEST_FOODS]({ commit, state }) {
            return api.getLatestFoods().then(response => {
                commit(constants.FETCH_LATEST_FOODS_SUCCESS, { foods: response.data });
                return response.data;
            });
        },
        [constants.FETCH_MOST_USED_FOODS]({ commit, state }) {
            return api.getMostUsedFoods().then(response => {
                commit(constants.FETCH_MOST_USED_FOODS_SUCCESS, { foods: response.data });
                return response.data;
            });
        },
        [constants.FETCH_FOOD]({ commit, state }, { id }) {
            return api.getFood(id).then(response => {
                return response.data;
            });
        },
        [constants.SAVE_FOOD]({ commit, state }, { food }) {
            return api.saveFood(food).then(response => {
                return response.data;
            });
        },
        [constants.DELETE_FOOD]({ commit, state }, { food }) {
            return api.deleteFood(food.id).then(response => {
                
            });
        },
        // Recipes
        [constants.FETCH_RECIPES]({ commit, state }) {
            return api.listRecipes().then(response => {
                return response.data;
            });
        },
        [constants.FETCH_RECIPE]({ commit, state }, { id }) {
            return api.getRecipe(id).then(response => {
                return response.data;
            });
        },
        [constants.SAVE_RECIPE]({ commit, state }, { recipe}) {
            return api.saveRecipe(recipe).then(response => {
                return response.data;
            });
        },
        [constants.DELETE_RECIPE]({ commit, state }, { recipe }) {
            return api.deleteRecipe(recipe.id).then(response => {

            });
        },
        // Nutrients
        [constants.FETCH_NUTRIENTS]({ commit, state }, { forceRefresh }) {
            if (state.nutrientsLoaded && !forceRefresh) {
                return Promise.resolve(state.nutrients);
            }

            return Promise.all([api.listNutrients(), api.getNutrientSettings()]).then(responses => {
                var nutrients = responses[0].data;
                var settings = responses[1].data;

                nutrients = applySettingsToNutrients(settings, nutrients);

                commit(constants.FETCH_NUTRIENTS_SUCCESS, { nutrients });
                return nutrients;
            });
        },
        [constants.SAVE_NUTRIENT_SETTINGS]({ commit, state }, { settings }) {
            return api.saveNutrientSettings(settings).then(response => {
                var savedSettings = response.data;
                var nutrients = applySettingsToNutrients(savedSettings, state.nutrients);

                commit(constants.FETCH_NUTRIENTS_SUCCESS, { nutrients })
                return nutrients;
            });
        },
        // Nutrition goals
        [constants.FETCH_NUTRITION_GOALS]({ commit, state }, { forceRefresh }) {
            if (state.nutritionGoalsLoaded && !forceRefresh) {
                return Promise.resolve(state.nutritionGoals);
            }
            return api.getNutritionGoals().then(response => {
                commit(constants.FETCH_NUTRITION_GOALS_SUCCESS, { goals: response.data })
                return response.data;
            });
        },
        [constants.FETCH_NUTRITION_GOAL]({ commit, state }, { id }) {
            return api.getNutritionGoal(id).then(response => {
                return response.data;
            });
        },
        [constants.FETCH_ACTIVE_NUTRITION_GOAL]({ commit, state }, { forceRefresh }) {
            if (state.nutritionGoalLoaded && !forceRefresh) {
                return Promise.resolve(state.nutritionGoal);
            }
            return api.getActiveNutritionGoal().then(response => {
                commit(constants.FETCH_ACTIVE_NUTRITION_GOAL_SUCCESS, { goal: response.data })
                return response.data;
            });
        },
        [constants.SAVE_NUTRITION_GOAL]({ commit, state }, { goal }) {
            return api.saveNutritionGoal(goal).then(response => {
                commit(constants.FETCH_NUTRITION_GOAL_SUCCESS, { goal: response.data })
                return response.data;
            });
        },
        [constants.ACTIVATE_NUTRITION_GOAL]({ commit, state }, { goal }) {
            return api.activateNutritionGoal(goal.id).then(response => {
                commit(constants.ACTIVATE_NUTRITION_GOAL_SUCCESS, { goal })
                return goal;
            });
        },
        [constants.DELETE_NUTRITION_GOAL]({ commit, state }, { goal }) {
            return api.deleteNutritionGoal(goal.id).then(response => {
                commit(constants.DELETE_NUTRITION_GOAL_SUCCESS, { goal })
                return goal;
            });
        },
        // Meal rhythm
        [constants.FETCH_MEAL_DEFINITIONS]({ commit, state }, { forceRefresh }) {
            if (state.mealDefinitionsLoaded && !forceRefresh) {
                return Promise.resolve(state.mealDefinitions);
            }
            return api.getMealDefinitions().then(response => {
                commit(constants.FETCH_MEAL_DEFINITIONS_SUCCESS, { definitions: response.data })
                return response.data;
            });
        },
        [constants.SAVE_MEAL_DEFINITIONS]({ commit, state }, { definitions }) {
            return api.saveMealDefinitions(definitions).then(response => {
                commit(constants.SAVE_MEAL_DEFINITIONS_SUCCESS, { definitions: response.data })
                return response.data;
            });
        }
    },
    mutations: {
        [constants.NUTRITION_CLEAR](state) {
            state.diaryDate = new Date();
            state.mealsStart = undefined;
            state.mealsEnd = undefined;
            state.meals = [];
            state.mealsDisplayStart = undefined;
            state.mealsDisplayEnd = undefined;
            state.mealDraft = undefined;
            state.rowDraft = undefined;
            state.nutrientsLoaded = false;
            state.nutrients = [];
            state.nutrientsGrouped = {};
            state.activeNutritionGoalLoaded = false;
            state.activateNutritionGoal = undefined;
            state.nutritionGoalsLoaded = false;
            state.nutritionGoals = [];
            state.mealDefinitionsLoaded = false;
            state.mealDefinitions = [];
            state.latestFoods = [];
            state.mostUsedFoods = [];
            state.ownFoods = [];
        },
        [constants.SELECT_MEAL_DIARY_DATE_SUCCESS](state, { date }) {
            state.diaryDate = date;
        },
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
        [constants.FETCH_NUTRITION_GOAL_SUCCESS](state, { goal }) {
            var old = state.nutritionGoals.find(g => g.id == goal.id);
            if (old) {
                 state.nutritionGoals.splice(state.nutritionGoals.findIndex(g => g.id == old.id), 1);
            }
            state.nutritionGoals.push(goal);
        },
        [constants.FETCH_ACTIVE_NUTRITION_GOAL_SUCCESS](state, { goal }) {
            state.activeNutritionGoal = goal;
            state.activeNutritionGoalLoaded = true;
        },
        [constants.ACTIVATE_NUTRITION_GOAL_SUCCESS](state, { goal }) {
            state.nutritionGoals.forEach(g => {
                if (g.id === goal.id) {
                    g.active = true;
                    state.activeNutritionGoal = g;
                    state.activeNutritionGoalLoaded = true;
                }
                else {
                    g.active = false;
                }
            });
        },
        [constants.DELETE_NUTRITION_GOAL_SUCCESS](state, { goal }) {
            deleteNutritionGoal(goal, state)
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
                else{
                    var old = state.meals.find(m => m.id == meal.id);
                    if (old) {
                        deleteMeal(old, state)
                    }
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
            var old = state.meals.find(m => m.id == meal.id);
            if (old) {
                deleteMeal(old, state)
            }
            addMeal(meal, state);
        },
        [constants.SAVE_MEAL_STARTED](state) {

        },
        [constants.SAVE_MEAL_SUCCESS](state, { id, meal }) {
            if (id) {
                var old = state.meals.find(m => m.id == id);
                if (old) {
                    deleteMeal(old, state)
                }
            }
            addMeal(meal, state);
        },
        [constants.SAVE_MEAL_FAILURE](state) {

        },
        [constants.DELETE_MEAL_STARTED](state) {

        },
        [constants.DELETE_MEAL_SUCCESS](state, { meal }) {
            deleteMeal(meal, state)
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
        [constants.DELETE_MEAL_ROW_SUCCESS](state, { row }) {
            deleteMealRow(row, state);
        },
        [constants.SAVE_MEAL_DIARY_NUTRIENTS_SUCCESS](state, { nutrients }) {
            state.nutrients = state.nutrients.map(n => {
                var index = nutrients.findIndex(id => n.id == id);

                return {...n, homeOrder: index >= 0 ? index : undefined};
            });
        },
        [constants.FETCH_LATEST_FOODS_SUCCESS](state, { foods }) {
            state.latestFoods = foods;
        },
        [constants.FETCH_MOST_USED_FOODS_SUCCESS](state, { foods }) {
            state.mostUsedFoods = foods;
        },
        [constants.FETCH_MY_FOODS_SUCCESS](state, { foods }) {
            state.ownFoods = foods;
        }
    }
}

    function applySettingsToNutrients(settings, nutrients) {
        settings.forEach(s => {
            var nutrient = nutrients.find(n => n.id == s.nutrientId);
            if (nutrient) {
                if (s.hideSummary != null) {
                    nutrient.hideSummary = s.hideSummary;
                    nutrient.userHideSummary = s.hideSummary;
                }
                if (s.hideDetails != null) {
                    nutrient.hideDetails = s.hideDetails;
                    nutrient.userHideDetails = s.hideDetails;
                }
                if (s.order != null) {
                    nutrient.order = s.order;
                    nutrient.userOrder = s.order;
                }
                if (s.homeOrder != null) {
                    nutrient.homeOrder = s.homeOrder;
                }
            }
        });
        return nutrients;
    }
    function findDay(meal, state) {
        var date = moment(meal.time).startOf('day');
        return state.mealDays.find(d => moment(d.date).isSame(date, 'day'));
    }
    function deleteMeal(meal, state){
        var day = findDay(meal, state);
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
            calculateEnergyDistribution(day);
        }
        state.meals.splice(state.meals.findIndex(m => m.id == meal.id), 1);
    }
    function addMeal(meal, state) {
        var date = moment(meal.time).startOf('day');
        var day = findDay(meal, state);
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
        calculateEnergyDistribution(day);
    }
    function calculateEnergyDistribution(item) {
        if (!item || !item.nutrients) {
            return;
        }
        utils.calculateEnergyDistribution(item.nutrients);
    }
    function updateMealRow(row, state) {
        var meal = state.meals.find(m => m.id == row.mealId);
        var day = findDay(meal, state);
        var rowIndex = meal.rows.findIndex(r => r.id == row.id);
        if (rowIndex >= 0) {
            var oldRow = meal.rows[rowIndex];
            for (var i in oldRow.nutrients) {
                meal.nutrients[i] -= oldRow.nutrients[i];
                if (day) {
                    day.nutrients[i] -= oldRow.nutrients[i];
                }
            }
            meal.rows.splice(rowIndex, 1, row);
        }
        else {            
            meal.rows.push(row);
        }
        for (var i in row.nutrients) {
            if (!meal.nutrients[i]) {
                meal.nutrients[i] = 0;
            }
            meal.nutrients[i] += row.nutrients[i];
            if (day) {
                day.nutrients[i] += row.nutrients[i];
            }
        }

        calculateEnergyDistribution(meal);
        calculateEnergyDistribution(day);
    }
    function deleteMealRow(row, state) {
        var meal = state.meals.find(m => m.id == row.mealId);
        var day = findDay(meal, state);
        var rowIndex = meal.rows.findIndex(r => r.id == row.id);
        if(rowIndex < 0){
            return;
        }
        if(meal.rows.length == 1){
            deleteMeal(meal, state);
            return;
        }
        var oldRow = meal.rows[rowIndex];
        for (var i in oldRow.nutrients) {
            meal.nutrients[i] -= oldRow.nutrients[i];
            day.nutrients[i] -= oldRow.nutrients[i];
        }
        
        meal.rows.splice(rowIndex, 1);

        calculateEnergyDistribution(meal);
        calculateEnergyDistribution(day);
    }
    function deleteNutritionGoal(goal, state) {
        state.nutritionGoals.splice(state.nutritionGoals.findIndex(x => x.id == goal.id), 1);
    }