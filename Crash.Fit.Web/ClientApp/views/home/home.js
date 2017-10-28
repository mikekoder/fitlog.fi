import constants from '../../store/constants'
import formatters from '../../formatters'
import toaster from '../../toaster'
import moment from 'moment'
import utils from '../../utils'
import nutrientsMixin from '../../mixins/nutrients'
import mealDefinitionsMixin from '../../mixins/meal-definitions'
import nutritionGoalMixin from '../../mixins/nutrition-goal'

export default {
    mixins: [nutrientsMixin, mealDefinitionsMixin, nutritionGoalMixin],
    data() {
        return {
            proteinId: constants.PROTEIN_ID,
            carbId: constants.CARB_ID,
            fatId: constants.FAT_ID,
            energyId: constants.ENERGY_ID,
            energyDistributionId: constants.ENERGY_DISTRIBUTION_ID,
            showAddFood: false,
            row: undefined,
            selectedNutrients: [],
            editNutrients: false,

        }
    },
    computed: {
        selectedDate() {
            return this.$store.state.nutrition.diaryDate;
        },
        dateText() {
            if (moment().isSame(this.selectedDate, 'd')) {
                return this.$t('today');
            }
            else if (moment().subtract(1, 'day').isSame(this.selectedDate, 'd')) {
                return this.$t('yesterday');
            }
            return this.date(this.selectedDate);
        },
        visibleNutrients() {
            return this.$nutrients.filter(n => n.homeOrder || n.homeOrder === 0).sort((n1, n2) => n1.homeOrder - n2.homeOrder);
        },
        mealDay() {
            var date = moment(this.selectedDate).startOf('day');     
            return this.$store.state.nutrition.mealDays.find(d => moment(d.date).isSame(date, 'day'));
        },
        meals() {
            var self = this;
            var start = moment(self.selectedDate).startOf('day');
            var end = moment(self.selectedDate).endOf('day');
            var defs = self.$mealDefinitions;
            var meals = self.$store.state.nutrition.meals.filter(m => moment(m.time).isBetween(start, end));
            var result = defs.map(d => { return { definition: d, meal: meals.find(m => m.definitionId == d.id) } });
            meals.filter(m => !m.definitionId).forEach(m => {
                var index = result.findIndex(r => r.definition && r.definition.startHour && r.definition.startHour > m.time.getHours());
                if (index == -1) {
                    result.push({ meal: m });
                }
                else
                    result.splice(index, 0, { meal: m });
            }
            );
            return result;
        },
        workouts() {
            var self = this;
            return this.$store.state.training.workouts.filter(w => moment(w.time).isBetween(self.start, self.end));
        },
        nutritionGoal() {
            return this.$store.state.nutrition.activeNutritionGoal;
        },
        dayNutrients() {
            var day = this.$store.state.nutrition.mealDays.find(d => moment(d.date).isSame(this.selectedDate, 'day'));
            return day.nutrients;
        },
        nutrientGroups() {
            return this.$store.state.nutrition.nutrientGroups.map(g => {
                return {
                    name: g.name,
                    nutrients: this.$nutrients.filter(n => n.fineliGroup == g.id).sort((n1, n2) => n1.name < n2.name ? -1 : 1)
                }
            });
        },
        mealCopy() {
            if (this.$store.state.clipboard.type == constants.MEAL) {
                return this.$store.state.clipboard.data;
            }
            return undefined;
        },
        rowCopy() {
            if (this.$store.state.clipboard.type == constants.MEAL_ROWS) {
                return this.$store.state.clipboard.data;
            }
            return undefined;
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'add-food': require('./add-food.vue'),
        'chart-pie-energy': require('../../components/energy-distribution-bar'),
        'nutrient-bar': require('../../components/nutrient-bar')
    },
    methods: {
        fetchData() {
            var self = this;
            var start = moment(self.selectedDate).startOf('day');
            var end = moment(self.selectedDate).endOf('day');
            self.$store.dispatch(constants.FETCH_MEALS, {
                start,
                end,
                success() {

                },
                failure() { }
            });
            self.$store.dispatch(constants.FETCH_WORKOUTS, { start: start, end: end });
        },
        changeDate(date) {
            var newDate;
            if (date == 'today') {
                newDate = new Date();
            }
            else if (date == 'yesterday') {
                newDate = moment().subtract(1, 'days').toDate();
            }
            else if (date === -1) {
                newDate = moment(this.selectedDate).subtract(1, 'days').toDate();
            }
            else if (date === 1) {
                newDate = moment(this.selectedDate).add(1, 'days').toDate();
            }
            else {
                newDate = date;
            }
            if (!moment(newDate).isSame(this.selectedDate, 'd')) {
                this.$store.dispatch(constants.SELECT_MEAL_DIARY_DATE, { date: newDate });
                this.fetchData();

            }
        },
        mealName(defMeal) {
            if (defMeal.definition) {
                return defMeal.definition.name;
            }
            return this.time(defMeal.meal.time);
        },
        addRow(defMeal) {
            this.row = {
                mealDefinitionId: defMeal.definition ? defMeal.definition.id : undefined,
                mealId: defMeal.meal ? defMeal.meal.id : undefined
            };
            this.showAddFood = true;
        },
        editRow(row) {
            this.row = row;
            this.showAddFood = true;
        },
        saveRow(row) {
            row.date = this.selectedDate;
            this.$store.dispatch(constants.SAVE_MEAL_ROW, {
                row,
                success() { },
                failure() { }
            });
            this.row = {};
            this.showAddFood = false;
        },
        copyMeal(meal) {
            this.$store.dispatch(constants.CLIPBOARD_COPY, {
                type: constants.MEAL,
                data: meal
            });
        },
        pasteMeal(mealDef) {
            var self = this;
            var meal = self.$store.state.clipboard.data;
            self.appendRows(mealDef, meal.rows);
        },
        copyRow(row) {
            this.$store.dispatch(constants.CLIPBOARD_COPY, {
                type: constants.MEAL_ROWS,
                data: [row]
            });
        },
        pasteRows(mealDef) {
            var self = this;
            var rows = self.$store.state.clipboard.data;
            self.appendRows(mealDef, rows);
        },
        appendRows(mealDef, rows) {
            var self = this;
            var meal = {
                id: mealDef.meal ? mealDef.meal.id : undefined,
                date: self.selectedDate,
                definitionId: mealDef.definition.id,
                rows : mealDef.meal && mealDef.meal.rows ? mealDef.meal.rows.map(r => { return { foodId: r.foodId, quantity: r.quantity, portionId: r.portionId }}) : []
            }
            rows.forEach(r => {
                meal.rows.push({ foodId: r.foodId, quantity: utils.parseFloat(r.quantity), portionId: r.portionId });
            });
            self.$store.dispatch(constants.SAVE_MEAL, {
                meal,
                success() { },
                failure() { }
            });
        },
        deleteRow(row) {
            this.$store.dispatch(constants.DELETE_MEAL_ROW, {
                row,
                success() { },
                failure() { }
            });
        },
        editSettings() {
            var selectedNutrients = [];
            this.visibleNutrients.forEach(n => { selectedNutrients.push(n.id); });
            for (var i = selectedNutrients.length; i < 6; i++) {
                selectedNutrients.push(undefined);
            }
            this.selectedNutrients = selectedNutrients;
            this.editNutrients = true;
        },
        saveSettings() {
            var self = this;
            var settings = {
                nutrients: self.selectedNutrients
            };

            this.$store.dispatch(constants.SAVE_MEAL_DIARY_SETTINGS, {
                settings,
                success() { },
                failure() { }
            });
            this.editNutrients = false;
        },
        nutrientGoal(nutrientId, meal) {
            return utils.nutrientGoal(this.$nutritionGoal, this.workouts, nutrientId, this.selectedDate, meal);
        },
        date: formatters.formatDate,
        time: formatters.formatTime,
        unit: formatters.formatUnit,
        decimal(value, precision) {
            if (!value) {
                return value;
            }

            return value.toFixed(precision);
        }
    },
    created() {
        var self = this;
        self.$store.dispatch(constants.FETCH_LATEST_FOODS, {
            success() { },
            failure() { }
        });
        self.$store.dispatch(constants.FETCH_MOST_USED_FOODS, {
            success() { },
            failure() { }
        });
        self.fetchData();
        this.$store.commit(constants.LOADING_DONE);
    }
}