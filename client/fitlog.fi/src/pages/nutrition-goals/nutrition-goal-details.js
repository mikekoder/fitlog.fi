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
            var self = this;
            var id = self.$route.params.id;
            if (id == constants.NEW_ID) {
                self.populate({ id: undefined, name: undefined, periods: [] });
                self.addPeriod();
                self.$store.commit(constants.LOADING_DONE);
            }
            else {
                self.$store.dispatch(constants.FETCH_NUTRITION_GOAL, {
                    id
                }).then(goal => {
                    self.populate(goal);
                    self.$store.commit(constants.LOADING_DONE);
                }).catch(_ => {
                    self.notifyError(self.$t('fetchFailed'));
                });
            }
            self.toggleGroup(self.$nutrientGroups[0]);
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
                    period.nutrients[this.nutrientsGrouped[i][j].id] = { min: undefined, max: undefined };
                }
            }
            this.$mealDefinitions.forEach(d => {
                period.mealDefinitions[d.id] = false;
            });

            this.periods.push(period);
            this.selectedPeriod = period;
        },
        deletePeriod(period) {
            var self = this;
            self.periods.splice(self.periods.findIndex(p => p == self.selectedPeriod), 1);
            self.selectedPeriod = undefined;
        },
        save() {
            var self = this;
            var goal = {
                id: self.id,
                name: self.name,
                periods: []
            };
            
            for (var i in self.periods) {
                var period = self.periods[i];
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

            self.$store.dispatch(constants.SAVE_NUTRITION_GOAL, {
                goal
            }).then(_ => {
                self.notifySuccess(self.$t('saveSuccessful'));
                    self.$router.replace({ name: 'nutrition-goals' });
            }).catch(_ => {
                self.notifyError(self.$t('saveFailed'));
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
            var self = this;
            if (period.wholeDay) {
                return this.$t('wholeDay');
            }
            var text = '';
            var count = 0;
            for (var id in period.mealDefinitions) {
                if (period.mealDefinitions[id]) {
                    var mealdef = self.$mealDefinitions.find(d => d.id == id);
                    text += ', ' + mealdef.name;
                    count++;
                }
            }
            if (text.length == 0 || count == self.$mealDefinitions.length) {
                return this.$t('everyMeal');
            }
            return text.substr(1);
        },
        populate(goal) {
            var self = this;
            self.id = goal.id;
            self.name = goal.name;
            if (goal.periods) {
                goal.periods.forEach(period => {
                    var mealDefs = period.mealDefinitions;
                    period.mealDefinitions = {};
                    mealDefs.forEach(defId => {
                        period.mealDefinitions[defId] = true;
                    });

                    var nutrients = period.nutrients;
                    period.nutrients = {};

                    self.$nutrients.forEach(nutrient => {
                        var value = nutrients.find(n => n.nutrientId == nutrient.id);
                        if (value) {
                            period.nutrients[nutrient.id] = { min: value.min, max: value.max };
                        }
                        else {
                            period.nutrients[nutrient.id] = { min: undefined, max: undefined };
                        }
                    });
                });
      
                self.periods = goal.periods;
            }
            else {
                self.periods = [];
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