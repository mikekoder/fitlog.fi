import constants from '../../store/constants'
import api from '../../api'
import formatters from '../../formatters'
import moment from 'moment'
import toaster from '../../toaster'
import utils from '../../utils'
import exercisesMixin from '../../mixins/exercises'

export default {
    mixins: [exercisesMixin],
    data() {
        return {
        }
    },
    computed: {
        muscleGroups() {
            return this.$store.state.training.muscleGroups;
        },
        workouts() {
            var self = this;
            return this.$store.state.training.workouts.filter(w => moment(w.time).isBetween(self.start, self.end));
        },
        activeRoutine() {
            return this.$store.state.training.activeRoutine;
        },
        start() {
            var self = this;
            return this.$store.state.training.workoutsDisplayStart;
        },
        end() {
            var self = this;
            return this.$store.state.training.workoutsDisplayEnd;
        },
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker')
    },
    methods: {
        showWeek() {
            var end = moment().endOf('day').toDate();
            var start = moment().startOf('isoWeek').toDate();
            this.showDateRange(start, end);
        },
        showMonth() {
            var end = moment().endOf('day').toDate();
            var start = moment().startOf('month').toDate();
            this.showDateRange(start, end);
        },
        showDays(days) {
            var end = moment().endOf('day').toDate();
            var start = moment().subtract(days - 1, 'days').startOf('day').toDate();
            this.showDateRange(start, end);
        },
        showDateRange(start, end) {
            var self = this;
            self.$store.dispatch(constants.SELECT_WORKOUT_DATE_RANGE, {
                start: start,
                end: end,
                success() {
                    self.fetchWorkouts();
                }
            });
        },
        fetchWorkouts() {
            var self = this;
            this.$store.dispatch(constants.FETCH_WORKOUTS, {
                start: self.start,
                end: self.end,
                success() {
                    self.$store.commit(constants.LOADING_DONE);
                }
            });
        },
        createWorkout(routineId, workoutId) {
            if (routineId && workoutId) {
                this.$router.push({ name: 'workout-details', params: { id: constants.NEW_ID }, query: { [constants.ROUTINE_PARAM]: routineId, [constants.WORKOUT_PARAM]: workoutId } });
            }
            else {
                this.$router.push({ name: 'workout-details', params: { id: constants.NEW_ID } });
            }
        },
        deleteWorkout(workout) {
            var self = this;
            self.$store.dispatch(constants.DELETE_WORKOUT, {
                workout,
                success() { },
                failure() {
                    toaster(self.$t('deleteFailed'));
                }
            });
        },
        date: formatters.formatDate,
        datetime: formatters.formatDateTime,
        decimal(value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        }
    },
    created() {

        var self = this;
        this.$store.dispatch(constants.FETCH_MUSCLEGROUPS, {
            success() { },
            failure() { }
        });
        this.$store.dispatch(constants.FETCH_ROUTINES, {
            success() { },
            failure() { }
        });
        if (self.start && self.end) {
            self.fetchWorkouts();
        }
        else {
            self.showDays(7);
        }

    }
}