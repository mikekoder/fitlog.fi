import constants from '../store/constants'

export default {
    computed: {
        $exercises() {
            return this.$store.state.training.exercises;
        }
    },
    created() {
        var delay = 100;
        var loader = () => {
            if(this.isLoggedIn){

              this.$store.dispatch(constants.FETCH_MUSCLEGROUPS, {}).then(muscleGroups => {
                if (this.$muscleGroupsLoaded) {
                  this.$muscleGroupsLoaded(muscleGroups);
                }
              });
       
              this.$store.dispatch(constants.FETCH_EQUIPMENT, {}).then(equipment => {
                if (this.$equipmentLoaded) {
                  this.$equipmentLoaded(equipment);
                }
              });
        
              this.$store.dispatch(constants.FETCH_EXERCISES, { }).then(exercises => {
                if (this.$exercisesLoaded) {
                  this.$exercisesLoaded(exercises);
                }

              });
              
              this.$store.dispatch(constants.FETCH_LATEST_EXERCISES, {}).then(exercises => {});
              
              this.$store.dispatch(constants.FETCH_MOST_USED_EXERCISES, {}).then(exercises => {});
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