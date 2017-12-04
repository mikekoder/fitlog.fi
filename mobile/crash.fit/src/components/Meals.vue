<template>
  <div>
    
  <q-card>
      <q-card-title class="card-title bg-grey-4">
        <div class="row">
          <div class="col col-lg-2" align="right">
            <q-btn round small glossy color="grey-6" @click="changeDate(-1)"><q-icon name="fa-chevron-left" /></q-btn>
          </div>
          <div class="col col-lg-2" align="center">
            <q-datetime v-model="selectedDate" type="date" :monday-first="true" :no-clear="true" :ok-label="$t('ok')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" @change="changeDate" @blur="datepickerVisible=false;" ref="datepicker" v-show="datepickerVisible" />
            <q-btn round small :flat="true" @click="()=> {datepickerVisible=true; $refs.datepicker.open();}">{{ dateText }}</q-btn>
            </div>
          <div class="col col-lg-2" align="left">
            <q-btn round small glossy color="grey-6" @click="changeDate(1)"><q-icon name="fa-chevron-right" /></q-btn>
          </div>
          <q-btn round small class="pull-right" icon="fa-gear" @click="showMealSettings"></q-btn>
          
        </div>
      </q-card-title>
      <q-card-separator />
      <q-card-main>
        <div class="row">
          <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index" align="center">{{ nutrient.shortName }}</div>
        </div>
        <div class="row">
          <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index"  align="center">
            <div v-if="nutrient.id == energyDistributionId" v-show="nutrients[proteinId] && nutrients[carbId] && nutrients[fatId]">
                <energy-distribution-bar v-bind:protein="nutrients[proteinId]" v-bind:carb="nutrients[carbId]" v-bind:fat="nutrients[fatId]"></energy-distribution-bar>
            </div>
            <div v-else>
              <nutrient-bar :goal="nutrientGoal(nutrient.id)" :value="nutrients[nutrient.id]" :precision="nutrient.precision"></nutrient-bar>
            </div>
        </div>
        </div>
      </q-card-main>
    </q-card>
    <q-scroll-area style="height: 70vh;">
      <q-card v-for="(mealdef, index) in meals" :key="index">
        <q-card-title class="card-title bg-grey-3">
          <div class="row">{{ mealName(mealdef) }}</div>
          <div class="row" v-if="mealdef.meal">
            <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index" align="center">
              <div v-if="nutrient.id == energyDistributionId">
                  <energy-distribution-bar v-bind:protein="mealdef.meal.nutrients[proteinId]" v-bind:carb="mealdef.meal.nutrients[carbId]" v-bind:fat="mealdef.meal.nutrients[fatId]"></energy-distribution-bar>
              </div>
              <div v-else>
                <nutrient-bar :goal="nutrientGoal(nutrient.id, mealdef.meal)" :value="mealdef.meal.nutrients[nutrient.id]" :precision="nutrient.precision"></nutrient-bar>
              </div>
            </div>
          </div>
        </q-card-title>
        <q-card-separator />
        <q-card-main v-if="mealdef.meal">
          <div v-for="(row,index) in mealdef.meal.rows" @click="clickRow(row)" :key="index">
            <div class="row food-portion">
              <div class="col">{{ row.foodName }} {{ row.quantity }} {{ row.portionName || 'g' }}</div>  
            </div>
            <div class="row nutrients">
              <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index" align="center">
                <div v-if="nutrient.id == energyDistributionId">
                  <energy-distribution-bar v-bind:protein="row.nutrients[proteinId]" v-bind:carb="row.nutrients[carbId]" v-bind:fat="row.nutrients[fatId]"></energy-distribution-bar>
                </div>
                <div v-else>
                  {{ decimal(row.nutrients[nutrient.id]) }}
                </div>
              </div>
            </div>
            <!--
            <q-btn round class="float-right" color="primary" style="top: -55px; right:-10px;" small icon="keyboard_arrow_up" v-on:click.stop="showFab=true"></q-btn>
            -->
          </div>
        </q-card-main>
        <q-card-actions align="end">
          <q-btn round glossy color="primary" icon="fa-plus" small @click="addRow(mealdef)"></q-btn>
        </q-card-actions>
      </q-card>
    </q-scroll-area>
    <meal-row-editor ref="editRow" @save="saveRow(arguments[0])" />
    <meal-settings ref="mealSettings" @save="saveSettings(arguments[0])" />
  </div>
</template>

<script>
import { openURL } from 'quasar'
import moment from 'moment'
import constants from '../store/constants'
import formatters from '../formatters'
import { QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator,QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem,QDatetime, ActionSheet } from 'quasar'
import EnergyDistributionBar from './energy-distribution-bar'
import NutrientBar from './nutrient-bar'
import MealRowEditor from './meal-row-editor'
import MealSettings from './meal-settings'
import utils from '../utils'
import nutrientsMixin from '../mixins/nutrients'
import mealDefinitionsMixin from '../mixins/meal-definitions'
import nutritionGoalMixin from '../mixins/nutrition-goal'

export default {
  mixins: [nutrientsMixin, mealDefinitionsMixin, nutritionGoalMixin],
  components: {
    QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator, QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem, QDatetime,
    EnergyDistributionBar,
    NutrientBar,
    MealRowEditor,
    MealSettings
  },
  data () {
    return {
      showFab: false,
      proteinId: constants.PROTEIN_ID,
      carbId: constants.CARB_ID,
      fatId: constants.FAT_ID,
      energyId: constants.ENERGY_ID,
      energyDistributionId: constants.ENERGY_DISTRIBUTION_ID,
      selectedRow: undefined,
      datepickerVisible: false
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
    workouts() {
        var self = this;
        return this.$store.state.training.workouts.filter(w => moment(w.time).isSame(self.selectedDate, 'day'));
    },
    nutritionGoal() {
        return this.$store.state.nutrition.activeNutritionGoal;
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
    },
    mealCopy() {
        if (this.$store.state.clipboard.type == constants.MEAL) {
            return this.$store.state.clipboard.data;
        }
        return undefined;
    },
    rowCopy() {
        if (this.$store.state.clipboard.type == constants.MEAL_ROWS) {
            return this.$store.state.clipboard.data;
        }
        return undefined;
    }
  },
  methods: {
    changeDate(date) {
        this.datepickerVisible = false;
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
            this.fetchData();
        }
    },
    fetchData() {
      var self = this;
      var start = moment(self.selectedDate).startOf('day');
      var end = moment(self.selectedDate).endOf('day');
      self.$store.dispatch(constants.FETCH_MEALS, {
        start,
        end,
        success: function () {
          self.$store.commit(constants.LOADING_DONE);
        },
        failure: function () { }
      });
      self.$store.dispatch(constants.FETCH_WORKOUTS, { start: start, end: end });
    },
    mealName(defMeal) {
      if (defMeal.definition) {
          return defMeal.definition.name;
      }
      return this.time(defMeal.meal.time);
    },
    date: formatters.formatDate,
    time: formatters.formatTime,
    unit: formatters.formatUnit,
    decimal (value, precision) {
        if (!value) {
            return value;
        }
        return value.toFixed(precision);
    },
    addRow(mealdef){
      this.selectedRow = { 
        mealDefinitionId: mealdef.definition ? mealdef.definition.id : undefined,
        mealId: mealdef.meal ? mealdef.meal.id : undefined, 
        food: undefined, 
        quantity: undefined, 
        portion: undefined
      };
      this.$refs.editRow.open(this.selectedRow);
    },
    clickRow(row){
      var self = this;
      ActionSheet.create({
        title: `${row.foodName} ${ row.quantity } ${ row.portionName || 'g' }`,
        gallery: true,
        actions: [
          {
            label: self.$t('edit'),
            icon: 'fa-edit',
            handler: function() {
              self.editRow(row);
            }
          },
          {
            label: self.$t('copy'),
            icon: 'fa-copy',
            handler: function() {
              self.copyRow(row);
            }
          },
          {
            label: self.$t('delete'),
            icon: 'fa-trash',
            handler: function() {
              self.deleteRow(row);
            }
          }
        ]
      });
    },
    editRow(row){
      this.selectedRow = row;
      this.$refs.editRow.open(this.selectedRow);
    },
    copyMeal(meal) {
        this.$store.dispatch(constants.CLIPBOARD_COPY, {
            type: constants.MEAL,
            data: meal
        });
    },
    pasteMeal(mealDef) {
        var self = this;
        var meal = self.$store.state.clipboard.data;
        self.appendRows(mealDef, meal.rows);
    },
    copyRow(row) {
        this.$store.dispatch(constants.CLIPBOARD_COPY, {
            type: constants.MEAL_ROWS,
            data: [row]
        });
    },
    pasteRows(mealDef) {
        var self = this;
        var rows = self.$store.state.clipboard.data;
        self.appendRows(mealDef, rows);
    },
    appendRows(mealDef, rows) {
        var self = this;
        var meal = {
            id: mealDef.meal ? mealDef.meal.id : undefined,
            date: self.selectedDate,
            definitionId: mealDef.definition.id,
            rows : mealDef.meal && mealDef.meal.rows ? mealDef.meal.rows.map(r => { return { foodId: r.foodId, quantity: r.quantity, portionId: r.portionId }}) : []
        }
        rows.forEach(r => {
            meal.rows.push({ foodId: r.foodId, quantity: utils.parseFloat(r.quantity), portionId: r.portionId });
        });
        self.$store.dispatch(constants.SAVE_MEAL, {
            meal,
            success() { },
            failure() { }
        });
    },
    deleteRow(mealdef, row){
      this.$store.dispatch(constants.DELETE_MEAL_ROW, {
          row,
          success() { 
            if(mealdef.meal.rows.length == 0){
              mealdef.meal = undefined;
            }
          },
          failure() { }
      });
      return true;
    },
    saveRow(row){
      row.date = this.selectedDate;
      this.$store.dispatch(constants.SAVE_MEAL_ROW, {
          row,
          success() {},
          failure() { }
      });
      this.selectedRow = {};
      this.$refs.editRow.close();
    },
    showMealSettings(){
      this.$refs.mealSettings.open();
    },
    nutrientGoal(nutrientId, meal) {
      return utils.nutrientGoal(this.$nutritionGoal, this.workouts, nutrientId, this.selectedDate, meal);
    },
  },
  created(){
    var self = this;
    self.$store.dispatch(constants.FETCH_NUTRIENTS, {
        success: function () { },
        failure: function () { }
    });
    self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
        success: function () {
            self.fetchData();
        },
        failure: function () {
          self.$store.commit(constants.LOADING_DONE);
         }
    });
    self.$store.dispatch(constants.FETCH_LATEST_FOODS, {
        success() { },
        failure() { }
    });
    self.$store.dispatch(constants.FETCH_MOST_USED_FOODS, {
        success() { },
        failure() { }
    });
    self.$store.dispatch(constants.FETCH_MY_FOODS, {
        success() { },
        failure() { }
    });
    
  }
}
</script>

<style lang="stylus">
.row.nutrients{ padding-bottom: 20px;}
</style>
