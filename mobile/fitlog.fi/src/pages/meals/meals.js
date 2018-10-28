import moment from 'moment'
import constants from '../../store/constants'
import utils from '../../utils'
import EnergyDistributionBar from '../../components/energy-distribution-bar'
import NutrientBar from '../../components/nutrient-bar'
import api from '../../api'
import nutrientsMixin from '../../mixins/nutrients'
import mealDefinitionsMixin from '../../mixins/meal-definitions'
import nutritionGoalMixin from '../../mixins/nutrition-goal'
import PageMixin from '../../mixins/page'

export default {
  mixins:[nutrientsMixin, mealDefinitionsMixin, nutritionGoalMixin, PageMixin],
  components: {
    EnergyDistributionBar,
    NutrientBar
  },
  data () {
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
      return this.$store.state.nutrition.meals.filter(m => moment(m.time).isBetween(this.start, this.end));
    },
    workouts() {
      return this.$store.state.training.workouts.filter(w => moment(w.time).isBetween(this.start, this.end));
    },
    days() {
      return this.$store.state.nutrition.mealDays.filter(md => moment(md.date).isBetween(this.start, this.end, null, '[]'));
    },
    visibleColumns() {
      return this.columns.filter(c => !c.group || c.group == this.selectedGroup);
    },
    start() {
      return this.$store.state.nutrition.mealsDisplayStart;
    },
    end() {
      return this.$store.state.nutrition.mealsDisplayEnd;
    }
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
    showMonths(months){
      var end = moment().endOf('day').toDate();
      var start = moment().subtract(months, 'months').add(1,'days').startOf('day').toDate();
      this.showDateRange(start, end);
    },
    showDateRange(start, end) {
      this.$store.dispatch(constants.SELECT_MEAL_DATE_RANGE, {
        start,
        end
      }).then(x => {
        this.fetchMeals();
      });
    },
    fetchMeals() {
      this.$store.commit(constants.LOADING);
      this.$store.dispatch(constants.FETCH_MEALS, {
        start: this.start,
        end: this.end
      }).then(_ => {
        this.$store.commit(constants.LOADING_DONE);
      });
      this.$store.dispatch(constants.FETCH_WORKOUTS, { start: self.start, end: self.end });
    },
    selectGroup(group) {
      this.selectedGroup = group;
    },
    toggleDay(day) {
      this.$set(this.dayStates, day.date.getTime(), !(this.dayStates[day.date.getTime()] && true))
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
    if (this.start && this.end) {
        this.fetchMeals();
    }
    else {
        this.showWeek();
    }
    this.selectGroup(this.groups[0].id);
  }
}
