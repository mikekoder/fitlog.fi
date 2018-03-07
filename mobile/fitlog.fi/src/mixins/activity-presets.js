import constants from '../store/constants'

export default {
    computed: {
        $activityPresets() {
            return this.$store.state.activities.activityPresets;
        }
    },
    created() {

        var self = this;
        var delay = 100;
        var loader = () => {
            if(self.isLoggedIn){
                self.$store.dispatch(constants.FETCH_ACTIVITY_PRESETS, {
                    success(presets) {
                        if (self.$activityPresetsLoaded) {
                            self.$activityPresetsLoaded(presets);
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