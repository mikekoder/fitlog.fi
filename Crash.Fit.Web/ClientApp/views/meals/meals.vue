<template>
    <div>
        <div v-if="!selectedMeal">
            <section class="content-header"><h1>{{ $t("meals.title") }}</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                                {{ date(start) }} - {{ date(end) }} <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu">
                                <li><a @click="showDay">{{ $t("today") }}</a></li>
                                <li><a @click="showWeek">{{ $t("currentWeek") }}</a></li>
                                <li><a @click="showMonth">{{ $t("currentMonth") }}</a></li>
                                <li role="separator" class="divider"></li>
                                <li><a @click="showDays(7)">7 {{ $t("days") }}</a></li>
                                <li><a @click="showDays(14)">14 {{ $t("days") }}</a></li>
                                <li><a @click="showDays(30)">30 {{ $t("days") }}</a></li>
                                <li role="separator" class="divider"></li>
                                <li class="custom-date"><span>{{ $t("chooseDateInterval") }}</span></li>
                                <li class="custom-date">
                                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="start" v-bind:format="'DD.MM.YYYY'" v-on:change="start=arguments[0]"></datetime-picker>
                                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="end" v-bind:format="'DD.MM.YYYY'" v-on:change="end=arguments[0]"></datetime-picker>
                                    <button class="btn btn-sm" @click="fetchMeals()">{{ $t("OK") }}</button>
                                </li>
                            </ul>
                        </div>
                        
                        <div class="btn-group" role="group" aria-label="...">
                            <template v-for="group in groups">
                                <button class="btn btn-default" v-bind:class="{ active: selectedGroup === group.id }" @click="selectGroup(group.id)">{{ $t('nutrients.group.'+group.id) }}</button>
                            </template>
                        </div>
                       
                        <button class="btn btn-primary" @click="createMeal">{{ $t("meals.create") }}</button>
                        <div class="outer" v-if="days.length > 0">
                            <div class="inner">
                                <table class="table" id="meal-list">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <template v-for="col in visibleColumns">
                                                <th class="nutrient" v-if="!col.hideSummary"><div><div>{{ col.title }}</div></div></th>
                                            </template>
                                            <th></th>
                                        </tr>
                                        <tr>
                                            <th class="time freeze">{{ $t("meals.columns.time") }}</th>
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
                                                        <div class="chart" v-if="col === energyDistributionColumn">
                                                            <chart-pie-energy v-bind:protein="day.nutrients[proteinId]" v-bind:carb="day.nutrients[carbId]" v-bind:fat="day.nutrients[fatId]"></chart-pie-energy>
                                                        </div>
                                                        <div v-else><div class="bar"></div>{{ decimal(day.nutrients[col.key], col.precision) }}</div>
                                                    </td>
                                                </template>
                                                <td></td>
                                            </tr>
                                            <tr class="meal" v-if="dayStates[day.date.getTime()]" v-for="meal in day.meals">
                                                <td class="freeze"><router-link :to="{ name: 'meals', params: { id: meal.id } }">{{ time(meal.time) }}</router-link></td>
                                                <template v-for="col in visibleColumns">
                                                    <td class="nutrient" v-if="!col.hideSummary">
                                                        <div class="chart" v-if="col === energyDistributionColumn">
                                                            <chart-pie-energy v-bind:protein="meal.nutrients[proteinId]" v-bind:carb="meal.nutrients[carbId]" v-bind:fat="meal.nutrients[fatId]"></chart-pie-energy>
                                                        </div>
                                                        <span v-else>{{ decimal(meal.nutrients[col.key], col.precision) }}</span>
                                                    </td>
                                                </template>
                                                <td><button class="btn btn-danger btn-xs" @click="deleteMeal(meal)">Poista</button></td>
                                            </tr>
                                        </template>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div v-if="days.length == 0">
                            <br />
                            {{ $t("meals.noMeals") }}
                        </div>
                       
                        <div v-for="meal in meals">
                            <div class="alert alert-info" role="alert" v-if="meal.deleted">Ateria {{ datetime(meal.time) }} poistettu. <button class="btn btn-link" @click="restoreMeal(meal)">Palauta</button></div>
                        </div>
                        
                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedMeal">
            <section class="content-header"><h1>{{ $t("meals.mealDetails") }}</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <meal-editor v-bind:meal="selectedMeal" v-bind:saveCallback="saveMeal" v-bind:cancelCallback="cancelMeal" v-bind:deleteCallback="deleteMeal" v-bind:copyCallback="copyMeal" />
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script>

    var api = require('../../api');
    var formatters = require('../../formatters')
    var moment = require('moment');
    var toaster = require('../../toaster');
    var utils = require('../../utils');
    var constants = require('../../store/constants')

module.exports = {
    data () {
        return {
            energyDistributionColumn: null,
            selectedGroup: '',
            start: null,
            end: null,
            dayStates: {},
            selectedMeal: null,
            proteinId: constants.PROTEIN_ID,
            carbId: constants.CARB_ID,
            fatId: constants.FAT_ID
        }
    },
    computed: {
        groups: function () {
            return this.$store.state.nutrition.nutrientGroups;
        },
        columns: function(){
            var columns = [];
            columns.push(this.energyDistributionColumn);
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
            return this.$store.state.nutrition.mealDays.filter(md => moment(md.date).isBetween(self.start, self.end));
        },
        visibleColumns: function () {
            var self = this;
            return this.columns.filter(function (c) {
                return !c.group || c.group == self.selectedGroup;
            });
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'chart-pie-energy': require('./energy-distribution-bar'),
        'meal-editor': require('./meal-editor')
    },
    methods: {
        showDay() {
            this.end = moment().endOf('day').toDate();
            this.start = moment().startOf('day').toDate();
            this.fetchMeals();
        },
        showWeek(){
            this.end = moment().endOf('day').toDate();
            this.start = moment().startOf('isoWeek').toDate();
            this.fetchMeals();
        },
        showMonth(){
            this.end = moment().endOf('day').toDate();
            this.start = moment().startOf('month').toDate();
            this.fetchMeals();
        },
        showDays(days) {
            this.end = moment().endOf('day').toDate();
            this.start = moment().subtract(days - 1, 'days').startOf('day').toDate();
            this.fetchMeals();
        },
        fetchMeals: function () {
            var self = this;
            this.$store.dispatch(constants.FETCH_MEALS, { start: self.start, end: self.end });
            this.$store.dispatch(constants.FETCH_WORKOUTS, { start: self.start, end: self.end });
        },
        selectGroup: function(group){
            this.selectedGroup = group;
        },
        toggleDay: function (day) {
            this.$set(this.dayStates,day.date.getTime(), !(this.dayStates[day.date.getTime()] && true))
        },
        createMeal: function(){
            this.showMeal({ time: utils.previousHalfHour()});
        },
        editMeal: function (id) {
            var self = this;
            api.getMeal(id).then(function (mealDetails) {
                self.showMeal(mealDetails);
            }).fail(function () {
                toaster.error('Aterian haku epäonnistui');
            });
        },
        saveMeal: function (meal) {
            var self = this;
            this.$store.dispatch(constants.SAVE_MEAL, {
                meal,
                success: function () {
                    self.$router.push({ name: 'meals' });
                    self.showSummary();
                },
                failure: function () {
                    toaster.error('Aterian tallentaminen epäonnistui');
                }
            });
        },
        cancelMeal: function (meal) {
            this.$router.push({ name: 'meals' });
            this.showSummary();
        },
        deleteMeal: function (meal) {
            var self = this;
            this.$store.dispatch(constants.DELETE_MEAL, {
                meal,
                success: function () {
                    self.$router.push({ name: 'meals' });
                    this.showSummary();
                },
                failure: function () {
                    toaster.error('Aterian poistaminen epäonnistui');
                }
            });
            this.showSummary();
        },
        restoreMeal: function (meal) {
            var self = this;
            api.restoreMeal(meal.id).then(function () {
                meal.deleted = false;
                self.buildDays();
                self.$router.push({ name: 'meals' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Aterian palauttaminen epäonnistui');
            });
        },
        copyMeal: function (meal) {
            var self = this;
            self.cancelMeal();
            meal.id = undefined;
            meal.time = new Date();
            setTimeout(function () {
                self.showMeal(meal);
            }, 100);
            
        },
        showMeal: function (meal) {
            this.selectedMeal = meal;
        },
        showSummary() {
            this.selectedMeal = null;
        },
        dayIsExpanded(day){
            return this.dayStates[day.date.getTime()] && true;
        },
        nutrientValues(nutrientId, day){

        },
        date: formatters.formatDate,
        time: formatters.formatTime,
        datetime: formatters.formatDateTime,
        unit: formatters.formatUnit,
        decimal: function (value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        }
    },
    created: function () {
        var self = this;
        this.energyDistributionColumn = { title: this.$t('meals.columns.energyDistribution'), unit: 'P/HH/R', hideSummary: false, hideDetails:true, group: 'MACROCMP'},
        this.$store.dispatch(constants.FETCH_NUTRIENTS, {});
        this.$store.dispatch(constants.FETCH_NUTRIENT_TARGETS, {});
        var id = this.$route.params.id;
        if (id) {
            api.getMeal(id).then(function (meal) {
                self.start = moment(meal.time).startOf('day');
                self.end = moment(meal.time).endOf('day');
                self.fetchMeals();
                self.showMeal(meal);
            }).fail(function () {
                toaster.error('Aterian haku epäonnistui');
            });
        } else {
            self.showWeek();
        }
        this.selectGroup(this.groups[0].id);
    },
    beforeRouteUpdate (to, from, next) {
        if (to.params.id) {
            this.editMeal(to.params.id);
        }
        else {
            this.showSummary();
        }
        next();
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