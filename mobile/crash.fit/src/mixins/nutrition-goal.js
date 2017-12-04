import constants from '../store/constants'

export default {
    computed: {
        $nutritionGoal() {
            return this.$store.state.nutrition.activeNutritionGoal;
        }
    },
    created() {
        var self = this;
        this.$store.dispatch(constants.FETCH_ACTIVE_NUTRITION_GOAL, {
            success() { },
            failure() { }
        });
    }
}