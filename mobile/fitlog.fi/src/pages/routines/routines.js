import constants from '../../store/constants'
import api from '../../api'
import utils from '../../utils'

export default {
  data () {
    return {
        selectedRoutine: null
    }
},
computed: {
    routines(){
        return this.$store.state.training.routines;
    },
    exercises() {
        return this.$store.state.training.exercises;
    }
},
methods: {
    createRoutine(){
        this.$router.push({ name: 'routine-details', params: { id: constants.NEW_ID } });
    },
    showRoutine(routine){
        this.$router.push({name: 'routine-details',params:{id:routine.id}});
    },
    activate(routine){
        var self = this;
        this.$store.dispatch(constants.ACTIVATE_ROUTINE, {
            routine,
            success() { },
            failure() {
                self.notifyError(self.$t('activationFailed'));
            }
        });
    },
    deleteRoutine(routine) {
        var self = this;
        this.$store.dispatch(constants.DELETE_ROUTINE, {
            routine,
            success() { },
            failure() {
                self.notifyError(self.$t('deleteFailed'));
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
