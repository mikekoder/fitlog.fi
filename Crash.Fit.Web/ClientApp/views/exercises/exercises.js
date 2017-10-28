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
