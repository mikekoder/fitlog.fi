import constants from '../store/constants'

export default {
    computed: {
        $nutrients() {
            return this.$store.state.nutrition.nutrients;
        }
    },
    created() {
        var self = this;
        var delay = 100;
        var loader = () => {
            if(self.isLoggedIn || delay > 3000){
                self.$store.dispatch(constants.FETCH_NUTRIENTS, {
                    success(nutrients) {
                        if (self.$nutrientsLoaded) {
                            self.$nutrientsLoaded(nutrients);
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