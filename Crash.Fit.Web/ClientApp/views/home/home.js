import constants from '../../store/constants'
import toaster from '../../toaster'
import moment from 'moment'
import utils from '../../utils'
import nutrientsMixin from '../../mixins/nutrients'
import mealDefinitionsMixin from '../../mixins/meal-definitions'
import nutritionGoalMixin from '../../mixins/nutrition-goal'
import DatetimePicker from '../../components/datetime-picker'
import EnergyDistributionBar from '../../components/energy-distribution-bar'
import NutrientBar from '../../components/nutrient-bar'
import MealRowEditor from './meal-row-editor.vue'
import api from '../../api'

export default {
    mixins: [nutrientsMixin, mealDefinitionsMixin, nutritionGoalMixin],
    data() {
        return {
            proteinId: constants.PROTEIN_ID,
            carbId: constants.CARB_ID,
            fatId: constants.FAT_ID,
            energyId: constants.ENERGY_ID,
            energyDistributionId: constants.ENERGY_DISTRIBUTION_ID,
            energyDifferenceId: constants.ENERGY_DIFFERENCE_ID,
            showEditMealRow: false,
            row: undefined,
            selectedNutrients: [],
            editNutrients: false,
            eatenEnergy: undefined
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
            return this.formatDate(this.selectedDate);
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
            meals.filter(m => !m.definitionId || defs.findIndex(d => d.id == m.definitionId) < 0).forEach(m => {
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
            return this.$store.state.training.workouts.filter(w => moment(w.time).isSame(self.selectedDate, 'day'));
        },
        nutritionGoal() {
            return this.$store.state.nutrition.activeNutritionGoal;
        },
        dayNutrients() {
            var day = this.$store.state.nutrition.mealDays.find(d => moment(d.date).isSame(this.selectedDate, 'day'));
            if (day) {
                return day.nutrients;
            }
            return undefined;
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
        },
        rmr() {
            if (this.$profile) {
                return this.$profile.rmr;
            }
            return null;
        },
        energyExpenditure() {
            var self = this;
            var expenditures = this.$store.state.training.energyExpenditures.filter(e => moment(e.time).isSame(self.selectedDate, 'day'));
            var sum = 0;
            expenditures.forEach(e => {
                sum += e.energyKcal;
            });
            return sum;
        },
        totalEnergy() {
            return this.eatenEnergy - this.rmr - this.energyExpenditure;
        },
        energyGoal() {
            return this.nutrientGoal(this.energyDifferenceId);
        },
        totalClass() {
            if (this.totalEnergy && this.energyGoal) {
                if (this.energyGoal.min && this.totalEnergy < this.energyGoal.min) {
                    return 'bg-danger';
                }
                if (this.energyGoal.max && this.totalEnergy > this.energyGoal.max) {
                    return 'bg-danger';
                }
                if (this.energyGoal.min || this.energyGoal.min == 0 || this.energyGoal.max || this.energyGoal.max == 0) {
                    return 'bg-success';
                }
            }
            return '';
        }
    },
    components: {
        DatetimePicker,
        EnergyDistributionBar,
        NutrientBar,
        MealRowEditor
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
                    self.updateComputedValues();
                },
                failure() { }
            });
            self.$store.dispatch(constants.FETCH_WORKOUTS, { start: start, end: end });
            self.$store.dispatch(constants.FETCH_ENERGY_EXPENDITURES, { start: start, end: end });
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
            return this.formatTime(defMeal.meal.time);
        },
        addRow(defMeal) {
            this.row = {
                mealDefinitionId: defMeal.definition ? defMeal.definition.id : undefined,
                mealId: defMeal.meal ? defMeal.meal.id : undefined
            };
            this.showEditMealRow = true;
        },
        editRow(row) {
            this.row = row;
            this.showEditMealRow = true;
        },
        saveRow(row) {
            var self = this;
            row.date = this.selectedDate;
            this.$store.dispatch(constants.SAVE_MEAL_ROW, {
                row,
                success() {
                    self.updateComputedValues();
                },
                failure() { }
            });
            this.row = {};
            this.showEditMealRow = false;
            
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
                success() {
                    self.updateComputedValues();
                },
                failure() { }
            });
        },
        deleteRow(row) {
            var self = this;
            this.$store.dispatch(constants.DELETE_MEAL_ROW, {
                row,
                success() {
                    self.updateComputedValues();
                },
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
        saveRowDraft() {
            var self = this;
            self.$store.dispatch(constants.SAVE_MEAL_ROW_DRAFT, { row: self.row });
        },
        createFood(name) {
            var self = this;
            self.saveRowDraft();
            self.$router.push({ name: 'food-details', params: { id: constants.NEW_ID, name: name, returnTo: { name: self.$route.name, params: { draft: true}} }});
        },
        createRecipe(name) {
            var self = this;
            self.saveRowDraft();
            self.$router.push({ name: 'recipe-details', params: { id: constants.NEW_ID, name: name, returnTo: { name: self.$route.name, params: { draft: true }} }});
        },
        updateComputedValues() {
            this.eatenEnergy = this.dayNutrients ? this.dayNutrients[this.energyId] ? this.dayNutrients[this.energyId] : 0 : 0;
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
        if (self.$route.params.draft && self.$store.state.nutrition.rowDraft) {
            var row = self.$store.state.nutrition.rowDraft;            
            if (self.$route.params.foodId) {
                row.foodId = self.$route.params.foodId;
                row.foodName = undefined;
                row.portionId = undefined;
                row.portionName = undefined;
                self.row = row;
                self.showEditMealRow = true;
            }
        }
    }
}