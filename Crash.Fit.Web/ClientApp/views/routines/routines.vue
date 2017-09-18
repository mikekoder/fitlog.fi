<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("routines") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="createRoutine">{{ $t("create") }}</button>
                </div>
            </div>
            <div class="row" v-if="routines.length > 0">
                <div class="col-sm-12">     
                    <table class="table" id="routine-list">
                        <thead>
                            <tr>
                                <th class="name">{{ $t("name") }}</th>
                                <th></th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="routine" v-for="routine in routines">
                                <td><router-link :to="{ name: 'routine-details', params: { id: routine.id } }">{{ routine.name }}</router-link></td>
                                <td>
                                    <span v-if="routine.active">{{ $t("active") }}</span>
                                    <button class="btn btn-primary" v-if="!routine.active" @click="activate(routine)">{{ $t("activate") }}</button>
                                </td>
                                <td><button class="btn btn-danger btn-xs" @click="deleteRoutine(routine)">{{ $t("delete") }}</button></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="routines.length == 0">
                <div class="col-sm-12">
                    <br />
                    {{ $t("noRoutines") }}
                </div>
            </div>
        </section>
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import api from '../../api'
    import formatters from '../../formatters'
    import toaster from '../../toaster'

export default {
    data () {
        return {
            selectedRoutine: null
        }
    },
    computed: {
        routines: function(){
            return this.$store.state.training.routines;
        },
        exercises() {
            return this.$store.state.training.exercises;
        }
    },
    methods: {
        createRoutine: function(){
            this.$router.push({ name: 'routine-details', params: { id: constants.NEW_ID } });
        },
        activate: function(routine){
            var self = this;
            this.$store.dispatch(constants.ACTIVATE_ROUTINE, {
                routine,
                success() { },
                failure() {
                    toaster(this.$t('activationFailed'));
                }
            });
        },
        deleteRoutine(routine) {
            var self = this;
            this.$store.dispatch(constants.DELETE_ROUTINE, {
                routine,
                success() { },
                failure() {
                    toaster(this.$t('deleteFailed'));
                }
            });
        }
    },
    created() {

        var self = this;
        this.$store.dispatch(constants.FETCH_EXERCISES, {
            forceRefresh: true,
            success() { },
            failure() { }
        });
        this.$store.dispatch(constants.FETCH_ROUTINES, {
            success() {
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() { }
        });
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