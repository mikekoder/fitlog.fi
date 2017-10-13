<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t('recipeDetails') }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-6 col-md-5 col-lg-4">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input class="form-control" v-model="name" />
                    </div>

                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <ul class="nav nav-tabs">
                        <li class="clickable" v-bind:class="{ active: tab === 'ingredients' }"><a @click="tab = 'ingredients'">{{ $t("foods") }}</a></li>
                        <li class="clickable" v-bind:class="{ active: tab === 'portions' }"><a @click="tab = 'portions'">{{ $t("portions") }}</a></li>
                        <li class="clickable" v-bind:class="{ active: tab === 'nutrients' }"><a @click="tab = 'nutrients'">{{ $t("nutrients") }}</a></li>
                    </ul>
                    <div v-if="tab === 'ingredients'">
                        <div class="row hidden-xs">
                            <div class="col-sm-4 col-text-40">
                              <label>{{ $t("food") }}</label>
                              <router-link :to="{ name: 'food-details', params: { id: constants.NEW_ID } }" target="_blank">{{ $t("createFood") }}</router-link>
                            </div>
                            <div class="col-sm-2 col-number-5"><label>{{ $t("amount") }}</label></div>
                            <div class="col-sm-3 col-text-20"><label>{{ $t("portion") }}</label></div>
                            <div class="col-sm-1 col-number-5"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1 col-actions-3">&nbsp;</div>
                        </div>
                        <template v-for="(row,index) in ingredients">
                            <div class="row">
                                <div class="col-sm-4 col-text-40">
                                    <label class="hidden-sm hidden-md hidden-lg">Ruoka</label>
                                    <router-link class="hidden-sm hidden-md hidden-lg" :to="{ name: 'food-details', params: { id: constants.NEW_ID } }" target="_blank">{{ $t("createFood") }}</router-link>
                                    <food-picker v-bind:value="row.food" v-on:change="row.food=arguments[0]" />
                                </div>
                                <div class="col-xs-3 col-number-5">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("amount") }}</label>
                                    <input type="number" class="form-control" v-model="row.quantity" />
                                </div>
                                <div class="col-xs-7 col-text-20">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("portion") }}</label>
                                    <div v-if="row.food">
                                        <select class="form-control" v-model="row.portion">
                                            <option v-bind:value="undefined">g</option>
                                            <option v-for="portion in row.food.portions" v-bind:value="portion">
                                                {{ portion.name }}
                                            </option>
                                        </select>
                                    </div>
                                    <div v-if="!row.food"><select class="form-control" disabled></select></div>
                                </div>
                                <div class="col-xs-2 col-number-5">
                                    <label class="hidden-sm hidden-md hidden-lg">Paino</label>
                                    <div v-if="row.food">{{ weight(row.quantity, row.portion) }}</div>
                                    <div v-if="!row.food">&nbsp;</div>
                                </div>
                                <div class="actions col-sm-1 col-xs-12">
                                    <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                    <button class="btn btn-danger btn-sm" @click="deleteRow(index)">{{ $t("delete") }}</button>
                                </div>
                            </div>
                            <div class="separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                        </template>
                        <div class="row">
                            <div class="col-sm-6 col-text-40">
                                <button class="btn" @click="addIngredient">{{ $t("add") }}</button>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xs-6 col-text-20">
                                {{ $t("rawWeight") }}
                            </div>
                            <div class="col-xs-3 col-number-5">
                                {{ decimal(recipeWeight) }}
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xs-6 col-text-20">
                                {{ $t("cookedWeight") }}
                            </div>
                            <div class="col-xs-3 col-number-5">
                                <input type="number" class="form-control" v-model="cookedWeight" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xs-6 col-text-20">
                                {{ $t("weightChange") }}
                            </div>
                            <div class="col-xs-3 col-number-5">
                                <span v-if="cookedWeight">{{ decimal(weightChange, 1) }} %</span>
                            </div>
                        </div>


                    </div>
                    <div v-if="tab === 'portions'">
                        <div class="row hidden-xs">
                            <div class="col-sm-4 col-text-20"><label>{{ $t("name") }}</label></div>
                            <div class="col-sm-4 col-text-20"><label>{{ $t("portions") }}/{{ $t("recipe") }}</label></div>
                            <div class="col-sm-2 col-number-5"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1 col-actions-3">&nbsp;</div>
                        </div>
                        <template v-for="(portion,index) in portions">
                            <div class="recipe-row row">
                                <div class="col-sm-4 col-text-20">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("name") }}</label>
                                    <input type="text" class="form-control" v-model="portion.name" />
                                </div>
                                <div class="col-xs-4 col-text-20">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("portions") }}/{{ $t("recipe") }}</label>
                                    <input type="number" class="form-control" v-model="portion.amount" />
                                </div>
                                <div class="col-xs-3 col-number-5">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("weight") }} (g)</label>
                                    <span v-if="portion.amount">{{ decimal((cookedWeight || recipeWeight)/portion.amount) }} </span>
                                </div>

                                <div class="col-xs-12 col-actions-3">
                                    <label>&nbsp;</label>
                                    <button class="btn btn-danger btn-sm" @click="deletePortion(index)">{{ $t("delete") }}</button>
                                </div>
                            </div>
                            <div class="recipe-row-separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                        </template>
                        <div class="row table-actions">
                            <div class="col-sm-12"><button class="btn" @click="addPortion">{{ $t("add") }}</button></div>
                        </div>
                    </div>
                    <div v-if="tab === 'nutrients'">
                        <table>
                            <thead>
                                <tr>
                                    <th></th>
                                    <th>{{ $t("recipe") }}</th>
                                    <th>100g</th>
                                    <template v-for="portion in portions">
                                        <th>{{ portion.name }}</th>
                                    </template>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody v-for="group in groups">
                                <tr>
                                    <th class="clickable" colspan="2" @click="toggleGroup(group.id)">
                                        <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                        <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                        {{ group.name }}
                                    </th>
                                </tr>
                                <tr v-for="nutrient in allNutrients[group.id]" v-if="groupOpenStates[group.id]">
                                    <td>{{ nutrient.name }}</td>
                                    <td>{{ decimal(recipeNutrients[nutrient.id], nutrient.precision) }}</td>
                                    <td>{{ decimal(recipeNutrients[nutrient.id] * 100 / recipeWeight, nutrient.precision) }}</td>
                                    <template v-for="portion in portions">
                                        <td><span v-if="portion.amount">{{ decimal(recipeNutrients[nutrient.id] / recipeWeight * ((cookedWeight || recipeWeight)/portion.amount), nutrient.precision) }}</span></td>
                                    </template>
                                    <td>{{ unit(nutrient.unit)}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <hr />
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger" @click="deleteRecipe" v-if="id">{{ $t("delete") }}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import api from '../../api'
    import formatters from '../../formatters'
    import utils from '../../utils'
    import toaster from '../../toaster'
export default {
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
        groups(){
            return this.$store.state.nutrition.nutrientGroups;
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
        addPortion(){
            this.portions.push({ name: null, weight: null, number: null});
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
                success() {
                    self.$router.replace({ name: 'recipes' });
                },
                failure() {
                    toaster.error(self.$t('recipeDetails.saveFailed'));
                }
            });
        },
        cancel() {
            this.$router.go(-1);
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
            self.populate({ id: undefined, name: undefined, ingredients: [] });
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

        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success() { },
            failure() { }
        });
        this.toggleGroup(this.groups[0].id);
    }
}
</script>
<style scoped>
</style>