import constants from '../../store/constants'
import api from '../../api'
import moment from 'moment'
import toaster from '../../toaster'
import utils from '../../utils'
import DatetimePicker from '../../components/datetime-picker'
import EnergyExpenditureEditor from './energy-expenditure-editor.vue'

export default {
    data () {
        return {
            showEditEnergyExpenditure: false,
            energyExpenditure: undefined
        }
    },
    computed: {
        energyExpenditures() {
            var self = this;
            return this.$store.state.training.energyExpenditures.filter(e => moment(e.time).isBetween(self.start, self.end));
        },
        start() {
            var self = this;
            return this.$store.state.training.energyExpendituresDisplayStart;
        },
        end() {
            var self = this;
            return this.$store.state.training.energyExpendituresDisplayEnd;
        }
    },
    components: {
        DatetimePicker,
        EnergyExpenditureEditor
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
            self.progress = [];
            self.$store.dispatch(constants.SELECT_ENERGY_EXPENDITURE_DATE_RANGE, {
                start: start,
                end: end,
                success() {
                    self.fetchEnergyExpenditures();
                }
            });
        },
        fetchEnergyExpenditures() {
            var self = this;
            this.$store.dispatch(constants.FETCH_ENERGY_EXPENDITURES, {
                start: self.start,
                end: self.end,
                success() {
                    self.$store.commit(constants.LOADING_DONE);
                }
            });
        },
        changeStart(date) {
            this.showDateRange(date, this.end);
        },
        changeEnd(date) {
            this.showDateRange(this.start, date);
        },
        createEnergyExpenditure() {
            this.energyExpenditure = { time: utils.previousHalfHour(), hours: 1, minutes: 0 };
            this.showEditEnergyExpenditure = true;
        },
        editEnergyExpenditure(expenditure) {
            this.energyExpenditure = {
                id: expenditure.id,
                time: expenditure.time,
                hours: expenditure.hours,
                minutes: expenditure.minutes,
                activityId: expenditure.activityId,
                activityName: expenditure.activityName,
                energyKcal: expenditure.energyKcal
            }
            this.showEditEnergyExpenditure = true;
        },
        saveEnergyExpenditure(energyExpenditure) {
            var self = this;
            self.$store.dispatch(constants.SAVE_ENERGY_EXPENDITURE, {
                energyExpenditure,
                success() {
                    toaster.info(self.$t('saveSuccessful'));
                    self.showEditEnergyExpenditure = false;
                    self.energyExpenditure = undefined;
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            })
        }
    },
    created() {
        var self = this;
        self.$store.dispatch(constants.FETCH_ACTIVITIES, {
            start: self.start,
            end: self.end,
            success() {
                self.$store.commit(constants.LOADING_DONE);
            }
        });
        if (self.start && self.end) {
            self.fetchEnergyExpenditures();
        }
        else {
            self.showDays(7);
        }
    },
    beforeRouteUpdate (to, from, next) {
        /*
        if (to.params.id) {
            this.editExercise(to.params.id);
        }
        else {
            this.showSummary();
        }*/
        next();
    }
}