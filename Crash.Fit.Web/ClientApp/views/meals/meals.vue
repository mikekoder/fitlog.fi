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
                                    <button class="btn btn-sm" @click="fetchMeals">OK</button>
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
                                            <tr @click="toggleDay(day)">
                                                <td class="freeze"><span>{{ date(day.time) }}</span></td>
                                                <td v-for="col in visibleColumns">
                                                    <div class="chart" v-if="col === energyDistributionColumn">
                                                        <chart-pie-energy v-bind:protein="123" v-bind:carb="123" v-bind:fat="123"></chart-pie-energy>
                                                    </div>
                                                    <span v-else>{{ day[col.key] }} 1234.567</span>
                                                </td>
                                                <td></td>
                                            </tr>
                                            <tr v-if="day.showDetails" v-for="meal in day.meals">
                                                <td class="freeze">{{ time(meal.time) }}</td>
                                                <td v-for="col in visibleColumns">{{ meal[col.key] }}</td>
                                                <td>
                                                    <button class="btn" @click="editMeal(meal)">Edit</button>
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
            days: [{ time: new Date(), meals: [{ time: new Date() }], showDetails: false }],
            selectedMeal: null
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
        'chart-pie-energy': require('./chart-pie-energy'),
        'meal-editor': require('./meal-editor')
    },
    methods: {
        showDay() {
            this.end = moment().endOf('day').toDate();
            this.start = moment().startOf('day').toDate();
            console.log(this.start, this.end);
        },
        showWeek(){
            this.end = moment().endOf('day').toDate();
            this.start = moment().startOf('isoWeek').toDate();
            console.log(this.start, this.end);
        },
        showMonth(){
            this.end = moment().endOf('day').toDate();
            this.start = moment().startOf('month').toDate();
            console.log(this.start, this.end);
        },
        showDays(days) {
            this.end = moment().endOf('day').toDate();
            this.start = moment().subtract(days, 'days').startOf('day').toDate();
            console.log(this.start, this.end);
        },
        fetchMeals: function () { 
            api.listMeals(this.start, this.end).then(function (meals) {

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
        editMeal: function(meal){
            this.showMeal(meal);
        },
        saveMeal: function (meal) {
            var rows = [];
            for (var i in meal.rows) {
                var mealRow = meal.rows[i];
                var row = {};
                if (mealRow.quantity) {
                    if (typeof (mealRow.quantity === 'number')) {
                        row.quantity = mealRow.quantity;
                    }
                    else {
                        row.quantity = parseFloat(mealRow.quantity.replace(',', '.'));
                    }
                }
                if (mealRow.food) {
                    row.foodId = mealRow.food.id;
                }
                if (mealRow.portion) {
                    row.portionId = mealRow.portion.id;
                }
                rows.push(row);
            }
            var meal = {
                id: meal.id,
                name: meal.name,
                time: moment(meal.time).format(),
                rows: rows,
                userId: null
            };
            api.saveMeal(meal).then(function (savedMeal) {

            });
        },
        cancelMeal: function (meal) {
            this.showSummary();
        },
        deleteMeal: function (meal) {
            this.showSummary();
        },
        showMeal: function (meal) {
            this.selectedMeal = meal;
        },
        showSummary() {
            this.selectedMeal = null;
        },
        date: formatters.formatDate,
        time: formatters.formatTime,
        unit: formatters.formatUnit
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
                    self.columns.push({ title: nutrients[i].shortName, unit: nutrients[i].unit, key: nutrients[i].id, visible: true, group: nutrients[i].fineliGroup });
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
            self.showDay();
        }
        
        console.log(this.$route);
    },
    mounted: function () {
        this.fetchMeals();
    }
}
</script>

<style scoped>
    li.custom-date{
        padding: 3px 10px;
    }
    li.custom-date button{
        margin-top: 3px;
    }
    .outer {
      position: relative;
    }
    .inner {
      overflow-x: auto;
      overflow-y: visible;
      margin-left: 100px;
    }
    #meal-summary{
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    #meal-summary td {
        padding-bottom: 0px;
    }
    #meal-summary td span{
        margin: 5px;
    }
    .freeze {
      position: absolute;
      margin-left: -100px;
      width: 80px;
    }


   
    th.time{
        width: 80px;
    }

    th.nutrient{
        width: 70px;
    }
    td div.chart{
        position: relative;
        top: -10px;
    }
    
   
</style>