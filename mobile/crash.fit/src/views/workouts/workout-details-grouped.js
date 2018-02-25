import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem,QDatetime, QFab, QFabAction, QTooltip,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator } from 'quasar'
import constants from '../../store/constants'
import utils from '../../utils'
import exercisesMixin from '../../mixins/exercises'
import { Toast } from 'quasar'

export default {
    mixins:{
        exercisesMixin
    },
  data () {
    return {
        id: null,
        time: undefined,
        groups:[],
        exercises: []
    }
  },
  components: {
        QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem,QDatetime, QFab, QFabAction, QTooltip, QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator
    },
    computed: {
    },
  methods: {
    addGroup(){
        var group = {
            exercise: undefined,
            sets: []
        }
        this.groups.push(group);
        this.addSet(group);
    },
    copyGroup(group){
        this.groups.push({...group});
    },
    deleteGroup(index){
        this.groups.splice(index, 1);
    },
    addSet(group) {
        var set = { 
            reps: undefined,
            weights: undefined
        };
        group.sets.push(set);
    },
    
    deleteSet(group, index){
        group.sets.splice(index, 1);
    },
    copySet(group, set){
        group.sets.push({...set});
    },

    searchExercise(text, done){
        var self = this;
        done(self.exercises.filter(e => e.name.indexOf(text) >= 0));
    },
    exerciseSelected(group,exercise){
        if(exercise.id){
            group.exercise = exercise;
            group.exerciseId = exercise.id;
            group.exerciseName = exercise.name;
        }
        else {
            group.exerciseName = exercise;
        }
    },
    save() {
        var self = this;
        var workout = {
            id: self.id,
            time: self.time,
            sets: [] 
        };
        self.groups.forEach(g => {
            g.sets.forEach(s => {
                workout.sets.push({ exerciseId: g.exercise ? g.exercise.id : g.exerciseId, exerciseName: g.exercise ? g.exercise.name : g.exerciseName, reps: utils.parseFloat(s.reps), weights: utils.parseFloat(s.weights) })
            });
        });
        self.$store.dispatch(constants.SAVE_WORKOUT, {
            workout,
            success() {
                self.$router.replace({ name: 'workouts' });
            },
            failure() {
                toaster.error(self.$t('saveFailed'));
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
                Toast.create(self.$t('deleteFailed'));
            }
        });
    },

    populate(workout) {
        var self = this;
        self.id = workout.id;
        self.time = workout.time;
        self.groups = [];
        self.$store.dispatch(constants.FETCH_EXERCISES, {
            success(exercises) {
                self.exercises = exercises.map(e => {return {...e, label: e.name, value: e }});
                self.groups = [];
                if(workout.sets){

                    var previousGroup = undefined;
                    var previousExerciseId = undefined;

                    workout.sets.forEach(s => {
                        var group;
                        if(s.exerciseId == previousExerciseId){
                            group = previousGroup;
                        }
                        else {
                            var exercise = self.exercises.find(e => e.id == s.exerciseId);
                            group = {
                                exercise: exercise,
                                sets: []
                            };
                            self.groups.push(group);
                        }
                        group.sets.push({ reps: s.reps, weights: s.weights});

                        previousGroup = group;
                        previousExerciseId = s.exerciseId;

                    });
                }
                else {
                    self.addGroup();
                }
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() {
                Toast.create(self.$t('fetchFailed'));
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
                Toast.create(self.$t('fetchFailed'));
            }
        });
    }
  },
  beforeDestroy () {

  }
}
