<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("mealsDetails.title") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
                    <div class="form-group">
                        <label>{{ $t("time") }}</label>
                        <datetime-picker class="vue-picker1" name="picker1" v-bind:value="time" v-on:change="time=arguments[0]"></datetime-picker>
                    </div>
                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <ul class="nav nav-tabs">
                        <li class="clickable" v-bind:class="{ active: !showNutrients }"><a @click="toggleNutrients(false)">{{ $t("meals.tabs.foods") }}</a></li>
                        <li class="clickable" v-bind:class="{ active: showNutrients }"><a @click="toggleNutrients(true)">{{ $t("meals.tabs.nutrients") }}</a></li>
                    </ul>
                    <div v-if="!showNutrients">
                        <div class="row hidden-xs">
                            <div class="col-sm-4"><label>{{ $t("food") }} <router-link :to="{ name: 'foods', params: { id: 'uusi' } }" target="_blank" v-if="!copyMode">{{ $t("createNew") }}</router-link></label></div>
                            <div class="col-sm-2"><label>{{ $t("amount") }}</label></div>
                            <div class="col-sm-3 col-lg-2"><label>{{ $t("portion") }}</label></div>
                            <div class="col-sm-1"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1">&nbsp;</div>
                        </div>
                        <template v-for="(row,index) in rows">
                            <div class="meal-row row">
                                <div class="food col-sm-4">
                                    <label class="hidden-sm hidden-md hidden-lg">Ruoka <router-link :to="{ name: 'foods', params: { id: 'uusi' } }" target="_blank" v-if="!copyMode">{{ $t("createNew") }}/router-link></label>
                                    <div v-if="copyMode">
                                        <input type="checkbox" v-model="row.copy" />
                                        <span>{{ row.food ? row.food.name : '' }}</span>
                                    </div>
                                    <food-picker v-else v-bind:value="row.food" v-on:change="setRowFood(row, arguments[0])" />
                                </div>
                                <div class="quantity col-xs-3 col-sm-2">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("amount") }}</label>
                                    <span v-if="copyMode">{{ row.quantity }}</span>
                                    <input v-else type="number" class="form-control" v-model="row.quantity" />
                                </div>
                                <div class="portion col-xs-7 col-sm-3 col-lg-2">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("portion") }}</label>
                                    <div v-if="row.food">
                                        <span v-if="copyMode">{{ row.portion ? row.portion.name : '' }}</span>
                                        <select v-else class="form-control" v-model="row.portion">
                                            <option v-bind:value="undefined">g</option>
                                            <option v-for="portion in row.food.portions" v-bind:value="portion">
                                                {{ portion.name }}
                                            </option>
                                        </select>
                                    </div>
                                    <!--
                                <div v-if="!row.food"><select class="form-control" disabled></select></div>
                                    -->
                                </div>
                                <div class="weight col-sm-1 col-xs-2">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("weight") }}</label>
                                    <div v-if="row.food">{{ weight(row.quantity, row.portion) }}</div>
                                    <div v-if="!row.food">&nbsp;</div>
                                </div>
                                <div class="actions col-sm-1 col-xs-12">
                                    <div>
                                        <button v-if="!copyMode" class="btn btn-danger btn-sm" @click="removeRow(index)">{{ $t("delete") }}</button>
                                    </div>
                                </div>
                            </div>
                            <div class="meal-row-separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                        </template>
                        <div class="row">
                            <div class="col-sm-12">
                                <div v-if="copyMode"><input type="checkbox" v-model="copyAllRows" /> <strong>{{ $t("all") }}</strong></div>
                                <button v-else class="btn" @click="addRow">{{ $t("add") }}</button>
                            </div>
                        </div>

                    </div>
                    <div v-if="showNutrients">
                        <table>
                            <thead>
                                <tr><th></th><th></th><th></th></tr>
                            </thead>
                            <tbody v-for="group in groups">
                                <tr>
                                    <th colspan="2" class="clickable" @click="toggleGroup(group.id)">
                                        <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                        <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                        {{ $t('nutrients.groups.'+group.id) }}
                                    </th>
                                </tr>
                                <tr v-for="nutrient in allNutrients[group.id]" v-if="groupOpenStates[group.id]">
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
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" v-if="!copyMode" @click="save">{{ $t("save") }}</button>
                    <button class="btn" v-if="!copyMode" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn" v-if="id && !copyMode" @click="startCopy">{{ $t("copy") }}</button>
                    <button class="btn btn-primary" v-if="copyMode" @click="confirmCopy">{{ $t("copyConfirm") }}</button>
                    <button class="btn" v-if="copyMode" @click="cancelCopy">{{ $t("copyCancel") }}</button>
                    <button class="btn btn-danger" v-if="id && !copyMode" @click="deleteMeal">{{ $t("delete") }}</button>
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
            time: null,
            name: null,
            rows: [],
            copyMode: false,
            copyAllRows: false,
            showNutrients: false,
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
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'food-picker': require('../foods/food-picker')
    },
    methods: {
        toggleNutrients: function(show){
            this.showNutrients = show;
        },
        addRow : function(){
            this.rows.push({food: null, quantity: 1, portion: undefined});
        },
        setRowFood: function(row, food){
            row.food = food;
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
            var self = this;
            var meal = {
                id: self.id,
                time: new Date(self.time),
                name: self.name,
                rows: self.rows.filter(r => r.food && r.quantity).map(r => { return { foodId: r.food.id, quantity: utils.parseFloat(r.quantity), portionId: r.portion ? r.portion.id : undefined } })
            };
            self.$store.dispatch(constants.SAVE_MEAL, {
                meal,
                success: function () {
                    self.$router.replace({ name: 'meals' });
                },
                failure: function () {
                    toaster(self.$t('mealDetails.saveFailed'));
                }
            });
        },
        cancel: function () {
            this.$router.go(-1);
        },
        deleteMeal: function () {
            var self = this;
            this.$store.dispatch(constants.DELETE_MEAL, {
                meal: { id: self.id },
                success: function () {
                    self.$router.push({ name: 'meals' });
                },
                failure: function () {
                    toaster.error(self.$t('mealDetails.deleteFailed'));
                }
            });
        },
        startCopy: function () {
            this.copyMode = true;
            this.copyAllRows = true;
        },
        confirmCopy: function (){
            var meal = {
                rows: this.rows.filter(r => r.copy).map(r => { return { foodId: r.food ? r.food.id : undefined, quantity: utils.parseFloat(r.quantity), portionId: r.portion ? r.portion.id : undefined } })
            };
            this.copyCallback(meal);
        },
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
        },
        toggleGroup: function (group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        groupIsExpanded(group) {
            return this.groupOpenStates[group] && true;
        },
        populate(meal){
            var self = this;
            self.id = meal.id;
            self.time = meal.time;
            self.name = meal.name;
            var foodIds = meal.rows.map(r => { return r.foodId });
            this.$store.dispatch(constants.FETCH_FOODS, {
                ids: foodIds,
                success: function (foods) {
                    self.rows = meal.rows.map(r => {
                        var food = foods.find(f => f.id == r.foodId);
                        return { food: food, quantity: r.quantity, portion: food ? food.portions.find(p => p.id === r.portionId) : undefined };
                    });
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure: function () {
                    toaster(self.$t('mealDetails.fetchFailed'));
                }
            });
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
        var id = self.$route.params.id;
        var action = self.$route.params.action;
        if (id == constants.NEW_ID) {
            self.populate({ id: undefined, time: utils.previousHalfHour(), rows: [{ food: undefined, quantity: undefined, portion: undefined }] });
        }
        else {
            if (action == constants.RESTORE_ACTION) {
                self.$store.dispatch(constants.RESTORE_MEAL, {
                    id,
                    success: function (meal) {
                        self.$router.replace({ name: 'meals' });
                    },
                    failure: function () {
                        toaster(self.$t('mealDetails.restoreFailed'));
                    }
                });
            }
            self.$store.dispatch(constants.FETCH_MEAL, {
                id,
                success: function (meal) {
                    self.populate(meal);
                },
                failure: function () {
                    toaster(self.$t('mealDetails.fetchFailed'));
                }
            });
        }
        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success: function () { },
            failure: function () { }
        });

        this.toggleGroup(this.groups[0].id);
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