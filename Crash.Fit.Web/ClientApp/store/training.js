var api = require('../api')
var constants = require('./constants')
var moment = require('moment')

const state = {

    muscleGroupsLoaded: false,
    muscleGroups: [],


    exercisesLoaded: false,
    exercises: [],


    routinesLoaded: false,
    routines: [],
    activeRoutineLoaded: false,
    activeRoutine: undefined,
    
    
    workoutsStart: null,
    workoutsEnd: null,
    workouts: [],
    workoutDays: {} // [day.getTime()] = true
}

// actions
const actions = {
     
    // MuscleGroups 
     [constants.FETCH_MUSCLEGROUPS] ({commit, state},{forceRefresh, success, failure}) {
         if(state.muscleGroupsLoaded && !forceRefresh){
             if(success){
                 success(state.muscleGroups);
             }
             return;
         }
         api.listMuscleGroups().then(function(muscleGroups){
             commit(constants.FETCH_MUSCLEGROUPS_SUCCESS,{muscleGroups});
             if(success){
                 success(muscleGroups);
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
    // Workouts
     [constants.FETCH_WORKOUTS] ({ commit, state }, {start, end, success, failure}) {
         if(state.workoutsStart && state.workoutsEnd){
             if(moment(start).isBefore(state.workoutsStart) || moment(end).isAfter(state.workoutsEnd)){
                 start = moment.min(moment(start), moment(state.workoutsEnd));
                 end = moment.max(moment(end), moment(state.workoutsStart));
             }
             else{
                 // within already loaded period
                 if(success){
                     success(state.workouts);
                 }
                 return;
             }
         }

         api.listWorkouts(start, end).then(function(workouts){
             commit(constants.FETCH_WORKOUTS_SUCCESS,{start, end, workouts})
             if(success){
                 success(workouts);
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
     [constants.SAVE_WORKOUT] ({commit, state},{workout, success, failure}){
         api.saveWorkout(workout).then(function(savedWorkout){
             commit(constants.SAVE_WORKOUT_SUCCESS,{id: workout.id, workout: savedWorkout})
             if(success){
                 success(savedWorkout);
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
     [constants.DELETE_WORKOUT] ({commit, state},{workout, success, failure}){
         api.deleteWorkout(workout.id).then(function(){
             commit(constants.DELETE_WORKOUT_SUCCESS,{workout})
             if(success){
                 success();
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
    // Exercises
     [constants.FETCH_EXERCISES] ({commit, state},{forceRefresh, success, failure}) {
         if(state.exercisesLoaded && !forceRefresh){
             if(success){
                 success(state.exercises);
             }
             return;
         }
         api.listExercises().then(function(exercises){
             commit(constants.FETCH_EXERCISES_SUCCESS,{exercises});
             if(success){
                 success(exercises);
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
     [constants.FETCH_EXERCISE] ({commit, state},{id, success, failure}) {
         api.getExercise(id).then(function(exercise){
             if(success){
                 success(exercise);
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
     [constants.SAVE_EXERCISE] ({commit, state},{exercise, success, failure}){
         api.saveExercise(exercise).then(function(savedExercise){
             commit(constants.SAVE_EXERCISE_SUCCESS,{id: exercise.id, exercise: savedExercise})
             if(success){
                 success(savedExercise);
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
     [constants.DELETE_EXERCISE] ({commit, state},{exercise, success, failure}){
         api.deleteExercise(exercise.id).then(function(){
             commit(constants.DELETE_EXERCISE_SUCCESS,{exercise})
             if(success){
                 success();
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
    // Routines
     [constants.FETCH_ROUTINES] ({commit, state},{forceRefresh, success, failure}) {
         if(state.routinesLoaded && !forceRefresh){
             if(success){
                 success(state.routines);
             }
             return;
         }
         api.listRoutines().then(function(routines){
            commit(constants.FETCH_ROUTINES_SUCCESS,{routines})
            if (success) {
                success(routines);
            }
            var active = routines.find(r => r.active);
            if(active){
                api.getRoutine(active.id).then(function(routine){
                    commit(constants.FETCH_ACTIVE_ROUTINE_SUCCESS,{routine});
                });
            }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
     [constants.FETCH_ROUTINE] ({commit, state},{id, success, failure}){
         api.getRoutine(id).then(function(routine){
             if(success){
                 success(routine);
             }
         }).fail(function(){
             if(failure){
                 failure();
             }
         });
     },
    [constants.SAVE_ROUTINE] ({commit, state},{routine, success, failure}){
        api.saveRoutine(routine).then(function(savedRoutine){
            commit(constants.SAVE_ROUTINE_SUCCESS,{id: routine.id, routine: savedRoutine})
            if(success){
                success(savedRoutine);
            }
        }).fail(function(){
            if(failure){
                failure();
            }
        });
    },
    [constants.DELETE_ROUTINE] ({commit, state},{routine, success, failure}){
        api.deleteRoutine(routine.id).then(function(){
            commit(constants.DELETE_ROUTINE_SUCCESS,{routine})
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
    [constants.FETCH_MUSCLEGROUPS_SUCCESS] (state, {muscleGroups}) {
        state.muscleGroups = muscleGroups;
        state.muscleGroupsLoaded = true;     
    },
    [constants.FETCH_EXERCISES_SUCCESS] (state, {exercises}) {
        state.exercises = exercises;
        state.exercisesLoaded = true;     
    },
    [constants.FETCH_ROUTINES_SUCCESS] (state, {routines}) {
        state.routines = routines;
        state.routinesLoaded = true;     
    },
    [constants.FETCH_ACTIVE_ROUTINE_SUCCESS] (state, {routine}) {
        state.activeRoutine = routine;
        state.activeRoutineLoaded = true;
    },
    [constants.FETCH_WORKOUTS_SUCCESS] (state, {start, end, workouts}) {
        for(var i in workouts){
            var workout = workouts[i];
            workout.time = new Date(workout.time);
            var day = moment(workout.time).startOf('day').toDate();
            state.workoutDays[day.getTime()] = true;
            if(!state.workoutsStart || !state.workoutEnd || moment(workout.time).isBefore(state.workoutsStart) || moment(workout.time).isAfter(state.workoutsEnd)){
                state.workouts.push(workout);
            }
        }
        if(!state.workoutsStart || moment(start).isBefore(state.workoutsStart)){
            state.workoutsStart = start;
        }
        if(!state.workoutsEnd || moment(end).isAfter(state.workoutsEnd)){
            state.workoutsEnd = end;
        }
    },
    [constants.SAVE_EXERCISE_SUCCESS] (state, {id, exercise}) {
        if(id){
            var old = state.exercises.find(x => x.id == id);
            if(old){
                removeExercise(old, state);
            }
        }
        state.exercises.push(exercise);
    },
    [constants.SAVE_ROUTINE_SUCCESS] (state, {id, routine}) {
        if(id){
            var old = state.routines.find(x => x.id == id);
            if(old){
                removeRoutine(old, state);
            }
        }
        state.routines.push(routine);
    },
    [constants.SAVE_WORKOUT_SUCCESS] (state, {id, workout}) {
        if(id){
            var old = state.workouts.find(x => x.id == id);
            if(old){
                removeWorkout(old, state)
            }
        }
        state.workouts.push(workout);
    },
    [constants.DELETE_EXERCISE_SUCCESS] (state, {exercise}) {
        removeExercise(exercise, state)
    },
    [constants.DELETE_ROUTINE_SUCCESS] (state, {routine}) {
        removeRoutine(routine, state)
    },
    [constants.DELETE_WORKOUT_SUCCESS] (state, {workout}) {
        removeWorkout(meal, state)
    },
    [constants.LOGOUT_SUCCESS] (state) {
        // TODO: clear state
    }
}
function removeExercise(exercise, state){
    state.exercises.splice(state.exercises.findIndex(x => x.id == exercise.id), 1);
}
function removeRoutine(routine, state){
    state.routines.splice(state.routines.findIndex(x => x.id == routine.id), 1);
}
function removeWorkout(workout, state){
    state.workouts.splice(state.workouts.findIndex(x => x.id == workout.id), 1);
}

module.exports = { state, actions, mutations }