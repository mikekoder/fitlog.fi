var $ = require('jquery')

$.ajaxSetup({
    headers: { 'X-CSRF-TOKEN': $('meta[name="X-CSRF-TOKEN"]').attr('content') },
    beforeSend: function (xhr) {
        var token = localStorage.getItem("access_token");
        xhr.setRequestHeader('Authorization', 'bearer ' + token);
    }
    /*, error: function (x, status, error) {
        if (x.status == 401) {
            alert("Istunto on vanhentunut. Kirjaudu sisään ja yritä sitten uudestaan");
            var loginWindow = new BrowserWindow({ width: 600, height: 300, frame: false, show: false });
            loginWindow.loadURL('/#/kirjaudu');
            //loginWindow.on('closed', () => { loginWindow = null; });
            loginWindow.show();
        }
        else {
            alert("An error occurred: " + status + "nError: " + error);
        }
    }*/
});
const baseUrl = '/api/'

const api = {
    baseUrl: baseUrl,

    // Users
    register: function(registration) {
        return $.ajax({
            url: baseUrl + 'users/register',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(registration)
        });
    },
    login: function(login) {
        return $.ajax({
            url: baseUrl + 'users/login',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(login)
        });
    },
    getProfile: function(){
        return $.get(baseUrl + 'users/me/');
    },
    saveProfile: function (profile) {
        return $.ajax({
            url: baseUrl + 'users/me/',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(profile)
        });
    },
    logout: function(){
        return $.post(baseUrl+'users/logout');
    },
    refreshToken: function (refreshToken) {
        return $.get(baseUrl + 'users/refresh-token/?refreshToken=' + refreshToken);
    },
    updateLogin: function(login) {
        return $.ajax({
            url: baseUrl + 'users/login',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(login)
        });
    },

    // Meals
    listMeals: function(start, end) {
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
    restoreMeal: function (id) {
        return $.ajax({
            url: baseUrl + 'meals/' + id + '/restore/',
            type: 'POST'
        });
    },
    saveMealRow: function (row) {
        var url = baseUrl + 'meals/row/';
        var method = 'POST';
        if (row.id) {
            url += row.id;
            method = 'PUT';
        }
        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(row)
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
    listRecipes: function(s) {
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
        return $.ajax({
            url: baseUrl + 'recipes/' + id,
            type: 'DELETE'
        });
    },
    // Nutrients
    listNutrients: function(){
        return $.get(baseUrl + 'nutrients/');
    },
    getNutrientTargets: function () {
        return $.get(baseUrl + 'nutrients/targets');
    },
    saveNutrientTargets: function (targets) {
        return $.ajax({
            url: baseUrl + 'nutrients/targets',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(targets)
        });
    },
    listDailyIntakes: function(gender, dob){

    },
    saveNutrientSettings: function(settings){
        return $.ajax({
            url: baseUrl + 'nutrients/settings',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(settings)
        });
    },
    saveHomeSettings: function(settings){
        return $.ajax({
            url: baseUrl + 'settings/home',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(settings)
        });
    },
    // Meal rhythm
    getMealDefinitions: function () {
        return $.get(baseUrl + 'meals/definitions');
    },
    saveMealDefinitions: function (definitions) {
        var url = baseUrl + 'meals/definitions';
        return $.ajax({
            url: url,
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(definitions)
        });
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
        return $.ajax({
            url: baseUrl + 'workouts/' + id,
            type: 'DELETE'
        });
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
        return $.ajax({
            url: baseUrl + 'exercises/' + id,
            type: 'DELETE'
        });
    },

    // Routines
    listRoutines: function () {
        return $.get(baseUrl + 'routines/');
    },
    getRoutine: function(id){
        return $.get(baseUrl + 'routines/' + id);
    },
    saveRoutine: function (routine) {
        var url = baseUrl + 'routines/';
        var method = 'POST';
        if (routine.id) {
            url += routine.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(routine)
        });
    },
    deleteRoutine: function(id){
        return $.ajax({
            url: baseUrl + 'routines/' + id,
            type: 'DELETE'
        });
    },
    activateRoutine: function(id){
        return $.ajax({
            url: baseUrl + 'routines/' + id + '/activate',
            type: 'POST'
        });
    },

    // Measurements
    listMeasures: function(){
        return $.get(baseUrl + 'measurements/measures');
    },
    saveMeasurements: function (measurements) {
        return $.ajax({
            url: baseUrl + 'measurements/',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(measurements)
        });
    },

    // Feedback
    saveFeedback: function (feedback) {
        var url = baseUrl + 'feedback/';
        var method = 'POST';
        if (feedback.id) {
            url += feedback.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(feedback)
        });
    },
    listBugs: function () {
        return $.get(baseUrl + 'feedback/bugs');
    },
    listImprovements: function () {
        return $.get(baseUrl + 'feedback/improvements');
    },
    getVotes: function () {
        return $.get(baseUrl + 'feedback/votes');
    },
    saveVote: function (feedbackId) {
        var url = baseUrl + 'feedback/' + feedbackId+'/vote';
        return $.ajax({
            url: url,
            type: 'POST',
            contentType: 'text/json'
        });
    }
};
module.exports = api