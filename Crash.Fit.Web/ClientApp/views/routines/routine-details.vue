<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t('routineDetails') }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
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
                <div class="col-sm-12 col-md-10">
                    <template v-for="(workout,index) in workouts">
                        <div class="box box-info box-solid">
                            <div class="box-header with-border">
                                <div class="row">
                                    <div class="col-xs-6"><input type="text" class="form-control" v-model="workout.name" /></div>
                                    <div class="col-xs-6">
                                        <button class="btn btn-sm" @click="moveWorkoutUp(index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                        <button class="btn btn-sm" @click="moveWorkoutDown(index)" :disabled="index === (workouts.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                        <button class="btn btn-danger pull-right btn-sm" @click="removeWorkout(index)">Poista</button>
                                    </div>
                                </div>
                            </div>
                            <div class="box-body">
                                <div class="workout-details">
                                    <div class="row hidden-xs">
                                        <div class="col-sm-4 col-md-4"><label>{{ $t("exercise") }}</label></div>
                                        <div class="col-sm-2 col-md-2"><label>{{ $t("sets") }}</label></div>
                                        <div class="col-sm-2 col-md-2"><label>{{ $t("reps") }}</label></div>
                                        <div class="col-sm-4">&nbsp;</div>
                                    </div>
                                    <template v-for="(exercise,index) in workout.exercises">
                                        <div class="row">
                                            <div class="col-sm-4 col-md-4">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("exercise") }}</label>
                                                <exercise-picker v-bind:exercises="exercises" v-bind:value="exercise.exercise" v-on:change="exercise.exercise=arguments[0]" v-on:nameChange="processNewExercise(exercise, arguments[0])" />
                                            </div>
                                            <div class="quantity col-sm-2 col-xs-4 col-md-2">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("sets") }}</label>
                                                <input type="number" min="1" step="1" class="form-control" v-model="exercise.sets" />
                                            </div>
                                            <div class="portion col-sm-2 col-xs-4 col-md-2">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("reps") }}</label>
                                                <input type="number" min="1" step="1" class="form-control" v-model="exercise.reps" />
                                            </div>
                                            <div class="actions col-sm-4 col-xs-4">
                                                <div>
                                                    <button class="btn btn-sm" @click="moveExerciseUp(workout,index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                                    <button class="btn btn-sm" @click="moveExerciseDown(workout,index)" :disabled="index === (workout.exercises.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                                    <!--
                                                <button class="btn btn-primary" @click="copyExercise(workout,index)">Kopioi</button>
                                                -->
                                                    <button class="btn btn-danger pull-right btn-sm" @click="removeExercise(workout,index)">{{ $t("delete") }}</button>
                                                </div>
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
                    <button class="btn btn-danger btn-sm" v-if="id" @click="removeWorkout">{{ $t("delete") }}</button>
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
            selectedWorkout: undefined
        }
    },
    computed: {
    },
    components: {
        'exercise-picker': require('../../components/exercise-picker')
    },
    methods: {
        addWorkout: function(){
            var count = this.workouts.length;
            this.workouts.push({ name: 'Päivä ' + (count + 1), exercises: [], expanded: true });
        },
        toggleWorkout: function(workout){
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
        removeWorkout(index) {
            this.workouts.splice(index, 1);
        },
        addExercise : function(workout){
            workout.exercises.push({exercise: null, sets: null, reps: null});
        },
        copyExercise(workout, index) {
            var original = workout.exercises[index];
            var copy = { exercise: original.exercise, sets: original.sets, reps: original.reps};
            workout.exercises.splice(index, 0, copy);
        },
        moveExerciseUp(workout, index) {
            var exercise = workout.exercises[index];
            workout.exercises.splice(index, 1);
            workout.exercises.splice(index - 1, 0, set);
        },
        moveExerciseDown(workout, index) {
            var exercise = workout.exercises[index];
            workout.exercises.splice(index, 1);
            workout.exercises.splice(index + 1, 0, set);
        },
        removeExercise(workout, index) {
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
                workouts: self.workouts.map(w => { return { id: w.id, name: w.name, exercises: w.exercises.filter(e => e.exercise).map(e => { return { exerciseId: e.exercise.id, exerciseName: e.exercise.name, sets: utils.parseFloat(e.sets), reps: utils.parseFloat(e.reps) } }) } })
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
                                exercises: w.exercises.map(we => { return { exercise: exercises.filter(e => e.id === we.exerciseId)[0], sets: we.sets, reps: we.reps } })
                            }
                        });
                    }
                    else {
                        self.workouts = [{ name: 'Päivä 1', exercises: [{}] }]
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
        var id = self.$route.params.id;
        if (id == constants.NEW_ID) {
            self.populate({ id: undefined, name: undefined, ingredients: [{ food: undefined, quantity: undefined, portion: undefined }] });
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
</style>