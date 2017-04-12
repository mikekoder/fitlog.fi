<template>
    <div>
        <div v-if="!selectedRoutine">
            <section class="content-header"><h1>Ohjelmat</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createRoutine"><i class="glyphicon glyphicon-plus"></i> Uusi ohjelma</button>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">     
                        <table class="table" id="routine-list">
                            <thead>
                                <tr>
                                    <th>Nimi</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="routine" v-for="routine in routines">
                                    <td>{{ routine.name }}</td>
                                    <td class="action">
                                        <button class="btn btn-sm" @click="editRoutine(routine)">Tiedot</button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedRoutine">
            <section class="content-header"><h1>Ohjelman tiedot</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <routine-editor v-bind:routine="selectedRoutine" v-bind:exercises="exercises" v-bind:saveCallback="saveRoutine" v-bind:cancelCallback="cancelRoutine" v-bind:deleteCallback="deleteRoutine" />
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var formatters = require('../../formatters')

module.exports = {
    data () {
        return {
            routines: [],
            exercises: [],
            selectedRoutine: null
        }
    },
    components: {
        'routine-editor': require('./routine-editor')
    },
    methods: {
        fetchRoutines: function () {
            var self = this;
            api.listRoutines().then(function (routines) {
                self.routines = routines;
            });
        },
        createRoutine: function(){
            this.showRoutine({});
        },
        editRoutine: function (routine) {
            var self = this;
            api.getRoutine(routine.id).then(function (routineDetails) {
                self.showRoutine(routineDetails);
            });

        },
        saveRoutine: function (routine) {
            var self = this;
            api.saveRoutine(routine).then(function (savedRoutine) {
                self.fetchRoutines();
                self.showSummary();
            });
        },
        cancelRoutine: function (routine) {
            this.showSummary();
        },
        deleteRoutine: function (routine) {
            var self = this;
            api.deleteRoutine(routine.id).then(function () {
                self.fetchRoutines();
                self.showSummary();
            });
        },
        showRoutine: function (routine) {
            this.selectedRoutine = routine;
        },
        showSummary() {
            this.selectedRoutine = null;
        }
    },
    created: function () {

        var self = this;
        api.listExercises().then(function (exercises) {
            self.exercises = exercises;
        });
        self.fetchRoutines();
    }
}
</script>

<style scoped>
  #routine-list
  {
    width: auto;
    table-layout: fixed;
  }
</style>