import constants from '../store/constants'

export default {
    computed: {
        $activityPresets() {
            return this.$store.state.activities.activityPresets;
        }
    },
    created() {
        var self = this;
        this.$store.dispatch(constants.FETCH_ACTIVITY_PRESETS, {
            success(presets) {
                if (self.$activityPresetsLoaded) {
                    self.$activityPresetsLoaded(presets);
                }
            },
            failure() { }
        });
    }
}