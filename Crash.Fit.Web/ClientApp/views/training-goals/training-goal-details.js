import constants from '../../store/constants'
import utils from '../../utils'
import api from '../../api'
import toaster from '../../toaster'

export default {
    data () {
        return {
            id: undefined,
            name: undefined,
            exercises: [],
            exerciseOptions: [],
            frequencyOptions:[]
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
            var option = this.frequencyOptions.find(f => f.value == value);
            if (option) {
                return option.text;
            }
            return value;
        },
        setFrequency(exercise, value) {
            exercise.frequency = value;
        },
        addExercise() {
            this.exercises.push({ exercise: undefined, sets: undefined, reps: undefined, frequency: 2 });
        },
        moveExerciseUp(index){
            var exercise = this.exercises[index];
            this.exercises.splice(index, 1);
            this.exercises.splice(index - 1, 0, exercise);
        },
        moveExerciseDown(index) {
            var exercise = this.exercises[index];
            this.exercises.splice(index, 1);
            this.exercises.splice(index + 1, 0, exercise);
        },
        deleteExercise(index) {
            this.exercises.splice(index, 1);
        },
         processNewExercise(exercise, exerciseName) {
            if (!exerciseName) {
                exercise.exercise = undefined;
            }
            else {
                var found = this.exerciseOptions.filter(e => e.name.toLowerCase().indexOf(exerciseName.toLowerCase()) >= 0);
                if (found.length == 0) {
                    var newExercise = { id: undefined, name: exerciseName };
                    this.exerciseOptions.push(newExercise);
                    exercise.exercise = newExercise;
                }
                else {
                    exercise.exercise = found[0];
                }
            }
        },
        save() {
            var self = this;
            var goal = {
                id: self.id,
                name: self.name,
                exercises: self.exercises
            };


            self.$store.dispatch(constants.SAVE_TRAINING_GOAL, {
                goal,
                success() {
                    toaster.info(self.$t('trainingGoals.saved'));
                     self.$router.replace({ name: 'training-goals' });
                },
                failure() {
                    toaster.error(self.$t('trainingGoals.saveFailed'));
                }
            });
        },
        populate(goal) {
            var self = this;
            self.id = goal.id;
            self.name = goal.name;
            self.exercises = goal.exercises;

            self.$store.dispatch(constants.FETCH_EXERCISES, {
                success(exercises) {
                    self.exerciseOptions = exercises;
                    if (goal.exercises && goal.exercises.length > 0) {
                        self.exercises = goal.exercises.map(ge => { return { exercise: self.exerciseOptions.find(e => e.id === ge.exerciseId), reps: ge.reps, weights: ge.weights, frequency: ge.frequency } });
                    }
                    else {
                        self.exercises = [];
                        self.addExercise();
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
        self.frequencyOptions = [
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
            self.populate({ id: undefined, name: undefined, exercises: [] });
            self.$store.commit(constants.LOADING_DONE);
        }
        else {
            self.$store.dispatch(constants.FETCH_TRAINING_GOAL, {
                id,
                success(goal) {
                    self.populate(goal);
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure() {
                    toaster.error(self.$t('fetchFailed'));
                }
            });
        }
    }
}