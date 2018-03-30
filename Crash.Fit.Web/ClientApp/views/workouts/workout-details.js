import constants from '../../store/constants'
import api from '../../api'
import utils from '../../utils'
import toaster from '../../toaster'
import draggable from 'vuedraggable'

export default {
    data () {
        return {
            id: null,
            time: null,
            duration: undefined,
            sets: [{ exercise: null, reps: null, weights: null }],
            //exercises: [],
            energyExpenditure: undefined,
            energySpecified: false
        }
    },
    computed: {
        exercises() {
            return this.$store.state.training.exercises;
        },
        totalMinutes() {
            var time = this.duration ? new Date(this.duration) : undefined;
            if (time) {
                return time.getHours() * 60 + time.getMinutes();
            }
            return null;
        },
        weight() {
            if (this.$profile) {
                return this.$profile.weight;
            }
            return null;
        },
        energyExpenditureEstimate() {
            if (this.totalMinutes && this.weight) {
                return constants.WORKOUT_ENERGY_EXPENDITURE * this.totalMinutes * this.weight;
            }
            return null;
        }
    },
    watch: {
        energyExpenditureEstimate() {
            if (!this.energySpecified || !this.energyExpenditure ) {
                this.energyExpenditure = this.formatDecimal(this.energyExpenditureEstimate);
            }
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'exercise-picker': require('../../components/exercise-picker'),
        draggable
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
        setExercise(set, exercise) {
            console.log('setExercise', exercise);
            set.exercise = exercise;
        },
        processNewExercise(set, exerciseName) {
            console.log('processNewExercise', exerciseName);
            if (!exerciseName) {
                set.exercise = undefined;
            }
        },
        save() {
            var self = this;
            var time = self.duration ? new Date(self.duration) : undefined;
            var workout = {
                id: self.id,
                time: self.time,
                hours: time ? time.getHours() : undefined,
                minutes: time ? time.getMinutes() : undefined,
                sets: self.sets.filter(s => s.exercise && s.reps).map(s => { return { exerciseId: s.exercise.id, exerciseName: s.exercise.name, reps: utils.parseFloat(s.reps), weights: utils.parseFloat(s.weights) } }),
                energyExpenditure: self.energySpecified ? self.energyExpenditure : undefined
            };
            self.$store.dispatch(constants.SAVE_WORKOUT, {
                workout,
                success() {
                    toaster.info(self.$t('saveSuccessful'));
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
        populate(workout) {
            var self = this;
            self.id = workout.id;
            self.time = workout.time;
            self.duration = '01.01.2000 ' + (workout.hours || 0) + ':' + (workout.minutes || 0);
            if (workout.id) {
                self.energySpecified = true;
            }
            self.$store.dispatch(constants.FETCH_EXERCISES, {
                success(exercises) {
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
                hours: 1,
                minutes: 0,
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