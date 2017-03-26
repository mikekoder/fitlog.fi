<template>
    <div>
        <div v-if="!selectedWorkout">
            <section class="content-header"><h1>Ateriat</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                                {{ date(start) }} - {{ date(end) }} <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu">
                                <li><a @click="showWeek">Kuluva viikko</a></li>
                                <li><a @click="showMonth">Kuluva kuukausi</a></li>
                                <li role="separator" class="divider"></li>
                                <li><a @click="showDays(7)">7 pv</a></li>
                                <li><a @click="showDays(14)">14 pv</a></li>
                                <li><a @click="showDays(30)">30 pv</a></li>
                                <li role="separator" class="divider"></li>
                                <li class="custom-date"><span>Valitse aikav&auml;li</span></li>
                                <li class="custom-date">
                                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="start" v-bind:format="'DD.MM.YYYY'" v-on:change="start=arguments[0]"></datetime-picker>
                                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="end" v-bind:format="'DD.MM.YYYY'" v-on:change="end=arguments[0]"></datetime-picker>
                                    <button class="btn btn-sm" @click="fetchWorkouts()">OK</button>
                                </li>
                            </ul>
                        </div>
                        <button class="btn btn-primary" @click="createWorkout"><i class="glyphicon glyphicon-plus"></i> Uusi treeni</button>
                        <div class="outer">
                            <div class="inner">
                                <table class="table" id="workout-summary">
                                    <thead>
                                        <tr>
                                            <th class="time"></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr class="workout" v-for="workout in workouts">
                                            <td class="freeze">{{ time(workout.time) }}</td>
                                            <td class="action">
                                                <button class="btn btn-sm" @click="editWorkout(workout)">Tiedot</button>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedWorkout">
            <section class="content-header"><h1>Treenin tiedot</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <workout-editor v-bind:workout="selectedWorkout" v-bind:saveCallback="saveWorkout" v-bind:cancelCallback="cancelWorkout" v-bind:deleteCallback="deleteWorkout" />
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var formatters = require('../../formatters')
    var c3 = require('c3');
    var moment = require('moment');

module.exports = {
    data () {
        return {   
            start: null,
            end: null,
            muscleGroups: [],
            workouts: [],
            selectedWorkout: null
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'workout-editor': require('./workout-editor')
    },
    methods: {
        showWeek(){
            this.end = moment().endOf('day').toDate();
            this.start = moment().startOf('isoWeek').toDate();
            this.fetchWorkouts();
        },
        showMonth(){
            this.end = moment().endOf('day').toDate();
            this.start = moment().startOf('month').toDate();
            this.fetchWorkouts();
        },
        showDays(days) {
            this.end = moment().endOf('day').toDate();
            this.start = moment().subtract(days - 1, 'days').startOf('day').toDate();
            this.fetchWorkouts();
        },
        fetchWorkouts: function () {
            var self = this;
            api.listWorkouts(this.start, this.end).then(function (workouts) {
                self.workouts = workouts;
            });
        },
        createWorkout: function(){
            this.showWorkout({ time: new Date()});
        },
        editWorkout: function (workout) {
            var self = this;
            api.getWorkout(workout.id).then(function (workoutDetails) {
                self.showWorkout(workoutDetails);
            });

        },
        saveWorkout: function (workout) {
            var self = this;
            api.saveWorkout(workout).then(function (savedWorkout) {
                self.fetchWorkouts();
                self.showSummary();
            });
        },
        cancelWorkout: function (workout) {
            this.showSummary();
        },
        deleteWorkout: function (workout) {
            var self = this;
            api.deleteWorkout(workout.id).then(function () {
                self.fetchWorkouts();
                self.showSummary();
            });
        },
        showWorkout: function (workout) {
            this.selectedWorkout = workout;
        },
        showSummary() {
            this.selectedWorkout = null;
        },
        date: formatters.formatDate,
        time: formatters.formatTime,
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
        api.listMuscleGroups().then(function (groups) {
            self.muscleGroups = groups;
        });
        var id = this.$route.params.id;
        if (id) {
            api.getWorkout(id).then(function (workout) {
                self.start = moment(workout.time).startOf('day');
                self.end = moment(workout.time).endOf('day');
            });
        } else {
            self.showWeek();
        }
    }
}
</script>

<style scoped>
    
</style>