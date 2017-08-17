<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("foodDetails") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-6 col-md-6 col-lg-4">
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
                        <li class="clickable" v-bind:class="{ active: tab === 'nutrients' }"><a @click="tab = 'nutrients'">{{ $t("nutrients") }}</a></li>
                        <li class="clickable" v-bind:class="{ active: tab === 'portions' }"><a @click="tab = 'portions'">{{ $t("portions") }}</a></li>
                    </ul>
                    <div v-if="tab === 'portions'">
                        <div class="row hidden-xs">
                            <div class="col-sm-4"><label>{{ $t("name") }}</label></div>
                            <div class="col-sm-2"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1">&nbsp;</div>
                        </div>
                        <template v-for="(portion,index) in portions">
                            <div class="row">
                                <div class="col-sm-4">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("name") }}</label>
                                    <input type="text" class="form-control" v-model="portion.name" />
                                </div>
                                <div class="quantity col-sm-2 col-xs-3">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("weight") }} (g)</label>
                                    <input type="number" class="form-control" v-model="portion.weight" />
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
                                    <th>{{ $t("amount") }}/
                                        <select v-model="nutrientPortion" @change="changeNutrientPortion">
                                            <option v-for="portion in nutrientPortions" v-bind:value="portion">{{ portion.name }}</option>
                                        </select>
                                    </th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody v-for="group in nutrientGroups">
                                <tr>
                                    <th class="clickable" colspan="2" @click="toggleGroup(group.id)">
                                        <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                        <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                        {{ $t(group.id) }}
                                    </th>
                                </tr>
                                <tr v-for="nutrient in nutrientsGrouped[group.id]" v-if="groupOpenStates[group.id]">
                                    <template v-if="!nutrient.computed">
                                        <td>{{ nutrient.name }}</td>
                                        <td><input v-model="nutrients[nutrient.id]" /></td>
                                        <td>{{ unit(nutrient.unit)}}</td>
                                    </template>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <hr />
            <div class="row errors" v-if="!isValid">
                <div class="col-sm-12">
                    <div class="alert alert-danger"><span v-for="error in errors">{{ error }}<br /></span></div>
                </div>
            </div>
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save" :disabled="!isValid">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger" v-if="id" @click="deleteFood">{{ $t("delete") }}</button>
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
    var defaultNutrientPortion = { id: undefined, name: '100g'};
    
module.exports = {
    data () {
        return {
            id: null,
            name: null,
            nutrients: {},
            portions: [],
            tab: 'nutrients',
            groupOpenStates: {},
            nutrientPortion: defaultNutrientPortion
        }
    },
    computed: {
        loading: function () {
            return this.$store.state.loading;
        },
        nutrientGroups: function () {
            return this.$store.state.nutrition.nutrientGroups;
        },
        nutrientsGrouped: function () {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        nutrientPortions: function(){
            var portions = [];
            portions.push(defaultNutrientPortion);
            if(this.portions.length === 0){
                portions.push({ name: this.$t('portion'), nutrientPortion: true });
            }
            else {
                this.portions.forEach(p => { portions.push(p); });
            }
            return portions;
        },
        errors: function(){
            var errors = [];
            if(!this.name){
                errors.push('Nimi puuttuu');
            }
            this.portions.forEach(p => {
                if(!p.name){
                    errors.push('Annoksen nimi puuttuu');
                }
                else if(!p.weight || p.weight == ''){
                    errors.push('Annoksen "'+p.name+'" paino puuttuu');
                }
            });
            return errors;
        },
        isValid: function(){
            return this.errors.length === 0;
        }
    },
    components: {
    },
    methods: {
        changeNutrientPortion: function(){
            if(this.nutrientPortion.nutrientPortion && this.portions.length === 0){
                this.portions.push(this.nutrientPortion);
            }
        },
        addPortion : function(){
            this.portions.push({ name: null, weight: null});
        },
        removePortion: function (index) {
            if(this.nutrientPortion == this.portions[index]){
                this.nutrientPortion = defaultNutrientPortion;
            }
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
            var food = {
                id: self.id,
                name: self.name,
                nutrients: [],
                portions: self.portions ? self.portions.map(p => { return { id: p.id, name: p.name, weight: utils.parseFloat(p.weight), nutrientPortion: p === self.nutrientPortion }}) : []
            };
            for (var i in self.nutrients) {
                if (self.nutrients[i] || self.nutrients[i] == 0) {
                    food.nutrients.push({ nutrientId: i, amount: utils.parseFloat(self.nutrients[i]) });
                }
            }

            self.$store.dispatch(constants.SAVE_FOOD, {
                food,
                success: function () {
                    self.$router.replace({ name: 'foods' });
                },
                failure: function () {
                    toaster.error(self.$t('foodDetails.saveFailed'));
                }
            });
        },
        cancel: function () {
            this.$router.go(-1);
        },
        deleteFood: function () {
            var self = this;
            self.$store.dispatch(constants.DELETE_FOOD, {
                food: { id: self.id },
                success: function () {
                    self.$router.push({ name: 'foods' });
                },
                failure: function () {
                    toaster.error(self.$t('foodDetails.deleteFailed'));
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
        populate(food) {
            var self = this;
            self.id = food.id;
            self.name = food.name;
            self.portions = food.portions || [];
            if(food.nutrientPortionId){
                self.nutrientPortion = self.portions.find(p => p.id === food.nutrientPortionId);
            }
            self.$store.dispatch(constants.FETCH_NUTRIENTS, {
                success: function () {
                    for (var i in self.nutrientsGrouped) {
                        var group = self.nutrientsGrouped[i];
                        for (var j in group) {
                            var nutrient = group[j];
                            var value = food.nutrients ? food.nutrients.find(n => n.nutrientId == nutrient.id) : undefined;
                            if (value) {
                                if(self.nutrientPortion){
                                    self.nutrients[nutrient.id] = value.portionAmount;                                   
                                }
                                else {
                                    self.nutrients[nutrient.id] = value.amount;
                                }
                            }
                            else {
                                self.nutrients[nutrient.id] = undefined;
                            }
                        }
                    }
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure: function () {
                    toaster.error(self.$t('foodDetails.fetchFailed'));
                }
            });
        }
    },
    created: function () {
        var self = this;
        var id = self.$route.params.id;
        if (id == constants.NEW_ID) {
            self.populate({ id: undefined, name: undefined, nutrients: []});
        }
        else {
            self.$store.dispatch(constants.FETCH_FOOD, {
                id,
                success: function (food) {
                    self.populate(food);
                },
                failure: function () {
                    toaster(self.$t('foodDetails.fetchFailed'));
                }
            });
        }

        self.toggleGroup(self.nutrientGroups[0].id);
    }
}
</script>
<style scoped>
    div.recipe-row {
        margin-bottom: 5px;
    }

    div.recipe-row-separator {
        padding: 0px;
    }

        div.recipe-row-separator hr {
            border: 1px solid #00c0ef;
        }

    div.food, div.quantity, div.portion, div.weight, div.actions {
        padding-right: 2px;
    }

    div.weight {
        padding-top: 5px;
    }

    @media (max-width: 767px) {
        div.food, div.quantity, div.portion, div.weight, div.actions {
            padding-right: 15px;
        }

        div.actions {
            text-align: right;
        }

            div.actions button {
                margin-top: 10px;
                margin-right: 0px;
            }
    }
</style>