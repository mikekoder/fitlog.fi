<template>
  <div>
    <template v-for="(goal,index) in goals">
    <q-card :key="index">
      <q-card-title>
        {{ daysFormatted(goal) }}
      </q-card-title>
      <q-card-separator />
      <q-card-main>
        {{ mealsFormatted(goal) }}
      </q-card-main>
    </q-card>
    </template>
  </div>
</template>

<script>
import constants from '../../store/constants'
import DAYS from '../../enums/days'
import {QCard,QCardTitle,QCardSeparator,QCardMain } from 'quasar'
export default {
  components: {QCard,QCardTitle,QCardSeparator,QCardMain},
  data () {
    return {
      goals:[]
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
                        self.goals = nutrientGoals;

                        self.$store.commit(constants.LOADING_DONE);
                    },
                    failure() { }
                });
            },
            failure() { }
        });
        
        //this.toggleGroup(this.groups[0].id);
    }
}
</script>

<style lang="stylus">
</style>
