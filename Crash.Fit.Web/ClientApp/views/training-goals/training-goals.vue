<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("trainingGoals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="createGoal">{{ $t("create") }}</button>
                </div>
            </div>
            <div class="row" v-if="goals.length > 0">
                <div class="col-sm-12">     
                    <table class="table" id="goal-list">
                        <thead>
                            <tr>
                                <th class="name">{{ $t("name") }}</th>
                                <th></th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="routine" v-for="goal in goals">
                                <td><router-link :to="{ name: 'training-goal-details', params: { id: goal.id } }">{{ goal.name }}</router-link></td>
                                <td>
                                    <span v-if="goal.active">{{ $t("active") }}</span>
                                    <button class="btn btn-primary" v-if="!goal.active" @click="activate(goal)">{{ $t("activate") }}</button>
                                </td>
                                <td><button class="btn btn-danger btn-xs" @click="deleteGoal(goal)">{{ $t("delete") }}</button></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="goals.length == 0">
                <div class="col-sm-12">
                    <br />
                    {{ $t("notrainingGoals") }}
                </div>
            </div>
        </section>
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import toaster from '../../toaster'

export default {
    data () {
        return {
            goals: []
        }
    },
    computed: {
  
    },
    components: {},
    methods: {
        createGoal(){
            this.$router.push({ name: 'training-goal-details', params: { id: constants.NEW_ID } });
        },
        activate(goal){
            var self = this;
            this.$store.dispatch(constants.ACTIVATE_TRAINING_GOAL, {
                goal,
                success() { },
                failure() {
                    toaster(this.$t('activationFailed'));
                }
            });
        },
        deleteGoal(goal) {
            var self = this;
            this.$store.dispatch(constants.DELETE_TRAINING_GOAL, {
                goal,
                success() { },
                failure() {
                    toaster(this.$t('deleteFailed'));
                }
            });
        }
    },
    created() {
        var self = this;
       
        self.$store.dispatch(constants.FETCH_TRAINING_GOALS, {
            success(goals) {
                self.goals = goals;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() { }
        });
    }
}
</script>

<style scoped>
</style>