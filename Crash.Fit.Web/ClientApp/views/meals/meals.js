import api from '../../api'
import moment from 'moment'
import toaster from '../../toaster'
import utils from '../../utils'
import constants from '../../store/constants'
import nutrientsMixin from '../../mixins/nutrients'
import mealDefinitionsMixin from '../../mixins/meal-definitions'
import nutritionGoalMixin from '../../mixins/nutrition-goal'
import DatetimePicker from '../../components/datetime-picker'
import EnergyDistributionBar from '../../components/energy-distribution-bar'
import NutrientBar from '../../components/nutrient-bar'

export default {
    mixins: [nutrientsMixin, mealDefinitionsMixin, nutritionGoalMixin],
    data() {
        return {
            selectedGroup: '',
            dayStates: {},
            selectedMeal: null,
            proteinId: constants.PROTEIN_ID,
            carbId: constants.CARB_ID,
            fatId: constants.FAT_ID,
            energyDistributionId: constants.ENERGY_DISTRIBUTION_ID,
            editNutrients: false
        }
    },
    computed: {
        groups() {
            return this.$store.state.nutrition.nutrientGroups;
        },
        columns() {
            var columns = [];
            //columns.push(this.energyDistributionColumn);
            for (var i in this.$nutrients) {
                var nutrient = this.$store.state.nutrition.nutrients[i];
                if (nutrient.hideSummary) {
                    continue;
                }
                columns.push({ title: nutrient.name, unit: nutrient.unit, precision: nutrient.precision, key: nutrient.id, hideSummary: nutrient.hideSummary, hideDetails: nutrient.hideDetails, group: nutrient.fineliGroup });
            }
            return columns;
        },
        meals() {
            var self = this;
            return this.$store.state.nutrition.meals.filter(m => moment(m.time).isBetween(self.start, self.end));
        },
        workouts() {
            var self = this;
            return this.$store.state.training.workouts.filter(w => moment(w.time).isBetween(self.start, self.end));
        },
        days() {
            var self = this;
            return this.$store.state.nutrition.mealDays.filter(md => moment(md.date).isBetween(self.start, self.end, null, '[]'));
        },
        visibleColumns() {
            var self = this;
            return this.columns.filter(c => !c.group || c.group == self.selectedGroup);
        },
        start() {
            var self = this;
            return this.$store.state.nutrition.mealsDisplayStart;
        },
        end() {
            var self = this;
            return this.$store.state.nutrition.mealsDisplayEnd;
        }
    },
    components: {
        DatetimePicker,
        EnergyDistributionBar,
        NutrientBar
    },
    methods: {
        changeStart(date) {
            this.showDateRange(date, this.end);
        },
        changeEnd(date) {
            this.showDateRange(this.start, date);
        },
        showDay() {
            var end = moment().endOf('day').toDate();
            var start = moment().startOf('day').toDate();
            this.showDateRange(start, end);
        },
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
            self.$store.dispatch(constants.SELECT_MEAL_DATE_RANGE, {
                start: start,
                end: end,
                success() {
                    self.fetchMeals();
                }
            });
        },
        editSettings() {
            this.editNutrients = true;
        },
        saveSettings() {
            this.editNutrients = false;
        },
        fetchMeals() {
            var self = this;
            self.$store.dispatch(constants.FETCH_MEALS, {
                start: self.start,
                end: self.end,
                success() {
                    self.$store.commit(constants.LOADING_DONE);
                }
            });
            self.$store.dispatch(constants.FETCH_WORKOUTS, { start: self.start, end: self.end });
        },
        selectGroup(group) {
            this.selectedGroup = group;
        },
        toggleDay(day) {
            this.$set(this.dayStates, day.date.getTime(), !(this.dayStates[day.date.getTime()] && true))
        },
        createMeal() {
            this.$router.push({ name: 'meal-details', params: { id: constants.NEW_ID } });
        },
        deleteMeal(meal) {
            var self = this;
            this.$store.dispatch(constants.DELETE_MEAL, {
                meal,
                success() {
                    var restoreUrl = self.$router.resolve({ name: 'meal-details', params: { id: meal.id, action: constants.RESTORE_ACTION } });
                    toaster.info(self.$t('mealDeleted') + ' <a href="' + restoreUrl.href + '">' + self.$t('restore') + '</a>');
                },
                failure() {
                    toaster.error(self.$t('deleteFailed'));
                }
            });
        },
        dayIsExpanded(day) {
            return this.dayStates[day.date.getTime()] && true;
        },
        nutrientGoal(nutrientId, day, meal) {
            return utils.nutrientGoal(this.$nutritionGoal, this.workouts, nutrientId, day, meal);
        },
        mealName(meal) {
            if (meal.definitionId) {
                var def = this.$store.state.nutrition.mealDefinitions.find(d => d.id == meal.definitionId);
                if (def) {
                    return def.name;
                }
            }
            return this.formatTime(meal.time);
        }
    },
    created() {
        var self = this;
        if (self.start && self.end) {
            self.fetchMeals();
        }
        else {
            self.showWeek();
        }
        self.selectGroup(self.groups[0].id);
    }
}