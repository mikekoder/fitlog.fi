<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("meals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <div class="btn-group">
                        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                            {{ date(start) }} - {{ date(end) }} <span class="caret"></span>
                        </button>
                        <ul class="dropdown-menu">
                            <li class="clickable"><a @click="showDay">{{ $t("today") }}</a></li>
                            <li class="clickable"><a @click="showWeek">{{ $t("currentWeek") }}</a></li>
                            <li class="clickable"><a @click="showMonth">{{ $t("currentMonth") }}</a></li>
                            <li role="separator" class="divider"></li>
                            <li class="clickable"><a @click="showDays(7)">7 {{ $t("days") }}</a></li>
                            <li class="clickable"><a @click="showDays(14)">14 {{ $t("days") }}</a></li>
                            <li class="clickable"><a @click="showDays(30)">30 {{ $t("days") }}</a></li>
                            <li role="separator" class="divider"></li>
                            <li class="custom-date"><span>{{ $t("timeInterval") }}</span></li>
                            <li class="custom-date">
                                <datetime-picker class="vue-picker1" name="picker1" v-bind:value="start" v-bind:format="'DD.MM.YYYY'" v-on:change="start=arguments[0]"></datetime-picker>
                                <datetime-picker class="vue-picker1" name="picker1" v-bind:value="end" v-bind:format="'DD.MM.YYYY'" v-on:change="end=arguments[0]"></datetime-picker>
                                <button class="btn btn-sm" @click="fetchMeals()">{{ $t("OK") }}</button>
                            </li>
                        </ul>
                    </div>
                    <button class="btn btn-primary" @click="createMeal()">{{ $t("log") }}</button> 
                    <div class="btn-group" role="group" aria-label="...">
                        <template v-for="group in groups">
                            <button class="btn btn-default" v-bind:class="{ active: selectedGroup === group.id }" @click="selectGroup(group.id)">{{ $t(group.id) }}</button>
                        </template>
                        <button class="btn pull-right" @click="editSettings" v-if="!editNutrients"><i class="fa fa-gear"></i></button>
                        <button class="btn pull-right" @click="saveSettings" v-if="editNutrients">{{ $t('save') }}</button>
                    </div>

                    <div class="outer" v-if="days.length > 0">
                        <div class="inner">
                            <table class="table" id="meal-list">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <template v-for="col in visibleColumns">
                                            <th class="nutrient" v-if="!col.hideSummary"><div><input type="checkbox" v-if="editNutrients" /><div>{{ col.title }}</div></div></th>
                                        </template>
                                        <th></th>
                                    </tr>
                                    <tr>
                                        <th class="time freeze">{{ $t("time") }}</th>
                                        <template v-for="col in visibleColumns">
                                            <th class="unit" v-if="!col.hideSummary">{{ unit(col.unit) }}</th>
                                        </template>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <template v-for="day in days">
                                        <tr class="day" @click="toggleDay(day)">
                                            <td class="freeze clickable">
                                                <i v-if="!dayStates[day.date.getTime()]" class="fa fa-chevron-down"></i>
                                                <i v-if="dayStates[day.date.getTime()]" class="fa fa-chevron-up"></i>
                                                {{ date(day.date) }}</td>
                                            <template v-for="col in visibleColumns">
                                                <td class="nutrient" v-if="!col.hideSummary">
                                                    <div class="chart" v-if="col.key === energyDistributionId">
                                                        <chart-pie-energy v-bind:protein="day.nutrients[proteinId]" v-bind:carb="day.nutrients[carbId]" v-bind:fat="day.nutrients[fatId]"></chart-pie-energy>
                                                    </div>
                                                    <div v-else>
                                                        <nutrient-bar v-bind:target="nutrientValues(col.key, day.date)" v-bind:value="day.nutrients[col.key]" v-bind:precision="col.precision"></nutrient-bar>
                                                    </div>
                                                </td>
                                            </template>
                                            <td></td>
                                        </tr>
                                        <tr class="meal" v-if="dayStates[day.date.getTime()]" v-for="meal in day.meals">
                                            <td class="freeze"><router-link :to="{ name: 'meal-details', params: { id: meal.id } }">{{ mealName(meal) }}</router-link></td>
                                            <template v-for="col in visibleColumns">
                                                <td class="nutrient" v-if="!col.hideSummary">
                                                    <div class="chart" v-if="col.key === energyDistributionId">
                                                        <chart-pie-energy v-bind:protein="meal.nutrients[proteinId]" v-bind:carb="meal.nutrients[carbId]" v-bind:fat="meal.nutrients[fatId]"></chart-pie-energy>
                                                    </div>
                                                    <span v-else>{{ decimal(meal.nutrients[col.key], col.precision) }}</span>
                                                </td>
                                            </template>
                                            <td><button class="btn btn-danger btn-xs" @click="deleteMeal(meal)">{{ $t("delete") }}</button></td>
                                        </tr>
                                    </template>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div v-if="days.length == 0">
                        <br />
                        {{ $t("noMeals") }}
                    </div>
                       
                    <div v-for="meal in meals">
                        <div class="alert alert-info" role="alert" v-if="meal.deleted">Ateria {{ datetime(meal.time) }} poistettu. <button class="btn btn-link" @click="restoreMeal(meal)">{{ $t("restore") }}</button></div>
                    </div>
                        
                </div>
            </div>
        </section>
    </div>
</template>

<script>

    import api from '../../api'
    import formatters from '../../formatters'
    import moment from 'moment'
    import toaster from '../../toaster'
    import utils from '../../utils'
    import constants from '../../store/constants'

export default {
    data () {
        return {
            selectedGroup: '',
            dayStates: {},
            selectedMeal: null,
            proteinId: constants.PROTEIN_ID,
            carbId: constants.CARB_ID,
            fatId: constants.FAT_ID,
            energyDistributionId: constants.ENERGY_DISTRIBUTION_ID,
            editNutrients: false
        }
    },
    computed: {
        groups() {
            return this.$store.state.nutrition.nutrientGroups;
        },
        columns: function(){
            var columns = [];
            //columns.push(this.energyDistributionColumn);
            for(var i in this.$store.state.nutrition.nutrients){
                var nutrient = this.$store.state.nutrition.nutrients[i];
                if (nutrient.hideSummary) {
                    continue;
                }
                columns.push({ title: nutrient.name, unit: nutrient.unit, precision: nutrient.precision, key: nutrient.id, hideSummary: nutrient.hideSummary, hideDetails: nutrient.hideDetails, group: nutrient.fineliGroup });
            }
            return columns;
        },
        meals: function(){
            var self = this;
            return this.$store.state.nutrition.meals.filter(m => moment(m.time).isBetween(self.start, self.end));
        },
        workouts: function(){
            var self = this;
            return this.$store.state.training.workouts.filter(w => moment(w.time).isBetween(self.start, self.end));
        },
        days: function(){
            var self = this;
            return this.$store.state.nutrition.mealDays.filter(md => moment(md.date).isBetween(self.start, self.end, null, '[]'));
        },
        visibleColumns() {
            var self = this;
            return this.columns.filter(function (c) {
                return !c.group || c.group == self.selectedGroup;
            });
        },
        nutrientTargets() {
            return this.$store.state.nutrition.nutrientTargets;
        },
        start: function(){
            var self = this;
            return this.$store.state.nutrition.mealsDisplayStart;
        },
        end: function(){
            var self = this;
            return this.$store.state.nutrition.mealsDisplayEnd;
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'chart-pie-energy': require('../../components/energy-distribution-bar'),
        'nutrient-bar': require('./nutrient-bar')
    },
    methods: {
        showDay() {
            var end = moment().endOf('day').toDate();
            var start = moment().startOf('day').toDate();
            this.showDateRange(start, end);
        },
        showWeek(){
            var end = moment().endOf('day').toDate();
            var start = moment().startOf('isoWeek').toDate();
            this.showDateRange(start, end);
        },
        showMonth(){
            var end = moment().endOf('day').toDate();
            var start = moment().startOf('month').toDate();
            this.showDateRange(start, end);
        },
        showDays(days) {
            var end = moment().endOf('day').toDate();
            var start = moment().subtract(days - 1, 'days').startOf('day').toDate();
            this.showDateRange(start, end);
        },
        showDateRange: function(start, end){
          var self = this;
            self.$store.dispatch(constants.SELECT_MEAL_DATE_RANGE, {
                start: start,
                end: end,
                success() {
                    self.fetchMeals();
                }
            });
        },
        editSettings() {
            this.editNutrients = true;
        },
        saveSettings() {
            this.editNutrients = false;
        },
        fetchMeals() {
            var self = this;
            self.$store.dispatch(constants.FETCH_MEALS, {
                start: self.start,
                end: self.end,
                success() {
                    self.$store.commit(constants.LOADING_DONE);
                }
            });
            self.$store.dispatch(constants.FETCH_WORKOUTS, { start: self.start, end: self.end });
        },
        selectGroup: function(group){
            this.selectedGroup = group;
        },
        toggleDay(day) {
            this.$set(this.dayStates,day.date.getTime(), !(this.dayStates[day.date.getTime()] && true))
        },
        createMeal: function(){
            this.$router.push({ name: 'meal-details', params: { id: constants.NEW_ID } });
        },
        deleteMeal(meal) {
            var self = this;
            this.$store.dispatch(constants.DELETE_MEAL, {
                meal,
                success() {
                    var restoreUrl = self.$router.resolve({ name: 'meal-details', params: { id: meal.id, action: constants.RESTORE_ACTION } });
                    toaster.info(self.$t('mealDeleted') + ' <a href="' + restoreUrl.href + '">' + self.$t('restore') + '</a>');
                },
                failure() {
                    toaster.error(self.$t('deleteFailed'));
                }
            });
        },
        dayIsExpanded(day){
            return this.dayStates[day.date.getTime()] && true;
        },
        nutrientValues(nutrientId, day) {
            var nutrientTarget;

            // exercise/rest day
            var start = moment(day).startOf('day)');
            var end = moment(day).endOf('day');
            var workouts = this.workouts.filter(w => moment(w.time).isBetween(start, end));
            if (workouts.length > 0) {
                nutrientTarget = this.nutrientTargets.find(t => t.exerciseDay && t.nutrientValues.find(v => v.nutrientId === nutrientId));
            }
            else {
                nutrientTarget = this.nutrientTargets.find(t => t.restDay && t.nutrientValues.find(v => v.nutrientId === nutrientId));
            }

            // weekday
            if (!nutrientTarget) {
                switch (day.getDay()) {
                    case 0:
                        nutrientTarget = this.nutrientTargets.find(t => t.sunday && t.nutrientValues.find(v => v.nutrientId === nutrientId));
                        break;
                    case 1:
                        nutrientTarget = this.nutrientTargets.find(t => t.monday && t.nutrientValues.find(v => v.nutrientId === nutrientId));
                        break;
                    case 2:
                        nutrientTarget = this.nutrientTargets.find(t => t.tuesday && t.nutrientValues.find(v => v.nutrientId === nutrientId));
                        break;
                    case 3:
                        nutrientTarget = this.nutrientTargets.find(t => t.wednesday && t.nutrientValues.find(v => v.nutrientId === nutrientId));
                        break;
                    case 4:
                        nutrientTarget = this.nutrientTargets.find(t => t.thursday && t.nutrientValues.find(v => v.nutrientId === nutrientId));
                        break;
                    case 5:
                        nutrientTarget = this.nutrientTargets.find(t => t.friday && t.nutrientValues.find(v => v.nutrientId === nutrientId));
                        break;
                    case 6:
                        nutrientTarget = this.nutrientTargets.find(t => t.saturday && t.nutrientValues.find(v => v.nutrientId === nutrientId));
                        break;

                }
            }
           
            // default
            if (!nutrientTarget) {
                nutrientTarget = this.nutrientTargets.find(t => !t.monday && !t.tuesday && !t.wednesday && !t.thursday && !t.friday && !t.saturday && !t.sunday && !t.exerciseDay && !t.restday && t.nutrientValues.find(v => v.nutrientId === nutrientId));
            }

            if (nutrientTarget) {
                var values = nutrientTarget.nutrientValues.find(v => v.nutrientId === nutrientId);
                if (values) {
                    return values;
                }
               
            }

            return { min: undefined, max: undefined };
        },
        date: formatters.formatDate,
        time: formatters.formatTime,
        datetime: formatters.formatDateTime,
        unit: formatters.formatUnit,
        decimal(value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        },
        mealName(meal) {
            if (meal.definitionId) {
                var def = this.$store.state.nutrition.mealDefinitions.find(d => d.id == meal.definitionId);
                if (def) {
                    return def.name;
                }
            }
            return this.time(meal.time);
        }
    },
    created() {
        var self = this;
        //self.energyDistributionColumn = { title: this.$t('energyDistribution'), unit: 'P/HH/R', hideSummary: false, hideDetails:true, group: 'MACROCMP'},
        self.$store.dispatch(constants.FETCH_NUTRIENTS, {});
        self.$store.dispatch(constants.FETCH_NUTRIENT_TARGETS, {});
        self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {});
        if(self.start && self.end){
          self.fetchMeals();
        }
        else {
          self.showWeek();    
        }
        self.selectGroup(self.groups[0].id);
    }
}
</script>

<style scoped>
    li.custom-date
    {
        padding: 3px 10px;
    }
    li.custom-date button
    {
        margin-top: 3px;
    }
    .outer 
    {
      position: relative;
    }
    .inner 
    {
      overflow-x: auto;
      overflow-y: visible;
      margin-left: 100px;
    }
    #meal-list
    {
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    #meal-list td 
    {
        padding-bottom: 0px;
    }
    #meal-list td span
    {
        margin: 5px;
    }
    .freeze 
    {
      position: absolute;
      margin-left: -100px;
      width: 100px;
      text-align: right;
    }
    th.time
    {
        top: 112px;
        border-width:0px;
        width: 100px;
    }
    #meal-list a
    {
        cursor: pointer;
    }
    th.nutrient
    {
        height: 120px;
        white-space: nowrap;
    }   
    th.nutrient > div
    {
       transform: translate(32px, 0px) rotate(-45deg);
       width: 40px;
    }
    th.nutrient > div > div 
    {
      border-bottom: 1px solid #ccc;
      padding: 5px 10px;
      width: 100px;
    }
    th.nutrient:nth-child(2) > div
    {
        transform: translate(50px, -5px) rotate(-45deg);
        width: 60px;
    }
    th.unit
    {
        padding:0px;
        text-align:center;
        border-right:1px solid #ccc;
    }
    td.nutrient
    {
        padding-left: 1px;
        padding-right: 1px;
        border-right:1px solid #ccc;
        text-align:center;
        width:40px;
        overflow: visible;
    }
</style>