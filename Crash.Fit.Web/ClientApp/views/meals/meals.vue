<template>
    <div>
        <div v-if="!selectedMeal">
            <section class="content-header"><h1>Ateriat</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                                {{ date(start) }} - {{ date(end) }} <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu">
                                <li><a @click="showDay">T&auml;n&auml;&auml;n</a></li>
                                <li><a @click="showWeek">Kuluva viikko</a></li>
                                <li><a @click="showMonth">Kuluva kuukausi</a></li>
                                <li role="separator" class="divider"></li>
                                <li><a @click="showDays(7)">7 pv</a></li>
                                <li><a @click="showDays(14)">14 pv</a></li>
                                <li><a @click="showDays(30)">30 pv</a></li>
                                <li role="separator" class="divider"></li>
                                <li class="custom-date"><span>Valitse aikav&auml;li</span></li>
                                <li class="custom-date">
                                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="start" v-bind:format="'DD.MM.YYYY'" v-on:change="start=arguments[0]"></datetime-picker>
                                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="end" v-bind:format="'DD.MM.YYYY'" v-on:change="end=arguments[0]"></datetime-picker>
                                    <button class="btn btn-sm" @click="fetchMeals()">OK</button>
                                </li>
                            </ul>
                        </div>
                        <div class="btn-group" role="group" aria-label="...">
                            <button class="btn btn-default" v-bind:class="{ active: selectedGroup==='MACROCMP' }" @click="showNutrients('MACROCMP')">Makrot</button>
                            <button class="btn btn-default" v-bind:class="{ active: selectedGroup==='MINERAL' }" @click="showNutrients('MINERAL')">Mineraalit</button>
                            <button class="btn btn-default" v-bind:class="{ active: selectedGroup==='VITAM' }" @click="showNutrients('VITAM')">Vitamiinit</button>
                        </div>
                        <button class="btn btn-primary" @click="createMeal"><i class="glyphicon glyphicon-plus"></i> Uusi ateria</button>
                        <div class="outer">
                            <div class="inner">
                                <table class="table" id="meal-summary">
                                    <thead>
                                        <tr>
                                            <th class="time freeze">&nbsp;<br />Aika</th>
                                            <template v-for="col in visibleColumns">
                                                <th class="nutrient" v-if="col.visible">{{ col.title }}<br /> {{ unit(col.unit) }}</th>
                                            </template>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <template v-for="day in days">
                                            <tr class="day" @click="toggleDay(day)">
                                                <td class="freeze"><span>{{ date(day.date) }}</span></td>
                                                <td v-for="col in visibleColumns">
                                                    <div class="chart" v-if="col === energyDistributionColumn">
                                                        <chart-pie-energy v-bind:protein="day.nutrients[proteinId]" v-bind:carb="day.nutrients[carbId]" v-bind:fat="day.nutrients[fatId]"></chart-pie-energy>
                                                    </div>
                                                    <span v-else>{{ decimal(day.nutrients[col.key], col.precision) }}</span>
                                                </td>
                                                <td></td>
                                            </tr>
                                            <tr class="meal" v-if="day.showDetails" v-for="meal in day.meals">
                                                <td class="freeze">{{ time(meal.time) }}</td>
                                                <td v-for="col in visibleColumns">
                                                    <div class="chart" v-if="col === energyDistributionColumn">
                                                        <chart-pie-energy v-bind:protein="meal.nutrients[proteinId]" v-bind:carb="meal.nutrients[carbId]" v-bind:fat="meal.nutrients[fatId]"></chart-pie-energy>
                                                    </div>
                                                    <span v-else>{{ decimal(meal.nutrients[col.key], col.precision) }}</span>
                                                </td>
                                                <td class="action">
                                                    <button class="btn btn-sm" @click="editMeal(meal)">Tiedot</button>
                                                </td>
                                            </tr>
                                        </template>
                                    </tbody>
                                </table>
                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedMeal">
            <section class="content-header"><h1>Aterian tiedot</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <meal-editor v-bind:meal="selectedMeal" v-bind:saveCallback="saveMeal" v-bind:cancelCallback="cancelMeal" v-bind:deleteCallback="deleteMeal" />
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var formatters = require('../../formatters')
    var c3 = require('c3');
    var moment = require('moment');

module.exports = {
    data () {
        return {
            columns: [],
            energyDistributionColumn: { title: 'Energiajakauma', visible: true, group: 'MACROCMP'},
            selectedGroup: 'MACROCMP',
            start: null,
            end: null,
            days:[],
            selectedMeal: null,
            proteinId: '1ddca711-0dcc-4708-93c2-1ac3b0b3d281',
            carbId: 'fa5f03f8-6aeb-4d5f-9100-f41cd606d36b',
            fatId: '9fa87e51-46af-4ca9-8c7d-98176cfa8b78'
        }
    },
    computed:{
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
            api.listMeals(this.start, this.end).then(function (meals) {
                var days = [];
                for (var i in meals) {
                    var meal = meals[i];
                    meal.time = new Date(meal.time);
                    var date = moment(meal.time).startOf('day');

                    var day = days.find(d => moment(d.date).isSame(date, 'day'));
                    if (!day) {
                        day = { date: date.toDate(), meals: [], nutrients: {}, showDetails: false };
                        days.push(day);
                    }
                    day.meals.push(meal);
                    for (var nutrientId in meal.nutrients) {
                        if (!day.nutrients[nutrientId]) {
                            day.nutrients[nutrientId] = 0;
                        }
                        day.nutrients[nutrientId] += meal.nutrients[nutrientId];
                    }
                }
                self.days = days.sort(function (a, b) {
                    return a.date.getTime() < b.date.getTime() ? 1 : -1;
                });
            });
        },
        showNutrients: function(group){
            this.selectedGroup = group;
        },
        toggleDay: function(day){
            day.showDetails = !day.showDetails;
        },
        createMeal: function(){
            this.showMeal({ time: new Date()});
        },
        editMeal: function (meal) {
            var self = this;
            api.getMeal(meal.id).then(function (mealDetails) {
                self.showMeal(mealDetails);
            });
            
        },
        saveMeal: function (meal) {
            var self = this;
            api.saveMeal(meal).then(function (savedMeal) {
                // TODO: edit values instead of reload
                // * reduce nutrients from meal day
                // * remove meal from a day
                // * add meal to a day (correct position by time)
                // * add nutrients to meal day
                /*
                if (meal.id) {
                    for (var i in days) {
                        for (var j in days[i].meals) {
                            if (days[i].meals[j].id === meal.id) {
                                for (var nutrientId in days[i].meals[j].nutrients) {
                                    days[i].nutrients[nutrientId] -= days[i].meals[j].nutrients[nutrientId];
                                }
                                days[i].meals.splice(j, 1);
                            }
                        }
                    }
                }

                savedMeal.time = new Date(savedMeal.time);
                var date = moment(savedMeal.time).startOf('day');
                var day = self.days.find(d => moment(d.date).isSame(date, 'day'));
                if (day) {
                    if (!meal.id) {
                        day.meals.push(meal);
                    }
                    for (var nutrientId in meal.nutrients) {
                        if (!day.nutrients[nutrientId]) {
                            day.nutrients[nutrientId] = 0;
                        }
                        day.nutrients[nutrientId] += meal.nutrients[nutrientId];
                    }
                }*/
                self.fetchMeals();
                self.showSummary();
            });
        },
        cancelMeal: function (meal) {
            this.showSummary();
        },
        deleteMeal: function (meal) {
            var self = this;
            api.deleteMeal(meal.id).then(function () {
                self.fetchMeals();
                self.showSummary();
            });
        },
        showMeal: function (meal) {
            this.selectedMeal = meal;
        },
        showSummary() {
            this.selectedMeal = null;
        },
        date: formatters.formatDate,
        time: formatters.formatTime,
        unit: formatters.formatUnit,
        decimal: function (value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        }
    },
    watch:{
        $route: function(){
            console.log(this.$route);
        }
    },
    created: function () {
        
        var self = this;
        api.listNutrients().then(function (nutrients) {
            for (var i in nutrients) {
                if (nutrients[i].uiVisible) {
                    self.columns.push({ title: nutrients[i].shortName, unit: nutrients[i].unit, precision: nutrients[i].precision, key: nutrients[i].id, visible: true, group: nutrients[i].fineliGroup });
                }
            }
            self.columns.push(self.energyDistributionColumn);
        });
        var id = this.$route.params.id;
        if (id) {
            api.getMeal(id).then(function (meal) {
                self.start = moment(meal.time).startOf('day');
                self.end = moment(meal.time).endOf('day');
            });
        } else {
            self.showWeek();
        }
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
    #meal-summary
    {
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    #meal-summary td 
    {
        padding-bottom: 0px;
    }
    #meal-summary td span
    {
        margin: 5px;
    }
    tr.day td
    {
        background-color:azure;
    }
    .freeze 
    {
      position: absolute;
      margin-left: -80px;
      width: 80px;
      text-align: right;
    }
    th.time
    {
        width: 80px;
    }
    th.nutrient
    {
        width: 70px;
    }
    td div.chart
    {
        width: 100px;
        height: 20px;
    }
    td.action {
        padding: 0px;
    }
</style>