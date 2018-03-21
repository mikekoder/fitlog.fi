import api from '../api'
import constants from './constants'
import moment from 'moment'

export default {
    state: {

        diaryDate: new Date(),

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
        workoutDays: {},
        workoutsDisplayStart: null,
        workoutsDisplayEnd: null,

        activeTrainingGoalLoaded: false,
        activeTrainingGoal: {},
        trainingGoalsLoaded: false,
        trainingGoals: {},

        activitiesLoaded: false,
        activities: [],
        energyExpenditures: [],
        energyExpendituresStart: null,
        energyExpendituresEnd: null,
        energyExpendituresDisplayStart: null,
        energyExpendituresDisplayEnd: null
    },
    actions: {
        // Diary
        [constants.SELECT_WORKOUT_DIARY_DATE]({ commit, state }, { date, success, failure }) {
            state.diaryDate = date;
        },
        // MuscleGroups 
        [constants.FETCH_MUSCLEGROUPS]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.muscleGroupsLoaded && !forceRefresh) {
                if (success) {
                    success(state.muscleGroups);
                }
                return;
            }
            api.listMuscleGroups().then(function (muscleGroups) {
                commit(constants.FETCH_MUSCLEGROUPS_SUCCESS, { muscleGroups });
                if (success) {
                    success(muscleGroups);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        // Workouts
        [constants.SELECT_WORKOUT_DATE_RANGE]({ commit, state }, { start, end, success, failure }) {
            commit(constants.SELECT_WORKOUT_DATE_RANGE_SUCCESS, { start, end });
            if (success) {
                success();
            }
        },
        [constants.FETCH_WORKOUTS]({ commit, state }, { start, end, force, success, failure }) {
            if (state.workoutsStart && state.workoutsEnd) {
                if (moment(start).isBefore(state.workoutsStart) || moment(end).isAfter(state.workoutsEnd) || force) {
                    start = moment.min(moment(start), moment(state.workoutsEnd));
                    end = moment.max(moment(end), moment(state.workoutsStart));
                }
                else {
                    // within already loaded period
                    if (success) {
                        success(state.workouts);
                    }
                    return;
                }
            }

            api.listWorkouts(start, end).then(function (workouts) {
                commit(constants.FETCH_WORKOUTS_SUCCESS, { start, end, workouts })
                if (success) {
                    success(workouts);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_WORKOUT]({ commit, state }, { id, success, failure }) {
            api.getWorkout(id).then(workout => {
                if (success) {
                    success(workout);
                }
            }).fail((xhr) => {
                if (failure) {
                    failure(xhr);
                }
            });
        },
        [constants.SAVE_WORKOUT]({ commit, state }, { workout, success, failure }) {
            api.saveWorkout(workout).then(savedWorkout => {
                savedWorkout.time = new Date(savedWorkout.time);
                commit(constants.SAVE_WORKOUT_SUCCESS, { id: workout.id, workout: savedWorkout })
                if (success) {
                    success(savedWorkout);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.DELETE_WORKOUT]({ commit, state }, { workout, success, failure }) {
            api.deleteWorkout(workout.id).then(() => {
                commit(constants.DELETE_WORKOUT_SUCCESS, { workout })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        // Exercises
        [constants.FETCH_EXERCISES]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.exercisesLoaded && !forceRefresh) {
                if (success) {
                    success(state.exercises);
                }
                return;
            }
            api.listExercises().then(function (exercises) {
                commit(constants.FETCH_EXERCISES_SUCCESS, { exercises });
                if (success) {
                    success(exercises);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_EXERCISE]({ commit, state }, { id, success, failure }) {
            api.getExercise(id).then(function (exercise) {
                if (success) {
                    success(exercise);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_EXERCISE]({ commit, state }, { exercise, success, failure }) {
            api.saveExercise(exercise).then(function (savedExercise) {
                commit(constants.SAVE_EXERCISE_SUCCESS, { id: savedExercise.id, exercise: savedExercise })
                if (success) {
                    success(savedExercise);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_1RM]({ commit, state }, { exerciseId, oneRepMax, success, failure }) {
            api.save1RM(exerciseId, oneRepMax).then(function () {
                commit(constants.SAVE_1RM_SUCCESS, {
                    exerciseId,
                    oneRepMax
                })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.DELETE_EXERCISE]({ commit, state }, { exercise, success, failure }) {
            api.deleteExercise(exercise.id).then(function () {
                commit(constants.DELETE_EXERCISE_SUCCESS, { exercise })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        // Routines
        [constants.FETCH_ROUTINES]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.routinesLoaded && !forceRefresh) {
                if (success) {
                    success(state.routines);
                }
                return;
            }
            api.listRoutines().then(function (routines) {
                commit(constants.FETCH_ROUTINES_SUCCESS, { routines })
                if (success) {
                    success(routines);
                }
                var active = routines.find(r => r.active);
                if (active) {
                    api.getRoutine(active.id).then(function (routine) {
                        commit(constants.FETCH_ACTIVE_ROUTINE_SUCCESS, { routine });
                    });
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_ROUTINE]({ commit, state }, { id, success, failure }) {
            api.getRoutine(id).then(function (routine) {
                if (success) {
                    success(routine);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_ROUTINE_WORKOUT]({ commit, state }, { id, success, failure }) {
            api.getRoutineWorkout(id).then(function (workout) {
                if (success) {
                    success(workout);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_ROUTINE]({ commit, state }, { routine, success, failure }) {
            api.saveRoutine(routine).then(function (savedRoutine) {
                commit(constants.SAVE_ROUTINE_SUCCESS, { id: routine.id, routine: savedRoutine })
                if (success) {
                    success(savedRoutine);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.DELETE_ROUTINE]({ commit, state }, { routine, success, failure }) {
            api.deleteRoutine(routine.id).then(function () {
                commit(constants.DELETE_ROUTINE_SUCCESS, { routine })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.ACTIVATE_ROUTINE]({ commit, state }, { routine, success, failure }) {
            api.activateRoutine(routine.id).then(function () {
                commit(constants.ACTIVATE_ROUTINE_SUCCESS, { routine })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        // Training goals
        [constants.FETCH_TRAINING_GOALS]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.trainingGoalsLoaded && !forceRefresh) {
                if (success) {
                    success(state.trainingGoals);
                }
                return;
            }
            api.getTrainingGoals().then(function (goals) {
                commit(constants.FETCH_TRAINING_GOALS_SUCCESS, { goals })
                if (success) {
                    success(goals);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_TRAINING_GOAL]({ commit, state }, { id, success, failure }) {
            api.getTrainingGoal(id).then(function (goal) {
                if (success) {
                    success(goal);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_ACTIVE_TRAINING_GOAL]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.trainingGoalLoaded && !forceRefresh) {
                if (success) {
                    success(state.trainingGoal);
                }
                return;
            }
            api.getActiveTrainingGoal().then(function (goal) {
                commit(constants.FETCH_ACTIVE_TRAINING_GOAL_SUCCESS, { goal })
                if (success) {
                    success(goal);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_TRAINING_GOAL]({ commit, state }, { goal, success, failure }) {
            api.saveTrainingGoal(goal).then(function (savedGoal) {
                commit(constants.FETCH_TRAINING_GOAL_SUCCESS, { goal: savedGoal })
                if (success) {
                    success(savedGoal);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.ACTIVATE_TRAINING_GOAL]({ commit, state }, { goal, success, failure }) {
            api.activateTrainingGoal(goal.id).then(function () {
                commit(constants.ACTIVATE_TRAINING_GOAL_SUCCESS, { goal })
                if (success) {
                    success(savedGoal);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.DELETE_TRAINING_GOAL]({ commit, state }, { goal, success, failure }) {
            api.deleteTrainingGoal(goal.id).then(function () {
                commit(constants.DELETE_TRAINING_GOAL_SUCCESS, { goal })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },

        // Activities
        [constants.FETCH_ACTIVITIES]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.activitiesLoaded && !forceRefresh) {
                if (success) {
                    success(state.activities);
                }
                return;
            }
            api.listActivities().then(function (activities) {
                commit(constants.FETCH_ACTIVITIES_SUCCESS, { activities });
                if (success) {
                    success(activities);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SELECT_ENERGY_EXPENDITURE_DATE_RANGE]({ commit, state }, { start, end, success, failure }) {
            commit(constants.SELECT_ENERGY_EXPENDITURE_DATE_RANGE_SUCCESS, { start, end });
            if (success) {
                success();
            }
        },
        [constants.FETCH_ENERGY_EXPENDITURES]({ commit, state }, { start, end, success, failure }) {
            if (state.energyExpendituresStart && state.energyExpendituresEnd) {
                if (moment(start).isBefore(state.energyExpendituresStart) || moment(end).isAfter(state.energyExpendituresEnd)) {
                    start = moment.min(moment(start), moment(state.energyExpendituresEnd));
                    end = moment.max(moment(end), moment(state.energyExpendituresStart));
                }
                else {
                    // within already loaded period
                    if (success) {
                        success(state.energyExpenditures);
                    }
                    return;
                }
            }

            api.listEnergyExpenditures(start, end).then(function (energyExpenditures) {
                commit(constants.FETCH_ENERGY_EXPENDITURES_SUCCESS, { start, end, energyExpenditures })
                if (success) {
                    success(energyExpenditures);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_ENERGY_EXPENDITURE]({ commit, state }, { energyExpenditure, success, failure }) {
            api.saveEnergyExpenditure(energyExpenditure).then(function (savedEnergyExpenditure) {
                commit(constants.SAVE_ENERGY_EXPENDITURE_SUCCESS, { energyExpenditure: savedEnergyExpenditure })
                if (success) {
                    success(savedEnergyExpenditure);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        }
    },

    // mutations
    mutations: {
        [constants.LOGOUT_SUCCESS](state) {
            state.diaryDate = new Date();
            state.muscleGroupsLoaded = false;
            state.muscleGroups = [];
            state.exercisesLoaded = false;
            state.exercises = [];
            state.routinesLoaded = false;
            state.routines = [];
            state.activeRoutineLoaded = false;
            state.activateRoutine = undefined;
            state.workoutsStart = undefined;
            state.workoutsEnd = undefined;
            state.workouts = [];
            state.workoutDays = {};
            state.workoutsDisplayStart = undefined;
            state.workoutsDisplayEnd = undefined;
            state.activeTrainingGoalLoaded = false;
            state.activateTrainingGoal = undefined;
            state.trainingGoalsLoaded = false;
            state.trainingGoals = [];
            state.activitiesLoaded = false;
            state.activities = [];
            state.energyExpenditures = [];
            state.energyExpendituresStart = undefined;
            state.energyExpendituresEnd = undefined;
            state.energyExpendituresDisplayStart = undefined;
            state.energyExpendituresDisplayEnd = undefined;
        },
        [constants.FETCH_MUSCLEGROUPS_SUCCESS](state, { muscleGroups }) {
            state.muscleGroups = muscleGroups;
            state.muscleGroupsLoaded = true;
        },
        [constants.FETCH_EXERCISES_SUCCESS](state, { exercises }) {
            state.exercises = exercises;
            state.exercisesLoaded = true;
        },
        [constants.FETCH_ROUTINES_SUCCESS](state, { routines }) {
            state.routines = routines;
            state.routinesLoaded = true;
        },
        [constants.FETCH_ACTIVE_ROUTINE_SUCCESS](state, { routine }) {
            state.activeRoutine = routine;
            state.activeRoutineLoaded = true;
        },
        [constants.SELECT_WORKOUT_DATE_RANGE_SUCCESS](state, { start, end }) {
            state.workoutsDisplayStart = start;
            state.workoutsDisplayEnd = end;
        },
        [constants.FETCH_WORKOUTS_SUCCESS](state, { start, end, workouts }) {
            workouts.forEach(workout => {
                var existing = state.workouts.find(w => w.id == workout.id);
                if (!existing) {
                    workout.time = new Date(workout.time);
                    var day = moment(workout.time).startOf('day').toDate();
                    state.workoutDays[day.getTime()] = true;
                    state.workouts.push(workout);
                }
            });
            if (!state.workoutsStart || moment(start).isBefore(state.workoutsStart)) {
                state.workoutsStart = start;
            }
            if (!state.workoutsEnd || moment(end).isAfter(state.workoutsEnd)) {
                state.workoutsEnd = end;
            }
        },
        [constants.SAVE_EXERCISE_SUCCESS](state, { id, exercise }) {
            if (id) {
                var old = state.exercises.find(x => x.id == id);
                if (old) {
                    deleteExercise(old, state);
                }
            }
            state.exercises.push(exercise);
        },
        [constants.SAVE_1RM_SUCCESS](state, { exerciseId, oneRepMax }) {
            var exercise = state.exercises.find(e => e.id == exerciseId);
            exercise.oneRepMax = oneRepMax;
        },
        [constants.SAVE_ROUTINE_SUCCESS](state, { id, routine }) {
            if (id) {
                var old = state.routines.find(x => x.id == id);
                if (old) {
                    deleteRoutine(old, state);
                }
            }
            state.routines.push(routine);
        },
        [constants.SAVE_WORKOUT_SUCCESS](state, { id, workout }) {
            if (id) {
                var old = state.workouts.find(x => x.id == id);
                if (old) {
                    deleteWorkout(old, state)
                }
            }
            workout.muscleGroupSets = {};
            workout.sets.forEach(s => {
                if (s.exerciseId) {
                    var exercise = state.exercises.find(e => e.id == s.exerciseId);
                    if (exercise) {
                        exercise.targets.forEach(t => {
                            if (workout.muscleGroupSets[t]) {
                                workout.muscleGroupSets[t] += 1;
                            }
                            else {
                                workout.muscleGroupSets[t] = 1;
                            }
                        });
                    }
                }
            });

            state.workouts.push(workout);
        },
        [constants.DELETE_EXERCISE_SUCCESS](state, { exercise }) {
            deleteExercise(exercise, state)
        },
        [constants.DELETE_ROUTINE_SUCCESS](state, { routine }) {
            deleteRoutine(routine, state)
        },
        [constants.ACTIVATE_ROUTINE_SUCCESS](state, { routine }) {
            state.routines.forEach(r => {
                if (r.id === routine.id) {
                    r.active = true;
                    state.activeRoutine = r;
                    state.activeRoutineLoaded = true;
                }
                else {
                    r.active = false;
                }
            });
        },
        [constants.DELETE_WORKOUT_SUCCESS](state, { workout }) {
            deleteWorkout(workout, state)
        },
        [constants.FETCH_TRAINING_GOALS_SUCCESS](state, { goals }) {
            state.trainingGoals = goals;
            state.trainingGoalsLoaded = true;
        },
        [constants.FETCH_ACTIVE_TRAINING_GOAL_SUCCESS](state, { goal }) {
            state.activeTrainingGoal = goal;
            state.activeTrainingGoalLoaded = true;
        },
        [constants.ACTIVATE_TRAINING_GOAL_SUCCESS](state, { goal }) {
            state.trainingGoals.forEach(g => {
                if (g.id === goal.id) {
                    g.active = true;
                    state.activeTrainingGoal = g;
                    state.activeTrainingGoalLoaded = true;
                }
                else {
                    g.active = false;
                }
            });
        },
        [constants.DELETE_TRAINING_GOAL_SUCCESS](state, { goal }) {
            deleteTrainingGoal(goal, state)
        },
        [constants.LOGOUT_SUCCESS](state) {
            // TODO: clear state
        },
        [constants.FETCH_ACTIVITIES_SUCCESS](state, { activities }) {
            state.activities = activities;
            state.activitiesLoaded = true;
        },
        [constants.SELECT_ENERGY_EXPENDITURE_DATE_RANGE_SUCCESS](state, { start, end }) {
            state.energyExpendituresDisplayStart = start;
            state.energyExpendituresDisplayEnd = end;
        },
        [constants.FETCH_ENERGY_EXPENDITURES_SUCCESS](state, { start, end, energyExpenditures }) {
            energyExpenditures.forEach(activity => {
                var existing = state.energyExpenditures.find(a => a.id == activity.id);
                if (!existing) {
                    activity.time = new Date(activity.time);
                    state.energyExpenditures.push(activity);
                }
            });
            if (!state.energyExpendituresStart || moment(start).isBefore(state.energyExpendituresStart)) {
                state.energyExpendituresStart = start;
            }
            if (!state.energyExpendituresEnd || moment(end).isAfter(state.energyExpendituresEnd)) {
                state.energyExpendituresEnd = end;
            }
        },
        [constants.SAVE_ENERGY_EXPENDITURE_SUCCESS](state, { id, energyExpenditure }) {
            if (id) {
                var old = state.energyExpenditures.find(x => x.id == id);
                if (old) {
                    deleteEnergyExpenditure(old, state);
                }
            }

            state.energyExpenditures.push(energyExpenditure);
        },
    }
}
function deleteExercise(exercise, state){
    state.exercises.splice(state.exercises.findIndex(x => x.id == exercise.id), 1);
}
function deleteRoutine(routine, state){
    state.routines.splice(state.routines.findIndex(x => x.id == routine.id), 1);
}
function deleteWorkout(workout, state){
    state.workouts.splice(state.workouts.findIndex(x => x.id == workout.id), 1);
}
function deleteEnergyExpenditure(energyExpenditure, state){
    state.energyExpenditures.splice(state.energyExpenditures.findIndex(x => x.id == energyExpenditure.id), 1);
}