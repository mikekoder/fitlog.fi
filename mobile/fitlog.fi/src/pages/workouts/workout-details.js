import constants from '../../store/constants'
import utils from '../../utils'
import exercisesMixin from '../../mixins/exercises'

export default {
    mixins:{
        exercisesMixin
    },
  data () {
    return {
        id: null,
        time: undefined,
        sets:[],
        exercises: []
    }
  },
    computed: {
    },
  methods: {
    addSet() {
        var set = { 
            exerciseId: undefined,
            exerciseName: undefined,
            reps: undefined,
            weights: undefined
        };
        this.sets.push(set);
    },
    deleteSet(index){
        this.sets.splice(index, 1);
    },
    copySet(index){
        var set = this.sets[index];
        this.sets.push({...set});
    },

    searchExercise(text, done){
        var self = this;
        done(self.exercises.filter(e => e.name.indexOf(text) >= 0));
    },
    exerciseSelected(set,exercise){
        if(exercise.id){
            set.exercise = exercise;
            set.exerciseId = exercise.id;
            set.exerciseName = exercise.name;
        }
        else{
            set.exerciseName = exercise;
        }
    },
    save() {
        var self = this;
        var workout = {
            id: self.id,
            time: self.time,
            sets: self.sets.map(s => { return { exerciseId: s.exercise ? s.exercise.id : s.exerciseId, exerciseName: s.exercise ? s.exercise.name : s.exerciseName, reps: utils.parseFloat(s.reps), weights: utils.parseFloat(s.weights) } }),
        };
        self.$store.dispatch(constants.SAVE_WORKOUT, {
            workout,
            success() {
                self.$router.replace({ name: 'workouts' });
            },
            failure() {
                self.notifyError(self.$t('saveFailed'));
            }
        });
    },
    cancel() {
        this.$router.go(-1);
    },
    deleteWorkout() {
        var self = this;
        self.$store.dispatch(constants.DELETE_WORKOUT, {
            recipe: { id: self.id },
            success() {
                self.$router.push({ name: 'workouts' });
            },
            failure() {
                self.notifyError(self.$t('deleteFailed'));
            }
        });
    },

    populate(workout) {
        var self = this;
        self.id = workout.id;
        self.time = workout.time;
        
        self.$store.dispatch(constants.FETCH_EXERCISES, {
            success(exercises) {
                self.exercises = exercises.map(e => {return {...e, label: e.name, value: e.name }});
                if(workout.sets){
                    self.sets = workout.sets.map(s => {
                        var exercise = self.exercises.find(e => e.id == s.exerciseId);
                        return { exercise: exercise, exerciseId: exercise.id, exerciseName: exercise.name, reps: s.reps, weight: s.weights};
                    });
                }
                else{
                    self.sets = [];
                    self.addSet();
                }
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() {
                self.notifyError(self.$t('fetchFailed'));
            }
        });
        
        
    }
},
  created () {
    var self = this;
    var id = self.$route.params.id;
    if (id == constants.NEW_ID) {
        self.populate({ id: undefined, time: utils.previousHalfHour() });
    }
    else {
        self.$store.dispatch(constants.FETCH_WORKOUT, {
            id,
            success(workout) {
                self.populate(workout);
            },
            failure(xhr) {
                self.notifyError(self.$t('fetchFailed'));
            }
        });
    }
  },
  mounted(){
        this.$refs.timeInput.focus();
    }
}
