import constants from '../../store/constants'
import api from '../../api'
import formatters from '../../formatters'
import utils from '../../utils'
import toaster from '../../toaster'
import nutrientsMixin from '../../mixins/nutrients'
import nutrientGroupsMixin from '../../mixins/nutrient-groups'

export default {
    mixins:[nutrientsMixin,nutrientGroupsMixin],
    data () {
        return {
            id: null,
            name: null,
            ingredients: [],
            portions: [],
            cookedWeight: undefined,
            tab: 'ingredients',
            groupOpenStates: {},
        }
    },
    computed: {
        constants() {
            return constants;
        },
        allNutrients() {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        recipeNutrients() {
            var self = this;
            var nutrients = {};
            for (var i in this.ingredients) {
                var row = this.ingredients[i];
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
        },
        recipeWeight() {
            var self = this;
            var weight = 0;
            for (var i in this.ingredients) {
                var row = this.ingredients[i];
                if (!row.food || !row.quantity) {
                    continue;
                }
                var ingredientWeight = self.weight(row.quantity, row.portion);
                weight += ingredientWeight;
            }
            return weight;
        },
        weightChange() {
            return (this.cookedWeight - this.recipeWeight) / this.recipeWeight * 100;
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'food-picker': require('../foods/food-picker')
    },
    methods: {
        addIngredient() {
            this.ingredients.push({ food: null, quantity: null, portion: undefined });
        },
        deleteIngredient(index) {
            this.ingredients.splice(index, 1);
        },
        addPortion(){
            this.portions.push({ name: null, weight: null, amount: null});
        },
        deletePortion(index) {
            this.portions.splice(index, 1);
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
        save() {
            var self = this;
            var recipe = {
                id: self.id,
                name: self.name,
                ingredients: self.ingredients.map(i => { return { foodId: i.food ? i.food.id : undefined, quantity: utils.parseFloat(i.quantity), portionId: i.portion ? i.portion.id : undefined } }),
                portions: self.portions ? self.portions.map(p => { return { id: p.id, name: p.name, amount: utils.parseFloat(p.amount), weight: utils.parseFloat(p.weight) } }) : [],
                cookedWeight: self.cookedWeight
            };
            self.$store.dispatch(constants.SAVE_RECIPE, {
                recipe,
                success(savedRecipe) {
                    if (self.$route.params.returnTo) {
                        self.$route.params.returnTo.params.foodId = savedRecipe.id;
                        self.$router.replace({ name: self.$route.params.returnTo.name, params: self.$route.params.returnTo.params });
                    }
                    else {
                        toaster.info(self.$t('saveSuccessful'));
                        self.$router.replace({ name: 'recipes' });
                    }
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            });
        },
        cancel() {
            var self = this;
            if (self.$route.params.returnTo) {
                self.$router.replace({ name: self.$route.params.returnTo.name, params: self.$route.params.returnTo.params });
            }
            else {
                this.$router.go(-1);
            }
        },
        deleteRecipe() {
            var self = this;
            self.$store.dispatch(constants.DELETE_RECIPE, {
                recipe: { id: self.id },
                success() {
                    self.$router.push({ name: 'recipes' });
                },
                failure() {
                    toaster(self.$t('recipeDetails.deleteFailed'));
                }
            });
        },
        unit: formatters.formatUnit,
        decimal(value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        },
        toggleGroup(group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        groupIsExpanded(group) {
            return this.groupOpenStates[group] && true;
        },
        populate(recipe) {
            var self = this;
            self.id = recipe.id;
            self.name = recipe.name;
            self.cookedWeight = recipe.cookedWeight;
            self.portions = recipe.portions || [];

            var foodIds = recipe.ingredients.map(i => { return i.foodId });
            self.$store.dispatch(constants.FETCH_FOODS, {
                ids: foodIds,
                success(foods) {
                    self.ingredients = recipe.ingredients.map(i => {
                        var food = foods.find(f => f.id == i.foodId);
                        return { food: food, quantity: i.quantity, portion: food ? food.portions.find(p => p.id === i.portionId) : undefined };
                    });
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure() {
                    toaster(self.$t('recipeDetails.fetchFailed'));
                }
            });

        }
    },
    watch: {
        copyAllingredients(newVal) {
            for (var i in this.ingredients) {
                this.ingredients[i].copy = newVal;
            }
        }
    },
    created() {
        var self = this;
        var id = self.$route.params.id;
        if (id == constants.NEW_ID) {
            self.populate({ id: undefined, name: self.$route.params.name, ingredients: [] });
            self.addIngredient();
        }
        else {
            self.$store.dispatch(constants.FETCH_RECIPE, {
                id,
                success(recipe) {
                    self.populate(recipe);
                },
                failure() {
                    toaster(self.$t('recipeDetails.fetchFailed'));
                }
            });
        }

        this.toggleGroup(this.$nutrientGroups[0].id);
    }
}