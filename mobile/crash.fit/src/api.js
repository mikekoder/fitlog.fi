import $ from 'jquery'
import config from './config'
import storage from './storage'

$.ajaxSetup({
    beforeSend(xhr) {
        var token = storage.getItem("access_token");
        xhr.setRequestHeader('Authorization', 'bearer ' + token);
    }
});

export default {
    baseUrl: config.apiBaseUrl,

    // Users
    register(registration) {
        return $.ajax({
            url: this.baseUrl + 'users/register',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(registration)
        });
    },
    login(login) {
        return $.ajax({
            url: this.baseUrl + 'users/login',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(login)
        });
    },
    getProfile(){
        return $.get(this.baseUrl + 'users/me/');
    },
    saveProfile (profile) {
        return $.ajax({
            url: this.baseUrl + 'users/me/',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(profile)
        });
    },
    logout(){
        return $.post(this.baseUrl+'users/logout');
    },
    refreshToken(refreshToken) {
        return $.get(this.baseUrl + 'users/refresh-token/?refreshToken=' + refreshToken);
    },
    updateLogin(login) {
        return $.ajax({
            url: this.baseUrl + 'users/login',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(login)
        });
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
        return $.get(this.baseUrl + 'meals', query);
    },
    getMeal(id){
        return $.get(this.baseUrl + 'meals/' + id);
    },
    saveMeal(meal) {
        var url = this.baseUrl + 'meals/';
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
    deleteMeal(id){
        return $.ajax({
            url: this.baseUrl + 'meals/' + id,
            type: 'DELETE'
        });
    },
    restoreMeal(id) {
        return $.ajax({
            url: this.baseUrl + 'meals/' + id + '/restore/',
            type: 'POST'
        });
    },
    saveMealRow(row) {
        var url = row.id ? `${this.baseUrl}meals/${row.mealId}/rows/${row.id}` : `${this.baseUrl}meals/rows`;
        var method = row.id ? 'PUT' : 'POST';

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(row)
        });
    },
    deleteMealRow(row) {
        var url = `${this.baseUrl}meals/${row.mealId}/rows/${row.id}`;
        var method = 'DELETE';
        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json'
        });
    },
    // Foods
    searchFoods(name){
        return $.get(this.baseUrl + 'foods/search', { name });
    },
    getLatestFoods() {
        return $.get(this.baseUrl + 'foods/latest');
    },
    getMostUsedFoods() {
        return $.get(this.baseUrl + 'foods/most-used');
    },
    listFoods(){
        return $.get(this.baseUrl + 'foods/');
    },
    getFood(id){
        return $.get(this.baseUrl + 'foods/' + id);
    },
    saveFood(food){
        var url = this.baseUrl + 'foods/';
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
    deleteFood(id){
        return $.ajax({
            url: this.baseUrl + 'foods/' + id,
            type: 'DELETE'
        });
    },

    // Recipes
    listRecipes(s) {
        return $.get(this.baseUrl + 'recipes');
    },
    getRecipe(id) {
        return $.get(this.baseUrl + 'recipes/' + id);
    },
    saveRecipe(recipe) {
        var url = this.baseUrl + 'recipes/';
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
    deleteRecipe(id) {
        return $.ajax({
            url: this.baseUrl + 'recipes/' + id,
            type: 'DELETE'
        });
    },
    // Nutrients
    listNutrients(){
        return $.get(this.baseUrl + 'nutrients/');
    },
    getNutritionGoals() {
        return $.get(this.baseUrl + 'nutrients/goals');
    },
    saveNutritionGoals (goals) {
        return $.ajax({
            url: this.baseUrl + 'nutrients/goals',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(goals)
        });
    },
    listDailyIntakes(gender, dob){

    },
    saveNutrientSettings(settings){
        return $.ajax({
            url: this.baseUrl + 'nutrients/settings',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(settings)
        });
    },
    saveHomeSettings(settings){
        return $.ajax({
            url: this.baseUrl + 'settings/home',
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(settings)
        });
    },
    // Meal rhythm
    getMealDefinitions() {
        return $.get(this.baseUrl + 'meals/definitions');
    },
    saveMealDefinitions(definitions) {
        var url = this.baseUrl + 'meals/definitions';
        return $.ajax({
            url: url,
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(definitions)
        });
    },

    // Workouts
    startWorkout(time){
        return $.ajax({
            url: this.baseUrl + 'workouts/start',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify({time})
        });
    },
    listWorkouts(start, end){
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return $.get(this.baseUrl + 'workouts', query);
    },
    getWorkout(id){
        return $.get(this.baseUrl + 'workouts/' + id);
    },
    saveWorkout(workout){
        var url = this.baseUrl + 'workouts/';
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
    deleteWorkout(id){
        return $.ajax({
            url: this.baseUrl + 'workouts/' + id,
            type: 'DELETE'
        });
    },

    // Muscles
    listMuscleGroups(){
        return $.get(this.baseUrl + 'muscles/groups');
    },

    // Exercises
    listExercises(){
        return $.get(this.baseUrl + 'exercises/');
    },
    getExercise(id){
        return $.get(this.baseUrl + 'exercises/' + id);
    },
    saveExercise(exercise){
        var url = this.baseUrl + 'exercises/';
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
    deleteExercise(id){
        return $.ajax({
            url: this.baseUrl + 'exercises/' + id,
            type: 'DELETE'
        });
    },

    // Routines
    listRoutines() {
        return $.get(this.baseUrl + 'routines/');
    },
    getRoutine(id){
        return $.get(this.baseUrl + 'routines/' + id);
    },
    saveRoutine (routine) {
        var url = this.baseUrl + 'routines/';
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
    deleteRoutine(id){
        return $.ajax({
            url: this.baseUrl + 'routines/' + id,
            type: 'DELETE'
        });
    },
    activateRoutine(id){
        return $.ajax({
            url: this.baseUrl + 'routines/' + id + '/activate',
            type: 'POST'
        });
    },

    // Measurements
    listMeasures(){
        return $.get(this.baseUrl + 'measurements/measures');
    },
    saveMeasurements (measurements) {
        return $.ajax({
            url: this.baseUrl + 'measurements/',
            type: 'POST',
            contentType: 'text/json',
            data: JSON.stringify(measurements)
        });
    },

    // Feedback
    saveFeedback (feedback) {
        var url = this.baseUrl + 'feedback/';
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
    listBugs() {
        return $.get(this.baseUrl + 'feedback/bugs');
    },
    listImprovements() {
        return $.get(this.baseUrl + 'feedback/improvements');
    },
    getVotes() {
        return $.get(this.baseUrl + 'feedback/votes');
    },
    saveVote(feedbackId) {
        var url = this.baseUrl + 'feedback/' + feedbackId+'/vote';
        return $.ajax({
            url: url,
            type: 'POST',
            contentType: 'text/json'
        });
    }
};