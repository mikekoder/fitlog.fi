<template>
    <div>
        <section class="content-header"><h1>Tavoitteet</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="addNutrientTarget">Lis&auml;&auml; tavoite</button>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="outer" v-if="nutrientTargets.length > 0">
                        <div class="inner">
                            <table class="nutrient-targets">
                                <thead>
                                    <tr>
                                        <th class="freeze"></th>
                                        <template v-for="target in nutrientTargets">
                                            <th><button class="btn btn-sm btn-danger">Poista</button></th>
                                        </template>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <th class="freeze">Vain p&auml;ivin&auml;</th>
                                        <template v-for="target in nutrientTargets">
                                            <td>
                                                <table class="days">
                                                    <tr><td>M</td><td>T</td><td>K</td><td>T</td><td>P</td><td>L</td><td>S</td><td class="divider"></td><td>T</td><td>L</td></tr>
                                                    <tr>
                                                        <td><input type="checkbox" v-model="target.monday" title="Maanantai" /></td>
                                                        <td><input type="checkbox" v-model="target.tuesday" title="Tiistai" /></td>
                                                        <td><input type="checkbox" v-model="target.wednesday" title="Keskiviikko" /></td>
                                                        <td><input type="checkbox" v-model="target.thursday" title="Torstain" /></td>
                                                        <td><input type="checkbox" v-model="target.friday" title="Perjantai" /></td>
                                                        <td><input type="checkbox" v-model="target.saturday" title="Lauantai" /></td>
                                                        <td><input type="checkbox" v-model="target.sunday" title="Sunnuntai" /></td>
                                                        <td class="divider"></td>
                                                        <td><input type="checkbox" v-model="target.exerciseDay" title="Treeni" /></td>
                                                        <td><input type="checkbox" v-model="target.restDay" title="Lepo" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </template>
                                    </tr>
                                </tbody>
                                <tbody v-for="group in groups">
                                    <tr>
                                        <th class="freeze" @click="toggleGroup(group.id)">
                                            <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                            <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                            {{ group.name }}
                                        </th>
                                        <template v-for="target in nutrientTargets">
                                            <td>&nbsp;</td>
                                        </template>
                                    </tr>
                                    <tr v-for="nutrient in nutrients[group.id]" v-if="groupOpenStates[group.id]">
                                        <td class="freeze"><span class="name">{{ nutrient.name }}</span> <span class="unit">{{ unit(nutrient.unit) }}</span></td>
                                        <template v-for="target in nutrientTargets">
                                            <td><input type="number" class="form-control input-4" v-model="target.targets[nutrient.id].min" /> - <input type="number" class="form-control input-4" v-model="target.targets[nutrient.id].max" /></td>
                                        </template>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <hr />
            <div class="row" v-if="nutrientTargets.length > 0">
                <div class="col-sm-12">
                    <button class="btn btn-primary">Tallenna</button>
                </div>
            </div>

        </section>
        
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var utils = require('../../utils');
    var api = require('../../api');
    var formatters = require('../../formatters')

module.exports = {
    data () {
        return {
            //nutrients: [],
            nutrientTargets: [],
            groupOpenStates: {},
        }
    },
    computed:{
        groups: function(){
            return this.$store.state.nutrition.nutrientGroups;
        },
        nutrients: function () {
            return this.$store.state.nutrition.nutrientsGrouped;
        }
    },
    components: {},
    methods: {
        addNutrientTarget: function () {
            var target = {
                monday: false,
                tuesday: false,
                wednesday: false,
                thursday: false,
                friday: false,
                saturday: false,
                sunday: false,
                exerciseDay: false,
                restDay: false,
                targets: {}
            };
            for (var i in this.nutrients) {
                for (var j in this.nutrients[i]) {
                    target.targets[this.nutrients[i][j].id] = { min: undefined, max: undefined };
                }
            }

            this.nutrientTargets.push(target);
        },
        deleteNutrientTarget: function(){

        },
        unit: formatters.formatUnit,
        toggleGroup: function (group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        groupIsExpanded(group) {
            return this.groupOpenStates[group] && true;
        }
    },
    created: function () {
        var self = this;
        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success: function () { },
            failure: function () { }
        });
        this.$store.dispatch(constants.FETCH_NUTRIENT_TARGETS, {
            success: function (targets) {
                self.nutrientTargets = targets;
            },
            failure: function () { }
        });
        this.toggleGroup(this.groups[0].id);
    }
}
</script>

<style scoped>
     .outer 
    {
      position: relative;
    }
    .inner 
    {
      overflow-x: auto;
      overflow-y: visible;
      margin-left: 170px;
    }
    .freeze 
    {
      position: absolute;
      margin-left: -170px;
      width: 170px;
      white-space: nowrap;
      overflow:hidden;
      text-overflow: ellipsis;
    }

    .freeze .name
    {
        display:block;
        width:140px;   
        white-space: nowrap;
        overflow:hidden;
        text-overflow: ellipsis;
    }
    .freeze .unit
    { 
        display:block;
        position: absolute;
        top: 5px;
        right: 5px;
    }
    td.freeze
    {
        padding-top:5px;
    }
    th.freeze
    {
        padding-top:0px;
    }
    table.nutrient-targets{
        width: auto;
        table-layout: fixed; 
    }
    table.nutrient-targets input 
    {
        padding-left: 6px;
        padding-right: 2px;
    }
    table.nutrient-targets thead > tr > th {
        text-align:center;
    }
    table.nutrient-targets thead > tr > th:nth-child(n+2),
    table.nutrient-targets tbody > tr > th:nth-child(n+2), 
    table.nutrient-targets tbody > tr >  td:nth-child(n+2){
        height: 20px;
        border-right: 1px solid black;
        border-left: 1px solid black;
    }
    table.days td{text-align:center;}
    table.days td.divider{width: 2px; border-left:1px solid black;}
    table.days input[type=checkbox]{ width: 19px; height: 19px;}
    input.form-control { display:initial;}
</style>