<template>
    <div>
        <div class="row" v-if="!anon">
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
                    <li v-bind:class="{ active: tab === 'nutrients' }"><a @click="tab = 'nutrients'">Ravinto-aineet</a></li>
                    <li v-bind:class="{ active: tab === 'portions' }"><a @click="tab = 'portions'">Annokset</a></li>
                </ul>
                <div v-if="tab === 'portions'">
                    <div class="row hidden-xs">
                        <div class="col-sm-4"><label>Nimi</label></div>
                        <div class="col-sm-2"><label>Paino (g)</label></div>
                        <div class="col-sm-1">&nbsp;</div>
                    </div>
                    <template v-for="(portion,index) in portions">
                        <div class="recipe-row row">
                            <div class="col-sm-4">
                                <label class="hidden-sm hidden-md hidden-lg">Nimi</label>
                                <input type="text" class="form-control" v-model="portion.name" />
                            </div>
                            <div class="quantity col-sm-2 col-xs-3">
                                <label class="hidden-sm hidden-md hidden-lg">Paino (g)</label>
                                <input type="number" class="form-control" v-model="portion.weight" />
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
                                <th>Määrä/100g</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody v-for="group in groups">
                            <tr>
                                <th colspan="2" @click="toggleGroup(group.id)">
                                    <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                    <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                    {{ group.name }}
                                </th>
                            </tr>
                            <tr v-for="nutrient in nutrients[group.id]" v-if="groupOpenStates[group.id]">
                                <td>{{ nutrient.name }}</td>
                                <td><input v-model="nutrient.amount" /></td>
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
                <button class="btn btn-danger" v-if="id" @click="deleteFood">Poista</button>
            </div>
        </div>
        <hr />
        <div class="row">
            <table>
                <tbody></tbody>
            </table>
        </div>
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var api = require('../../api');
    var formatters = require('../../formatters');
    var utils = require('../../utils');

module.exports = {
    data () {
        return {
            id: null,
            name: null,
            portions: [],
            //nutrients: {},
            tab: 'nutrients',
            groupOpenStates: {},
        }
    },
    computed: {
        groups: function () {
            return this.$store.state.nutrition.nutrientGroups;
        },
        nutrients: function () {
            return this.$store.state.nutrition.nutrientsGrouped;
        }
    },
    props: {
        food: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null,
        anon: false
    },
    components: {
    },
    methods: {
        addPortion : function(){
            this.portions.push({ name: null, weight: null});
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
            var food = {
                id: this.id,
                name: this.name,
                nutrients: [],
                portions: this.portions.map(p => { return { id: p.id, name: p.name, weight: utils.parseFloat(p.weight) }})
            };
            for (var i in this.nutrients) {
                for (var j in this.nutrients[i]) {
                    if (this.nutrients[i][j].amount) {
                        food.nutrients.push({ nutrientId: this.nutrients[i][j].id, amount: utils.parseFloat(this.nutrients[i][j].amount) });
                    }
                }
            }

            this.saveCallback(food);
        },
        cancel: function () {
            this.cancelCallback();
        },
        deleteFood: function () {
            this.deleteCallback(this.food);
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
        }
    },
    created: function () {
        var self = this;
        this.id = this.food.id;
        this.name = this.food.name;
        this.portions = this.food.portions || [];
        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success: function () { },
            failure: function () { }
        });
        this.toggleGroup(this.groups[0].id);
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