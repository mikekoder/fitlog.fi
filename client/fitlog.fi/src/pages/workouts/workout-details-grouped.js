import constants from '../../store/constants'
import utils from '../../utils'
import ExercisesMixin from '../../mixins/exercises'
import PageMixin from '../../mixins/page'
import Help from './workout-help'
import WorkoutComment from './workout-comment'
import ExercisePicker from '../../components/exercise-picker.vue'
import api from '../../api'
import Vue from 'vue';

export default {
    mixins:[ExercisesMixin, PageMixin],
    components: {
      'workout-help': Help,
      'exercise-picker':ExercisePicker,
      'workout-comment': WorkoutComment
    },
  data () {
    return {
      id: null,
      time: undefined,
      duration: undefined,
      comment: null,
      groups:[],
      energyExpenditure: undefined,
      energySpecified: false,
      selectedGroup: undefined
    }
  },
    computed: {
      canSave(){
        return this.time && this.groups.length > 0 && this.groups.find(g => g.exercise);
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
  methods: {
    addGroup(){
      var group = {
        exercise: undefined,
        sets: [],
        collapsed: false
      }
      this.groups.push(group);
      this.addSet(group);
      this.selectExercise(group);
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
      done(this.exercises.filter(e => e.name.indexOf(text) >= 0));
    },
    save() {
        var duration = this.duration ? new Date(this.duration) : undefined;
        var workout = {
          id: this.id,
          time: this.time,
          hours: duration ? duration.getHours() : undefined,
          minutes: duration ? duration.getMinutes() : undefined,
          comment: this.comment,
          sets: [],
          energyExpenditure: this.energySpecified ? this.energyExpenditure : undefined
        };
        this.groups.forEach(g => {
          g.sets.forEach(s => {
            workout.sets.push({ exerciseId: g.exercise ? g.exercise.id : g.exerciseId, exerciseName: g.exercise ? g.exercise.name : g.exerciseName, reps: utils.parseFloat(s.reps), weights: utils.parseFloat(s.weights) })
          });
        });
        this.$store.dispatch(constants.SAVE_WORKOUT, {
          workout
        }).then(savedWorkout => {
          this.id = savedWorkout.id;
          this.notifySuccess(this.$t('saveSuccessful'));
        }).catch(_ => {
          this.notifyError(this.$t('saveFailed'));
        });
    },
    cancel() {
        this.$router.go(-1);
    },
    deleteWorkout() {
        this.$store.dispatch(constants.DELETE_WORKOUT, {
            recipe: { id: this.id }
        }).then(_ => {
            this.$router.push({ name: 'workouts' });
        }).catch(_ => {
            this.notifyError(this.$t('deleteFailed'));
        });
    },

    populate(workout) {
        this.id = workout.id;
        this.time = workout.time;
        this.duration = '01.01.2000 ' + (workout.hours || 0) + ':' + (workout.minutes || 0);
        this.comment = workout.comment;
        this.groups = [];
        var exerciseIds = workout.sets.map(s => s.exerciseId);
        api.getExercises(exerciseIds).then(response => {
            var exercises = response.data;
            if(workout.sets && workout.sets.length > 0){

                var previousGroup = undefined;
                var previousExerciseId = undefined;

                workout.sets.forEach(s => {
                    var group;
                    var exercise = exercises.find(e => e.id == s.exerciseId);

                    if(s.exerciseId == previousExerciseId){
                        group = previousGroup;
                    }
                    else {
                        group = {
                            exercise: exercise,
                            sets: [],
                            collapsed: true
                        };
                        this.groups.push(group);
                    }
                    if(workout.id){
                        group.sets.push({ reps: s.reps, weights: s.weights});
                    }
                    else {
                        var weights = (s.load || s.load == 0) && exercise.oneRepMax ? s.load / 100 * exercise.oneRepMax : undefined;
                        if (weights) {
                            weights = utils.roundToNearest(weights, 2.5);
                        }
                        group.sets.push({ reps: s.reps, weights: weights});
                    }
                    previousGroup = group;
                    previousExerciseId = s.exerciseId;

                });
            }
            else {
              //this.addGroup();
            }
            this.$store.commit(constants.LOADING_DONE);
        }).catch(_ => {
            this.notifyError(this.$t('fetchFailed'));
        });
    },
    selectExercise(group){
      this.selectedGroup = group;
      this.$refs.exercisePicker.show(this.selectedGroup.exercise);
    },
    exerciseSelected(exercise){
      this.selectedGroup.exercise = exercise;
      this.$refs.exercisePicker.hide();
    },
    showHelp(){
        this.$refs.help.open();
    },
    showComment(){
      this.$refs.comment.open(this.comment);
    },
    commentOk(comment){
      this.comment = comment;
    }
  },
  created () {
    var id = this.$route.params.id;
    var routineId = this.$route.query[constants.ROUTINE_PARAM];
    var routineWorkoutId = this.$route.query[constants.WORKOUT_PARAM];

    if (id == constants.NEW_ID) {
        var workout = {
            id: undefined, 
            time: utils.previousHalfHour(),
            hours: 1,
            minutes: 0,
            sets: []
        };
        if(routineId && routineWorkoutId){
            this.$store.dispatch(constants.FETCH_ROUTINE, {
                id: routineId
            }).then(routine => {
                var routineWorkout = routine.workouts.find(w => w.id === routineWorkoutId);
                routineWorkout.exercises.forEach(e => {
                    var loadFrom = e.loadFrom || e.loadFrom == 0 ? e.loadFrom : e.loadTo;
                    var loadTo = e.loadTo || e.loadTo == 0 ? e.loadTo : e.loadFrom;
                    var step = (loadFrom || loadFrom == 0) && (loadTo || loadTo == 0) ? (loadTo - loadFrom) / (e.sets - 1) : undefined;
                    for (var i = 0; i < e.sets; i++){
                        workout.sets.push({ exerciseId: e.exerciseId, reps: e.reps, load: (step || step == 0) ? loadFrom + i * step : undefined });
                    }
                });
                this.populate(workout);
            }).catch(_ => {
                this.notifyError(this.$t('fetchFailed'));
            });
        }
        else {
            this.populate(workout);
        }  
    }
    else {
        this.$store.dispatch(constants.FETCH_WORKOUT, {
            id
        }).then(workout => {
            this.populate(workout);
        }).catch(_ => {
            this.notifyError(this.$t('fetchFailed'));
        });
    }
  },
  beforeDestroy () {

  }
}
