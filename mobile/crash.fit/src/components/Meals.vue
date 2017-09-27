<template>
  <div>
    
  <q-card>
      <q-card-title>
        <div class="row">
          <div class="col col-lg-2"><q-btn round  @click="changeDate(-1)"><q-icon name="fa-chevron-left" /></q-btn></div>
          <div class="col col-lg-2">
            <q-datetime v-model="selectedDate" type="date" :monday-first="true" :no-clear="true" :ok-label="$t('ok')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" @change="changeDate" @blur="datepickerVisible=false;" ref="datepicker" v-show="datepickerVisible" />
            <q-btn round :flat="true" @click="()=> {datepickerVisible=true; $refs.datepicker.open();}">{{ dateText }}</q-btn>
            </div>
          <div class="col col-lg-2"><q-btn round  @click="changeDate(1)"><q-icon name="fa-chevron-right" /></q-btn></div>
          <q-btn round class="pull-right" icon="fa-gear" @click="showMealSettings"></q-btn>
          
        </div>
      </q-card-title>
      <q-card-separator />
      <q-card-main>
        <div class="row">
          <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index">{{ nutrient.shortName }}</div>
        </div>
        <div class="row">
          <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index">
            <div v-if="nutrient.id == energyDistributionId" v-show="nutrients[proteinId] && nutrients[carbId] && nutrients[fatId]">
                <energy-distribution v-bind:protein="nutrients[proteinId]" v-bind:carb="nutrients[carbId]" v-bind:fat="nutrients[fatId]"></energy-distribution>
            </div>
            <div v-else>{{ decimal(nutrients[nutrient.id]) }}</div>
        </div>
        </div>
      </q-card-main>
    </q-card>
    <q-scroll-area style="height: 70vh;">
      <q-card v-for="(mealdef, index) in meals" :key="index">
        <q-card-title>
          <div class="row">{{ mealName(mealdef) }}</div>
          <div class="row" v-if="mealdef.meal">
            <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index">
              <div v-if="nutrient.id == energyDistributionId">
                  <energy-distribution v-bind:protein="mealdef.meal.nutrients[proteinId]" v-bind:carb="mealdef.meal.nutrients[carbId]" v-bind:fat="mealdef.meal.nutrients[fatId]"></energy-distribution>
              </div>
              <div v-else>{{ decimal(mealdef.meal.nutrients[nutrient.id]) }}</div>
              </div>
          </div>
        </q-card-title>
        <q-card-separator />
        <q-card-main v-if="mealdef.meal">
          <div v-for="(row,index) in mealdef.meal.rows" @click="editRow(row)" :key="index">
            <div class="row food-portion">
              <div class="col">{{ row.foodName }} {{ row.quantity }} {{ row.portionName || 'g' }}</div>  
            </div>
            <div class="row nutrients">
              <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index">
                <div v-if="nutrient.id == energyDistributionId">
                  <energy-distribution v-bind:protein="row.nutrients[proteinId]" v-bind:carb="row.nutrients[carbId]" v-bind:fat="row.nutrients[fatId]"></energy-distribution>
                </div>
                <div v-else>{{ decimal(row.nutrients[nutrient.id]) }}</div>
                </div>
            </div>
            <q-btn round class="float-right" color="primary" style="top: -55px; right:-10px;" small icon="fa-trash" v-on:click.stop="deleteRow(mealdef,row)"></q-btn>
          </div>
        </q-card-main>
        <q-card-actions>
          <q-btn round color="primary" icon="fa-plus" small @click="addRow(mealdef)"></q-btn>
        </q-card-actions>
      </q-card>
    </q-scroll-area>
    <edit-row ref="editRow" v-on:save="saveRow(arguments[0])" />
    <meal-settings ref="mealSettings" v-on:save="saveSettings(arguments[0])" />
  </div>
</template>

<script>
import { openURL } from 'quasar'
import moment from 'moment'
import constants from '../store/constants'
import formatters from '../formatters'
import { QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator,QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem,QDatetime } from 'quasar'
export default {
  components: {
    QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator, QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem, QDatetime,
    'energy-distribution': require('./energy-distribution-bar'),
    'edit-row': require('./edit-meal-row'),
    'meal-settings': require('./meal-settings')
  },
  data () {
    return {
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
            this.fetchMeals();
        }
    },
    fetchMeals() {
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
    editRow(row){
      this.selectedRow = row;
      this.$refs.editRow.open(this.selectedRow);
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
    }
  },
  created(){
    var self = this;
    self.$store.dispatch(constants.FETCH_NUTRIENTS, {
        success: function () { },
        failure: function () { }
    });
    self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
        success: function () {
            self.fetchMeals();
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
