<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("exercises") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="createExercise">{{ $t("create") }}</button>
                </div>
            </div>
            <div class="row" v-if="exercises.length > 0">
                <div class="col-sm-12">
                    <table class="table" id="exercise-list">
                        <thead>
                            <tr>
                                <th>{{ $t("name") }}</th>
                                <th>{{ $t("sets") }}</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="exercise in exercises">
                                <td><router-link :to="{ name: 'exercise-details', params: { id: exercise.id } }">{{ exercise.name }}</router-link></td>
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
                    {{ $t("noExercises") }}
                </div>
            </div>
        </section>
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var toaster = require('../../toaster');

module.exports = {
    data () {
        return { }
    },
    computed: {
        exercises: function () {
            return this.$store.state.training.exercises;
        }
    },
    methods: {
        createExercise: function () {
            this.$router.push({ name: 'exercise-details', params: { id: constants.NEW_ID } });
        },
        deleteExercise: function (exercise) {
            var self = this;
            self.$store.dispatch(constants.DELETE_EXERCISE, {
                exercise,
                success: function () {
                },
                failure: function () {
                    toaster(self.$t('deleteFailed'));
                }
            });
        }
    },
    created: function () {
        var self = this;

        self.$store.dispatch(constants.FETCH_EXERCISES, {
            success: function () {
                self.$store.commit(constants.LOADING_DONE);
            },
            failure: function () {
                toaster(self.$t('efetchFailed'));
            }
        });
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