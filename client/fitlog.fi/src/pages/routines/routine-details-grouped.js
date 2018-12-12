import constants from '../../store/constants'
import utils from '../../utils'
import Vue from 'vue'
import Help from './routine-help'
import ExercisesMixin from '../../mixins/exercises'
import PageMixin from '../../mixins/page'
import ExercisePicker from '../../components/exercise-picker.vue'
import api from '../../api'

export default {
    mixins: [PageMixin],
    components: {
        'routine-help': Help,
        'exercise-picker':ExercisePicker
    },
    data () {
        return {
            tab: 'tab-0',
            id: undefined,
            name: undefined,
            workouts: [{ name: 'Päivä 1', groups: [] }],
            selectedWorkout: undefined,
            selectedGroup: undefined,
            frequencyPresets:[]
        }
    },
    computed: {
        canSave(){
            return this.name && this.workouts.length > 0;
        }
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
            this.workouts.push({ name: 'Päivä ' + (count + 1), groups: [], frequency: 1 });
        },
        deleteWorkout(index) {
            this.workouts.splice(index, 1);
        },
        addGroup(workout){
            var group = {exercise: null, rows:[], collapsed: false};
            this.addRow(group);
            workout.groups.push(group);
            this.selectExercise(group);
        },
        deleteGroup(workout,index){
            workout.groups.splice(index, 1);
        },

        addRow(group){
            group.rows.push({ sets: undefined, reps: undefined, loadFrom: undefined });
        },
        copyRow(group, row) {
            //var row = group.rows[index];
            group.rows.push({ ...row})
        },
        deleteRow(group, index) {
            group.rows.splice(index, 1);
        },
        exerciseSelected(group,exercise){
            if(!exercise.id){
                exercise = this.exercises.find(e => e.id == exercise);
            }
            group.exercise = exercise;
            //group.exerciseId = exercise.id;
            group.exerciseName = exercise.name;
        },
        processNewExercise(workoutExercise, exerciseName) {
            if (!exerciseName) {
                workoutExercise.exercise = undefined;
            }
        },
        save() {
            var routine = {
                id: this.id,
                name: this.name,
                workouts: this.workouts.map(w => {
                    var exercises = [];
                    w.groups.filter(g => g.exercise).forEach((g, index) => {
                        g.rows.forEach(r => {
                            exercises.push({
                                exerciseId: g.exercise.id,
                                exerciseName: g.exercise.name,
                                sets: utils.parseFloat(r.sets),
                                reps: utils.parseFloat(r.reps),
                                load: utils.parseFloat(r.load),
                                loadFrom: r.loadFrom ? utils.parseFloat(r.loadFrom) : null,
                                loadTo: r.loadTo ? utils.parseFloat(r.loadTo) : null
                            });
                        });
                    });
                    return {
                        id: w.id,
                        name: w.name,
                        frequency: w.frequency,
                        exercises: exercises
                    };
                })
            };

            //alert(JSON.stringify(routine));
           
            this.$store.dispatch(constants.SAVE_ROUTINE, {
                routine
            }).then(_ => {
                this.notifySuccess(this.$t('saveSuccessful'));
                    this.$router.replace({ name: 'routines' });
            }).catch(_ => {
                this.notifyError(this.$t('saveFailed'));
            });
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteRoutine() {
            this.$store.dispatch(constants.DELETE_ROUTINE, {
                routine: { id: this.id }
            }).then(_ => {
                this.$router.push({ name: 'routines' });
            }).catch(_ => {
                this.notifyError(this.$t('deleteFailed'));
            });
        },
        populate(routine) {
            this.id = routine.id;
            this.name = routine.name;
            this.workouts = [];
            var exerciseIds;
            if(routine.workouts && routine.workouts.length > 0){
              exerciseIds = routine.workouts.map(w => w.exercises.map(e => e.exerciseId)).reduce((a,b) => a.concat(b));
            }
            else {
              exerciseIds = [];
            }
            api.getExercises(exerciseIds).then(response => {
              var exercises = response.data;
              if (routine.workouts && routine.workouts.length > 0) {
                  routine.workouts.forEach(w => {
                      var workout = {
                          id: w.id,
                          name: w.name,
                          frequency: w.frequency,
                          groups: []
                      };

                      var previousGroup = undefined;
                      var previousExerciseId = undefined;

                      w.exercises.forEach(e => {
                          var group;
                          if(e.exerciseId == previousExerciseId){
                              group = previousGroup;
                          }
                          else {
                              var exercise = exercises.find(e2 => e2.id == e.exerciseId);
                              group = {
                                  exercise: exercise,
                                  rows: [],
                                  collapsed: true
                              };
                              workout.groups.push(group);
                          }

                          group.rows.push({
                              sets: e.sets,
                              reps: e.reps,
                              loadFrom: e.loadFrom,
                              loadTo: e.loadTo
                          });

                          previousGroup = group;
                          previousExerciseId = e.exerciseId;
                      });

                      this.workouts.push(workout);
                  });
              }
              else {
                  this.workouts = [];
                  this.addWorkout();
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
        }
    },
    created() {
         this.frequencyPresets = [
            { value: 1, label: `1 ${this.$t('timesAbbr')} / ${this.$t('weekAbbr')}` },
            { value: 2, label: `2 ${this.$t('timesAbbr')} / ${this.$t('weekAbbr')}` },
            { value: 3, label: `3 ${this.$t('timesAbbr')} / ${this.$t('weekAbbr')}` },
            { value: 4, label: `4 ${this.$t('timesAbbr')} / ${this.$t('weekAbbr')}` },
            { value: 1/2, label: `1 ${this.$t('timesAbbr')} / 2 ${this.$t('weekAbbr')}` },
            { value: 3/2, label: `3 ${this.$t('timesAbbr')} / 2 ${this.$t('weekAbbr')}` },
            { value: 5/2, label: `5 ${this.$t('timesAbbr')} / 2 ${this.$t('weekAbbr')}` },
        ];
        var id = this.$route.params.id;
        if (id == constants.NEW_ID) {
            this.populate({ id: undefined, name: undefined, workouts: [] });
        }
        else {
            this.$store.dispatch(constants.FETCH_ROUTINE, {
                id
            }).then(routine => {
                this.populate(routine);
            }).catch(_ => {
                this.notifyError(this.$t('fetchFailed'));
            });
        }

        
    },
    mounted() {
        if(!this.name){
            this.$refs.nameInput.focus();
        }
    }
}
