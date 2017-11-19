import constants from '../../store/constants'
import api from '../../api'
import formatters from '../../formatters'
import utils from '../../utils'
import toaster from '../../toaster'

export default {
    data () {
        return {
            id: undefined,
            name: undefined,
            workouts: [{ name: 'Päivä 1', exercises: [] }],
            exercises: [],
            selectedWorkout: undefined,
            frequencyPresets:[]
        }
    },
    computed: {
    },
    components: {
        'exercise-picker': require('../../components/exercise-picker')
    },
    methods: {
        frequencyText(value) {
            if (!value) {
                return '';
            }
            var option = this.frequencyPresets.find(f => f.value == value);
            if (option) {
                return option.text;
            }
            return `${value} ${this.$t('timesAbbr')} / ${this.$t('weekAbbr')}`;
        },
        addWorkout(){
            var count = this.workouts.length;
            this.workouts.push({ name: 'Päivä ' + (count + 1), exercises: [], expanded: true, frequency: 1 });
        },
        toggleWorkout(workout){
            workout.expanded = !workout.expanded;
        },
        moveWorkoutUp(index) {
            var workout = this.workouts[index];
            this.workouts.splice(index, 1);
            this.workouts.splice(index - 1, 0, workout);
        },
        moveWorkoutDown(index) {
            var workout = this.workouts[index];
            this.workouts.splice(index, 1);
            this.workouts.splice(index + 1, 0, workout);
        },
        deleteWorkout(index) {
            this.workouts.splice(index, 1);
        },
        addExercise(workout){
            workout.exercises.push({exercise: null, sets: null, reps: null, loadFrom: null, loadTo: null});
        },
        copyExercise(workout, index) {
            var original = workout.exercises[index];
            var copy = { exercise: original.exercise, sets: original.sets, reps: original.reps};
            workout.exercises.splice(index, 0, copy);
        },
        moveExerciseUp(workout, index) {
            var exercise = workout.exercises[index];
            workout.exercises.splice(index, 1);
            workout.exercises.splice(index - 1, 0, exercise);
        },
        moveExerciseDown(workout, index) {
            var exercise = workout.exercises[index];
            workout.exercises.splice(index, 1);
            workout.exercises.splice(index + 1, 0, exercise);
        },
        deleteExercise(workout, index) {
            workout.exercises.splice(index, 1);
        },
        processNewExercise(workoutExercise, exerciseName) {
            if (!exerciseName) {
                workoutExercise.exercise = undefined;
            }
            else {
                var found = this.exercises.filter(e => e.name.toLowerCase().indexOf(exerciseName.toLowerCase()) >= 0);
                if (found.length == 0) {
                    var exercise = { id: undefined, name: exerciseName };
                    this.exercises.push(exercise);
                    workoutExercise.exercise = exercise;
                }
                else {
                    workoutExercise.exercise = found[0];
                }
            }
        },
        save() {
            var self = this;
            var routine = {
                id: self.id,
                name: self.name,
                workouts: self.workouts.map(w => {
                    return {
                        id: w.id,
                        name: w.name,
                        frequency: w.frequency,
                        exercises: w.exercises.filter(e => e.exercise).map(e => {
                            return {
                                exerciseId: e.exercise.id,
                                exerciseName: e.exercise.name,
                                sets: utils.parseFloat(e.sets),
                                reps: utils.parseFloat(e.reps),
                                loadFrom: e.loadFrom ? utils.parseFloat(e.loadFrom) : null,
                                loadTo: e.loadTo ? utils.parseFloat(e.loadTo) : null
                            }
                        })
                    }
                })
            };
            self.$store.dispatch(constants.SAVE_ROUTINE, {
                routine,
                success() {
                    toaster.info(self.$t('saveSuccessful'));
                    self.$router.replace({ name: 'routines' });
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            })
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteRoutine() {
            var self = this;
            self.$store.dispatch(constants.DELETE_ROUTINE, {
                routine: { id: self.id },
                success() {
                    self.$router.push({ name: 'routines' });
                },
                failure() {
                    toaster.error(self.$t('deleteFailed'));
                }
            });
        },
        unit: formatters.formatUnit,
        decimal(value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        },
        populate(routine) {
            var self = this;
            self.id = routine.id;
            self.name = routine.name;
            self.$store.dispatch(constants.FETCH_EXERCISES, {
                success(exercises) {
                    self.exercises = exercises;
                    if (routine.workouts) {
                        self.workouts = routine.workouts.map(w => {
                            return {
                                id: w.id,
                                name: w.name,
                                frequency: w.frequency,
                                exercises: w.exercises.map(we => {
                                    return {
                                        exercise: exercises.filter(e => e.id === we.exerciseId)[0],
                                        sets: we.sets,
                                        reps: we.reps,
                                        loadFrom: we.loadFrom,
                                        loadTo: we.loadTo
                                    }
                                })
                            }
                        });
                    }
                    else {
                        self.workouts = [];
                        self.addWorkout();
                    }
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure() {
                    toaster.error(self.$t('fetchFailed'));
                }
            });

        }
    },
    created() {
        var self = this;
         self.frequencyPresets = [
            { value: 1, text: `1 ${self.$t('timesAbbr')} / ${this.$t('weekAbbr')}` },
            { value: 2, text: `2 ${self.$t('timesAbbr')} / ${this.$t('weekAbbr')}` },
            { value: 3, text: `3 ${self.$t('timesAbbr')} / ${this.$t('weekAbbr')}` },
            { value: 4, text: `4 ${self.$t('timesAbbr')} / ${this.$t('weekAbbr')}` },
            { value: 1/2, text: `1 ${self.$t('timesAbbr')} / 2 ${this.$t('weekAbbr')}` },
            { value: 3/2, text: `3 ${self.$t('timesAbbr')} / 2 ${this.$t('weekAbbr')}` },
            { value: 5/2, text: `5 ${self.$t('timesAbbr')} / 2 ${this.$t('weekAbbr')}` },
        ];
        var id = self.$route.params.id;
        if (id == constants.NEW_ID) {
            self.populate({ id: undefined, name: undefined });
        }
        else {
            self.$store.dispatch(constants.FETCH_ROUTINE, {
                id,
                success(routine) {
                    self.populate(routine);
                },
                failure() {
                    toaster.error(self.$t('routineDetails.fetchFailed'));
                }
            });
        }
    },
    mounted() {
    }
}