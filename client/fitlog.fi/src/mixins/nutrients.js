import constants from '../store/constants'

export default {
    computed: {
      $nutrients() {
        return this.$store.state.nutrition.nutrients;
      }
    },
    created() {
      var delay = 100;
      var loader = () => {
        if(this.isLoggedIn){
          this.$store.dispatch(constants.FETCH_NUTRIENTS, { }).then(nutrients => {
            if (this.$nutrientsLoaded) {
              this.$nutrientsLoaded(nutrients);
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