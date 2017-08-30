<template>
    <div>
        <section class="content-header"></section>
        <section class="content">
            <div v-if="isLoggedIn">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn" @click="changeDate(-1)"><i class="fa fa-chevron-left"></i></button>
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                                {{ dateText }} <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu">
                                <li class="clickable"><a @click="changeDate('today')">{{ $t("today") }}</a></li>
                                <li class="clickable"><a @click="changeDate('yesterday')">{{ $t("yesterday") }}</a></li>
                                <li role="separator" class="divider"></li>
                                <li class="custom-date">
                                    <datetime-picker v-bind:value="selectedDate" v-bind:format="'DD.MM.YYYY'" v-on:change="changeDate(arguments[0])"></datetime-picker>
                                </li>
                            </ul>
                        </div>
                        <button class="btn" @click="changeDate(1)"><i class="fa fa-chevron-right"></i></button>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-12">
                        <div class="box box-solid">
                            <div class="box-body">
                                <div class="row">
                                    <div class="col-xs-3">{{ $t('protein') }}</div>
                                    <div class="col-xs-3">{{ $t('carbs') }}</div>
                                    <div class="col-xs-3">{{ $t('fat') }}</div>
                                    <div class="col-xs-3">{{ $t('energy') }}</div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-3">{{ nutrients[proteinId] }}</div>
                                    <div class="col-xs-3">{{ nutrients[carbId] }}</div>
                                    <div class="col-xs-3">{{ nutrients[fatId] }}</div>
                                    <div class="col-xs-3">{{ nutrients[energyId] }}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <div class="box box-solid box-primary" v-for="meal in meals">
                            <div class="box-header with-border">
                                <h3 class="box-title">{{ mealName(meal) }}</h3>
                                <div class="row" v-if="meal.nutrients">
                                    <div class="col-xs-3">{{ meal.nutrients[proteinId] }}</div>
                                    <div class="col-xs-3">{{ meal.nutrients[carbId] }}</div>
                                    <div class="col-xs-3">{{ meal.nutrients[fatId] }}</div>
                                    <div class="col-xs-3">{{ meal.nutrients[energyId] }}</div>
                                </div>
                            </div>
                            <div class="box-body" v-if="meal.meal">
                                <template v-for="row in meal.meal.rows">
                                    <div class="row">
                                        <div class="col-sm-12"> {{ row.foodName }} {{ row.quantity }} {{ row.portionName || 'g' }}</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-3">{{ row.nutrients[proteinId] }}</div>
                                        <div class="col-xs-3">{{ row.nutrients[carbId] }}</div>
                                        <div class="col-xs-3">{{ row.nutrients[fatId] }}</div>
                                        <div class="col-xs-3">{{ row.nutrients[energyId] }}</div>
                                    </div>
                                </template>
                            </div>
                            <div class="box-footer">
                                <button class="btn" @click="addFood(meal)">{{ $t('addFood') }}</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-if="!isLoggedIn">
                Fitlog on ilmainen ruoka- ja treenipäiväkirja.
            </div>
        </section>
        <section v-if="showAddFood">
            <add-food v-bind:show="showAddFood" v-bind:row="row" v-on:save="saveFood(arguments[0])"></add-food>
        </section>
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var formatters = require('../../formatters')
module.exports = {
    data () {
        return {
            proteinId: constants.PROTEIN_ID,
            carbId: constants.CARB_ID,
            fatId: constants.FAT_ID,
            energyId: constants.ENERGY_ID,
            showAddFood: false,
            row: undefined
        }
    },
    computed: {
        selectedDate: function () {
            return this.$store.state.home.date;
        },
        dateText: function () {
            if (moment().isSame(this.selectedDate, 'd')) {
                return this.$t('today');
            }
            else if (moment().subtract(1,'day').isSame(this.selectedDate, 'd')) {
                return this.$t('yesterday');
            }
            return this.date(this.selectedDate);
        },
        meals: function () {
            var self = this;
            var start = moment(self.selectedDate).startOf('day');
            var end = moment(self.selectedDate).endOf('day');
            var defs = self.$store.state.nutrition.mealDefinitions;
            var meals = self.$store.state.nutrition.meals.filter(m => moment(m.time).isBetween(start, end));
            var result = defs.map(d => { return { definition: d, meal: meals.find(m => m.definitionId == d.id) } });
            meals.filter(m => !m.definitionId).forEach(m => {
                var index = result.findIndex(r => r.definition && r.definition.startHour && r.definition.startHour > m.time.getHours());
                if (index == -1) {
                    result.push({ meal: m });
                }
                else
                    result.splice(index, 0, { meal: m });
                }
            );
            return result;
        },
        nutrients: function () {
            var result = {};
            this.meals.filter(m => m.meal).forEach(m => {
                for (var i in m.meal.nutrients) {
                    if (!result[i]) {
                        result[i] = 0;
                    }
                    result[i] += m.meal.nutrients[i];
                }
            });
            return result;
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'add-food': require('./add-food')
    },
    methods: {
        fetchMeals: function () {
            var self = this;
            var start = moment(self.selectedDate).startOf('day');
            var end = moment(self.selectedDate).endOf('day');
            self.$store.dispatch(constants.FETCH_MEALS, {
                start,
                end,
                success: function () {

                },
                failure: function () { }
            });
        },
        changeDate: function (date) {
            var newDate;
            if (date == 'today') {
                newDate = new Date();
            }
            else if (date == 'yesterday') {
                newDate = moment().subtract(1, 'days').toDate();
            }
            else if (date === -1) {
                newDate = moment(this.selectedDate).subtract(1, 'days').toDate();
            }
            else if (date === 1){
                newDate = moment(this.selectedDate).add(1, 'days').toDate();
            }
            else {
                newDate = date;
            }
            if (!moment(newDate).isSame(this.selectedDate, 'd')) {
                this.$store.dispatch(constants.SELECT_HOME_DATE, { date: newDate });
                this.fetchMeals();
            }
        },
        mealName: function (defMeal) {
            if (defMeal.definition) {
                return defMeal.definition.name;
            }
            return this.time(defMeal.meal.time);
        },
        addFood: function (defMeal) {
            this.row = {
                mealDefinitionId: defMeal.definition ? defMeal.definition.id : undefined,
                mealId: defMeal.meal ? defMeal.meal.id : undefined
            };
            this.showAddFood = true;
        },
        saveFood: function (data) {
            data.date = this.selectedDate;
            data.foodId = data.food.id;
            data.portionId = data.portion ? data.portion.id : undefined;
            this.$store.dispatch(constants.ADD_MEAL_ROW, {
                row: data,
                success: function () {},
                failure: function () { }
            });
            this.row = {};
            this.showAddFood = false;
        },
        date: formatters.formatDate,
        time: formatters.formatTime
    },
    created() {
        var self = this;

        self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
            success: function () {
                self.fetchMeals();
            },
            failure: function () { }
        });
        this.$store.commit(constants.LOADING_DONE);
    }
}
</script>

<style scoped>
    button.dropdown-toggle{ width: 200px; }
    table.nutrition{width: auto;}
    table.nutrition td{ width: 50px;}
</style>