<template>
    <div>
        <div class="row">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>Aika</label>
                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="time" v-on:change="time=arguments[0]"></datetime-picker>
                </div>
                
            </div>
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>Nimi</label>
                    <input class="form-control" v-model="name" />
                </div>
                
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-12">
                    <div class="row hidden-xs">
                        <div class="col-sm-4"><label>Harjoitus</label></div>
                        <div class="col-sm-2"><label>Toistot</label></div>
                        <div class="col-sm-3"><label>Painot</label></div>
                        <div class="col-sm-1">&nbsp;</div>
                    </div>
                    <template v-for="(row,index) in rows">
                        <div class="workout-row row">
                            <div class="food col-sm-4">
                                <label class="hidden-sm hidden-md hidden-lg">Harjoitus</label>
                                <exercise-picker v-bind:exercises="exercises" v-bind:value="row.exercise" v-on:change="row.exercise=arguments[0]" v-on:nameChange="processNewExercise(row, arguments[0])" />
                            </div>
                            <div class="quantity col-sm-2 col-xs-4">
                                <label class="hidden-sm hidden-md hidden-lg">Toistot</label>
                                <input type="number" class="form-control" v-model="row.reps" />
                            </div>
                            <div class="portion col-sm-3 col-xs-4">
                                <label class="hidden-sm hidden-md hidden-lg">Painot</label>
                                <input type="number" class="form-control" v-model="row.weights" />
                            </div>
                            <div class="actions col-sm-1 col-xs-4">
                                <div>
                                    <button class="btn btn-danger" @click="removeRow(index)">Poista</button>
                                </div>
                            </div>
                        </div>
                        <div class="workout-row-separator row hidden-sm hidden-md hidden-lg">
                            <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>
                    <div class="row">
                        <div class="col-sm-12"><button class="btn" @click="addRow"><i class="fa fa-plus"></i> Lisää</button></div>
                    </div>
            </div>
        </div>
        <hr />
        <div class="row">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="save">Tallenna</button>
                <button class="btn" @click="cancel">Peruuta</button>
                <button class="btn btn-link" v-if="id" @click="deleteWorkout">Poista</button>
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
            name: null,
            rows: [],
            exercises: []
        }
    },
    props: {
        workout: null,
        user: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'exercise-picker': require('./exercise-picker')
    },
    methods: {
        addRow : function(){
            this.rows.push({exercise: null, reps: null, weights: null});
        },
        removeRow: function (index) {
            this.rows.splice(index, 1);
        },
        processNewExercise: function (row, exerciseName) {
            if (!exerciseName) {
                row.exercise = undefined;
            }
            else {
                var found = this.exercises.filter(e => e.name.toLowerCase().indexOf(exerciseName.toLowerCase()) >= 0);
                if (found.length == 0) {
                    var exercise = { id: undefined, name: exerciseName };
                    this.exercises.push(exercise);
                    row.exercise = exercise;
                }
                else {
                    row.exercise = found[0];
                }
            }
        },
        save: function () {
            var workout = {
                id: this.id,
                time: this.time,
                name: this.name,
                rows: this.rows.filter(r => r.exercise && r.reps).map(r => { return { exerciseId: r.exercise.id, reps: utils.parseFloat(r.reps), weights: utils.parseFloat(r.weights) } })
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
        this.name = this.workout.name;
        api.listExercises().then(function (exercises) {
            //self.exercises = exercises;
        });
    },
    mounted: function () {
    }
}
</script>
<style scoped>
    div.workout-row
    {
        margin-bottom:5px;
    }
    div.workout-row-separator
    {
        padding: 0px;
    }
    div.workout-row-separator hr
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