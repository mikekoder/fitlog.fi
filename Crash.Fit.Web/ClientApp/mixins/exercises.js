import constants from '../store/constants'

export default {
    computed: {
        $exercises() {
            return this.$store.state.training.exercises;
        }
    },
    created() {
        var self = this;
        this.$store.dispatch(constants.FETCH_EXERCISES, {
            success(exercises) {
                if (self.$exercisesLoaded) {
                    self.$exercisesLoaded(exercises);
                }
            },
            failure() { }
        });
    }
}