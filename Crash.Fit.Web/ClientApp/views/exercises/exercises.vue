<template>
    <div>
        <div v-if="!selectedExercise">
            <section class="content-header"><h1>Liikkeet</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createExercise"><i class="glyphicon glyphicon-plus"></i> Uusi liike</button>
                    </div>
                </div>
                <div class="row" v-if="exercises.length > 0">
                    <div class="col-sm-12">
                        <table class="table" id="exercise-list">
                            <thead>
                                <tr>
                                    <th>Nimi</th>
                                    <th>Sarjoja</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="exercise in exercises">
                                    <td><router-link :to="{ name: 'exercises', params: { id: exercise.id } }">{{ exercise.name }}</router-link></td>
                                    <td>{{ exercise.usageCount }}</td>
                                    <td>
                                        <button class="btn btn-danger btn-xs" @click="deleteExercise(exercise)">Poista</button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="row" v-if="exercises.length == 0">
                    <div class="col-sm-12">
                        <br />
                        Ei liikkeit&auml;
                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedExercise">
            <section class="content-header"><h1>Liikkeen tiedot</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <exercise-editor v-bind:exercise="selectedExercise" v-bind:saveCallback="saveExercise" v-bind:cancelCallback="cancelExercise" v-bind:deleteCallback="deleteExercise" />
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var toaster = require('../../toaster');

module.exports = {
    data () {
        return {
            exercises: [],
            muscleGroups: [],
            selectedExercise: null
        }
    },
    components: {
        'exercise-editor': require('./exercise-editor')
    },
    methods: {
        loadExercises: function () {
            var self = this;
            api.listExercises().then(function (exercises) {
                self.exercises = exercises;
            });
        },
        createExercise: function(){
            this.showExercise({id: null, name: null});
        },
        editExercise: function (id) {
            var self = this;
            api.getExercise(id).then(function (exerciseDetails) {
                self.showExercise(exerciseDetails);
            });
        },
        saveExercise: function (exercise) {
            var self = this;
            api.saveExercise(exercise).then(function (savedExercise) {
                self.loadExercises();
                self.$router.push({ name: 'exercises' });
                self.showSummary();
            });
        },
        cancelExercise: function (exercise) {
            this.$router.push({ name: 'exercises' });
            this.showSummary();
        },
        deleteExercise: function (exercise) {
            var self = this;
            api.deleteExercise(exercise.id).then(function () {
                self.loadExercises();
                self.$router.push({ name: 'exercises' });
                self.showSummary();
            });
        },
        showExercise: function (exercise) {
            this.selectedExercise = exercise;
        },
        showSummary() {
            this.selectedExercise = null;
        },

    },
    created: function () {
        this.loadExercises();
        var id = this.$route.params.id;
        if (id) {
            this.editExercise(id);
        }
    },
    beforeRouteUpdate (to, from, next) {
        if (to.params.id) {
            this.editExercise(to.params.id);
        }
        else {
            this.showSummary();
        }
        next();
    }
}
</script>

<style scoped>
    #exercise-list{
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    #exercise-list td {
        padding-bottom: 0px;
    }
    #exercise-list td span{
        margin: 5px;
    }
</style>