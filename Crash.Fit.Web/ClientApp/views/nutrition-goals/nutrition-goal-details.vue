<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutritionGoals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input class="form-control" v-model="name" />
                    </div>

                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="addPeriod">{{ $t("addPeriod") }}</button>
                    <button class="btn btn-danger" @click="deletePeriod" v-if="selectedPeriod">{{ $t("deleteSelectedPeriod") }}</button>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-xs-4 col-sm-3 col-md-2 master">
                    <div class="box box-solid" v-for="period in periods" v-bind:class="{ selected: period == selectedPeriod }" @click="selectPeriod(period)">
                        <div class="box-header with-border">
                            {{ daysFormatted(period) }}
                        </div>
                        <div class="box-body">
                            {{ mealsFormatted(period) }}
                        </div>
                    </div>
                </div>
                <div class="col-xs-8" v-if="selectedPeriod">
                    <label>{{ $t('onlyDays') }}</label>
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
                            <td><i class="fa fa-heartbeat"></i></td>
                            <td><i class="fa fa-bed"></i></td>
                        </tr>
                        <tr>
                            <td><input type="checkbox" v-model="selectedPeriod.monday" v-bind:title="$t('monday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.tuesday" v-bind:title="$t('tuesday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.wednesday" v-bind:title="$t('wednesday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.thursday" v-bind:title="$t('thursday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.friday" v-bind:title="$t('friday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.saturday" v-bind:title="$t('saturday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.sunday" v-bind:title="$t('sunday')" /></td>
                            <td class="divider"></td>
                            <td><input type="checkbox" v-model="selectedPeriod.exerciseDay" v-bind:title="$t('exerciseDay')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.restDay" v-bind:title="$t('restDay')" /></td>
                        </tr>
                    </table>
                    
                    <table>
                        <tbody>
                            <tr>
                                <td><input type="radio" :value="true" v-model="selectedPeriod.wholeDay" /></td>
                                <td>{{ $t('wholeDay') }}</td>
                            </tr>
                            <tr>
                                <td><input type="radio" :value="false" v-model="selectedPeriod.wholeDay" /></td>
                                <td>{{ $t('perMeal') }}</td>
                            </tr>
                            <template v-if="!selectedPeriod.wholeDay">
                            <tr>
                                <td colspan="2"><label>{{ $t('onlyMeals') }}</label></td>
                            </tr>
                            <tr v-for="mealdef in $mealDefinitions">
                                <td><input type="checkbox" v-model="selectedPeriod.mealDefinitions[mealdef.id]"/></td>
                                <td>{{ mealdef.name }}</td>
                            </tr>
                            </template>
                        </tbody>
                    </table>
                     <table class="nutrient-goals">
                        <tbody v-for="group in $nutrientGroups">
                            <tr>
                                <th class="clickable" @click="toggleGroup(group.id)">
                                    <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                    <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                    {{ $t(group.id) }}
                                </th>
                                <td>&nbsp;</td>
                            </tr>
                            <tr v-for="nutrient in nutrients[group.id]" v-if="groupOpenStates[group.id]">
                                <td><span class="name">{{ nutrient.name }}</span> <span class="unit">{{ unit(nutrient.unit) }}</span></td>
                                <td><input type="number" class="form-control input-4" v-model="selectedPeriod.nutrients[nutrient.id].min" /> - <input type="number" class="form-control input-4" v-model="selectedPeriod.nutrients[nutrient.id].max" /></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="periods.length == 0">
              <div class="col-sm-12">
                <br />
                {{ $t("noPeriods") }}
              </div>
            </div>
            <hr />
            <div class="row" v-if="periods.length > 0">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t('save') }}</button>
                </div>
            </div>
          
            <div class="row">
                <div class="col-sm-12">
                    <p>
                        {{ $t('nutritionGoalsInfo') }}
                    </p>
                </div>
            </div>
        </section>
        
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import utils from '../../utils'
    import api from '../../api'
    import formatters from '../../formatters'
    import toaster from '../../toaster'
    import DAYS from '../../enums/days'
    import nutrientsMixin from '../../mixins/nutrients'
    import nutrientGroupsMixin from '../../mixins/nutrient-groups'
    import mealDefinitionsMixin from '../../mixins/meal-definitions'

export default {
    mixins:[nutrientsMixin, nutrientGroupsMixin, mealDefinitionsMixin],
    data () {
        return {
            id: undefined,
            name: undefined,
            periods: [],
            groupOpenStates: {},
            selectedPeriod: undefined
        }
    },
    computed: {
        nutrients() {
            return this.$store.state.nutrition.nutrientsGrouped;
        }
    },
    components: {},
    methods: {
        $nutrientsLoaded() {
            var self = this;
            var id = self.$route.params.id;
            if (id == constants.NEW_ID) {
                self.populate({ id: undefined, name: undefined, periods: [] });
                self.addPeriod();
                self.$store.commit(constants.LOADING_DONE);
            }
            else {
                self.$store.dispatch(constants.FETCH_NUTRITION_GOAL, {
                    id,
                    success(goal) {
                        self.populate(goal);
                        self.$store.commit(constants.LOADING_DONE);
                    },
                    failure() {
                        toaster.error(self.$t('routineDetails.fetchFailed'));
                    }
                });
            }
            self.toggleGroup(self.$nutrientGroups[0].id);
        },
        addPeriod() {
            var period = {
                monday: false,
                tuesday: false,
                wednesday: false,
                thursday: false,
                friday: false,
                saturday: false,
                sunday: false,
                exerciseDay: false,
                restDay: false,
                wholeDay: true,
                mealDefinitions: {},
                nutrients: {}
            };
            for (var i in this.nutrients) {
                for (var j in this.nutrients[i]) {
                    period.nutrients[this.nutrients[i][j].id] = { min: undefined, max: undefined };
                }
            }

            this.periods.push(period);
            this.selectedPeriod = period;
        },
        deletePeriod(period) {
            var self = this;
            self.periods.splice(self.periods.findIndex(p => p == self.selectedPeriod), 1);
            self.selectedPeriod = undefined;
        },
        toggleGroup(group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        groupIsExpanded(group) {
            return this.groupOpenStates[group] && true;
        },
        save() {
            var self = this;
            var goal = {
                id: self.id,
                name: self.name,
                periods: []
            };
            
            for (var i in self.periods) {
                var period = self.periods[i];
                var mealDefinitions = [];
                for (var id in period.mealDefinitions) {
                    if (period.mealDefinitions[id]) {
                        mealDefinitions.push(id);
                    }
                }
                var nutrients = [];
                for (var id in period.nutrients) {
                    if (period.nutrients[id].min || period.nutrients[id].max || period.nutrients[id].min == 0 || period.nutrients[id].max == 0) {
                        nutrients.push({ nutrientId: id, min: period.nutrients[id].min, max: period.nutrients[id].max });
                    }
                }
                goal.periods.push({
                    id: period.id,
                    monday: period.monday,
                    tuesday: period.tuesday,
                    wednesday: period.wednesday,
                    thursday: period.thursday,
                    friday: period.friday,
                    saturday: period.saturday,
                    sunday: period.sunday,
                    exerciseDay: period.exerciseDay,
                    restDay: period.restDay,
                    wholeDay: period.wholeDay,
                    mealDefinitions,
                    nutrients
                });
            }

            self.$store.dispatch(constants.SAVE_NUTRITION_GOAL, {
                goal,
                success() {
                    toaster.info(self.$t('nutrientGoals.saved'));
                },
                failure() {
                    toaster.error(self.$t('nutrientGoals.saveFailed'));
                }
            });
        },
        selectPeriod(period) {
            this.selectedPeriod = period;
        },
        daysFormatted(goal) {
            var text = '';
            var count = 0;
            var days = 0;
            if (goal.monday) {
                text += ' ' + this.$t("mondayAbbr");
                count++;
                days |= DAYS.MONDAY;
            }
            if (goal.tuesday) {
                text += ' ' + this.$t("tuesdayAbbr");
                count++;
                days |= DAYS.TUESDAY;
            }
            if (goal.wednesday) {
                text += ' ' + this.$t("wednesdayAbbr");
                count++;
                days |= DAYS.WEDNESDAY;
            }
            if (goal.thursday) {
                text += ' ' + this.$t("thursdayAbbr");
                count++;
                days |= DAYS.THURSDAY;
            }
            if (goal.friday) {
                text += ' ' + this.$t("fridayAbbr");
                count++;
                days |= DAYS.FRIDAY;
            }
            if (goal.saturday) {
                text += ' ' + this.$t("saturdayAbbr");
                count++;
                days |= DAYS.SATURDAY;
            }
            if (goal.sunday) {
                text += ' ' + this.$t("sundayAbbr");
                count++;
                days |= DAYS.SUNDAY;
            }
            if (goal.exerciseDay) {
                text += ' ' + this.$t("exerciseDay");
                count++;
                days |= DAYS.EXERCISEDAY;
            }
            if (goal.restDay) {
                text += ' ' + this.$t("restDay");
                count++;
                days |= DAYS.RESTDAY;
            }
            if (days == DAYS.WEEKDAYS) {
                return this.$t('weekdays');
            }
            if (days == DAYS.WEEKEND) {
                 return this.$t('weekends');
            }
            if ((days & DAYS.WEEK) == DAYS.WEEK) {
                return this.$t('everyDay');
            }
            if (((days & DAYS.EXERCISEDAY) == DAYS.EXERCISEDAY) && ((days & DAYS.RESTDAY) == DAYS.RESTDAY)) {
                return this.$t('everyDay');
            }
            if (count == 0 || count == 9) {
                return this.$t('everyDay');
            }
            return text;
        },
        mealsFormatted(period) {
            var self = this;
            if (period.wholeDay) {
                return this.$t('wholeDay');
            }
            var text = '';
            var count = 0;
            for (var id in period.mealDefinitions) {
                if (period.mealDefinitions[id]) {
                    var mealdef = self.$mealDefinitions.find(d => d.id == id);
                    text += ', ' + mealdef.name;
                    count++;
                }
            }
            if (text.length == 0 || count == self.$mealDefinitions.length) {
                return this.$t('everyMeal');
            }
            return text.substr(1);
        },
        unit: formatters.formatUnit,
        populate(goal) {
            var self = this;
            self.id = goal.id;
            self.name = goal.name;
            if (goal.periods) {
                goal.periods.forEach(period => {
                    var mealDefs = period.mealDefinitions;
                    period.mealDefinitions = {};
                    mealDefs.forEach(defId => {
                        period.mealDefinitions[defId] = true;
                    });

                    var nutrients = period.nutrients;
                    period.nutrients = {};

                    self.$nutrients.forEach(nutrient => {
                        var value = nutrients.find(n => n.nutrientId == nutrient.id);
                        if (value) {
                            period.nutrients[nutrient.id] = { min: value.min, max: value.max };
                        }
                        else {
                            period.nutrients[nutrient.id] = { min: undefined, max: undefined };
                        }
                    });
                });
      
                self.periods = goal.periods;
            }
            else {
                self.periods = [];
            }
        }
    },
    created() {
        
    }
}
</script>

<style scoped>
    div.selected {
        border: 1px solid #00c0ef;
    }
    table.nutrient-goals{
        width: auto;
        table-layout: fixed; 
    }
    table.nutrient-goals input 
    {
        padding-left: 6px;
        padding-right: 2px;
    }
    table.nutrient-goals thead > tr > th {
        text-align:center;
    }
    table.nutrient-goals thead > tr > th:nth-child(n+2),
    table.nutrient-goals tbody > tr > th:nth-child(n+2), 
    table.nutrient-goals tbody > tr >  td:nth-child(n+2){
        height: 20px;
        border-right: 1px solid black;
        border-left: 1px solid black;
    }
    table.days td{text-align:center;}
    table.days td.divider{width: 2px; border-left:1px solid black;}
    table input[type=checkbox], table input[type=radio]{ width: 25px; height: 25px;}
    input.form-control { display:initial;}
</style>