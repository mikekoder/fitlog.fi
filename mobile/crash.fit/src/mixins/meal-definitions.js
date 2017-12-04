import constants from '../store/constants'

export default {
    computed: {
        $mealDefinitions() {
            return this.$store.state.nutrition.mealDefinitions;
        }
    },
    created() {
        var self = this;
        this.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
            success(mealDefinitions) {
                if (self.$mealDefinitionsLoaded) {
                    self.$mealDefinitionsLoaded(mealDefinitions);
                }
            },
            failure() { }
        });
    }
}