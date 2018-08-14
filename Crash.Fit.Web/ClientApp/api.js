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
    deleteProfile () {
        return $.ajax({
            url: this.baseUrl + 'users/me/',
            type: 'DELETE'
        });
    },
    logout(){
        return $.post(this.baseUrl + 'users/logout');
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
    loginWithToken(provider, token){
        return $.get(`${this.baseUrl}users/token-login?provider=${provider}&token=${token}`);
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
    searchFoodsMostNutrients(nutrientId){
        return $.get(this.baseUrl + 'foods/search/most-nutrients', { nutrientId, count: 100 });
    },
    searchFoodsLeastNutrients(nutrientId){
        return $.get(this.baseUrl + 'foods/search/least-nutrients', { nutrientId, count: 100 });
    },
    searchExternalFood(ean){
        return $.get(this.baseUrl + 'foods/search-external', { ean });
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
        return $.get(this.baseUrl + 'nutrition/nutrients');
    },
    getNutrientSettings(){
        return $.get(this.baseUrl + 'nutrition/settings');
    },
    getNutritionGoals() {
        return $.get(this.baseUrl + 'nutrition/goals');
    },
    getNutritionGoal(id) {
        return $.get(this.baseUrl + 'nutrition/goals/' + id);
    },
    getActiveNutritionGoal() {
        return $.get(this.baseUrl + 'nutrition/goals/active');
    },
    saveNutritionGoal(goal) {
        var url = this.baseUrl + 'nutrition/goals/';
        var method = 'POST';
        if (goal.id) {
            url += goal.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(goal)
        });
    },
    activateNutritionGoal(id){
        return $.ajax({
            url: this.baseUrl + 'nutrition/goals/' + id + '/activate',
            type: 'POST'
        });
    },
    deleteNutritionGoal(id){
        return $.ajax({
            url: this.baseUrl + 'nutrition/goals/' + id,
            type: 'DELETE'
        });
    },
    listDailyIntakes(gender, dob){

    },
    saveNutrientSettings(settings){
        return $.ajax({
            url: this.baseUrl + 'nutrition/settings',
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
    getNutrientHistory(start, end) {
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return $.get(this.baseUrl + 'nutrition/nutrients/history', query);
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
    deleteMealDefinition(definition) {
        var url = this.baseUrl + 'meals/definitions/'+id;
        return $.ajax({
            url: url,
            type: 'DELETE',
            contentType: 'text/json'
        });
    },
    // Workouts
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
    save1RM(exerciseId, oneRepMax){
        var url = this.baseUrl + 'exercises/' + exerciseId + '/onerepmax';
        return $.ajax({
            url: url,
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(oneRepMax)
        });
    },
    deleteExercise(id){
        return $.ajax({
            url: this.baseUrl + 'exercises/' + id,
            type: 'DELETE'
        });
    },
    getExerciseHistory(exerciseId, start, end){
        var query = {
            exerciseId
        };
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return $.get(this.baseUrl + 'exercises/history', query);
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
    getTrainingGoals() {
        return $.get(this.baseUrl + 'training/goals');
    },
    getTrainingGoal(id) {
        return $.get(this.baseUrl + 'training/goals/' + id);
    },
    getActiveTrainingGoal() {
        return $.get(this.baseUrl + 'training/goals/active');
    },
    saveTrainingGoal(goal) {
        var url = this.baseUrl + 'training/goals/';
        var method = 'POST';
        if (goal.id) {
            url += goal.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(goal)
        });
    },
    activateTrainingGoal(id){
        return $.ajax({
            url: this.baseUrl + 'training/goals/' + id + '/activate',
            type: 'POST'
        });
    },
    deleteTrainingGoal(id){
        return $.ajax({
            url: this.baseUrl + 'training/goals/' + id,
            type: 'DELETE'
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
    getMeasurementHistory(measureId, start, end){
        var query = {
            measureId
        };
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return $.get(this.baseUrl + 'measurements/history', query);
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
    },

    // Activities
    listActivities() {
        return $.get(this.baseUrl + 'activities');
    },
    listEnergyExpenditures(start, end){
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return $.get(this.baseUrl + 'activities/energyexpenditures', query);
    },
    saveEnergyExpenditure(energyExpenditure){
        var url = this.baseUrl + 'activities/energyexpenditures/';
        var method = 'POST';
        if (energyExpenditure.id) {
            url += energyExpenditure.id;
            method = 'PUT';
        }

        return $.ajax({
            url: url,
            type: method,
            contentType: 'text/json',
            data: JSON.stringify(energyExpenditure)
        });
    },
    deleteEnergyExpenditure(id){
        return $.ajax({
            url: this.baseUrl + 'activities/energyexpenditures/' + id,
            type: 'DELETE'
        });
    },
    listActivityPresets() {
        return $.get(this.baseUrl + 'activities/presets');
    },
    saveActivityPresets(presets) {
        var url = this.baseUrl + 'activities/presets';
        return $.ajax({
            url: url,
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify(presets)
        });
    },
    saveActivityPresetForDay(date, activityPresetId) {
        var url = this.baseUrl + 'activities/day-preset';
        return $.ajax({
            url: url,
            type: 'PUT',
            contentType: 'text/json',
            data: JSON.stringify({ date, activityPresetId })
        });
    },
    listActivityPresetDays(start, end){
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return $.get(this.baseUrl + 'activities/day-presets', query);
    },
};