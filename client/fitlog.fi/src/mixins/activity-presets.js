import constants from '../store/constants'

export default {
    computed: {
        $activityPresets() {
            return this.$store.state.activities.activityPresets;
        }
    },
    created() {
        var delay = 100;
        var loader = () => {
            if(this.isLoggedIn){
              this.$store.dispatch(constants.FETCH_ACTIVITY_PRESETS, { }).then(presets => {
                if (this.$activityPresetsLoaded) {
                  this.$activityPresetsLoaded(presets);
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