<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t('routineDetails') }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12 col-text-30">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input type="text" class="form-control" v-model="name" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <h4>{{ $t("workouts") }}</h4>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <template v-for="(workout,index) in workouts">
                        <div class="box box-solid">
                            <div class="box-header with-border">
                                <div class="row hidden-xs">
                                    <div class="col-xs-6 col-text-30">{{ $t('name') }}</div>
                                    <div class="col-xs-6 col-text-20">{{ $t('frequency') }}</div>
                                    <div class="col-xs-6 col-actions-3"></div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-6 col-text-30">
                                        <label class="hidden-sm hidden-md hidden-lg">{{ $t("name") }}</label>
                                        <input type="text" class="form-control" v-model="workout.name" />
                                    </div>
                                    <div class="col-xs-6 col-text-20">
                                        <label class="hidden-sm hidden-md hidden-lg">{{ $t("frequency") }}</label>
                                        <div class="btn-group">
                                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">{{ frequencyText(workout.frequency) }} <span class="caret"></span></button>
                                            <ul class="dropdown-menu">
                                                <li v-for="frequency in frequencyPresets">
                                                    <a @click="workout.frequency = frequency.value">{{ frequency.text }}</a>
                                                </li>
                                                <li>
                                                    <div class="input-group">
                                                        <input type="number" class="form-control" v-model="workout.frequency" />
                                                        <span class="input-group-addon">
                                                            {{ $t('timesAbbr') }} / {{ $t('weekAbbr') }}
                                                        </span>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div> 
                                    </div>
                                    <div class="col-xs-6 col-actions-3">
                                        <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                        <button class="btn btn-sm" @click="moveWorkoutUp(index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                        <button class="btn btn-sm" @click="moveWorkoutDown(index)" :disabled="index === (workouts.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                        <button class="btn btn-danger pull-right btn-sm" @click="deleteWorkout(index)">Poista</button>
                                    </div>
                                </div>
                            </div>
                            <div class="box-body">
                                <div class="workout-details">
                                    <div class="row hidden-xs">
                                        <div class="col-sm-4 col-text-30"><label>{{ $t("exercise") }}</label></div>
                                        <div class="col-sm-2 col-number-5"><label>{{ $t("sets") }}</label></div>
                                        <div class="col-sm-2 col-number-5"><label>{{ $t("reps") }}</label></div>
                                        <div class="col-sm-2 col-load"><label>{{ $t("load") }} (%)</label></div>
                                        <div class="col-sm-4 col-actions-3">&nbsp;</div>
                                    </div>
                                    <template v-for="(exercise,index) in workout.exercises">
                                        <div class="row">
                                            <div class="col-sm-4 col-text-30">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("exercise") }}</label>
                                                <exercise-picker v-bind:exercises="exercises" v-bind:value="exercise.exercise" v-on:change="exercise.exercise=arguments[0]" v-on:nameChange="processNewExercise(exercise, arguments[0])" />
                                            </div>
                                            <div class="col-xs-4 col-number-5">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("sets") }}</label>
                                                <input type="number" min="1" step="1" class="form-control" v-model="exercise.sets" />
                                            </div>
                                            <div class="col-xs-4 col-number-5">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("reps") }}</label>
                                                <input type="number" min="1" step="1" class="form-control" v-model="exercise.reps" />
                                            </div>
                                            <div class="col-xs-4 col-load">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("load") }}</label>
                                                <input type="number" min="0" step="5" class="form-control" v-model="exercise.loadFrom" />
                                                <i class="fa fa-arrow-right"></i>
                                                <input type="number" min="0" step="5" class="form-control" v-model="exercise.loadTo" />
                                            </div>
                                            <div class="actions col-sm-4 col-xs-4 col-actions-3">
                                                <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                                <button class="btn btn-sm" @click="moveExerciseUp(workout,index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                                <button class="btn btn-sm" @click="moveExerciseDown(workout,index)" :disabled="index === (workout.exercises.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                                <button class="btn btn-danger btn-sm" @click="deleteExercise(workout,index)">{{ $t("delete") }}</button>
                                            </div>
                                        </div>
                                        <div class="workout-set-separator row hidden-sm hidden-md hidden-lg">
                                            <div class="col-sm-12"><hr /></div>
                                        </div>
                                    </template>
                                    <div class="row">&nbsp;</div>
                                    <div class="row">
                                        <div class="col-sm-12"><button class="btn" @click="addExercise(workout)">{{ $t("add") }}</button></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </template>
                    <div class="row">
                        <div class="col-sm-12">
                            <button class="btn" @click="addWorkout">{{ $t("addWorkout") }}</button>
                        </div>
                    </div>
                </div>
            </div>

            <hr />
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger btn-sm" v-if="id" @click="deleteWorkout">{{ $t("delete") }}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script>
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
</script>
<style scoped>
    div.workout-header
    {
        background-color: #ccc;
        padding: 10px;
    }
    div.workout-details 
    {
        padding-bottom: 20px;
    }
    .box 
    {
        max-width: 1000px;
    }

    label{ display:block;}
    input{ padding: 6px;}
</style>