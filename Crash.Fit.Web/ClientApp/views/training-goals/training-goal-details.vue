<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutritionGoals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input class="form-control" v-model="name" />
                    </div>

                </div>
            </div>
            <div class="exercise-separator row hidden-sm hidden-md hidden-lg">
                <div class="col-sm-12"><hr /></div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="row hidden-xs">
                        <div class="col-sm-2 col-text-40"><label>{{ $t("exercise") }}</label></div>
                        <div class="col-sm-2 col-text-10"><label :title="$t('frequencyInfo')" data-toggle="tooltip" data-placement="top">{{ $t("workoutsPerWeek") }} <i class="fa fa-question-circle-o"></i></label></div>
                        <div class="col-sm-2 col-number-5"><label>{{ $t("sets") }}</label></div>
                        <div class="col-sm-2 col-number-5"><label>{{ $t("reps") }}</label></div>    
                        <div class="col-sm-2 col-load"><label>{{ $t("load") }} (%)</label></div>
                        <div class="col-sm-2 col-actions-3">&nbsp;</div>
                    </div>
                    <template v-for="(exercise,index) in exercises">
                        <div class="row">
                            <div class="col-sm-2 col-text-40">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("exercise") }}</label>
                                <exercise-picker v-bind:exercises="exerciseOptions" v-bind:value="exercise.exercise" v-on:change="exercise.exercise=arguments[0]" v-on:nameChange="processNewExercise(exercise, arguments[0])" />
                            </div>
                            <div class="col-xs-4 col-text-10">
                                <label class="hidden-sm hidden-md hidden-lg" :title="$t('frequencyInfo')" data-toggle="tooltip" data-placement="top">{{ $t("workoutsPerWeek") }} <i class="fa fa-question-circle-o"></i></label>
                                <!--
                                <input type="number" min="0" step="0.166" class="form-control" v-model="exercise.frequency" />
                                -->
                                <div class="btn-group">
                                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">{{ frequencyText(exercise.frequency) }} <span class="caret"></span></button>
                                    <ul class="dropdown-menu">
                                        <li v-for="frequency in frequencyOptions">
                                            <a @click="exercise.frequency = frequency.value">{{ frequency.text }}</a>
                                        </li>
                                        <li>
                                            <div class="input-group col-number-5">
                                              <input type="number" v-model="exercise.frequency" />
                                            </div>
                                        </li>
                                    </ul>
                                </div> 
                            </div>
                            <div class="col-xs-4 col-number-5">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("sets") }}</label>
                                <input type="number" min="0" step="1" class="form-control" v-model="exercise.sets" />
                            </div>
                            <div class="col-xs-4 col-number-5">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("reps") }}</label>
                                <input type="number" min="0" step="1" class="form-control" v-model="exercise.reps" />
                            </div>  
                            <div class="col-xs-4 col-load">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("load") }}</label>
                                <input type="number" min="0" step="5" class="form-control" v-model="exercise.loadFrom" />
                                <i class="fa fa-minus"></i>
                                <input type="number" min="0" step="5" class="form-control" v-model="exercise.loadTo" />
                            </div>
                            <div class="col-xs-4 col-actions-3">
                                <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                <button class="btn btn-sm" @click="moveExerciseUp(index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                <button class="btn btn-sm" @click="moveExerciseDown(index)" :disabled="index === (exercises.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                <button class="btn btn-danger btn-sm" @click="deleteExercise(index)">{{ $t("delete") }}</button>
                            </div>
                        </div>
                        <div class="exercise-separator row hidden-sm hidden-md hidden-lg">
                            <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>
                </div>
            </div>
            <div class="row" v-if="exercises.length == 0">
              <div class="col-sm-12">
                <br />
                {{ $t("noExercises") }}
              </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12"><button class="btn" @click="addExercise">{{ $t("add") }}</button></div>
            </div>
            <hr />
            <div class="row" v-if="exercises.length > 0">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t('save') }}</button>
                </div>
            </div>
          
            <div class="row">
                <div class="col-sm-12">
                    <p>
                        {{ $t('trainingGoalsInfo') }}
                    </p>
                </div>
            </div>
        </section>
        
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import utils from '../../utils'
    import api from '../../api'
    import formatters from '../../formatters'
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
</script>

<style scoped>
    div.exercise-separator {
        padding: 0px;
    }

    div.exercise-separator hr {
        border: 1px solid #00c0ef;
    }
</style>