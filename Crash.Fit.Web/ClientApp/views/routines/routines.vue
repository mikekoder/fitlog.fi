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
                <div class="row" v-if="routines.length > 0">
                    <div class="col-sm-12">     
                        <table class="table" id="routine-list">
                            <thead>
                                <tr>
                                    <th class="name">Nimi</th>
                                    <th></th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="routine" v-for="routine in routines">
                                    <td><router-link :to="{ name: 'routines', params: { id: routine.id } }">{{ routine.name }}</router-link></td>
                                    <td>
                                        <span v-if="routine.active">Aktiivinen</span>
                                        <button class="btn btn-primary" v-if="!routine.active">Aktiiviseksi</button>
                                    </td>
                                    <td><button class="btn btn-danger btn-xs" @click="deleteRoutine(routine)">Poista</button></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="row" v-if="routines.length == 0">
                    <div class="col-sm-12">
                        <br />
                        Ei ohjelmia
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
    var toaster = require('../../toaster');

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
            }).fail(function () {
                toaster.error('Ohjelmien haku epäonnistui');
            });
        },
        createRoutine: function(){
            this.showRoutine({ });
        },
        editRoutine: function (id) {
            var self = this;
            api.getRoutine(id).then(function (routineDetails) {
                self.showRoutine(routineDetails);
            }).fail(function () {
                toaster.error('Ohjelman haku epäonnistui');
            });

        },
        saveRoutine: function (routine) {
            var self = this;
            api.saveRoutine(routine).then(function (savedRoutine) {
                self.fetchRoutines();
                self.$router.push({ name: 'routines' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Ohjelman tallennus epäonnistui');
            });
        },
        cancelRoutine: function (routine) {
            this.$router.push({ name: 'routines' });
            this.showSummary();
        },
        deleteRoutine: function (routine) {
            var self = this;
            api.deleteRoutine(routine.id).then(function () {
                self.fetchRoutines();
                self.$router.push({ name: 'routines' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Ohjelman poistaminen epäonnistui');
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
        }).fail(function () {
            toaster.error('Liikkeiden haku epäonnistui');
        });
        self.fetchRoutines();
        var id = this.$route.params.id;
        if (id) {
            this.editRoutine(id);
        }
    },
    beforeRouteUpdate (to, from, next) {
        if (to.params.id) {
            this.editRoutine(to.params.id);
        }
        else {
            this.showSummary();
        }
        next();
    }
}
</script>

<style scoped>
  #routine-list
  {
    width: auto;
    table-layout: fixed;
  }
  th.name{
     min-width: 150px;
  }
</style>