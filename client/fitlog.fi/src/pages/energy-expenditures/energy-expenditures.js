import constants from '../../store/constants'
import api from '../../api'
import moment from 'moment'
import utils from '../../utils'
import EnergyExpenditureDetails from './energy-expenditure-details.vue'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    components:{
        EnergyExpenditureDetails
    },
    data () {
        return {
        }
    },
    computed: {
        energyExpenditures() {
          return this.$store.state.training.energyExpenditures.filter(e => moment(e.time).isBetween(this.start, this.end));
        },
        start() {
          return this.$store.state.training.energyExpendituresDisplayStart;
        },
        end() {
          return this.$store.state.training.energyExpendituresDisplayEnd;
        }
    },
    methods: {
        showDays(days) {
          var end = moment().endOf('day').toDate();
          var start = moment().subtract(days - 1, 'days').startOf('day').toDate();
          this.showDateRange(start, end);
        },
        showMonths(months){
          var end = moment().endOf('day').toDate();
          var start = moment().subtract(months, 'months').add(1,'days').startOf('day').toDate();
          this.showDateRange(start, end);
        },
        showDateRange(start, end) {
          this.progress = [];
          this.$store.dispatch(constants.SELECT_ENERGY_EXPENDITURE_DATE_RANGE, {
            start: start,
            end: end
          }).then(_ => {
            this.fetchEnergyExpenditures();
          });
        },
        fetchEnergyExpenditures() {
          this.$store.dispatch(constants.FETCH_ENERGY_EXPENDITURES, {
            start: this.start,
            end: this.end
          }).then(_ => {
            this.$store.commit(constants.LOADING_DONE);
          });
        },
        changeStart(date) {
          this.showDateRange(date, this.end);
        },
        changeEnd(date) {
          this.showDateRange(this.start, date);
        },
        createEnergyExpenditure() {
          var expenditure = { time: utils.previousHalfHour(), hours: 1, minutes: 0 };
          this.$refs.editEnergyExpenditureDetails.show(expenditure);
        },
        editEnergyExpenditure(expenditure) {
          this.$refs.editEnergyExpenditureDetails.show(expenditure);
        },
        saveEnergyExpenditure(energyExpenditure) {
          this.$store.dispatch(constants.SAVE_ENERGY_EXPENDITURE, {
            energyExpenditure
          }).then(_ => {
            this.notifySuccess(this.$t('saveSuccessful'));
            this.$refs.editEnergyExpenditureDetails.hide();
          }).catch(_ => {
            this.notifyError(this.$t('saveFailed'));
          });
        },
        deleteEnergyExpenditure(energyExpenditure){
          this.$store.dispatch(constants.DELETE_ENERGY_EXPENDITURE, {
                energyExpenditure
            }).then(_ => {
              this.notifySuccess(this.$t('deleteSuccessful'));
            }).catch(_ => {
              this.notifyError(this.$t('deleteFailed'));
            });
        },
        clickEnergyExpenditure(expenditure){
          var actions = [
            {
              label: this.$t('edit'),
              icon: 'fas fa-edit',
              handler: () => {
                this.editEnergyExpenditure(expenditure);
              }
            },
            {
              label: this.$t('delete'),
              icon: 'fas fa-trash',
              handler: () => {
                this.deleteEnergyExpenditure(expenditure);
              }
            }
          ];

          if(expenditure.workoutId){
            actions.splice(1, 0, {
              label: this.$t('showWorkout'),
              icon: 'fas fa-heartbeat',
              handler: () => {
                this.$router.push({ name: 'workout-details', params: { id: expenditure.workoutId } });
              }
            });
          }
            
          this.$q.actionSheet({
            title: this.formatDateTime(expenditure.time),
            grid: true,
            actions: actions,
            dismiss: {
              label: this.$t('cancel'),
              handler: () => {
                  
              }
            }
          });
            
        }
    },
    created() {
      this.$store.dispatch(constants.FETCH_ACTIVITIES, {
        start: this.start,
        end: this.end
      }).then(_ => {
        this.$store.commit(constants.LOADING_DONE);
      });
      if (this.start && this.end) {
        this.fetchEnergyExpenditures();
      }
      else {
        this.showDays(7);
      }
    },
    beforeRouteUpdate (to, from, next) {
        next();
    }
}