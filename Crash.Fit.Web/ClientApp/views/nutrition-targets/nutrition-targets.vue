<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutritionTargets.title") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="addNutrientTarget">{{ $t("nutritionTargets.addTarget") }}</button>
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
                                        <template v-for="(target,index) in nutrientTargets">
                                            <th><button class="btn btn-sm btn-danger" @click="deleteNutrientTarget(index)">{{ $t("delete") }}</button></th>
                                        </template>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <th class="freeze">{{ $t("nutritionTargets.onlyDays") }}</th>
                                        <template v-for="target in nutrientTargets">
                                            <td>
                                                <table class="days">
                                                    <tr>
                                                        <td>{{ $t("mondayShort") }}</td>
                                                        <td>{{ $t("tuesdayShort") }}</td>
                                                        <td>{{ $t("wednesdayShort") }}</td>
                                                        <td>{{ $t("thursdayShort") }}</td>
                                                        <td>{{ $t("fridayShort") }}</td>
                                                        <td>{{ $t("saturdayShort") }}</td>
                                                        <td>{{ $t("sundayShort") }}</td>
                                                        <td class="divider"></td>
                                                        <td>{{ $t("exerciseDayShort") }}</td>
                                                        <td>{{ $t("restDayShort") }}</td>
                                                    </tr>
                                                    <tr>
                                                        <td><input type="checkbox" v-model="target.monday" v-bind:title="$t('monday')" /></td>
                                                        <td><input type="checkbox" v-model="target.tuesday" v-bind:title="$t('tuesday')" /></td>
                                                        <td><input type="checkbox" v-model="target.wednesday" v-bind:title="$t('wednesday')" /></td>
                                                        <td><input type="checkbox" v-model="target.thursday" v-bind:title="$t('thursday')" /></td>
                                                        <td><input type="checkbox" v-model="target.friday" v-bind:title="$t('friday')" /></td>
                                                        <td><input type="checkbox" v-model="target.saturday" v-bind:title="$t('saturday')" /></td>
                                                        <td><input type="checkbox" v-model="target.sunday" v-bind:title="$t('sunday')" /></td>
                                                        <td class="divider"></td>
                                                        <td><input type="checkbox" v-model="target.exerciseDay" v-bind:title="$t('exerciseDay')" /></td>
                                                        <td><input type="checkbox" v-model="target.restDay" v-bind:title="$t('restDay')" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </template>
                                    </tr>
                                </tbody>
                                <tbody v-for="group in groups">
                                    <tr>
                                        <th class="freeze clickable" @click="toggleGroup(group.id)">
                                            <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                            <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                            {{ $t('nutrients.groups.'+group.id) }}
                                        </th>
                                        <template v-for="target in nutrientTargets">
                                            <td>&nbsp;</td>
                                        </template>
                                    </tr>
                                    <tr v-for="nutrient in nutrients[group.id]" v-if="groupOpenStates[group.id]">
                                        <td class="freeze"><span class="name">{{ nutrient.name }}</span> <span class="unit">{{ unit(nutrient.unit) }}</span></td>
                                        <template v-for="target in nutrientTargets">
                                            <td><input type="number" class="form-control input-4" v-model="target.nutrientValues[nutrient.id].min" /> - <input type="number" class="form-control input-4" v-model="target.nutrientValues[nutrient.id].max" /></td>
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
                    <button class="btn btn-primary" @click="save">{{ $t('save') }}</button>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <p>
                        {{ $t('nutritionTargets.info') }}
                    </p>
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
    var toaster = require('../../toaster');
module.exports = {
    data () {
        return {
            //nutrients: [],
            nutrientTargets: [],
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
                nutrientValues: {}
            };
            for (var i in this.nutrients) {
                for (var j in this.nutrients[i]) {
                    target.nutrientValues[this.nutrients[i][j].id] = { min: undefined, max: undefined };
                }
            }

            this.nutrientTargets.push(target);
        },
        deleteNutrientTarget: function(index){
            this.nutrientTargets.splice(index, 1);
        },
        toggleGroup: function (group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        groupIsExpanded(group) {
            return this.groupOpenStates[group] && true;
        },
        save: function () {
            var self = this;
            var targets = [];
            for (var i in self.nutrientTargets) {
                var days = self.nutrientTargets[i];
                var target = {
                    monday: days.monday,
                    tuesday: days.tuesday,
                    wednesday: days.wednesday,
                    thursday: days.thursday,
                    friday: days.friday,
                    saturday: days.saturday,
                    sunday: days.sunday,
                    exerciseDay: days.exerciseDay,
                    restDay: days.restDay,
                    nutrientValues: []
                };
                for (var j in days.nutrientValues) {
                    var value = days.nutrientValues[j];
                    if (value.min || value.min == 0 || value.max || value.max == 0) {
                        target.nutrientValues.push({ nutrientId: j, min: value.min, max: value.max });
                    }
                }
                targets.push(target);
            }
            self.$store.dispatch(constants.SAVE_NUTRIENT_TARGETS, {
                targets,
                success: function () {
                    toaster.info(self.$t('nutrientTargets.saved'));
                },
                failure: function () {
                    toaster.error(self.$t('nutrientTargets.saveFailed'));
                }
            });
        },
        unit: formatters.formatUnit
    },
    created: function () {
        var self = this;
        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success: function () {
                self.$store.dispatch(constants.FETCH_NUTRIENT_TARGETS, {
                    success: function (targets) {
                        var nutrientTargets = [];
                        for (var i in targets) {
                            var target = {
                                monday: targets[i].monday,
                                tuesday: targets[i].tuesday,
                                wednesday: targets[i].wednesday,
                                thursday: targets[i].thursday,
                                friday: targets[i].friday,
                                saturday: targets[i].saturday,
                                sunday: targets[i].sunday,
                                exerciseDay: targets[i].exerciseDay,
                                restDay: targets[i].restDay,
                                nutrientValues: {}
                            };
                            for (var g in self.nutrients) {
                                for (var n in self.nutrients[g]) {
                                    target.nutrientValues[self.nutrients[g][n].id] = { min: undefined, max: undefined };
                                }
                            }
                            for (var j in targets[i].nutrientValues) {
                                var value = targets[i].nutrientValues[j];
                                target.nutrientValues[value.nutrientId] = { min: value.min, max: value.max };
                            }
                            nutrientTargets.push(target);
                        }
                        self.nutrientTargets = nutrientTargets;

                        self.$store.commit(constants.LOADING_DONE);
                    },
                    failure: function () { }
                });
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