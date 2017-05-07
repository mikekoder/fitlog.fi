<template>
    <div class="recipe-details">
        <div class="row" v-if="!anon">
            <div class="col-sm-6 col-md-5 col-lg-4">
                <div class="form-group">
                    <label>Nimi</label>
                    <input class="form-control" v-model="name" />
                </div>
                
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-12">
                <ul class="nav nav-tabs">
                    <li v-bind:class="{ active: tab === 'ingredients' }"><a @click="tab = 'ingredients'">Ruoka-aineet</a></li>
                    <li v-bind:class="{ active: tab === 'portions' }"><a @click="tab = 'portions'">Annokset</a></li>
                    <li v-bind:class="{ active: tab === 'nutrients' }"><a @click="tab = 'nutrients'">Ravinto-aineet</a></li>
                </ul>
                <div v-if="tab === 'ingredients'">
                    <div class="row hidden-xs">
                        <div class="col-sm-4"><label>Ruoka <router-link :to="{ name: 'foods', params: { mode: 'uusi' } }" target="_blank">Luo uusi</router-link></label></div>
                        <div class="col-sm-2"><label>Määrä</label></div>
                        <div class="col-sm-3 col-lg-2"><label>Annos</label></div>
                        <div class="col-sm-1"><label>Paino (g)</label></div>
                        <div class="col-sm-1">&nbsp;</div>
                    </div>
                    <template v-for="(row,index) in ingredients">
                        <div class="recipe-row row">
                            <div class="food col-sm-4">
                                <label class="hidden-sm hidden-md hidden-lg">Ruoka <router-link :to="{ name: 'foods', params: { mode: 'uusi' } }" target="_blank">Luo uusi</router-link></label>
                                <food-picker v-bind:value="row.food" v-on:change="row.food=arguments[0]" />
                            </div>
                            <div class="quantity col-sm-2 col-xs-3">
                                <label class="hidden-sm hidden-md hidden-lg">Määrä</label>
                                <input type="number" class="form-control" v-model="row.quantity" />
                            </div>
                            <div class="portion col-xs-7 col-sm-3 col-lg-2 ">
                                <label class="hidden-sm hidden-md hidden-lg">Annos</label>
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
                                    <button class="btn btn-danger btn-sm" @click="removeRow(index)">Poista</button>
                                </div>
                            </div>
                        </div>
                        <div class="recipe-row-separator row hidden-sm hidden-md hidden-lg">
                            <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>
                    <div class="row">
                        <div class="col-sm-12"><button class="btn" @click="addIngredient"><i class="fa fa-plus"></i> Lisää</button></div>
                    </div>
                   
                </div>
                <div v-if="tab === 'portions'">
                    <div class="row hidden-xs">
                        <div class="col-sm-4 col-lg-2"><label>Nimi</label></div>
                        <div class="col-sm-2 col-lg-1"><label>Paino (g)</label></div>
                        <div class="col-sm-3 col-lg-1"><label>Annoksia</label></div>
                        <div class="col-sm-1">&nbsp;</div>
                    </div>
                    <template v-for="(portion,index) in portions">
                        <div class="recipe-row row">
                            <div class="col-sm-4 col-lg-2">
                                <label class="hidden-sm hidden-md hidden-lg">Nimi</label>
                                <input type="text" class="form-control" v-model="portion.name" />
                            </div>
                            <div class="quantity col-sm-2 col-xs-3 col-lg-1">
                                <label class="hidden-sm hidden-md hidden-lg">Paino (g)</label>
                                <input type="number" class="form-control" v-model="portion.weight" @blur="portionWeightChanged(portion)" />
                            </div>
                            <div class="portion col-sm-3 col-xs-7 col-lg-1">
                                <label class="hidden-sm hidden-md hidden-lg">Annoksia/resepti</label>
                                <input type="number" class="form-control" v-model="portion.number" @blur="portionNumberChanged(portion)" />
                            </div>
                            <div class="actions col-sm-1 col-xs-12">
                                <div>
                                    <button class="btn btn-danger btn-sm" @click="removePortion(index)">Poista</button>
                                </div>
                            </div>
                        </div>
                        <div class="recipe-row-separator row hidden-sm hidden-md hidden-lg">
                            <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>
                    <div class="row">
                        <div class="col-sm-12"><button class="btn" @click="addPortion"><i class="fa fa-plus"></i> Lisää</button></div>
                    </div>
                </div>
                <div v-if="tab === 'nutrients'">
                    <table>
                        <thead>
                            <tr>
                                <th></th>
                                <th>Resepti</th>
                                <th>100g</th>
                                <template v-for="portion in portions">
                                    <th>{{ portion.name }}</th>
                                </template>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr><th colspan="2">Makrot</th></tr>
                            <tr v-for="nutrient in allNutrients['MACROCMP']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(recipeNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ decimal(recipeNutrients[nutrient.id] * 100 / recipeWeight, nutrient.precision) }}</td>
                                <template v-for="portion in portions">
                                    <td><span v-if="portion.weight">{{ decimal(recipeNutrients[nutrient.id] / recipeWeight * portion.weight, nutrient.precision) }}</span></td>
                                </template>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                        <tbody>
                            <tr><th colspan="2">Vitamiinit</th></tr>
                            <tr v-for="nutrient in allNutrients['VITAM']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(recipeNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ decimal(recipeNutrients[nutrient.id] * 100 / recipeWeight, nutrient.precision) }}</td>
                                <template v-for="portion in portions">
                                    <td><span v-if="portion.weight">{{ decimal(recipeNutrients[nutrient.id] / recipeWeight * portion.weight, nutrient.precision) }}</span></td>
                                </template>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                        <tbody>
                            <tr><th colspan="2">Mineraalit</th></tr>
                            <tr v-for="nutrient in allNutrients['MINERAL']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(recipeNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ decimal(recipeNutrients[nutrient.id] * 100 / recipeWeight, nutrient.precision) }}</td>
                                <template v-for="portion in portions">
                                    <td><span v-if="portion.weight">{{ decimal(recipeNutrients[nutrient.id] / recipeWeight * portion.weight, nutrient.precision) }}</span></td>
                                </template>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                        <tbody>
                            <tr><th colspan="2">Hiilihydraatit</th></tr>
                            <tr v-for="nutrient in allNutrients['CARBOCMP']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(recipeNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ decimal(recipeNutrients[nutrient.id] * 100 / recipeWeight, nutrient.precision) }}</td>
                                <template v-for="portion in portions">
                                    <td><span v-if="portion.weight">{{ decimal(recipeNutrients[nutrient.id] / recipeWeight * portion.weight, nutrient.precision) }}</span></td>
                                </template>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                        <tbody>
                            <tr><th colspan="2">Rasvat</th></tr>
                            <tr v-for="nutrient in allNutrients['FAT']">
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
        <div class="row main-actions" v-if="!anon">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="save">Tallenna</button>
                <button class="btn" @click="cancel">Peruuta</button>
                <button class="btn btn-danger" @click="deleteMeal" v-if="id">Poista</button>
            </div>
        </div>
        <hr />
        <div class="row">
            <table>
                <tbody>

                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var formatters = require('../../formatters');

module.exports = {
    data () {
        return {
            id: null,
            name: null,
            ingredients: [],
            portions: [],
            allNutrients: {},
            tab: 'ingredients'
        }
    },
    computed: {
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
        }
    },
    props: {
        recipe: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null,
        anon: false
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
            var recipe = {
                id: this.id,
                name: this.name,
                ingredients: this.ingredients,
                portions: this.portions
            };
            this.saveCallback(recipe);
        },
        cancel: function () {
            this.cancelCallback();
        },
        deleteMeal: function () {
            this.deleteCallback(this.recipe);
        },
        unit: formatters.formatUnit,
        decimal: function (value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        },
        portionWeightChanged: function (portion) {
            if (portion.weight) {
                if (typeof (portion.weight) !== 'number') {
                    portion.weight = parseFloat(portion.weight.replace(',', '.'));
                }
                portion.number = this.decimal(this.recipeWeight / portion.weight,1);
            }
            
        },
        portionNumberChanged: function (portion) {
            if (portion.number) {
                if (typeof (portion.number) !== 'number') {
                    portion.number = parseFloat(portion.number.replace(',', '.'));
                }
                portion.weight = this.decimal(this.recipeWeight / portion.number, 0);
            }
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
        this.id = this.recipe.id;
        this.name = this.recipe.name;
        this.portions = this.recipe.portions || [];
        if (this.recipe.ingredients) {
            var apiCalls = [];
            for (var i in this.recipe.ingredients) {
                apiCalls.push(api.getFood(this.recipe.ingredients[i].foodId));
            }
            Promise.all(apiCalls).then(function (foods) {
                self.ingredients = self.recipe.ingredients.map(i => {
                    var food = foods.find(f => f.id == i.foodId);
                    return { food: food, quantity: i.quantity, portion: food.portions.find(p => p.id === i.portionId) };
                });
            });
        }
        else {
            this.ingredients = [];
        }
        api.listNutrients().then(function (allNutrients) {
            var nutrients = {};
            for (var i in allNutrients) {
                var nutrient = allNutrients[i];
                if (nutrients[nutrient.fineliGroup]) {
                    nutrients[nutrient.fineliGroup].push(nutrient);
                }
                else {
                    nutrients[nutrient.fineliGroup] = [nutrient];
                }
                
            }
            self.allNutrients = nutrients;
        });
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