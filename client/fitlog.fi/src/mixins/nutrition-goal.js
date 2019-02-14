import constants from '../store/constants'

export default {
    computed: {
        $nutritionGoal() {
            return this.$store.state.nutrition.activeNutritionGoal;
        }
    },
    created() {
      var delay = 100;
      var loader = () => {
        if(this.isLoggedIn){
          this.$store.dispatch(constants.FETCH_ACTIVE_NUTRITION_GOAL, { });
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