<template>
    <div>
        <section class="content-header"></section>
        <section class="content">
            <div v-if="isLoggedIn">
                <div class="row">
                    <div class="col-xs-12">
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
                    <div class="col-xs-12">
                        <div class="box box-solid">
                            <div class="box-body">
                                <template v-if="!editNutrients">
                                    <div class="row">
                                        <div class="col-xs-12"><button class="btn pull-right" @click="editSettings" v-if="!editNutrients"><i class="fa fa-gear"></i></button></div>
                                    </div>
                                    <div class="row day-nutrients">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <span class="hidden-md hidden-lg">{{ nutrient.shortName }}</span>
                                            <span class="hidden-xs hidden-sm">{{ nutrient.name }}</span>
                                        </div>
                                        
                                    </div>
                                    <div class="row day-nutrients" v-if="meals.find(m => m.meal)">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <div v-if="nutrient.id == energyDistributionId">
                                                <chart-pie-energy v-bind:protein="nutrients[proteinId]" v-bind:carb="nutrients[carbId]" v-bind:fat="nutrients[fatId]"></chart-pie-energy>
                                            </div>
                                            <div v-else>{{ decimal(nutrients[nutrient.id], nutrient.precision) }}</div>
                                        </div>
                                    </div>
                                </template>
                                <template v-else>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <h4>{{ $t('nutrientsToShow') }}</h4>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-2" v-for="(id,i) in selectedNutrients">
                                            <select class="form-control" v-model="selectedNutrients[i]">
                                                <option v-bind:value="undefined"></option>
                                                <template v-for="group in nutrientGroups">
                                                    <option disabled>{{ group.name }}</option>
                                                    <option v-for="nutrient in group.nutrients" v-bind:value="nutrient.id">
                                                    {{ nutrient.name }} ({{ unit(nutrient.unit) }})
                                                    </option>
                                                </template>
                                            </select>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <button class="btn btn-primary pull-right" @click="saveSettings" v-if="editNutrients">{{ $t('save') }}</button>
                                            <span class="pull-right">&nbsp;</span>
                                            <button class="btn pull-right" @click="editNutrients=false" v-if="editNutrients">{{ $t('cancel') }}</button>
                                        </div>
                                    </div>
                                </template>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-12">
                        <div class="box box-solid box-primary" v-for="meal in meals">
                            <div class="box-header with-border">
                                <h3 class="box-title">{{ mealName(meal) }}</h3>
                            </div>
                            <div class="box-body" v-if="meal.meal">
                                <div class="row meal-nutrients" v-if="meal.meal.nutrients">
                                    <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <span class="hidden-md hidden-lg">{{ nutrient.shortName }}</span>
                                            <span class="hidden-xs hidden-sm">{{ nutrient.name }}</span>
                                        <div v-if="nutrient.id == energyDistributionId">
                                            <chart-pie-energy v-bind:protein="meal.meal.nutrients[proteinId]" v-bind:carb="meal.meal.nutrients[carbId]" v-bind:fat="meal.meal.nutrients[fatId]"></chart-pie-energy>
                                        </div>
                                        <div v-else>{{ decimal(meal.meal.nutrients[nutrient.id], nutrient.precision) }}</div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-12">
                                        <hr />
                                    </div>
                                </div>
                                <template v-for="row in meal.meal.rows">
                                    <div class="row">
                                        <div class="col-xs-8 col-sm-10 food"><span>{{ row.foodName }} {{ row.quantity }} {{ row.portionName || 'g' }}</span></div>
                                        <div class="col-xs-4 col-sm-2">
                                            <button class="btn pull-right" @click="deleteFood(row)"><i class="fa fa-trash-o"></i></button>
                                            <span class="pull-right">&nbsp;</span>
                                            <button class="btn pull-right" @click="editFood(row)"><i class="fa fa-edit"></i></button>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <div v-if="nutrient.id == energyDistributionId">
                                                <chart-pie-energy v-bind:protein="row.nutrients[proteinId]" v-bind:carb="row.nutrients[carbId]" v-bind:fat="row.nutrients[fatId]"></chart-pie-energy>
                                            </div>
                                            <div v-else>{{ decimal(row.nutrients[nutrient.id], nutrient.precision) }}</div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <hr />
                                        </div>
                                    </div>
                                </template>
                                
                            </div>
                            <div class="box-footer">
                                <div class="row">
                                    <div class="col-xs-12">
                                        <button class="btn" @click="addFood(meal)">{{ $t('addFood') }}</button>
                                    </div>
                                </div>
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
            <add-food v-bind:show="showAddFood" v-bind:row="row" v-on:save="saveFood(arguments[0])" v-on:close="showAddFood=false"></add-food>
        </section>
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import formatters from '../../formatters'
    import toaster from '../../toaster'
    import moment from 'moment'

export default {
    data () {
        return {
            proteinId: constants.PROTEIN_ID,
            carbId: constants.CARB_ID,
            fatId: constants.FAT_ID,
            energyId: constants.ENERGY_ID,
            energyDistributionId: constants.ENERGY_DISTRIBUTION_ID,
            showAddFood: false,
            row: undefined,
            selectedNutrients: [],
            editNutrients: false,

        }
    },
    computed: {
        selectedDate() {
            return this.$store.state.nutrition.diaryDate;
        },
        dateText() {
            if (moment().isSame(this.selectedDate, 'd')) {
                return this.$t('today');
            }
            else if (moment().subtract(1,'day').isSame(this.selectedDate, 'd')) {
                return this.$t('yesterday');
            }
            return this.date(this.selectedDate);
        },
        visibleNutrients() {
            return this.$store.state.nutrition.nutrients.filter(n => n.homeOrder || n.homeOrder === 0).sort((n1,n2) => n1.homeOrder - n2.homeOrder);
        },
        meals() {
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
        nutrients() {
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
        },
        nutrientGroups() {
            var nutrients = this.$store.state.nutrition.nutrients;
            return this.$store.state.nutrition.nutrientGroups.map(g => {
                return {
                    name: g.name,
                    nutrients: nutrients.filter(n => n.fineliGroup == g.id).sort((n1, n2) => n1.name < n2.name ? -1 : 1)
                }
            });
            return 
        }
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
        'add-food': require('./add-food'),
        'chart-pie-energy': require('../../components/energy-distribution-bar'),
    },
    methods: {
        fetchMeals() {
            var self = this;
            var start = moment(self.selectedDate).startOf('day');
            var end = moment(self.selectedDate).endOf('day');
            self.$store.dispatch(constants.FETCH_MEALS, {
                start,
                end,
                success() {

                },
                failure() { }
            });
        },
        changeDate(date) {
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
                this.$store.dispatch(constants.SELECT_MEAL_DIARY_DATE, { date: newDate });
                this.fetchMeals();
            }
        },
        mealName(defMeal) {
            if (defMeal.definition) {
                return defMeal.definition.name;
            }
            return this.time(defMeal.meal.time);
        },
        addFood(defMeal) {
            this.row = {
                mealDefinitionId: defMeal.definition ? defMeal.definition.id : undefined,
                mealId: defMeal.meal ? defMeal.meal.id : undefined
            };
            this.showAddFood = true;
        },
        editFood(row) {
           this.row = row;
           this.showAddFood = true;
        },
        saveFood(row) {
            row.date = this.selectedDate;
            this.$store.dispatch(constants.SAVE_MEAL_ROW, {
                row,
                success() {},
                failure() { }
            });
            this.row = {};
            this.showAddFood = false;
        },
        deleteFood() {

        },
        editSettings() {
            var selectedNutrients = [];
            this.visibleNutrients.forEach(n => { selectedNutrients.push(n.id); });
            for (var i = selectedNutrients.length; i < 6; i++) {
                selectedNutrients.push(undefined);
            }
            this.selectedNutrients = selectedNutrients;
            this.editNutrients = true;
        },
        saveSettings() {
            var self = this;
            var settings = {
                nutrients: self.selectedNutrients
            };
            
            this.$store.dispatch(constants.SAVE_MEAL_DIARY_SETTINGS, {
                settings,
                success() {},
                failure() { }
            });
            this.editNutrients = false;
        },
        date: formatters.formatDate,
        time: formatters.formatTime,
        unit: formatters.formatUnit,
        decimal(value, precision) {
            if (!value) {
                return value;
            }

            return value.toFixed(precision);
        }
    },
    created() {
        var self = this;
        self.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success() {},
            failure() { }
        });
        self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
            success() {
                self.fetchMeals();
            },
            failure() { }
        });
        self.$store.dispatch(constants.FETCH_LATEST_FOODS, {
            success() { },
            failure() { }
        });
        self.$store.dispatch(constants.FETCH_MOST_USED_FOODS, {
            success() { },
            failure() { }
        });
        this.$store.commit(constants.LOADING_DONE);
    }
}
</script>

<style scoped>
    .day-nutrients, .meal-nutrients{font-weight: bold; }
    table.nutrition{width: auto;}
    table.nutrition td{ width: 50px;}
    div.food{ padding-top:5px;}
    .box-body hr{ margin: 2px 0px;}
    .box-body div.row:last-child hr{display:none;}
    option[disabled]{font-weight: bold;}
    div.row > button.pull-right{margin-right:15px;}
</style>