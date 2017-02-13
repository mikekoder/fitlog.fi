let $ = require('jquery')
const baseUrl = '/api/'

const api = {
    //baseUrl: '/api/',

    // Meals
    listMeals: function (start, end) {
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return $.get(baseUrl + 'meals', query);
    },
    getMeal: function(id){
        return $.get(baseUrl + 'meals/' + id);
    },
    saveMeal: function(meal){

    },
    deleteMeal: function(id){

    },

    // foods
    searchFoods: function(name){

    },
    listFoods: function(){

    },
    getFood: function(id){
        return $.get(baseUrl + 'foods/' + id);
    },
    saveFood: function(food){

    },
    deleteFood: function(id){

    },

    // Nutrients
    listNutrients: function(){

    },
    listDailyIntakes: function(gender, dob){

    },

    // Workouts
    listWorkous: function(start, end){

    },
    getWorkout: function(id){
        return $.get(baseUrl + 'workouts/' + id);
    },
    saveWorkout: function(workout){

    },
    deleteWorkout: function(id){

    },

    // Exercises
    listExercises: function(){

    },
    getExercise: function(id){
        return $.get(baseUrl + 'exercises/' + id);
    },
    saveExercise: function(exercise){

    },
    deleteExercise: function(id){

    },

    // Routines
    listRoutines: function(){

    },
    getRoutine: function(id){
        return $.get(baseUrl + 'routines/' + id);
    },
    saveRoutine: function(routine){

    },
    deleteRoutine: function(id){

    }
};
module.exports = api