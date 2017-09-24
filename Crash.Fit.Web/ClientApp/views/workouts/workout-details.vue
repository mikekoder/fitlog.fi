<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("workoutDetails") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
                    <div class="form-group">
                        <label>{{ $t("time") }}</label>
                        <datetime-picker class="vue-picker1" name="picker1" v-bind:value="time" v-on:change="time=arguments[0]"></datetime-picker>
                    </div>

                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="row hidden-xs">
                        <div class="col-sm-4 col-md-4 col-lg-2"><label>{{ $t("exercise") }}</label></div>
                        <div class="col-sm-2 col-md-2 col-lg-1"><label>{{ $t("reps") }}</label></div>
                        <div class="col-sm-3 col-md-2 col-lg-1"><label>{{ $t("weights") }}</label></div>
                        <div class="col-sm-3">&nbsp;</div>
                    </div>
                    <template v-for="(set,index) in sets">
                        <div class="row">
                            <div class="col-sm-4 col-md-4 col-lg-2">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("exercise") }}</label>
                                <exercise-picker v-bind:exercises="exercises" v-bind:value="set.exercise" v-on:change="set.exercise=arguments[0]" v-on:nameChange="processNewExercise(set, arguments[0])" />
                            </div>
                            <div class="quantity col-sm-2 col-xs-4 col-md-2 col-lg-1">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("reps") }}</label>
                                <input type="number" min="0" class="form-control" v-model="set.reps" />
                            </div>
                            <div class="portion col-sm-3 col-xs-4 col-md-2 col-lg-1">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("weights") }}</label>
                                <input type="number" min="0" step="2.5" class="form-control" v-model="set.weights" />
                            </div>
                            <div class="actions col-sm-3 col-xs-4">
                                <button class="btn btn-sm" @click="moveSetUp(index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                <button class="btn btn-sm" @click="moveSetDown(index)" :disabled="index === (sets.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                <button class="btn btn-primary" @click="copySet(index)">{{ $t("copy") }}</button>
                                <button class="btn btn-danger btn-sm" @click="removeSet(index)">{{ $t("delete") }}</button>
                            </div>
                        </div>
                        <div class="workout-set-separator row hidden-sm hidden-md hidden-lg">
                            <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>

                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12"><button class="btn" @click="addSet">{{ $t("add") }}</button></div>
            </div>
            <hr />
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger" v-if="id" @click="deleteWorkout">{{ $t("delete") }}</button>
                </div>
            </div>
            <hr />
            <div class="row">
                <table>
                    <tbody></tbody>
                </table>
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
        removeSet(index) {
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
                    if (workout.sets) {
                        self.sets = workout.sets.map(s => { return { exercise: self.exercises.filter(e => e.id === s.exerciseId)[0], reps: s.reps, weights: s.weights } });
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
                            for(var i = 0; i < e.sets; i++){
                                workout.sets.push({ exerciseId: e.exerciseId, reps: e.reps });
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
</script>
<style scoped>
    div.workout-set
    {
        margin-bottom:5px;
    }
    div.workout-set-separator
    {
        padding: 0px;
    }
    div.workout-set-separator hr
    {
        border: 1px solid #00c0ef;
    }
    div.food, div.quantity, div.portion, div.weight, div.actions
    {
        padding-right:2px;
    }
    div.weight 
    {
        padding-top:5px;
    }
    @media (max-width: 767px) {
        div.food, div.quantity, div.portion, div.weight, div.actions
        {
            padding-right:15px;
        }
        div.actions
        {
            text-align:right;
            padding-top:15px;
        }
        div.actions button
        {
            margin-top:10px;
            margin-right:0px;
        }
    }
</style>