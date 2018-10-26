import constants from '../store/constants'

export default {
    computed: {
        $exercises() {
            return this.$store.state.training.exercises;
        }
    },
    created() {
        var self = this;
        var delay = 100;
        var loader = () => {
            if(self.isLoggedIn){

                self.$store.dispatch(constants.FETCH_MUSCLEGROUPS, {}).then(muscleGroups => {
                    if (self.$muscleGroupsLoaded) {
                        self.$muscleGroupsLoaded(muscleGroups);
                    }
                });
       
                self.$store.dispatch(constants.FETCH_EQUIPMENT, {}).then(equipment => {
                    if (self.$equipmentLoaded) {
                        self.$equipmentLoaded(equipment);
                    }
                });
         
                self.$store.dispatch(constants.FETCH_EXERCISES, { }).then(exercises => {
                    if (self.$exercisesLoaded) {
                        self.$exercisesLoaded(exercises);
                    }

                });
                
                self.$store.dispatch(constants.FETCH_LATEST_EXERCISES, {}).then(exercises => {});
                
                self.$store.dispatch(constants.FETCH_MOST_USED_EXERCISES, {}).then(exercises => {});
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