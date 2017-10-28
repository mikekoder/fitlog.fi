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
        activate(routine){
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