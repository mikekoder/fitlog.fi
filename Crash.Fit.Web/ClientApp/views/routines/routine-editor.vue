<template>
    <div>
        <div class="row">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>Nimi</label>
                    <input type="text" class="form-control" v-model="name" />
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-12">
                <h4>Treenit</h4>
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
                                    <div class="col-sm-4 col-md-4"><label>Liike</label></div>
                                    <div class="col-sm-2 col-md-2"><label>Sarjat</label></div>
                                    <div class="col-sm-2 col-md-2"><label>Toistot</label></div>
                                    <div class="col-sm-4">&nbsp;</div>
                                </div>
                                <template v-for="(exercise,index) in workout.exercises">
                                    <div class="row">
                                        <div class="col-sm-4 col-md-4">
                                            <label class="hidden-sm hidden-md hidden-lg">Liike</label>
                                            <exercise-picker v-bind:exercises="exercises" v-bind:value="exercise.exercise" v-on:change="exercise.exercise=arguments[0]" v-on:nameChange="processNewExercise(exercise, arguments[0])" />
                                        </div>
                                        <div class="quantity col-sm-2 col-xs-4 col-md-2">
                                            <label class="hidden-sm hidden-md hidden-lg">Sarjat</label>
                                            <input type="number" min="1" step="1" class="form-control" v-model="exercise.sets" />
                                        </div>
                                        <div class="portion col-sm-2 col-xs-4 col-md-2">
                                            <label class="hidden-sm hidden-md hidden-lg">Toistot</label>
                                            <input type="number" min="1" step="1" class="form-control" v-model="exercise.reps" />
                                        </div>
                                        <div class="actions col-sm-4 col-xs-4">
                                            <div>
                                                <button class="btn btn-sm" @click="moveExerciseUp(workout,index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                                <button class="btn btn-sm" @click="moveExerciseDown(workout,index)" :disabled="index === (workout.exercises.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                                <!--
                                                <button class="btn btn-primary" @click="copyExercise(workout,index)">Kopioi</button>
                                                -->
                                                <button class="btn btn-danger pull-right btn-sm" @click="removeExercise(workout,index)">Poista</button>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="workout-set-separator row hidden-sm hidden-md hidden-lg">
                                        <div class="col-sm-12"><hr /></div>
                                    </div>
                                </template>
                                <div class="row">&nbsp;</div>
                                <div class="row">
                                    <div class="col-sm-12"><button class="btn" @click="addExercise(workout)"><i class="fa fa-plus"></i> Lisää</button></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn" @click="addWorkout"><i class="fa fa-plus"></i> Lisää treeni</button>
                    </div>
                </div>
            </div>
        </div>
        
        <hr />
        <div class="row main-actions">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="save">Tallenna</button>
                <button class="btn" @click="cancel">Peruuta</button>
                <button class="btn btn-danger btn-sm" v-if="id" @click="deleteWorkout">Poista</button>
            </div>
        </div>
        <hr />
        <div class="row">
            <table>
                <tbody>

                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var formatters = require('../../formatters');
    var utils = require('../../utils');

module.exports = {
    data () {
        return {
            id: undefined,
            name: undefined,
            workouts: [{ name: 'Päivä 1', exercises: [] }],
            exercises: [],
            selectedWorkout: undefined
        }
    },
    props: {
        routine: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null
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
        moveWorkoutUp: function (index) {
            var workout = this.workouts[index];
            this.workouts.splice(index, 1);
            this.workouts.splice(index - 1, 0, workout);
        },
        moveWorkoutDown: function (index) {
            var workout = this.workouts[index];
            this.workouts.splice(index, 1);
            this.workouts.splice(index + 1, 0, workout);
        },
        removeWorkout: function (index) {
            this.workouts.splice(index, 1);
        },
        addExercise : function(workout){
            workout.exercises.push({exercise: null, sets: null, reps: null});
        },
        copyExercise: function (workout, index) {
            var original = workout.exercises[index];
            var copy = { exercise: original.exercise, sets: original.sets, reps: original.reps};
            workout.exercises.splice(index, 0, copy);
        },
        moveExerciseUp: function (workout, index) {
            var exercise = workout.exercises[index];
            workout.exercises.splice(index, 1);
            workout.exercises.splice(index - 1, 0, set);
        },
        moveExerciseDown: function (workout, index) {
            var exercise = workout.exercises[index];
            workout.exercises.splice(index, 1);
            workout.exercises.splice(index + 1, 0, set);
        },
        removeExercise: function (workout, index) {
            workout.exercises.splice(index, 1);
        },
        processNewExercise: function (workoutExercise, exerciseName) {
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
        save: function () {
            var routine = {
                id: this.id,
                name: this.name,
                workouts: this.workouts.map(w => { return { id: w.id, name: w.name, exercises: w.exercises.map(e => { return { exerciseId: e.exercise.id, exerciseName: e.exercise.name, sets: utils.parseFloat(e.sets), reps: utils.parseFloat(e.reps) } }) } })
            };
            this.saveCallback(routine);
        },
        cancel: function () {
            this.cancelCallback();
        },
        deleteWorkout: function () {
            this.deleteCallback(this.workout);
        },
        unit: formatters.formatUnit,
        decimal: function (value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        }
    },
    created: function () {
        var self = this;
        this.id = this.routine.id;
        this.name = this.routine.name;
        api.listExercises().then(function (exercises) {
            self.exercises = exercises;
            if (self.routine.workouts) {
                self.workouts = self.routine.workouts.map(w => {
                    return {
                        id: w.id,
                        name: w.name,
                        exercises: w.exercises.map(we => { return { exercise: exercises.filter(e => e.id === we.exerciseId)[0], sets: we.sets, reps: we.reps } })
                    }
                });
            }
            else {
                self.workouts = [{ name: 'Päivä 1', exercises: [{}]}]
            }
        });
    },
    mounted: function () {
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