import constants from '../store/constants'

export default {
    computed: {
        $mealDefinitions() {
            return this.$store.state.nutrition.mealDefinitions;
        }
    },
    created() {
        var delay = 100;
        var loader = () => {
            if(this.isLoggedIn){
              this.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, { }).then(mealDefinitions => {
                if (this.$mealDefinitionsLoaded) {
                  this.$mealDefinitionsLoaded(mealDefinitions);
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