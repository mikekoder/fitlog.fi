import constants from '../../store/constants'
import utils from '../../utils'
import api from '../../api'
import DAYS from '../../enums/days'
import nutrientsMixin from '../../mixins/nutrients'
import nutrientGroupsMixin from '../../mixins/nutrient-groups'
import mealDefinitionsMixin from '../../mixins/meal-definitions'
import Help from './nutrition-goal-help.vue'
import PageMixin from '../../mixins/page'

export default {
    components:{
        'nutrition-goal-help': Help
    },
    mixins:[nutrientsMixin, nutrientGroupsMixin, mealDefinitionsMixin,PageMixin],
    data () {
        return {
            tab: 'tab-0',
            selectedGroup: undefined,
            id: undefined,
            name: undefined,
            periods: [],
            energyDistributionId: constants.ENERGY_DISTRIBUTION_ID
        }
    },
    computed: {
        nutrientGroups() {
            return this.$store.state.nutrition.nutrientGroups;
        },
        nutrientsGrouped() {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        canSave(){
            return this.name && true;
        }
    },
    methods: {
        toggleGroup(group) {
            if (this.selectedGroup == group) {
                this.selectedGroup = undefined;
            }
            else {
                this.selectedGroup = group;
            }
        },
        $nutrientsLoaded() {
          var id = this.$route.params.id;
          if (id == constants.NEW_ID) {
            this.populate({ id: undefined, name: undefined, periods: [] });
            this.addPeriod();
            this.$store.commit(constants.LOADING_DONE);
          }
          else {
            this.$store.dispatch(constants.FETCH_NUTRITION_GOAL, {
              id
            }).then(goal => {
              this.populate(goal);
              this.$store.commit(constants.LOADING_DONE);
            }).catch(_ => {
              this.notifyError(this.$t('fetchFailed'));
            });
          }
          this.toggleGroup(this.$nutrientGroups[0]);
        },
        addPeriod() {
          var period = {
            monday: false,
            tuesday: false,
            wednesday: false,
            thursday: false,
            friday: false,
            saturday: false,
            sunday: false,
            exerciseDay: false,
            restDay: false,
            wholeDay: true,
            mealDefinitions: {},
            nutrients: {}
          };
 
          for (var i in this.nutrientsGrouped) {
            for (var j in this.nutrientsGrouped[i]) {
              this.$set(period.nutrients,this.nutrientsGrouped[i][j].id, { min: undefined, max: undefined });
            }
          }
          this.$mealDefinitions.forEach(d => {
              period.mealDefinitions[d.id] = false;
          });

          this.periods.push(period);
          this.selectedPeriod = period;
        },
        deletePeriod(period) {
          this.periods.splice(this.periods.findIndex(p => p == this.selectedPeriod), 1);
          this.selectedPeriod = undefined;
        },
        save() {
          var goal = {
            id: this.id,
            name: this.name,
            periods: []
          };
            
          for (var i in this.periods) {
            var period = this.periods[i];
            var mealDefinitions = [];
            for (var id in period.mealDefinitions) {
                if (period.mealDefinitions[id]) {
                    mealDefinitions.push(id);
                }
            }
            var nutrients = [];
            for (var id in period.nutrients) {
                if (period.nutrients[id].min || period.nutrients[id].max || period.nutrients[id].min == 0 || period.nutrients[id].max == 0) {
                    nutrients.push({ nutrientId: id, min: period.nutrients[id].min, max: period.nutrients[id].max });
                }
            }
            goal.periods.push({
                id: period.id,
                monday: period.monday,
                tuesday: period.tuesday,
                wednesday: period.wednesday,
                thursday: period.thursday,
                friday: period.friday,
                saturday: period.saturday,
                sunday: period.sunday,
                exerciseDay: period.exerciseDay,
                restDay: period.restDay,
                wholeDay: period.wholeDay,
                mealDefinitions,
                nutrients
            });
          }

        this.$store.dispatch(constants.SAVE_NUTRITION_GOAL, {
            goal
          }).then(_ => {
            this.notifySuccess(this.$t('saveSuccessful'));
            this.$router.replace({ name: 'nutrition-goals' });
          }).catch(_ => {
            this.notifyError(this.$t('saveFailed'));
          });
        },
        selectPeriod(period) {
            this.selectedPeriod = period;
        },
        daysFormatted(goal) {
          var text = '';
          var count = 0;
          var days = 0;
          if (goal.monday) {
            text += ' ' + this.$t("mondayAbbr");
            count++;
            days |= DAYS.MONDAY;
          }
          if (goal.tuesday) {
            text += ' ' + this.$t("tuesdayAbbr");
            count++;
            days |= DAYS.TUESDAY;
          }
          if (goal.wednesday) {
            text += ' ' + this.$t("wednesdayAbbr");
            count++;
            days |= DAYS.WEDNESDAY;
          }
          if (goal.thursday) {
            text += ' ' + this.$t("thursdayAbbr");
            count++;
            days |= DAYS.THURSDAY;
          }
          if (goal.friday) {
            text += ' ' + this.$t("fridayAbbr");
            count++;
            days |= DAYS.FRIDAY;
          }
          if (goal.saturday) {
            text += ' ' + this.$t("saturdayAbbr");
            count++;
            days |= DAYS.SATURDAY;
          }
          if (goal.sunday) {
            text += ' ' + this.$t("sundayAbbr");
            count++;
            days |= DAYS.SUNDAY;
          }
          if (goal.exerciseDay) {
            text += ' ' + this.$t("exerciseDay");
            count++;
            days |= DAYS.EXERCISEDAY;
          }
          if (goal.restDay) {
            text += ' ' + this.$t("restDay");
            count++;
            days |= DAYS.RESTDAY;
          }
          if (days == DAYS.WEEKDAYS) {
            return this.$t('weekdays');
          }
          if (days == DAYS.WEEKEND) {
            return this.$t('weekends');
          }
          if ((days & DAYS.WEEK) == DAYS.WEEK) {
            return this.$t('everyDay');
          }
          if (((days & DAYS.EXERCISEDAY) == DAYS.EXERCISEDAY) && ((days & DAYS.RESTDAY) == DAYS.RESTDAY)) {
            return this.$t('everyDay');
          }
          if (count == 0 || count == 9) {
            return this.$t('everyDay');
          }
          return text;
        },
        mealsFormatted(period) {
            if (period.wholeDay) {
              return this.$t('wholeDay');
            }
            var text = '';
            var count = 0;
            for (var id in period.mealDefinitions) {
              if (period.mealDefinitions[id]) {
                var mealdef = this.$mealDefinitions.find(d => d.id == id);
                text += ', ' + mealdef.name;
                count++;
              }
            }
            if (text.length == 0 || count == this.$mealDefinitions.length) {
                return this.$t('everyMeal');
            }
            return text.substr(1);
        },
        populate(goal) {
          this.id = goal.id;
          this.name = goal.name;
          if (goal.periods) {
            goal.periods.forEach(period => {
              var mealDefs = period.mealDefinitions;
              period.mealDefinitions = {};
              mealDefs.forEach(defId => {
                period.mealDefinitions[defId] = true;
              });

              var nutrients = period.nutrients;
              period.nutrients = {};

              this.$nutrients.forEach(nutrient => {
                var value = nutrients.find(n => n.nutrientId == nutrient.id);
                if (value) {
                  this.$set(period.nutrients, nutrient.id, { min: value.min, max: value.max });
                }
                else {
                  this.$set(period.nutrients, nutrient.id, { min: undefined, max: undefined });
                }
              });
            });
  
            this.periods = goal.periods;
          }
          else {
            this.periods = [];
          }
        },
        cancel() {
            this.$router.go(-1);
        },
        showHelp(){
            this.$refs.help.open();
        }
    },
    created() {
        
    }
}