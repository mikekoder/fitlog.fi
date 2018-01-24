import constants from '../../store/constants'
import api from '../../api'
import utils from '../../utils'
import toaster from '../../toaster'
import nutrientsMixin from '../../mixins/nutrients'
import mealDefinitionsMixin from '../../mixins/meal-definitions'
import api from '../../api'

export default {
    mixins:[nutrientsMixin, mealDefinitionsMixin],
    data () {
        return {
            id: null,
            date: null,
            time: null,
            selectedTime: null,
            mealDefinitionId: undefined,
            name: null,
            rows: [],
            copyMode: false,
            copyAllRows: false,
            showNutrients: false,
            groupOpenStates: {}
        }
    },
    computed: {
        mealDefinitions() {
            return this.$store.state.nutrition.mealDefinitions;
        },
        mealName() {
            var self = this;
            if (self.mealDefinitionId) {
                var meal = self.mealDefinitions.find(m => m.id == self.mealDefinitionId);
                if (meal) {
                    return meal.name;
                }
            }
            return this.formatTime(self.time);
        },
        constants() {
            return constants;
        },
        groups(){
            return this.$store.state.nutrition.nutrientGroups;
        },
        allNutrients() {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        mealNutrients() {
            var self = this;
            var nutrients = {};
            for (var i in this.rows) {
                var row = this.rows[i];
                if(!row.food || !row.quantity){
                    continue;
                }
                var weight = self.weight(row.quantity, row.portion);
                for (var j in row.food.nutrients) {
                    var foodNutrient = row.food.nutrients[j];
                    if (nutrients[foodNutrient.nutrientId]) {
                        nutrients[foodNutrient.nutrientId] += (weight * foodNutrient.amount / 100);
                    }
                    else {
                        nutrients[foodNutrient.nutrientId] = (weight * foodNutrient.amount / 100);
                    }
                }
            }
            return nutrients;
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'food-picker': require('../foods/food-picker')
    },
    methods: {
        toggleNutrients(show){
            this.showNutrients = show;
        },
        addRow(){
            this.rows.push({food: null, quantity: 1, portion: undefined});
        },
        setRowFood(row, food){
            row.food = food;
        },
        deleteRow(index) {
            this.rows.splice(index, 1);
        },
        weight(quantity, portion) {
            if (!quantity) {
                return '';
            }
            if (typeof (quantity) !== 'number') {
                quantity = parseFloat(quantity.replace(',', '.'));
            }
            
            if (portion) {
                return quantity * portion.weight;
            }
            return quantity;
        },
        createRequestModel() {
            var self = this;
            var time = new Date(self.time);
            var meal = {
                id: self.id,
                date: new Date(self.date),
                time: time.getHours() + ':' + time.getMinutes(),
                definitionId: self.mealDefinitionId,
                name: self.name,
                rows: self.rows.filter(r => r.food && r.quantity).map(r => { return { foodId: r.food.id, quantity: utils.parseFloat(r.quantity), portionId: r.portion ? r.portion.id : undefined } })
            };
            return meal;
        },
        save() {
            var self = this;
            var meal = self.createRequestModel();
            self.$store.dispatch(constants.SAVE_MEAL, {
                meal,
                success() {
                    toaster.info(self.$t('saveSuccessful'));
                    self.$router.replace({ name: 'meals' });
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            });
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteMeal() {
            var self = this;
            this.$store.dispatch(constants.DELETE_MEAL, {
                meal: { id: self.id },
                success() {
                    self.$router.push({ name: 'meals' });
                },
                failure() {
                    toaster.error(self.$t('deleteFailed'));
                }
            });
        },
        startCopy() {
            this.copyMode = true;
            this.copyAllRows = true;
        },
        confirmCopy() {
            this.id = undefined;
            this.time = utils.previousHalfHour();
            this.rows = this.rows.filter(r => r.copy);
            this.copyMode = false;
        },
        cancelCopy() {
            this.copyMode = false;
            this.copyAllRows = true;
        },
        toggleGroup(group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        groupIsExpanded(group) {
            return this.groupOpenStates[group] && true;
        },
        populate(meal){
            var self = this;
            self.id = meal.id;
            self.date = meal.time;
            self.time = meal.time;
            self.selectedTime = meal.time;
            self.mealDefinitionId = meal.definitionId;
            self.name = meal.name;
            if (meal.draft) {
                self.rows = meal.rows;
                self.$store.commit(constants.LOADING_DONE);
            }
            else {
                var foodIds = meal.rows.map(r => { return r.foodId });
                this.$store.dispatch(constants.FETCH_FOODS, {
                    ids: foodIds,
                    success(foods) {
                        self.rows = meal.rows.map(r => {
                            var food = foods.find(f => f.id == r.foodId);
                            return { food: food, quantity: r.quantity, portion: food ? food.portions.find(p => p.id === r.portionId) : undefined };
                        });
                        self.$store.commit(constants.LOADING_DONE);
                    },
                    failure() {
                        toaster.error(self.$t('fetchFailed'));
                    }
                });
            }
        },
        setDate(offset) {
            this.date = moment().add(offset, 'days').toDate();
        },
        setTime(time) {
            this.time = this.selectedTime;
            this.mealDefinitionId = undefined;
        },
        setMealDefinition(definition) {
            this.mealDefinitionId = definition.id;
            this.time = undefined;
        },
        saveDraft() {
            var self = this;
            var meal = {
                draft: true,
                id: self.id,
                date: self.date,
                time: self.time,
                definitionId: self.mealDefinitionId,
                name: self.name,
                rows: self.rows
            };
            self.$store.dispatch(constants.SAVE_MEAL_DRAFT, { meal });
        },
        createFood(name, rowIndex) {
            var self = this;
            self.saveDraft();
            self.$router.push({ name: 'food-details', params: { id: constants.NEW_ID, name: name, returnTo: { name: self.$route.name, params: { draft: true, rowIndex }} }});
        },
        createRecipe(name, rowIndex) {
            var self = this;
            self.saveDraft();
            self.$router.push({ name: 'recipe-details', params: { id: constants.NEW_ID, name: name, returnTo: { name: self.$route.name, params: { draft: true, rowIndex }} }});
        }
    },
    watch: {
        copyAllRows(newVal) {
            for (var i in this.rows) {
                this.rows[i].copy = newVal;
            }
        }
    },
    created() {
        var self = this;

        // returning from "create food"
        if (self.$route.params.draft && self.$store.state.nutrition.mealDraft) {
            var meal = self.$store.state.nutrition.mealDraft;            
            if (self.$route.params.foodId) {
                api.getFood(self.$route.params.foodId).then(food => {
                    if (self.$route.params.rowIndex && meal.rows.length > self.$route.params.rowIndex) {
                        meal.rows[self.$route.params.rowIndex].food = food;
                        meal.rows[self.$route.params.rowIndex].portion = undefined;
                    }
                    else if (!meal.rows[meal.rows.length - 1].food) {
                        meal.rows[meal.rows.length - 1].food = food;
                    }
                    else {
                        meal.rows.push({ food: food, quantity: 1, portion: undefined });
                    }
                });
            }
            self.populate(meal);
        }
        else {
            var id = self.$route.params.id;
            var action = self.$route.params.action;
            if (id == constants.NEW_ID) {
                self.populate({ id: undefined, time: utils.previousHalfHour(), rows: [] });
                self.addRow();
            }
            else {
                if (action == constants.RESTORE_ACTION) {
                    self.$store.dispatch(constants.RESTORE_MEAL, {
                        id,
                        success(meal) {
                            self.$router.replace({ name: 'meals' });
                        },
                        failure() {
                            toaster.error(self.$t('restoreFailed'));
                        }
                    });
                }
                else {
                    self.$store.dispatch(constants.FETCH_MEAL, {
                        id,
                        success(meal) {
                            self.populate(meal);
                        },
                        failure() {
                            toaster.error(self.$t('fetchFailed'));
                        }
                    });
                }
            }
        }
        
        this.toggleGroup(this.groups[0].id);
    },
    mounted() {
    }
}