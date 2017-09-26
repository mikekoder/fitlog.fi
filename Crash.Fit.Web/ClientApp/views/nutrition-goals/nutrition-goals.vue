<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutritionGoals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="addNutrientGoal">{{ $t("addGoal") }}</button>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-xs-4 col-sm-3 col-md-2 master">
                    <div class="box box-solid" v-for="goal in nutrientGoals" v-bind:class="{ selected: goal == selectedGoal }" @click="selectGoal(goal)">
                        <div class="box-header with-border">
                            {{ daysFormatted(goal) }}
                        </div>
                        <div class="box-body">
                            {{ mealsFormatted(goal) }}
                        </div>
                    </div>
                </div>
                <div class="col-xs-8" v-if="selectedGoal">
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
                            <td><input type="checkbox" v-model="selectedGoal.monday" v-bind:title="$t('monday')" /></td>
                            <td><input type="checkbox" v-model="selectedGoal.tuesday" v-bind:title="$t('tuesday')" /></td>
                            <td><input type="checkbox" v-model="selectedGoal.wednesday" v-bind:title="$t('wednesday')" /></td>
                            <td><input type="checkbox" v-model="selectedGoal.thursday" v-bind:title="$t('thursday')" /></td>
                            <td><input type="checkbox" v-model="selectedGoal.friday" v-bind:title="$t('friday')" /></td>
                            <td><input type="checkbox" v-model="selectedGoal.saturday" v-bind:title="$t('saturday')" /></td>
                            <td><input type="checkbox" v-model="selectedGoal.sunday" v-bind:title="$t('sunday')" /></td>
                            <td class="divider"></td>
                            <td><input type="checkbox" v-model="selectedGoal.exerciseDay" v-bind:title="$t('exerciseDay')" /></td>
                            <td><input type="checkbox" v-model="selectedGoal.restDay" v-bind:title="$t('restDay')" /></td>
                        </tr>
                    </table>

                     <table class="nutrient-goals">
                        <tbody v-for="group in groups">
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
                                <td><input type="number" class="form-control input-4" v-model="selectedGoal.nutrientValues[nutrient.id].min" /> - <input type="number" class="form-control input-4" v-model="selectedGoal.nutrientValues[nutrient.id].max" /></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="nutrientGoals.length == 0">
              <div class="col-sm-12">
                <br />
                {{ $t("noNutrientGoals") }}
              </div>
            </div>
            <hr />
            <div class="row" v-if="nutrientGoals.length > 0">
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
export default {
    data () {
        return {
            //nutrients: [],
            nutrientGoals: [],
            groupOpenStates: {},
            selectedGoal: undefined
        }
    },
    computed: {
        groups(){
            return this.$store.state.nutrition.nutrientGroups;
        },
        nutrients() {
            return this.$store.state.nutrition.nutrientsGrouped;
        }
    },
    components: {},
    methods: {
        addNutrientGoal() {
            var goal = {
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
                    goal.nutrientValues[this.nutrients[i][j].id] = { min: undefined, max: undefined };
                }
            }

            this.nutrientGoals.push(goal);
            this.selectedGoal = goal;
        },
        deleteNutrientGoal(index){
            this.nutrientGoals.splice(index, 1);
        },
        toggleGroup(group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        groupIsExpanded(group) {
            return this.groupOpenStates[group] && true;
        },
        save() {
            var self = this;
            var goals = [];
            for (var i in self.nutrientGoals) {
                var days = self.nutrientGoals[i];
                var goal = {
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
                        goal.nutrientValues.push({ nutrientId: j, min: value.min, max: value.max });
                    }
                }
                goals.push(goal);
            }
            self.$store.dispatch(constants.SAVE_NUTRITION_GOALS, {
                goals,
                success() {
                    toaster.info(self.$t('nutrientGoals.saved'));
                },
                failure() {
                    toaster.error(self.$t('nutrientGoals.saveFailed'));
                }
            });
        },
        selectGoal(goal) {
            this.selectedGoal = goal;
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
        mealsFormatted(goal) {
            return 'Kaikilla aterioilla';
        },
        unit: formatters.formatUnit
    },
    created() {
        var self = this;
        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success() {
                self.$store.dispatch(constants.FETCH_NUTRITION_GOALS, {
                    success(goals) {
                        var nutrientGoals = [];
                        for (var i in goals) {
                            var goal = {
                                monday: goals[i].monday,
                                tuesday: goals[i].tuesday,
                                wednesday: goals[i].wednesday,
                                thursday: goals[i].thursday,
                                friday: goals[i].friday,
                                saturday: goals[i].saturday,
                                sunday: goals[i].sunday,
                                exerciseDay: goals[i].exerciseDay,
                                restDay: goals[i].restDay,
                                nutrientValues: {}
                            };
                            for (var g in self.nutrients) {
                                for (var n in self.nutrients[g]) {
                                    goal.nutrientValues[self.nutrients[g][n].id] = { min: undefined, max: undefined };
                                }
                            }
                            for (var j in goals[i].nutrientValues) {
                                var value = goals[i].nutrientValues[j];
                                goal.nutrientValues[value.nutrientId] = { min: value.min, max: value.max };
                            }
                            nutrientGoals.push(goal);
                        }
                        self.nutrientGoals = nutrientGoals;

                        self.$store.commit(constants.LOADING_DONE);
                    },
                    failure() { }
                });
            },
            failure() { }
        });
        
        this.toggleGroup(this.groups[0].id);
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
    table.days input[type=checkbox]{ width: 25px; height: 25px;}
    input.form-control { display:initial;}
</style>