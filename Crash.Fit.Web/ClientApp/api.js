let $ = require('jquery')
$.ajaxSetup({ headers: { 'X-CSRF-TOKEN': $('meta[name="X-CSRF-TOKEN"]').attr('content') } });
const baseUrl = '/api/'

const api = {
    baseUrl: baseUrl,

    // Users
    register(registration) {
        return $.ajax({
            url: baseUrl + 'users/register',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(registration)
        });
    },
    login(login) {
        return $.ajax({
            url: baseUrl + 'users/login',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(login)
        });
    },
    getUser(){
        return $.get(baseUrl + 'users/me/');
    },
    logout(){
        return $.post(baseUrl+'users/logout');
    },

    // Meals
    listMeals(start, end) {
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
    saveMeal: function (meal) {
        var url = baseUrl + 'meals/';
        var method = 'POST';
        if (meal.id) {
            url += meal.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(meal)
        });
    },
    deleteMeal: function(id){

    },

    // foods
    searchFoods: function(name){
        return $.get(baseUrl + 'foods/search', { name });
    },
    listFoods: function(){
        return $.get(baseUrl + 'foods/');
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
        return $.get(baseUrl + 'nutrients/');
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