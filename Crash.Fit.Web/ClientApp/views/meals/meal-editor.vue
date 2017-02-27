<template>
    <div>
        <div class="row" v-if="!anon">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>Aika</label>
                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="time" v-on:change="time=arguments[0]"></datetime-picker>
                </div>
                
            </div>
            <div class="col-sm-5 col-md-3 col-lg-2">
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
                    <li v-bind:class="{ active: !showNutrients }"><a @click="toggleNutrients(false)">Ruoka-aineet</a></li>
                    <li v-bind:class="{ active: showNutrients }"><a @click="toggleNutrients(true)">Ravinto-aineet</a></li>
                </ul>
                <div v-if="!showNutrients">
                    <div class="row hidden-xs">
                        <div class="col-sm-4"><label>Ruoka</label></div>
                        <div class="col-sm-2"><label>Määrä</label></div>
                        <div class="col-sm-3"><label>Annos</label></div>
                        <div class="col-sm-1"><label>Paino</label></div>
                        <div class="col-sm-1">&nbsp;</div>
                    </div>
                    <template v-for="(row,index) in rows">
                        <div class="meal-row row">
                            <div class="food col-sm-4">
                                <label class="hidden-sm hidden-md hidden-lg">Ruoka</label>
                                <div v-if="copyMode">
                                    <input type="checkbox" v-model="row.copy" />
                                </div><food-picker v-bind:value="row.food" v-on:change="row.food=arguments[0]" />
                            </div>
                            <div class="quantity col-sm-2 col-xs-3">
                                <label class="hidden-sm hidden-md hidden-lg">Määrä</label>
                                <input type="number" class="form-control" v-model="row.quantity" />
                            </div>
                            <div class="portion col-sm-3 col-xs-7">
                                <label class="hidden-sm hidden-md hidden-lg">Annos</label>
                                <div v-if="row.food">
                                    <select class="form-control" v-model="row.portion">
                                        <option v-bind:value="null">g</option>
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
                                    <button class="btn btn-danger" @click="removeRow(index)">Poista</button>
                                </div>
                            </div>
                        </div>
                        <div class="meal-row-separator row hidden-sm hidden-md hidden-lg">
                            <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>
                    <div class="row">
                        <div class="col-sm-12"><div v-if="copyMode"><input type="checkbox" v-model="copyAllRows" /> Kaikki</div><button class="btn" @click="addRow"><i class="fa fa-plus"></i> Lisää</button></div>
                    </div>
                    <!--
                    <table>
                        <thead>
                            <tr>
                                <th><div v-if="copyMode">Kopioi</div></th>
                                <th class="food col-sm-6">Ruoka</th>
                                <th class="quantity">Määrä</th>
                                <th class="portion">Annos</th>
                                <th class="weight">Paino</th>
                                <th class="actions"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(row,index) in rows">
                                <td><div v-if="copyMode"><input type="checkbox" v-model="row.copy" /></div></td>
                                <td class="food"><food-picker v-bind:value="row.food" v-on:change="row.food=arguments[0]" /></td>
                                <td class="quantity"><input type="number" class="form-control" v-model="row.quantity" /></td>
                                <td class="portion">
                                    <select class="form-control" v-if="row.food" v-model="row.portion">
                                        <option v-bind:value="null">g</option>
                                        <option v-for="portion in row.food.portions" v-bind:value="portion">
                                            {{ portion.name }}
                                        </option>
                                    </select>
                                </td>
                                <td class="weight">
                                    <div v-if="row.food">
                                        <span v-if="row.food">{{ weight(row.quantity, row.portion) }}</span>
                                    </div>
                                </td>
                                <td class="actions"><button class="btn btn-link" @click="removeRow(index)">Poista</button></td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td><div v-if="copyMode"><input type="checkbox" v-model="copyAllRows" /> Kaikki</div></td>
                                <td colspan="5"><button class="btn" @click="addRow"><i class="glyphicon glyphicon-plus"></i> Lisää</button></td>
                            </tr>
                        </tfoot>
                    </table>-->
                </div>
                <div v-if="showNutrients">
                    <table>
                        <thead>
                            <tr><th></th><th></th><th></th></tr>
                        </thead>
                        <tbody>
                            <tr><th colspan="2">Makrot</th></tr>
                            <tr v-for="nutrient in allNutrients['MACROCMP']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(mealNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                        <tbody>
                            <tr><th colspan="2">Vitamiinit</th></tr>
                            <tr v-for="nutrient in allNutrients['VITAM']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(mealNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                        <tbody>
                            <tr><th colspan="2">Mineraalit</th></tr>
                            <tr v-for="nutrient in allNutrients['MINERAL']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(mealNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                        <tbody>
                            <tr><th colspan="2">Hiilihydraatit</th></tr>
                            <tr v-for="nutrient in allNutrients['CARBOCMP']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(mealNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                        <tbody>
                            <tr><th colspan="2">Rasvat</th></tr>
                            <tr v-for="nutrient in allNutrients['FAT']">
                                <td>{{ nutrient.name }}</td>
                                <td>{{ decimal(mealNutrients[nutrient.id], nutrient.precision) }}</td>
                                <td>{{ unit(nutrient.unit)}}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <hr />
        <div class="row" v-if="!anon">
            <div class="col-sm-12">
                <button class="btn btn-primary" v-if="!copyMode" @click="save">Tallenna</button>
                <button class="btn" v-if="!copyMode" @click="cancel">Peruuta</button>
                <button class="btn" v-if="id && !copyMode" @click="startCopy">Kopioi</button>
                <button class="btn btn-primary" v-if="copyMode" @click="confirmCopy">Vahvista kopiointi</button>
                <button class="btn" v-if="copyMode" @click="cancelCopy">Peruuta kopiointi</button>
                <button class="btn btn-link" v-if="id && !copyMode">Poista</button>
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
            time: null,
            name: null,
            rows: [],
            copyMode: false,
            copyAllRows: false,
            allNutrients: {},
            showNutrients: false
        }
    },
    computed: {
        mealNutrients: function () {
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
    props: {
        meal: null,
        user: null,
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
        toggleNutrients: function(show){
            this.showNutrients = show;
        },
        addRow : function(){
            this.rows.push({food: null, quantity: 1, portion: null});
        },
        removeRow: function (index) {
            this.rows.splice(index, 1);
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
            var meal = {
                id: this.id,
                time: this.time,
                name: this.name,
                rows: this.rows
            };
            this.saveCallback(meal);
        },
        cancel: function () {
            this.cancelCallback();
        },
        delete: function(){ },
        startCopy: function () {
            this.copyMode = true;
            this.copyAllRows = true;
        },
        confirmCopy: function () { },
        cancelCopy: function () {
            this.copyMode = false;
            this.copyAllRows = true;
        },
        unit: formatters.formatUnit,
        decimal: function (value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        }
    },
    watch: {
        copyAllRows: function (newVal) {
            for (var i in this.rows) {
                this.rows[i].copy = newVal;
            }
        }
    },
    created: function () {
        var self = this;
        this.time = this.meal.time;
        this.name = this.meal.name;
        
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
    },
    mounted: function () {
    }
}
</script>
<style scoped>
    div.meal-row
    {
        margin-bottom:5px;
    }
    div.meal-row-separator
    {
        padding: 0px;
    }
    div.meal-row-separator hr
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