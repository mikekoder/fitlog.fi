<template>
    <div>
        <div class="row">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>Aika</label>
                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="time" v-on:change="time=arguments[0]"></datetime-picker>
                </div>
                
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-12">
                    <div class="row hidden-xs">
                        <div class="col-sm-4 col-md-4 col-lg-2"><label>Liike</label></div>
                        <div class="col-sm-2 col-md-2 col-lg-1"><label>Toistot</label></div>
                        <div class="col-sm-3 col-md-2 col-lg-1"><label>Painot</label></div>
                        <div class="col-sm-3">&nbsp;</div>
                    </div>
                    <template v-for="(set,index) in sets">
                        <div class="row">
                            <div class="col-sm-4 col-md-4 col-lg-2">
                                <label class="hidden-sm hidden-md hidden-lg">Liike</label>
                                <exercise-picker v-bind:exercises="exercises" v-bind:value="set.exercise" v-on:change="set.exercise=arguments[0]" v-on:nameChange="processNewExercise(set, arguments[0])" />
                            </div>
                            <div class="quantity col-sm-2 col-xs-4 col-md-2 col-lg-1">
                                <label class="hidden-sm hidden-md hidden-lg">Toistot</label>
                                <input type="number" min="0" class="form-control" v-model="set.reps" />
                            </div>
                            <div class="portion col-sm-3 col-xs-4 col-md-2 col-lg-1">
                                <label class="hidden-sm hidden-md hidden-lg">Painot</label>
                                <input type="number" min="0" step="2.5" class="form-control" v-model="set.weights" />
                            </div>
                            <div class="actions col-sm-3 col-xs-4">
                                <button class="btn btn-sm" @click="moveSetUp(index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                <button class="btn btn-sm" @click="moveSetDown(index)" :disabled="index === (sets.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                <button class="btn btn-primary" @click="copySet(index)">Kopioi</button>
                                <button class="btn btn-danger btn-sm" @click="removeSet(index)">Poista</button>
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
            <div class="col-sm-12"><button class="btn" @click="addSet"><i class="fa fa-plus"></i> Lisää</button></div>
        </div>
        <hr />
        <div class="row main-actions">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="save">Tallenna</button>
                <button class="btn" @click="cancel">Peruuta</button>
                <button class="btn btn-danger" v-if="id" @click="deleteWorkout">Poista</button>
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
            id: null,
            time: null,
            sets: [{ exercise: null, reps: null, weights: null }],
            exercises: [],
        }
    },
    props: {
        workout: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'exercise-picker': require('../../components/exercise-picker')
    },
    methods: {
        addSet : function(){
            this.sets.push({exercise: null, reps: null, weights: null});
        },
        copySet: function (index) {
            var original = this.sets[index];
            var copy = { exercise: original.exercise, reps: original.reps, weights: original.weights};
            this.sets.splice(index, 0, copy);
        },
        moveSetUp: function(index){
            var set = this.sets[index];
            this.sets.splice(index, 1);
            this.sets.splice(index - 1, 0, set);
        },
        moveSetDown: function (index) {
            var set = this.sets[index];
            this.sets.splice(index, 1);
            this.sets.splice(index + 1, 0, set);
        },
        removeSet: function (index) {
            this.sets.splice(index, 1);
        },
        processNewExercise: function (set, exerciseName) {
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
        save: function () {
            var workout = {
                id: this.id,
                time: this.time,
                sets: this.sets.filter(s => s.exercise && s.reps).map(s => { return { exerciseId: s.exercise.id, exerciseName: s.exercise.name, reps: utils.parseFloat(s.reps), weights: utils.parseFloat(s.weights) } })
            };
            this.saveCallback(workout);
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
        this.id = this.workout.id;
        this.time = this.workout.time;
        this.exercises = this.$store.state.training.exercises;
        
        if (this.workout.sets) {
            this.sets = this.workout.sets.map(s => { return { exercise: self.exercises.filter(e => e.id === s.exerciseId)[0], reps: s.reps, weights: s.weights } });
        }
        else {
            this.sets = [{ exercise: null, reps: null, weights: null }];
        }
    },
    mounted: function () {
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