<template>
    <div>
        <div v-if="!selectedWorkout">
            <section class="content-header"><h1>Treenit</h1></section>
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
                        <div class="btn-group" v-if="workoutOptions.length > 0">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Uusi treeni <span class="caret"></span></button>
                            <ul class="dropdown-menu">
                                <li v-for="workout in workoutOptions">
                                    <a @click="createWorkout(workout.id)">{{ workout.name }}</a>
                                </li>
                                <li role="separator" class="divider"></li>
                                <li>
                                    <a @click="createWorkout(undefined)">Vapaa treeni</a>
                                </li></ul>
                        </div>
                        <div v-if="workoutOptions.length == 0">
                            <button @click="createWorkout(undefined)">Lis‰‰ treeni</button>
                        </div>
                        <div class="outer" v-if="workouts.length > 0">
                            <div class="inner">
                                <table class="table" id="workout-list">
                                    <thead>
                                        <tr>
                                            <th class="time freeze"><div><div>&nbsp;</div></div></th>
                                            <template v-for="muscleGroup in muscleGroups">
                                                <th class="muscle-group"><div><div>{{ muscleGroup.name}}</div></div></th>
                                            </template>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr class="workout" v-for="workout in workouts">
                                            <td class="freeze"><router-link :to="{ name: 'workouts', params: { id: workout.id } }">{{ datetime(workout.time) }}</router-link></td>
                                            <template v-for="muscleGroup in muscleGroups">
                                                <td class="muscle-group">{{ workout.muscleGroupSets[muscleGroup.id] }}</td>
                                            </template>
                                            <td><button class="btn btn-danger btn-xs" @click="deleteWorkout(workout)">Poista</button></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class="row" v-if="workouts.length == 0">
                            <div class="col-sm-12">
                                <br />
                                Ei treenej&auml;
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
    var constants = require('../../store/constants')
    var api = require('../../api');
    var formatters = require('../../formatters')
    var moment = require('moment');
    var toaster = require('../../toaster');
    var utils = require('../../utils');

module.exports = {
    data () {
        return {   
            start: null,
            end: null,
            //muscleGroups: [],
            //workouts: [],
            //exercises: [],
            selectedWorkout: null,
            //workoutOptions: []
        }
    },
    computed: {
        muscleGroups: function(){
            return this.$store.state.training.muscleGroups;
        },
        exercises: function () {
            return this.$store.state.training.exercises;
        },
        workouts: function(){
            var self = this;
            return this.$store.state.training.workouts.filter(w => moment(w.time).isBetween(self.start, self.end));
        },
        workoutOptions: function () {
            if (this.$store.state.training.activeRoutine) {
                return this.$store.state.training.activeRoutine.workouts;
            }
            return [];
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
            this.$store.dispatch(constants.FETCH_WORKOUTS, { start: self.start, end: self.end });
        },
        createWorkout: function (routineWorkoutId) {
            if (routineWorkoutId) {

            }
            this.showWorkout({ time: utils.previousHalfHour() });
        },
        editWorkout: function (id) {
            var self = this;
            api.getWorkout(id).then(function (workoutDetails) {
                self.showWorkout(workoutDetails);
            }).fail(function () {
                toaster.error('Treenin haku ep‰onnistui');
            });

        },
        saveWorkout: function (workout) {
            var self = this;
            api.saveWorkout(workout).then(function (savedWorkout) {
                self.fetchWorkouts();
                self.$router.push({ name: 'workouts' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Treenin tallennus ep‰onnistui');
            });
        },
        cancelWorkout: function (workout) {
            this.$router.push({ name: 'workouts' });
            this.showSummary();
        },
        deleteWorkout: function (workout) {
            var self = this;
            api.deleteWorkout(workout.id).then(function () {
                self.fetchWorkouts();
                self.$router.push({ name: 'workouts' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Treenin poistaminen ep‰onnistui');
            });
        },
        showWorkout: function (workout) {
            this.selectedWorkout = workout;
        },
        showSummary() {
            this.selectedWorkout = null;
        },
        date: formatters.formatDate,
        datetime: formatters.formatDateTime,
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
        this.$store.dispatch(constants.FETCH_MUSCLEGROUPS, {
            success: function () { },
            failure: function () { }
        });
        this.$store.dispatch(constants.FETCH_EXERCISES, {
            success: function () { },
            failure: function () { }
        });
        this.$store.dispatch(constants.FETCH_ROUTINES, {
            success: function () { },
            failure: function () { }
        });
        var id = this.$route.params.id;
        if (id) {
            api.getWorkout(id).then(function (workout) {
                self.start = moment(workout.time).startOf('day');
                self.end = moment(workout.time).endOf('day');
                self.fetchWorkouts();
                self.showWorkout(workout);
            }).fail(function () {
                toaster.error('Treenin haku ep‰onnistui');
            });
        } else {
            self.showDays(7);
        }
    },
    beforeRouteUpdate (to, from, next) {
        if (to.params.id) {
            this.editWorkout(to.params.id);
        }
        else {
            this.showSummary();
        }
        next();
    }
}
</script>

<style scoped>
    .outer 
    {
      position: relative;
    }
    .inner 
    {
      overflow-x: auto;
      overflow-y: visible;
      margin-left: 100px;
    } 
    #workout-list
    {
        width: auto;
        table-layout: fixed; 
    }
    th.time
    {
        width: 120px;
        white-space: nowrap;
        border-width:0px;
    }
    
    th.time > div
    {
        transform: translate(86px, 29px) rotate(-45deg);
    }
    th.time > div > div
    {
      border-bottom: 1px solid #ccc;
      padding: 5px 10px;
      width:100px;
    }
    .freeze 
    {
      position: absolute;
      margin-left: -120px;
      width: 120px;
      text-align: right;
    }
    th.time
    {
        
    }
   
    #workout-list  td:nth-child(1)
    {
        border-right: 1px solid #ccc;
    }
    th.muscle-group
    {
        height: 100px;
        white-space: nowrap;
    }
    th.muscle-group > div
    {
       transform: translate(15px, 8px) rotate(-45deg);
       width: 20px;
    }
    th.muscle-group > div > div 
    {
      border-bottom: 1px solid #ccc;
      padding: 5px 10px;
      width:100px;
    }
    td.muscle-group
    {
        border-right:1px solid #ccc;
        text-align:center;
    }
     td.freeze, td.muscle-group {
        padding-top: 12px;
    }
</style>