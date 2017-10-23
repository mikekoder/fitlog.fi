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
    import constants from '../../store/constants'
    import toaster from '../../toaster'
    import exercisesMixin from '../../mixins/exercises'

    export default {
        mixins: [exercisesMixin],
        data() {
            return {}
        },
        computed: {
        },
        methods: {
            createExercise() {
                this.$router.push({ name: 'exercise-details', params: { id: constants.NEW_ID } });
            },
            deleteExercise(exercise) {
                var self = this;
                self.$store.dispatch(constants.DELETE_EXERCISE, {
                    exercise,
                    success() {
                    },
                    failure() {
                        toaster(self.$t('deleteFailed'));
                    }
                });
            }
        },
        created() {
            this.$store.commit(constants.LOADING_DONE);
        }
    }
</script>

<style scoped>
    #exercise-list {
        width: auto;
        table-layout: fixed;
        /*width: 100%;*/
    }

        #exercise-list td {
            padding-bottom: 0px;
        }

            #exercise-list td span {
                margin: 5px;
            }
</style>