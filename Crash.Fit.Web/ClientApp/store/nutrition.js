var api = require('../api')
var constants = require('./constants')
var moment = require('moment')

const state = {

    mealsStart: null,
    mealsEnd: null,
    mealDays:[],
    meals: [],

    nutrientGroups: [
        {id:'MACROCMP',name:'Makrot'}, 
        {id:'VITAM', name:'Vitamiinit'}, 
        {id:'MINERAL',name:'Mineraalit'}, 
        {id:'CARBOCMP', name:'Hiilihydraatit'}, 
        {id:'FAT',name:'Rasvat'}],

    nutrientsLoaded: false,
    nutrients: [],
    nutrientsGrouped: {},
    nutrientTargetsLoaded: false,
    nutrientTargets: {}
}

// actions
const actions = {
    [constants.FETCH_NUTRIENTS] ({commit, state},{forceRefresh, success, failure}) {
        if(state.nutrientsLoaded && !forceRefresh){
            if(success){
                success(state.nutrients);
            }
            return;
        }
        api.listNutrients().then(function(nutrients){
            commit(constants.FETCH_NUTRIENTS_SUCCESS,{nutrients})
            if(success){
                success(nutrients);
            }
        }).fail(function(){
            if(failure){
                failure();
            }
        });
    },
    [constants.FETCH_NUTRIENT_TARGETS] ({commit, state},{forceRefresh, success, failure}) {
        if(state.nutrientTargetsLoaded && !forceRefresh){
            if(success){
                success(state.nutrientTargets);
            }
            return;
        }
        api.getNutrientTargets().then(function(targets){
            commit(constants.FETCH_NUTRIENT_TARGETS_SUCCESS,{targets})
            if(success){
                success(targets);
            }
        }).fail(function(){
            if(failure){
                failure();
            }
        });
    },
    [constants.SAVE_NUTRIENT_TARGETS] ({commit, state},{targets, success, failure}){
        api.saveNutrientTargets(targets).then(function(savedTargets){
            commit(constants.FETCH_NUTRIENT_TARGETS_SUCCESS,{savedTargets})
            if(success){
                success(savedTargets);
            }
        }).fail(function(){
            if(failure){
                failure();
            }
        });
    },
    [constants.SAVE_NUTRIENT_SETTINGS] ({commit, state},{settings, success, failure}){
        api.saveNutrientSettings(settings).then(function(nutrients){
            commit(constants.FETCH_NUTRIENTS_SUCCESS,{nutrients})
            if(success){
                success(nutrients);
            }
        }).fail(function(){
            if(failure){
                failure();
            }
        });
    },
    [constants.FETCH_MEALS] ({ commit, state }, {start, end, success, failure}) {
        if(state.mealsStart && state.mealsEnd){
            if(moment(start).isBefore(state.mealsStart) || moment(end).isAfter(state.mealsEnd)){
                start = moment.min(moment(start), moment(state.mealsEnd));
                end = moment.max(moment(end), moment(state.mealsStart));
            }
            else{
                // within already loaded period
                return;
            }
        }

        api.listMeals(start, end).then(function(meals){
            commit(constants.FETCH_MEALS_SUCCESS,{start, end, meals})
            if(success){
                success();
            }
        }).fail(function(){
            if(failure){
                failure();
            }
        });
    },
    [constants.SAVE_MEAL] ({commit, state},{meal, success, failure}){
        api.saveMeal(meal).then(function(savedMeal){
            commit(constants.SAVE_MEAL_SUCCESS,{id: meal.id, meal: savedMeal})
            if(success){
                success();
            }
        }).fail(function(){
            if(failure){
                failure();
            }
        });
    },
    [constants.DELETE_MEAL] ({commit, state},{meal, success, failure}){
        api.deleteMeal(meal.id).then(function(){
            commit(constants.DELETE_MEAL_SUCCESS,{meal})
            if(success){
                success();
            }
        }).fail(function(){
            if(failure){
                failure();
            }
        });
    }
}

// mutations
const mutations = {
    [constants.FETCH_NUTRIENTS_STARTED] (state) {
        state.nutrientsLoading = true;
    },
    [constants.FETCH_NUTRIENTS_SUCCESS] (state, {nutrients}) {
        state.nutrientsLoading = false;
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
    },
    [constants.FETCH_NUTRIENTS_FAILURE] (state) {
        state.nutrientsLoading = false;
    },
    [constants.FETCH_NUTRIENT_TARGETS_SUCCESS] (state, {targets}) {
        state.nutrientTargets = targets;
    },
    [constants.FETCH_MEALS_STARTED] (state) {
        state.mealsLoading = true;
    },
    [constants.FETCH_MEALS_SUCCESS] (state, {start, end, meals}) {
        state.mealsLoading = false;
        var days = state.mealDays;
        for(var i in meals){
            var meal = meals[i];
            meal.time = new Date(meal.time);
            if(!state.mealsStart || !state.mealsEnd || moment(meal.time).isBefore(state.mealsStart) || moment(meal.time).isAfter(state.mealsEnd)){
                addMeal(meal, state);
            }
        }
        if(!state.mealsStart || moment(start).isBefore(state.mealsStart)){
            state.mealsStart = start;
        }
        if(!state.mealsEnd || moment(end).isAfter(state.mealsEnd)){
            state.mealsEnd = end;
        }
        state.mealDays = days.sort(function (a, b) {
            return a.date.getTime() < b.date.getTime() ? 1 : -1;
        });
    },
    [constants.FETCH_MEALS_FAILURE] (state) {
        state.mealsLoading = false;
    },
    [constants.SAVE_MEAL_STARTED] (state) {
        
    },
    [constants.SAVE_MEAL_SUCCESS] (state, {id, meal}) {
        if(id){
            var old = state.meals.find(m => m.id == id);
            if(old){
                removeMeal(old, state)
            }
        }
        addMeal(meal, state);
    },
    [constants.SAVE_MEAL_FAILURE] (state) {
        
    },
    [constants.DELETE_MEAL_STARTED] (state) {
        
    },
    [constants.DELETE_MEAL_SUCCESS] (state, {meal}) {
        removeMeal(meal, state)
    },
    [constants.DELETE_MEAL_FAILURE] (state) {
        
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

module.exports = { state, actions, mutations }