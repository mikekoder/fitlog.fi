import constants from '../store/constants'

export default {
    computed: {
        $nutrients() {
            return this.$store.state.nutrition.nutrients;
        }
    },
    created() {
        var self = this;
        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success(nutrients) {
                if (self.$nutrientsLoaded) {
                    self.$nutrientsLoaded(nutrients);
                }
            },
            failure() { }
        });
    }
}