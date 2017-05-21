var api = require('../api')
var constants = require('./constants')
var moment = require('moment')

const state = {

    workoutsStart: null,
    workoutsEnd: null,
    workoutsLoading: false,
    workouts: [],

    routinesLoading: false,
    routines: [],
    activeRoutine: undefined
}

// actions
const actions = {
    [constants.FETCH_ROUTINES] ({commit, state},{forceRefresh, success, failure}) {
        if(state.routines != null && state.routines.length > 0 && !forceRefresh){
            if(success){
                success(state.routines);
            }
            return;
        }
        api.listRoutines().then(function(routines){
            commit(constants.FETCH_ROUTINES_SUCCESS,{routines})

            var active = routines.find(r => r.active);
            if(active){
                api.getRoutine(active.id).then(function(routine){
                    commit(constants.FETCH_ACTIVE_ROUTINE_SUCCESS,{routine});
                    if(success){
                        success(routines);
                    }
                });
            }
        }).fail(function(){
            commit(constants.FETCH_ROUTINES_FAILURE)
            if(failure){
                failure();
            }
        });
    },
[constants.FETCH_WORKOUTS] ({ commit, state }, {start, end, success, failure}) {
    if(state.workoutsStart && state.workoutsEnd){
        if(moment(start).isBefore(state.workoutsStart) || moment(end).isAfter(state.workoutsEnd)){
            start = moment.min(moment(start), moment(state.workoutsEnd));
            end = moment.max(moment(end), moment(state.workoutsStart));
        }
        else{
            // within already loaded period
            return;
        }
    }

    commit(constants.FETCH_WORKOUTS_STARTED)
    api.listWorkouts(start, end).then(function(workouts){
        commit(constants.FETCH_WORKOUTS_SUCCESS,{start, end, workouts})
        if(success){
            success(workouts);
        }
    }).fail(function(){
        commit(constants.FETCH_WORKOUTS_FAILURE)
        if(failure){
            failure();
        }
    });
},
[constants.SAVE_WORKOUT] ({commit, state},{workout, success, failure}){
    commit(constants.SAVE_WORKOUT_STARTED)
    api.saveWorkout(workout).then(function(savedWorkout){
        commit(constants.SAVE_WORKOUT_SUCCESS,{id: workout.id, workout: savedWorkout})
        if(success){
            success(savedWorkout);
        }
    }).fail(function(){
        commit(constants.SAVE_WORKOUT_FAILURE)
        if(failure){
            failure();
        }
    });
},
[constants.DELETE_WORKOUT] ({commit, state},{workout, success, failure}){
    commit(constants.DELETE_WORKOUT_STARTED)
    api.deleteWorkout(workout.id).then(function(){
        commit(constants.DELETE_WORKOUT_SUCCESS,{workout})
        if(success){
            success();
        }
    }).fail(function(){
        commit(constants.DELETE_WORKOUT_FAILURE)
        if(failure){
            failure();
        }
    });
}
}

// mutations
const mutations = {
    [constants.FETCH_ROUTINES_STARTED] (state) {
        state.routinesLoading = true;
    },
    [constants.FETCH_ROUTINES_SUCCESS] (state, {routines}) {
        state.routinesLoading = false;
        state.routines = routines;;
    },
    [constants.FETCH_ROUTINES_FAILURE] (state) {
        state.routinesLoading = false;
    },
    [constants.FETCH_WORKOUTS_STARTED] (state) {
        state.workoutsLoading = true;
    },
    [constants.FETCH_ACTIVE_ROUTINE_SUCCESS] (state, {routine}) {
        state.activeRoutine = routine;
    },
    [constants.FETCH_WORKOUTS_SUCCESS] (state, {start, end, workouts}) {
        state.workoutsLoading = false;
        var days = state.mealDays;
        for(var i in workouts){
            var workout = workouts[i];
            workout.time = new Date(workout.time);
            if(!state.workoutsStart || !state.workoutEnd || moment(workout.time).isBefore(state.workoutsStart) || moment(workout.time).isAfter(state.workoutsEnd)){
                addMeal(meal, state);
            }
        }
        if(!state.workoutsStart || moment(start).isBefore(state.workoutsStart)){
            state.workoutsStart = start;
        }
        if(!state.workoutsEnd || moment(end).isAfter(state.workoutsEnd)){
            state.workoutsEnd = end;
        }
    },
    [constants.FETCH_WORKOUTS_FAILURE] (state) {
        state.workoutsLoading = false;
    },
    [constants.SAVE_WORKOUT_STARTED] (state) {
        
    },
    [constants.SAVE_WORKOUT_SUCCESS] (state, {id, workout}) {
        if(id){
            var old = state.workouts.find(w => w.id == id);
            if(old){
                removeWorkout(old, state)
            }
        }
        state.workouts.push(workout);

    },
    [constants.SAVE_WORKOUT_FAILURE] (state) {
        
    },
    [constants.DELETE_WORKOUT_STARTED] (state) {
        
    },
    [constants.DELETE_WORKOUT_SUCCESS] (state, {workout}) {
        removeWorkout(meal, state)
    },
    [constants.DELETE_WORKOUT_FAILURE] (state) {
        
    }
}
function removeWorkout(workout, state){
    state.workouts.splice(state.workouts.findIndex(w => w.id == workout.id), 1);
}

module.exports = { state, actions, mutations }