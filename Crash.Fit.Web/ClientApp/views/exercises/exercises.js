import constants from '../../store/constants'
import toaster from '../../toaster'
import exercisesMixin from '../../mixins/exercises'

export default {
    mixins: [exercisesMixin],
    data() {
        return {
            exercise1RM: undefined,
            oneRepMax: undefined
        }
    },
    computed: {
    },
    methods: {
        createExercise() {
            this.$router.push({ name: 'exercise-details', params: { id: constants.NEW_ID } });
        },
        deleteExercise(exercise) {
            var self = this;
            self.$store.dispatch(constants.DELETE_EXERCISE, {
                exercise,
                success() {
                },
                failure() {
                    toaster(self.$t('deleteFailed'));
                }
            });
        },
        edit1RM(exercise) {
            this.exercise1RM = exercise;
            this.oneRepMax = exercise.oneRepMax;
        },
        cancel1RM() {
            this.exercise1RM = undefined;
            this.oneRepMax = undefined;
        },
        save1RM() {
            var self = this;
            self.$store.dispatch(constants.SAVE_1RM, {
                exerciseId: self.exercise1RM.id,
                oneRepMax: self.oneRepMax,
                success() {
                    self.exercise1RM = undefined;
                    self.oneRepMax = undefined;
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            })
        }
    },
    created() {
        this.$store.commit(constants.LOADING_DONE);
    }
}
