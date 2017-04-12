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
                <div class="row">
                    <div class="col-sm-12">
                        <table class="table" id="exercise-list">
                            <thead>
                                <tr>
                                    <th>Nimi</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="exercise in exercises">
                                    <td>{{ exercise.name }}</td>
                                    <td>
                                        <button class="btn" @click="editExercise(exercise)">Tiedot</button>
                                        <button class="btn btn-link" @click="deleteExercise(exercise)">Poista</button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
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
        editExercise: function (exercise) {
            var self = this;
            api.getExercise(exercise.id).then(function (exerciseDetails) {
                self.showExercise(exerciseDetails);
            });
        },
        saveExercise: function (exercise) {
            var self = this;
            api.saveExercise(exercise).then(function (savedExercise) {
                self.loadExercises();
                self.showList();
            });
        },
        cancelExercise: function (exercise) {
            this.showList();
        },
        deleteExercise: function (exercise) {
            var self = this;
            api.deleteExercise(exercise.id).then(function () {
                self.loadExercises();
                self.showList();
            });
        },
        showExercise: function (exercise) {
            this.selectedExercise = exercise;
        },
        showList() {
            this.selectedExercise = null;
        },

    },
    created: function () {
        this.loadExercises();
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