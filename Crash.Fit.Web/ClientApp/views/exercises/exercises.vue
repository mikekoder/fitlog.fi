<template>
    <div>
        <div v-if="!selectedExercise">
            <section class="content-header"><h1>Harjoitukset</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createExercise"><i class="glyphicon glyphicon-plus"></i> Uusi harjoitus</button>

                        <table class="table" id="meal-summary">
                            <thead>
                                <tr>
                                    <th>Nimi</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="exercise in exercises">
                                    <td>{{ exercise.name }}</td>
                                    <td><button class="btn" @click="editExercise(exercise)">Tiedot</button></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedExercise">
            <section class="content-header"><h1>Harjoituksen tiedot</h1></section>
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
            selectedExercise: null
        }
    },
    components: {
        'exercise-editor': require('./exercise-editor')
    },
    methods: {
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
                self.showSummary();
            });
        },
        cancelExercise: function (exercise) {
            this.showList();
        },
        deleteExercise: function (exercise) {
            this.showList();
        },
        showExercise: function (exercise) {
            this.selectedExercise = exercise;
        },
        showSummary() {
            this.selectedExercise = null;
        },

    },
    watch:{
        $route: function(){
            console.log(this.$route);
        }
    },
    created: function () {
        var self = this;
        api.listExercises().then(function (exercises) {
            for (var i in exercises) {
                self.exercises.push(exercises[i]);
            }
        });
    }
}
</script>

<style scoped>
</style>