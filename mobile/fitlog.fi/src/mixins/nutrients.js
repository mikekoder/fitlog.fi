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
            if(self.isLoggedIn){
                self.$store.dispatch(constants.FETCH_NUTRIENTS, { }).then(nutrients => {
                    if (self.$nutrientsLoaded) {
                        self.$nutrientsLoaded(nutrients);
                    }
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