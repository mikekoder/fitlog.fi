import constants from '../../store/constants'
import api from '../../api'
import formatters from '../../formatters'
import utils from '../../utils'
import toaster from '../../toaster'

export default {
    data () {
        return {
            id: null,
            time: null,
            sets: [{ exercise: null, reps: null, weights: null }],
            exercises: [],
        }
    },
    computed: {
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'exercise-picker': require('../../components/exercise-picker')
    },
    methods: {
        addSet(){
            this.sets.push({exercise: null, reps: null, weights: null});
        },
        copySet(index) {
            var original = this.sets[index];
            var copy = { exercise: original.exercise, reps: original.reps, weights: original.weights};
            this.sets.splice(index, 0, copy);
        },
        moveSetUp(index){
            var set = this.sets[index];
            this.sets.splice(index, 1);
            this.sets.splice(index - 1, 0, set);
        },
        moveSetDown(index) {
            var set = this.sets[index];
            this.sets.splice(index, 1);
            this.sets.splice(index + 1, 0, set);
        },
        deleteSet(index) {
            this.sets.splice(index, 1);
        },
        processNewExercise(set, exerciseName) {
            if (!exerciseName) {
                set.exercise = undefined;
            }
            else {
                var found = this.exercises.filter(e => e.name.toLowerCase().indexOf(exerciseName.toLowerCase()) >= 0);
                if (found.length == 0) {
                    var exercise = { id: undefined, name: exerciseName };
                    this.exercises.push(exercise);
                    set.exercise = exercise;
                }
                else {
                    set.exercise = found[0];
                }
            }
        },
        save() {
            var self = this;
            var workout = {
                id: self.id,
                time: self.time,
                sets: self.sets.filter(s => s.exercise && s.reps).map(s => { return { exerciseId: s.exercise.id, exerciseName: s.exercise.name, reps: utils.parseFloat(s.reps), weights: utils.parseFloat(s.weights) } })
            };
            self.$store.dispatch(constants.SAVE_WORKOUT, {
                workout,
                success() {
                    self.$router.replace({ name: 'workouts' });
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            })
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteWorkout() {
            var self = this;
            self.$store.dispatch(constants.DELETE_WORKOUT, {
                workout: { id: self.id },
                success() {
                    self.$router.replace({ name: 'workouts' });
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
        populate(workout) {
            var self = this;
            self.id = workout.id;
            self.time = workout.time;
            self.$store.dispatch(constants.FETCH_EXERCISES, {
                success(exercises) {
                    self.exercises = exercises;
                    if (workout.sets && workout.sets.length > 0) {
       
                        self.sets = workout.sets.map(s => {
                            var exercise = self.exercises.find(e => e.id === s.exerciseId);
                            
                            if (workout.id) {
                                 return { exercise, reps: s.reps, weights: s.weights };
                            }
                            // template
                            var weights = (s.load || s.load == 0) && exercise.oneRepMax ? s.load / 100 * exercise.oneRepMax : undefined;
                            if (weights) {
                                weights = utils.roundToNearest(weights, 2.5);
                            }
                            return { exercise, reps: s.reps, weights };
                        });
                        
                    }
                    else {
                        self.sets = [{ exercise: null, reps: null, weights: null }];
                    }
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure() {
                    toaster.error(self.$t('routineDetails.fetchFailed'));
                }
            });
        }
    },
    created() {
        

        var self = this;
        var id = self.$route.params.id;
        var routineId = self.$route.query[constants.ROUTINE_PARAM];
        var routineWorkoutId = self.$route.query[constants.WORKOUT_PARAM];
        if (id == constants.NEW_ID) {
            var workout = {
                id: undefined, 
                time: utils.previousHalfHour(),
                sets: []
            };
            if(routineId && routineWorkoutId){
                self.$store.dispatch(constants.FETCH_ROUTINE, {
                    id: routineId,
                    success(routine) {
                        var routineWorkout = routine.workouts.find(w => w.id === routineWorkoutId);
                        routineWorkout.exercises.forEach(e => {
                            var loadFrom = e.loadFrom || e.loadFrom == 0 ? e.loadFrom : e.loadTo;
                            var loadTo = e.loadTo || e.loadTo == 0 ? e.loadTo : e.loadFrom;
                            var step = (loadFrom || loadFrom == 0) && (loadTo || loadTo == 0) ? (loadTo - loadFrom) / (e.sets - 1) : undefined;
                            for (var i = 0; i < e.sets; i++){
                                workout.sets.push({ exerciseId: e.exerciseId, reps: e.reps, load: (step || step == 0) ? loadFrom + i * step : undefined });
                            }
                        });
                        self.populate(workout);
                    },
                    failure() {
                        toaster.error(self.$t('fetchFailed'));
                    }
                });
            }
            else {
                self.populate(workout);
            }   
        }
        else {
            self.$store.dispatch(constants.FETCH_WORKOUT, {
                id,
                success(workout) {
                    self.populate(workout);
                },
                failure() {
                    toaster.error(self.$t('fetchFailed'));
                }
            });
        }

    },
    mounted() {
    }
}