<template>
    <div>
        <div v-if="!selectedExercise">
            <section class="content-header"><h1>{{ $t("exercises.title") }}</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createExercise">{{ $t("exercises.create") }}</button>
                    </div>
                </div>
                <div class="row" v-if="exercises.length > 0">
                    <div class="col-sm-12">
                        <table class="table" id="exercise-list">
                            <thead>
                                <tr>
                                    <th>{{ $t("exercises.columns.name") }}</th>
                                    <th>{{ $t("exercises.columns.sets") }}</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="exercise in exercises">
                                    <td><router-link :to="{ name: 'exercises', params: { id: exercise.id } }">{{ exercise.name }}</router-link></td>
                                    <td>{{ exercise.usageCount }}</td>
                                    <td>
                                        <button class="btn btn-danger btn-xs" @click="deleteExercise(exercise)">{{ $t("delete") }}</button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="row" v-if="exercises.length == 0">
                    <div class="col-sm-12">
                        <br />
                        {{ $t("exercises.noExercises") }}
                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedExercise">
            <section class="content-header"><h1>{{ $t("exercises.exerciseDetails") }}</h1></section>
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
    var constants = require('../../store/constants')
    var api = require('../../api');
    var toaster = require('../../toaster');

module.exports = {
    data () {
        return {
            selectedExercise: null
        }
    },
    computed: {
        muscleGroups: function () {
            return this.$store.state.training.muscleGroups;
        },
        exercises: function () {
            return this.$store.state.training.exercises;
        }
    },
    components: {
        'exercise-editor': require('./exercise-editor')
    },
    methods: {
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
            this.$store.dispatch(constants.SAVE_EXERCISE, {
                exercise,
                success: function () {
                    self.$router.push({ name: 'exercises' });
                    self.showSummary();
                },
                failure: function () {
                    toaster(this.$t('exercises.saveFailed'));
                }
            });
        },
        cancelExercise: function (exercise) {
            this.$router.push({ name: 'exercises' });
            this.showSummary();
        },
        deleteExercise: function (exercise) {
            var self = this;
            this.$store.dispatch(constants.DELETE_EXERCISE, {
                exercise,
                success: function () {
                    self.$router.push({ name: 'exercises' });
                    self.showSummary();
                },
                failure: function () {
                    toaster(this.$t('exercises.deleteFailed'));
                }
            });
        },
        showExercise: function (exercise) {
            this.selectedExercise = exercise;
        },
        showSummary() {
            this.selectedExercise = null;
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