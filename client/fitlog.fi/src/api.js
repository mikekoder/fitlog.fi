import config from './config'
import storage from './storage'
import axios from 'axios'
import { version } from '../package.json'
import { Platform } from 'quasar'

axios.interceptors.request.use(function (config) {
    var token = storage.getItem("access_token");
    config.headers = { Authorization: `Bearer ${token}`};
    try {
      if(Platform.is.desktop){
        config.headers.ClientVersion = version + ' (web)';
      }
      else {
        config.headers.ClientVersion = version;
      }
      
    }
    catch{}
    return config;
  }, function (error) {
    return Promise.reject(error);
});
/*
$.ajaxSetup({
    beforeSend(xhr) {
        var token = storage.getItem("access_token");
        xhr.setRequestHeader('Authorization', 'bearer ' + token);
    }
});
*/
export default {
    baseUrl: config.apiBaseUrl,

    // Users
    register(registration) {
        return axios.post(this.baseUrl + 'users/register', registration);
    },
    login(login) {
        return axios.post(this.baseUrl + 'users/login', login);
    },
    getProfile(){
        return axios.get(this.baseUrl + 'users/me/');
    },
    saveProfile (profile) {
        return axios.put(this.baseUrl + 'users/me/',  profile);
    },
    deleteProfile () {
        return axios.delete(this.baseUrl + 'users/me/');
    },
    logout(){
        return axios.post(this.baseUrl + 'users/logout');
    },
    refreshToken(refreshToken) {
        return axios.get(this.baseUrl + 'users/refresh-token/?refreshToken=' + refreshToken);
    },
    updateLogin(login) {
        return axios.put(this.baseUrl + 'users/login', login);
    },
    loginWithToken(provider, token){
        return axios.get(`${this.baseUrl}users/token-login?provider=${provider}&token=${token}`);
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
        return axios.get(this.baseUrl + 'meals', {params:query});
    },
    getMeal(id){
        return axios.get(this.baseUrl + 'meals/' + id);
    },
    saveMeal(meal) {
        var url = this.baseUrl + 'meals/';
        if (meal.id) {
            url += meal.id;
            return axios.put(url, meal);
        }
        else {
            return axios.post(url, meal);
        }
    },
    deleteMeal(id){
        return axios.delete( this.baseUrl + 'meals/' + id);
    },
    restoreMeal(id) {
        return axios.post( this.baseUrl + 'meals/' + id + '/restore/');
    },
    saveMealRow(row) {
        if(row.id){
            return axios.put(`${this.baseUrl}meals/${row.mealId}/rows/${row.id}`, row);
        }
        else {
            return axios.post(`${this.baseUrl}meals/rows`, row);
        }
    },
    deleteMealRow(row) {
        return axios.delete(`${this.baseUrl}meals/${row.mealId}/rows/${row.id}`);
    },
    // Foods
    searchFoods(name){
        return axios.get(this.baseUrl + 'foods/search', { params:{name }});
    },
    searchFoodsMostNutrients(nutrientId){
        return axios.get(this.baseUrl + 'foods/search/most-nutrients', {params: { nutrientId, count: 100 }});
    },
    searchFoodsLeastNutrients(nutrientId){
        return axios.get(this.baseUrl + 'foods/search/least-nutrients', {params: { nutrientId, count: 100 }});
    },
    searchExternalFood(ean){
        return axios.get(this.baseUrl + 'foods/search-external', {params:{ ean }});
    },
    getLatestFoods() {
        return axios.get(this.baseUrl + 'foods/latest');
    },
    getMostUsedFoods() {
        return axios.get(this.baseUrl + 'foods/most-used');
    },
    listFoods(){
        return axios.get(this.baseUrl + 'foods/');
    },
    getFood(id){
        return axios.get(this.baseUrl + 'foods/' + id);
    },
    saveFood(food){
        var url = this.baseUrl + 'foods/';
        if (food.id) {
            url += food.id;
            return axios.put(url, food);
        }
        else {
            return axios.post(url, food);
        }
    },
    deleteFood(id){
        return axios.delete(this.baseUrl + 'foods/' + id);
    },

    // Recipes
    listRecipes(s) {
        return axios.get(this.baseUrl + 'recipes');
    },
    getRecipe(id) {
        return axios.get(this.baseUrl + 'recipes/' + id);
    },
    saveRecipe(recipe) {
        var url = this.baseUrl + 'recipes/';
        if (recipe.id) {
            url += recipe.id;
            return axios.put(url, recipe);
        }
        else {
            return axios.post(url, recipe);
        }
    },
    deleteRecipe(id) {
        return axios.delete( this.baseUrl + 'recipes/' + id);
    },
    // Nutrients
    listNutrients(){
        return axios.get(this.baseUrl + 'nutrition/nutrients');
    },
    getNutrientSettings(){
        return axios.get(this.baseUrl + 'nutrition/settings');
    },
    getNutritionGoals() {
        return axios.get(this.baseUrl + 'nutrition/goals');
    },
    getNutritionGoal(id) {
        return axios.get(this.baseUrl + 'nutrition/goals/' + id);
    },
    getActiveNutritionGoal() {
        return axios.get(this.baseUrl + 'nutrition/goals/active');
    },
    saveNutritionGoal(goal) {
        var url = this.baseUrl + 'nutrition/goals/';
        if (goal.id) {
            url += goal.id;
            return axios.put(url, goal);
        }
        else {
            return axios.post(url, goal);
        }
    },
    activateNutritionGoal(id){
        return axios.post( this.baseUrl + 'nutrition/goals/' + id + '/activate');
    },
    deleteNutritionGoal(id){
        return axios.delete(this.baseUrl + 'nutrition/goals/' + id);
    },
    listDailyIntakes(gender, dob){

    },
    saveNutrientSettings(settings){
        return axios.put( this.baseUrl + 'nutrition/settings', settings);
    },
    saveHomeSettings(settings){
        return axios.put( this.baseUrl + 'settings/home', settings);
    },
    getNutrientHistory(start, end) {
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return axios.get(this.baseUrl + 'nutrition/nutrients/history', {params: query});
    },
    // Meal rhythm
    getMealDefinitions() {
        return axios.get(this.baseUrl + 'meals/definitions');
    },
    saveMealDefinitions(definitions) {
        var url = this.baseUrl + 'meals/definitions';
        return axios.put(url, definitions);
    },
    deleteMealDefinition(definition) {
        return axios.delete(this.baseUrl + 'meals/definitions/'+id);
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
        return axios.get(this.baseUrl + 'workouts', query);
    },
    getWorkout(id){
        return axios.get(this.baseUrl + 'workouts/' + id);
    },
    saveWorkout(workout){
        var url = this.baseUrl + 'workouts/';
        if (workout.id) {
            url += workout.id;
            return axios.put(url, workout);
        }
        else {
            return axios.post(url, workout);
        }
    },
    deleteWorkout(id){
        return axios.delete( this.baseUrl + 'workouts/' + id);
    },

    // Muscles
    listMuscleGroups(){
        return axios.get(this.baseUrl + 'muscles/groups');
    },

    // Equipment
    listEquipments(){
        return axios.get(this.baseUrl + 'exercises/equipment');
    },

    // Exercises
    listExercises(){
        return axios.get(this.baseUrl + 'exercises/');
    },
    searchExercises(name, muscleGroupId, equipmentId, ids){
        var query = {
            name,
            muscleGroupId,
            equipmentId,
            ids
        };
        return axios.get(this.baseUrl + 'exercises/search', {params: query});
    },
    listLatestExercises(){
        return axios.get(this.baseUrl + 'exercises/latest');
    },
    listMostUsedExercises(){
        return axios.get(this.baseUrl + 'exercises/most-used');
    },
    getExercise(id){
        return axios.get(this.baseUrl + 'exercises/' + id);
    },
    getExercises(ids){
      if(!ids || ids.length == 0){
        return Promise.resolve({ data: []});
      }
      
      var idList = [...new Set(ids)].join(',');
      var query = {
        ids: idList
      };
      return axios.get(this.baseUrl + 'exercises/list/', {params: query});
    },
    saveExercise(exercise){
        var url = this.baseUrl + 'exercises/';
        if (exercise.id) {
            url += exercise.id;
            return axios.put(url, exercise);
        }
        else {
            return axios.post(url, exercise);
        }
    },
    save1RM(exerciseId, oneRepMax){
        return axios.put(this.baseUrl + 'exercises/' + exerciseId + '/onerepmax', oneRepMax);
    },
    deleteExercise(id){
        return axios.delete(this.baseUrl + 'exercises/' + id);
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
        return axios.get(this.baseUrl + 'exercises/history', {params: query});
    },
    transferExerciseData(fromExerciseId, toExerciseId, transferWorkouts, transferRoutines, transfer1rm){
      var data = {
        fromExerciseId,
        toExerciseId,
        transferWorkouts,
        transferRoutines,
        transfer1rm
      };
      return axios.post(this.baseUrl + 'exercises/transfer', data);
    },

    // Routines
    listRoutines() {
        return axios.get(this.baseUrl + 'routines/');
    },
    getRoutine(id){
        return axios.get(this.baseUrl + 'routines/' + id);
    },
    saveRoutine (routine) {
        var url = this.baseUrl + 'routines/';
        if (routine.id) {
            url += routine.id;
            return axios.put(url, routine);
        }
        else {
            return axios.post(url, routine);
        }
    },
    deleteRoutine(id){
        return axios.delete(this.baseUrl + 'routines/' + id);
    },
    activateRoutine(id){
        return axios.post(this.baseUrl + 'routines/' + id + '/activate');
    },
    getTrainingGoals() {
        return axios.get(this.baseUrl + 'training/goals');
    },
    getTrainingGoal(id) {
        return axios.get(this.baseUrl + 'training/goals/' + id);
    },
    getActiveTrainingGoal() {
        return axios.get(this.baseUrl + 'training/goals/active');
    },
    saveTrainingGoal(goal) {
        var url = this.baseUrl + 'training/goals/';
        if (goal.id) {
            url += goal.id;
            return axios.put(url, goal);
        }
        else {
            return axios.post(url, goal);
        }
    },
    activateTrainingGoal(id){
        return axios.post(this.baseUrl + 'training/goals/' + id + '/activate');
    },
    deleteTrainingGoal(id){
        return axios.delete( this.baseUrl + 'training/goals/' + id);
    },

    // Measurements
    listMeasures(){
        return axios.get(this.baseUrl + 'measurements/measures');
    },
    saveMeasurements (measurements) {
        return axios.post(this.baseUrl + 'measurements/', measurements);
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
        return axios.get(this.baseUrl + 'measurements/history', {params:query});
    },

    // Feedback
    saveFeedback (feedback) {
        var url = this.baseUrl + 'feedback/';
        if (feedback.id) {
            url += feedback.id;
            return axios.put(url, feedback);
        }
        else {
            return axios.post(url, feedback);
        }
    },
    listBugs() {
        return axios.get(this.baseUrl + 'feedback/bugs');
    },
    listImprovements() {
        return axios.get(this.baseUrl + 'feedback/improvements');
    },
    getVotes() {
        return axios.get(this.baseUrl + 'feedback/votes');
    },
    saveVote(feedbackId) {
        return axios.post(this.baseUrl + 'feedback/' + feedbackId+'/vote');
    },

    // Activities
    listActivities() {
        return axios.get(this.baseUrl + 'activities');
    },
    listEnergyExpenditures(start, end){
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return axios.get(this.baseUrl + 'activities/energyexpenditures', {params: query});
    },
    saveEnergyExpenditure(energyExpenditure){
        var url = this.baseUrl + 'activities/energyexpenditures/';
        if (energyExpenditure.id) {
            url += energyExpenditure.id;
            return axios.put(url, energyExpenditure);
        }
        else {
            return axios.post(url, energyExpenditure);
        }
    },
    deleteEnergyExpenditure(id){
        return axios.delete(this.baseUrl + 'activities/energyexpenditures/' + id);
    },
    listActivityPresets() {
        return axios.get(this.baseUrl + 'activities/presets');
    },
    saveActivityPresets(presets) {
        return axios.put(this.baseUrl + 'activities/presets', presets);
    },
    saveActivityPresetForDay(date, activityPresetId) {
        return axios.put(this.baseUrl + 'activities/day-preset', { date, activityPresetId });
    },
    listActivityPresetDays(start, end){
        var query = {};
        if (start) {
            query.start = start.toISOString();
        }
        if (end) {
            query.end = end.toISOString();
        }
        return axios.get(this.baseUrl + 'activities/day-presets', {params:query});
    },
    getSettings(key){
      return axios.get(this.baseUrl + 'settings/' + key);
    },
    updateSettings(key, data){
      return axios.put(this.baseUrl + 'settings', { key, data: JSON.stringify(data)});
    }
};