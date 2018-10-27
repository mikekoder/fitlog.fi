import api from '../api'
import constants from './constants'
import moment from 'moment'

export default {
    state: {

        diaryDate: new Date(),

        muscleGroupsLoaded: false,
        muscleGroups: [],

        equipmentsLoaded: false,
        equipments: [],


        exercisesLoaded: false,
        exercises: [],
        latestExercisesLoaded: false,
        latestExercises: [],
        mostUsedExercisesLoaded: false,
        mostUsedExercises: [],

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
        [constants.SELECT_WORKOUT_DIARY_DATE]({ commit, state }, { date }) {
            state.diaryDate = date;
        },
        // MuscleGroups 
        [constants.FETCH_MUSCLEGROUPS]({ commit, state }, { forceRefresh}) {
            if (state.muscleGroupsLoaded && !forceRefresh) {
                return Promise.resolve(state.muscleGroups);
            }
            return api.listMuscleGroups().then(response => {
                commit(constants.FETCH_MUSCLEGROUPS_SUCCESS, { muscleGroups: response.data });
                return response.data;
            });
        },
        // Equipment
        [constants.FETCH_EQUIPMENT]({ commit, state }, { forceRefresh }) {
            if (state.equipmentsLoaded && !forceRefresh) {
                return Promise.resolve(state.equipments);
            }
            return api.listEquipments().then(response => {
                commit(constants.FETCH_EQUIPMENT_SUCCESS, { equipments: response.data });
                return response.data;
            });
        },
        // Workouts
        [constants.SELECT_WORKOUT_DATE_RANGE]({ commit, state }, { start, end}) {
            commit(constants.SELECT_WORKOUT_DATE_RANGE_SUCCESS, { start, end });
        },
        [constants.FETCH_WORKOUTS]({ commit, state }, { start, end, force}) {
            if (state.workoutsStart && state.workoutsEnd) {
                if (moment(start).isBefore(state.workoutsStart) || moment(end).isAfter(state.workoutsEnd) || force) {
                    start = moment.min(moment(start), moment(state.workoutsEnd));
                    end = moment.max(moment(end), moment(state.workoutsStart));
                }
                else {
                    return Promise.resolve(state.workouts);
                }
            }

            return api.listWorkouts(start, end).then(response => {
                commit(constants.FETCH_WORKOUTS_SUCCESS, { start, end, workouts: response.data })
                return response.data;
            });
        },
        [constants.FETCH_WORKOUT]({ commit, state }, { id }) {
            return api.getWorkout(id).then(response => {
                return response.data;
            });
        },
        [constants.SAVE_WORKOUT]({ commit, state }, { workout }) {
            return api.saveWorkout(workout).then(response => {
                var savedWorkout = response.data;
                savedWorkout.time = new Date(savedWorkout.time);
                commit(constants.SAVE_WORKOUT_SUCCESS, { id: workout.id, workout: savedWorkout })
                return savedWorkout;
            });
        },
        [constants.DELETE_WORKOUT]({ commit, state }, { workout }) {
            return api.deleteWorkout(workout.id).then(response => {
                commit(constants.DELETE_WORKOUT_SUCCESS, { workout })
            });
        },
        // Exercises
        [constants.FETCH_EXERCISES]({ commit, state }, { forceRefresh }) {
            if (state.exercisesLoaded && !forceRefresh) {
                return Promise.resolve(state.exercises);
            }
            return api.listExercises().then(response => {
                commit(constants.FETCH_EXERCISES_SUCCESS, { exercises: response.data });
                return response.data;
            });
        },

        [constants.FETCH_LATEST_EXERCISES]({ commit, state }, { forceRefresh}) {
            if (state.latestExercisesLoaded && !forceRefresh) {
                return Promise.resolve(state.latestExercises);
            }
            api.listLatestExercises().then(response => {
                commit(constants.FETCH_LATEST_EXERCISES_SUCCESS, { exercises: response.data });
                return response.data;
            });
        },
        [constants.FETCH_MOST_USED_EXERCISES]({ commit, state }, { forceRefresh }) {
            if (state.mostUsedExercisesLoaded && !forceRefresh) {
                return Promise.resolve(state.mostUsedExercises);
            }
            api.listMostUsedExercises().then(response => {
                commit(constants.FETCH_MOST_USED_EXERCISES_SUCCESS, { exercises: response.data });
                return response.data;
            });
        },
        [constants.FETCH_EXERCISE]({ commit, state }, { id }) {
            return api.getExercise(id).then(response => {
                return response.data;
            });
        },
        [constants.SAVE_EXERCISE]({ commit, state }, { exercise}) {
            return api.saveExercise(exercise).then(response => {
                var savedExercise = response.data;
                commit(constants.SAVE_EXERCISE_SUCCESS, { id: savedExercise.id, exercise: savedExercise })
                return savedExercise;
            });
        },
        [constants.SAVE_1RM]({ commit, state }, { exerciseId, oneRepMax }) {
            return api.save1RM(exerciseId, oneRepMax).then(response => {
                commit(constants.SAVE_1RM_SUCCESS, { exerciseId, oneRepMax });
            });
        },
        [constants.DELETE_EXERCISE]({ commit, state }, { exercise }) {
            return api.deleteExercise(exercise.id).then(response => {
                commit(constants.DELETE_EXERCISE_SUCCESS, { exercise })
            });
        },
        // Routines
        [constants.FETCH_ROUTINES]({ commit, state }, { forceRefresh }) {
            if (state.routinesLoaded && !forceRefresh) {
                return Promise.resolve(state.routines);
            }
            return api.listRoutines().then(response => {
                var routines = response.data;
                commit(constants.FETCH_ROUTINES_SUCCESS, { routines })
                var active = routines.find(r => r.active);
                if (active) {
                    api.getRoutine(active.id).then(response2 => {
                        commit(constants.FETCH_ACTIVE_ROUTINE_SUCCESS, { routine: response2.data });
                    });
                }
                return routines;
            });
        },
        [constants.FETCH_ROUTINE]({ commit, state }, { id }) {
            return api.getRoutine(id).then(response => {
                return response.data;
            });
        },
        [constants.FETCH_ROUTINE_WORKOUT]({ commit, state }, { id }) {
            return api.getRoutineWorkout(id).then(response => {
                return response.data;
            });
        },
        [constants.SAVE_ROUTINE]({ commit, state }, { routine }) {
            return api.saveRoutine(routine).then(response => {
                commit(constants.SAVE_ROUTINE_SUCCESS, { id: routine.id, routine: response.data });
                return response.data;
            });
        },
        [constants.DELETE_ROUTINE]({ commit, state }, { routine }) {
            return api.deleteRoutine(routine.id).then(response => {
                commit(constants.DELETE_ROUTINE_SUCCESS, { routine })
            });
        },
        [constants.ACTIVATE_ROUTINE]({ commit, state }, { routine }) {
            return api.activateRoutine(routine.id).then(response => {
                api.getRoutine(routine.id).then(response2 => {
                    commit(constants.ACTIVATE_ROUTINE_SUCCESS, { routine: response2.data });
                });
            });
        },
        // Training goals
        [constants.FETCH_TRAINING_GOALS]({ commit, state }, { forceRefresh }) {
            if (state.trainingGoalsLoaded && !forceRefresh) {
                return Promise.resolve(state.trainingGoals);
            }
            return api.getTrainingGoals().then(response => {
                commit(constants.FETCH_TRAINING_GOALS_SUCCESS, { goals: response.data });
                return response.data;
            });
        },
        [constants.FETCH_TRAINING_GOAL]({ commit, state }, { id }) {
            return api.getTrainingGoal(id).then(response => {
                return response.data;
            });
        },
        [constants.FETCH_ACTIVE_TRAINING_GOAL]({ commit, state }, { forceRefresh }) {
            if (state.trainingGoalLoaded && !forceRefresh) {
                return Promise.resolve(state.trainingGoal);
            }
            return api.getActiveTrainingGoal().then(response => {
                commit(constants.FETCH_ACTIVE_TRAINING_GOAL_SUCCESS, { goal: response.data });
                return response.data;
            });
        },
        [constants.SAVE_TRAINING_GOAL]({ commit, state }, { goal }) {
            return api.saveTrainingGoal(goal).then(response => {
                commit(constants.FETCH_TRAINING_GOAL_SUCCESS, { goal: response.data });
                return response.data;
            });
        },
        [constants.ACTIVATE_TRAINING_GOAL]({ commit, state }, { goal }) {
            return api.activateTrainingGoal(goal.id).then(response => {
                commit(constants.ACTIVATE_TRAINING_GOAL_SUCCESS, { goal });
                return response.data;
            });
        },
        [constants.DELETE_TRAINING_GOAL]({ commit, state }, { goal }) {
            return api.deleteTrainingGoal(goal.id).then(response => {
                commit(constants.DELETE_TRAINING_GOAL_SUCCESS, { goal });
            });
        },

        // Activities
        [constants.FETCH_ACTIVITIES]({ commit, state }, { forceRefresh }) {
            if (state.activitiesLoaded && !forceRefresh) {
                return Promise.resolve(state.activities);
            }
            return api.listActivities().then(response => {
                commit(constants.FETCH_ACTIVITIES_SUCCESS, { activities: response.data });
                return response.data;
            });
        },
        [constants.SELECT_ENERGY_EXPENDITURE_DATE_RANGE]({ commit, state }, { start, end }) {
            commit(constants.SELECT_ENERGY_EXPENDITURE_DATE_RANGE_SUCCESS, { start, end });
            return Promise.resolve();
        },
        [constants.FETCH_ENERGY_EXPENDITURES]({ commit, state }, { start, end }) {
            if (state.energyExpendituresStart && state.energyExpendituresEnd) {
                if (moment(start).isBefore(state.energyExpendituresStart) || moment(end).isAfter(state.energyExpendituresEnd)) {
                    start = moment.min(moment(start), moment(state.energyExpendituresEnd));
                    end = moment.max(moment(end), moment(state.energyExpendituresStart));
                }
                else {
                    return Promise.resolve(state.energyExpenditures);
                }
            }

            return api.listEnergyExpenditures(start, end).then(response => {
                commit(constants.FETCH_ENERGY_EXPENDITURES_SUCCESS, { start, end, energyExpenditures: response.data })
                return response.data;
            });
        },
        [constants.SAVE_ENERGY_EXPENDITURE]({ commit, state }, { energyExpenditure }) {
            return api.saveEnergyExpenditure(energyExpenditure).then(response => {
                commit(constants.SAVE_ENERGY_EXPENDITURE_SUCCESS, { energyExpenditure: response.data })
                return response.data;
            });
        },
        [constants.DELETE_ENERGY_EXPENDITURE]({ commit, state }, { energyExpenditure }) {
            return api.deleteEnergyExpenditure(energyExpenditure.id).then(reponse => {
                commit(constants.DELETE_ENERGY_EXPENDITURE_SUCCESS, { energyExpenditure })
                return Promise.resolve(energyExpenditure);
            });
        }
    },

    // mutations
    mutations: {
        [constants.TRAINING_CLEAR](state) {
            state.diaryDate = new Date();
            state.muscleGroupsLoaded = false;
            state.muscleGroups = [];
            state.exercisesLoaded = false;
            state.exercises = [];
            state.routinesLoaded = false;
            state.routines = [];
            state.activeRoutineLoaded = false;
            state.activeRoutine = undefined;
            state.workoutsStart = undefined;
            state.workoutsEnd = undefined;
            state.workouts = [];
            state.workoutDays = {};
            state.workoutsDisplayStart = undefined;
            state.workoutsDisplayEnd = undefined;
            state.activeTrainingGoalLoaded = false;
            state.activeTrainingGoal = undefined;
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
        [constants.FETCH_EQUIPMENT_SUCCESS](state, { equipments }) {
            state.equipments = equipments;
            state.equipmentsLoaded = true;
        },
        [constants.FETCH_EXERCISES_SUCCESS](state, { exercises }) {
            state.exercises = exercises;
            state.exercisesLoaded = true;
        },
        [constants.FETCH_LATEST_EXERCISES_SUCCESS](state, { exercises }) {
            state.latestExercises = exercises;
            state.latestExercisesLoaded = true;
        },
        [constants.FETCH_MOST_USED_EXERCISES_SUCCESS](state, { exercises }) {
            state.mostUsedExercises = exercises;
            state.mostUsedExercisesLoaded = true;
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
                    state.activeRoutine = routine;
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
        [constants.SAVE_ENERGY_EXPENDITURE_SUCCESS](state, { energyExpenditure }) {
            if (energyExpenditure.id) {
                var old = state.energyExpenditures.find(x => x.id == energyExpenditure.id);
                if (old) {
                    deleteEnergyExpenditure(old, state);
                }
            }

            state.energyExpenditures.push(energyExpenditure);
        },
        [constants.DELETE_ENERGY_EXPENDITURE_SUCCESS](state, { energyExpenditure }) {
            deleteEnergyExpenditure(energyExpenditure, state); 
        }
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