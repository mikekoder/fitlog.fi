import { QTabs, QTab, QTabPane, QField, QInput, QScrollArea, QSearch, QAutocomplete, QSelect, QBtn, QModal, QList, QItem, QToggle } from 'quasar'
import constants from '../../store/constants'
import utils from '../../utils'

export default {
      components: {
        QTabs, QTab, QTabPane, QField, QInput, QScrollArea, QSearch, QAutocomplete, QSelect, QBtn, QModal, QList, QItem, QToggle
    },
    data() {
        return {
            id: null,
            name: null,
            percentageBW: null,
            targets: []
        }
    },
    computed: {
        muscleGroups() {
            return this.$store.state.training.muscleGroups;
        }
    },
    methods: {
        save() {
            var self = this;

            var exercise = {
                id: self.id,
                name: self.name,
                percentageBW: self.percentageBW,
                targets: self.targets
            };

            self.$store.dispatch(constants.SAVE_EXERCISE, {
                exercise,
                success() {
                    self.$router.replace({ name: 'exercises' });
                },
                failure() {
                }
            });
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteExercise() {
            var self = this;
            self.$store.dispatch(constants.DELETE_EXERCISE, {
                exercise: { id: self.id },
                success() {
                    self.$router.push({ name: 'exercises' });
                },
                failure() {
                }
            });
        }
    },
    created() {
        var self = this;
        var id = this.$route.params.id;
        self.$store.dispatch(constants.FETCH_MUSCLEGROUPS, {
            success(muscleGroups) {
                if (id == constants.NEW_ID) {
                    self.id = undefined;
                    self.name = undefined;
                    self.targets = [];
                    self.$store.commit(constants.LOADING_DONE);
                }
                else {
                    self.$store.dispatch(constants.FETCH_EXERCISE, {
                        id,
                        success(exercise) {
                            self.id = exercise.id;
                            self.name = exercise.name;
                            self.percentageBW = exercise.percentageBW;
                            self.targets = exercise.targets;
                            self.$store.commit(constants.LOADING_DONE);
                        },
                        failure() {
                        }
                    });
                }

                
            },
            failure() {
            }
        });
    }
}
