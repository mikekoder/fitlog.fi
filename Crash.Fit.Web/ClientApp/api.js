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
        return $.ajax({
            url: baseUrl + 'meals/' + id,
            type: 'DELETE'
        });
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
        var url = baseUrl + 'foods/';
        var method = 'POST';
        if (food.id) {
            url += food.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(food)
        });
    },
    deleteFood: function(id){
        return $.ajax({
            url: baseUrl + 'foods/' + id,
            type: 'DELETE'
        });
    },

    // Recipes
    listRecipes(s) {
        return $.get(baseUrl + 'recipes');
    },
    getRecipe: function (id) {
        return $.get(baseUrl + 'recipes/' + id);
    },
    saveRecipe: function (recipe) {
        var url = baseUrl + 'recipes/';
        var method = 'POST';
        if (recipe.id) {
            url += recipe.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(recipe)
        });
    },
    deleteRecipe: function (id) {

    },
    // Nutrients
    listNutrients: function(){
        return $.get(baseUrl + 'nutrients/');
    },
    listDailyIntakes: function(gender, dob){

    },

    // Workouts
    listWorkouts: function(start, end){
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return $.get(baseUrl + 'workouts', query);
    },
    getWorkout: function(id){
        return $.get(baseUrl + 'workouts/' + id);
    },
    saveWorkout: function(workout){
        var url = baseUrl + 'workouts/';
        var method = 'POST';
        if (workout.id) {
            url += workout.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(workout)
        });
    },
    deleteWorkout: function(id){

    },

    // Muscles
    listMuscleGroups: function(){
        return $.get(baseUrl + 'muscles/groups');
    },

    // Exercises
    listExercises: function(){
        return $.get(baseUrl + 'exercises/');
    },
    getExercise: function(id){
        return $.get(baseUrl + 'exercises/' + id);
    },
    saveExercise: function(exercise){
        var url = baseUrl + 'exercises/';
        var method = 'POST';
        if (exercise.id) {
            url += exercise.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(exercise)
        });
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