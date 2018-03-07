import constants from '../store/constants'

export default {
    computed: {
        $exercises() {
            return this.$store.state.training.exercises;
        }
    },
    created() {
        var self = this;
        var delay = 100;
        var loader = () => {
            if(self.isLoggedIn){
                self.$store.dispatch(constants.FETCH_EXERCISES, {
                    success(exercises) {
                        if (self.$exercisesLoaded) {
                            self.$exercisesLoaded(exercises);
                        }
                    },
                    failure() { }
                });
            }
            else {
                setTimeout(() => {
                    delay = delay * 2;
                    loader();
                }, delay);
            }
        };

        loader();
    }
}