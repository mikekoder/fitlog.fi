import constants from '../../store/constants'
import utils from '../../utils'
import Vue from 'vue'
import Help from './routine-help'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    components: {
        'routine-help': Help
    },
    data () {
        return {
            tab: 'tab-0',
            id: undefined,
            name: undefined,
            workouts: [{ name: 'Päivä 1', groups: [] }],
            exercises: [],
            selectedWorkout: undefined,
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
            this.addGroup(this.workouts[count]);
        },
        deleteWorkout(index) {
            this.workouts.splice(index, 1);
        },
        addGroup(workout){
            var group = {exercise: null, rows:[]};
            this.addRow(group);
            workout.groups.push(group);
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
            var self = this;
            if(!exercise.id){
                exercise = self.exercises.find(e => e.id == exercise);
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
            var self = this;
            var routine = {
                id: self.id,
                name: self.name,
                workouts: self.workouts.map(w => {
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
           
            self.$store.dispatch(constants.SAVE_ROUTINE, {
                routine,
                success() {
                    self.notifySuccess(self.$t('saveSuccessful'));
                    self.$router.replace({ name: 'routines' });
                },
                failure() {
                    self.notifyError(self.$t('saveFailed'));
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
                    self.notifyError(self.$t('deleteFailed'));
                }
            });
        },
        populate(routine) {
            var self = this;
            self.id = routine.id;
            self.name = routine.name;
            self.workouts = [];
            self.$store.dispatch(constants.FETCH_EXERCISES, {
                success(exercises) {
                    self.exercises = exercises.map(e => {return {...e, label: e.name, value: e }});
                    if (routine.workouts) {
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
                                    var exercise = self.exercises.find(e2 => e2.id == e.exerciseId);
                                    group = {
                                        exercise: exercise,
                                        rows: []
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

                            self.workouts.push(workout);
                        });
                    }
                    else {
                        self.workouts = [];
                        self.addWorkout();
                    }
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure() {
                    self.notifyError(self.$t('fetchFailed'));
                }
            });

        },
        showHelp(){
            this.$refs.help.open();
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
                    self.notifyError(self.$t('fetchFailed'));
                }
            });
        }

        
    },
    mounted() {
        if(!this.name){
            this.$refs.nameInput.focus();
        }
    }
}
