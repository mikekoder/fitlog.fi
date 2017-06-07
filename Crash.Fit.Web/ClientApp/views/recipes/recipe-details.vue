<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t('recipeDetails.title') }}</h1></section>
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
                            <div class="col-sm-4"><label>{{ $t("food") }} <router-link :to="{ name: 'foods', params: { mode: 'uusi' } }" target="_blank">{{ $t("recipes.newFood") }}</router-link></label></div>
                            <div class="col-sm-2"><label>{{ $t("amount") }}</label></div>
                            <div class="col-sm-3 col-lg-2"><label>{{ $t("portion") }}</label></div>
                            <div class="col-sm-1"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1">&nbsp;</div>
                        </div>
                        <template v-for="(row,index) in ingredients">
                            <div class="recipe-row row">
                                <div class="food col-sm-4">
                                    <label class="hidden-sm hidden-md hidden-lg">Ruoka <router-link :to="{ name: 'foods', params: { mode: 'uusi' } }" target="_blank">{{ $t("recipes.newFood") }}</router-link></label>
                                    <food-picker v-bind:value="row.food" v-on:change="row.food=arguments[0]" />
                                </div>
                                <div class="quantity col-sm-2 col-xs-3">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("amount") }}</label>
                                    <input type="number" class="form-control" v-model="row.quantity" />
                                </div>
                                <div class="portion col-xs-7 col-sm-3 col-lg-2 ">
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
                                <div class="weight col-sm-1 col-xs-2">
                                    <label class="hidden-sm hidden-md hidden-lg">Paino</label>
                                    <div v-if="row.food">{{ weight(row.quantity, row.portion) }}</div>
                                    <div v-if="!row.food">&nbsp;</div>
                                </div>
                                <div class="actions col-sm-1 col-xs-12">
                                    <div>
                                        <button class="btn btn-danger btn-sm" @click="removeRow(index)">{{ $t("delete") }}</button>
                                    </div>
                                </div>
                            </div>
                            <div class="recipe-row-separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                        </template>
                        <div class="row">
                            <div class="col-sm-6">
                                <button class="btn" @click="addIngredient">{{ $t("add") }}</button>
                            </div>
                            <div class="col-sm-3 col-xs-3">
                                {{ $t("recipes.rawWeight") }}
                            </div>
                            <div class="col-sm-2 col-xs-2">
                                {{ decimal(recipeWeight) }}
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-offset-6 col-sm-3 col-xs-3">
                                {{ $t("recipes.cookedWeight") }}
                            </div>
                            <div class="col-sm-2 col-xs-2">
                                <input type="number" v-model="cookedWeight" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-offset-6 col-sm-3 col-xs-3">
                                {{ $t("recipes.weightChange") }}
                            </div>
                            <div class="col-sm-2 col-xs-2">
                                <span v-if="cookedWeight">{{ decimal(weightChange, 1) }} %</span>
                            </div>
                        </div>


                    </div>
                    <div v-if="tab === 'portions'">
                        <div class="row hidden-xs">
                            <div class="col-sm-4 col-lg-2"><label>{{ $t("name") }}</label></div>
                            <div class="col-sm-3 col-lg-1"><label>{{ $t("recipes.portions") }}</label></div>
                            <div class="col-sm-2 col-lg-1"><label>{{ $t("recipes.weight") }} (g)</label></div>
                            <div class="col-sm-1">&nbsp;</div>
                        </div>
                        <template v-for="(portion,index) in portions">
                            <div class="recipe-row row">
                                <div class="col-sm-4 col-lg-2">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("name") }}</label>
                                    <input type="text" class="form-control" v-model="portion.name" />
                                </div>
                                <div class="portion col-sm-3 col-xs-7 col-lg-1">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("portions") }}/{{ $t("recipe") }}</label>
                                    <input type="number" class="form-control" v-model="portion.amount" />
                                </div>
                                <div class="quantity col-sm-2 col-xs-3 col-lg-1">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("weight") }} (g)</label>
                                    <span v-if="portion.amount">{{ decimal((cookedWeight || recipeWeight)/portion.amount) }} </span>
                                </div>

                                <div class="actions col-sm-1 col-xs-12">
                                    <div>
                                        <button class="btn btn-danger btn-sm" @click="removePortion(index)">{{ $t("delete") }}</button>
                                    </div>
                                </div>
                            </div>
                            <div class="recipe-row-separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                        </template>
                        <div class="row">
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
                                        <td><span v-if="portion.weight">{{ decimal(recipeNutrients[nutrient.id] / recipeWeight * portion.weight, nutrient.precision) }}</span></td>
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
    var constants = require('../../store/constants')
    var api = require('../../api');
    var formatters = require('../../formatters');
    var utils = require('../../utils');
    var toaster = require('../../toaster');
module.exports = {
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
        loading: function () {
            return this.$store.state.loading;
        },
        groups: function(){
            return this.$store.state.nutrition.nutrientGroups;
        },
        allNutrients: function () {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        recipeNutrients: function () {
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
        addIngredient: function () {
            this.ingredients.push({ food: null, quantity: null, portion: undefined });
        },
        addPortion : function(){
            this.portions.push({ name: null, weight: null, number: null});
        },
        removePortion: function (index) {
            this.portions.splice(index, 1);
        },
        weight: function (quantity, portion) {
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
        save: function () {
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
                success: function () {
                    self.$router.replace({ name: 'recipes' });
                },
                failure: function () {
                    toaster.error(self.$t('recipeDetails.saveFailed'));
                }
            });
        },
        cancel: function () {
            this.$router.go(-1);
        },
        deleteRecipe: function () {
            var self = this;
            self.$store.dispatch(constants.DELETE_RECIPE, {
                recipe: { id: self.id },
                success: function () {
                    self.$router.push({ name: 'recipes' });
                },
                failure: function () {
                    toaster(self.$t('recipeDetails.deleteFailed'));
                }
            });
        },
        unit: formatters.formatUnit,
        decimal: function (value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        },
        toggleGroup: function (group) {
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
                success: function (foods) {
                    self.ingredients = recipe.ingredients.map(i => {
                        var food = foods.find(f => f.id == i.foodId);
                        return { food: food, quantity: i.quantity, portion: food ? food.portions.find(p => p.id === i.portionId) : undefined };
                    });
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure: function () {
                    toaster(self.$t('recipeDetails.fetchFailed'));
                }
            });

        }
    },
    watch: {
        copyAllingredients: function (newVal) {
            for (var i in this.ingredients) {
                this.ingredients[i].copy = newVal;
            }
        }
    },
    created: function () {
        var self = this;
        var id = self.$route.params.id;
        if (id == constants.NEW_ID) {
            self.populate({ id: undefined, name: undefined, ingredients: [{ food: undefined, quantity: undefined, portion: undefined }] });
        }
        else {
            self.$store.dispatch(constants.FETCH_RECIPE, {
                id,
                success: function (recipe) {
                    self.populate(recipe);
                },
                failure: function () {
                    toaster(self.$t('recipeDetails.fetchFailed'));
                }
            });
        }

        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success: function () { },
            failure: function () { }
        });
        this.toggleGroup(this.groups[0].id);
    }
}
</script>
<style scoped>
    .recipe-details 
    {
        max-width:1200px;
    }
    div.recipe-row
    {
        margin-bottom:5px;
    }
    div.recipe-row-separator
    {
        padding: 0px;
    }
    div.recipe-row-separator hr
    {
        border: 1px solid #00c0ef;
    }
    div.food, div.quantity, div.portion, div.weight, div.actions
    {
        padding-right:2px;
    }
    div.weight 
    {
        padding-top:5px;
    }
    @media (max-width: 767px) {
        div.food, div.quantity, div.portion, div.weight, div.actions
        {
            padding-right:15px;
        }
        div.actions
        {
            text-align:right;
        }
        div.actions button
        {
            margin-top:10px;
            margin-right:0px;
        }
    }
</style>