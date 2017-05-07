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
                        <div class="outer" v-if="days.length > 0">
                            <div class="inner">
                                <table class="table" id="meal-list">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <template v-for="col in visibleColumns">
                                                <th class="nutrient" v-if="col.visible"><div><div>{{ col.title }}</div></div></th>
                                            </template>
                                            <th></th>
                                        </tr>
                                        <tr>
                                            <th class="time freeze">Aika</th>
                                            <template v-for="col in visibleColumns">
                                                <th class="unit" v-if="col.visible">{{ unit(col.unit) }}</th>
                                            </template>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <template v-for="day in days">
                                            <tr class="day" @click="toggleDay(day)">
                                                <td class="freeze">
                                                    <i v-if="!day.showDetails" class="fa fa-arrow-down"></i>
                                                    <i v-if="day.showDetails" class="fa fa-arrow-up"></i>
                                                    {{ date(day.date) }}</td>
                                                <td class="nutrient" v-for="col in visibleColumns">
                                                    <div class="chart" v-if="col === energyDistributionColumn">
                                                        <chart-pie-energy v-bind:protein="day.nutrients[proteinId]" v-bind:carb="day.nutrients[carbId]" v-bind:fat="day.nutrients[fatId]"></chart-pie-energy>
                                                    </div>
                                                    <span v-else>{{ decimal(day.nutrients[col.key], col.precision) }}</span>
                                                </td>
                                                <td></td>
                                            </tr>
                                            <tr class="meal" v-if="day.showDetails" v-for="meal in day.meals">
                                                <td class="freeze"><router-link :to="{ name: 'meals', params: { id: meal.id } }">{{ time(meal.time) }}</router-link></td>
                                                <td class="nutrient" v-for="col in visibleColumns">
                                                    <div class="chart" v-if="col === energyDistributionColumn">
                                                        <chart-pie-energy v-bind:protein="meal.nutrients[proteinId]" v-bind:carb="meal.nutrients[carbId]" v-bind:fat="meal.nutrients[fatId]"></chart-pie-energy>
                                                    </div>
                                                    <span v-else>{{ decimal(meal.nutrients[col.key], col.precision) }}</span>
                                                </td>
                                                <td><button class="btn btn-danger btn-xs" @click="deleteMeal(meal)">Poista</button></td>
                                            </tr>
                                        </template>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div v-if="days.length == 0">
                            <br />
                            Ei aterioita
                        </div>
                       
                        <div v-for="meal in meals">
                            <div class="alert alert-info" role="alert" v-if="meal.deleted">Ateria {{ datetime(meal.time) }} poistettu. <button class="btn btn-link" @click="restoreMeal(meal)">Palauta</button></div>
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

module.exports = {
    data () {
        return {
            columns: [],
            energyDistributionColumn: { title: 'Energiajakauma', unit: 'P/HH/R', visible: true, group: 'MACROCMP'},
            selectedGroup: 'MACROCMP',
            start: null,
            end: null,
            meals:[],
            days: [],
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
                self.meals = meals;
                self.buildDays();
            }).fail(function () {
                toaster.error('Aterioiden haku epäonnistui');
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
            api.saveMeal(meal).then(function (savedMeal) {
                self.fetchMeals();
                self.$router.push({ name: 'meals' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Aterian tallennus epäonnistui');
            });
        },
        cancelMeal: function (meal) {
            this.$router.push({ name: 'meals' });
            this.showSummary();
        },
        deleteMeal: function (meal) {
            var self = this;
            api.deleteMeal(meal.id).then(function () {
                meal.deleted = true;
                self.buildDays();
                self.$router.push({ name: 'meals' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Aterian poistaminen epäonnistui');
            });
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
        buildDays: function () {

            var dayStates = this.days.map(d => { return { date: d.date, showDetails: d.showDetails } });

            var days = [];
            for (var i in this.meals) {
                var meal = this.meals[i];
                if (meal.deleted) {
                    continue;
                }

                meal.time = new Date(meal.time);
                var date = moment(meal.time).startOf('day');

                var day = days.find(d => moment(d.date).isSame(date, 'day'));
                if (!day) {
                    state = dayStates.find(d => moment(d.date).isSame(date, 'day'));
                    day = { date: date.toDate(), meals: [], nutrients: {}, showDetails: state && state.showDetails };
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
            this.days = days.sort(function (a, b) {
                return a.date.getTime() < b.date.getTime() ? 1 : -1;
            });
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
        api.listNutrients().then(function (nutrients) {
            self.columns.push(self.energyDistributionColumn);
            for (var i in nutrients) {
                if (nutrients[i].uiVisible) {
                    self.columns.push({ title: nutrients[i].name, unit: nutrients[i].unit, precision: nutrients[i].precision, key: nutrients[i].id, visible: true, group: nutrients[i].fineliGroup });
                }
            }
        }).fail(function () {
            toaster.error('Ravintoaineiden haku epäonnistui');
        });

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