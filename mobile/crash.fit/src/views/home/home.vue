<template>
  <div v-touch-swipe.horizontal="swipe">
    <q-pull-to-refresh :handler="refresh" :pull-message="$t('')" :release-message="$t('')" :refresh-message="$t('')" :distance="20" style="height: 125px;">

  
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
          <q-btn round small glossy color="grey-6" class="pull-right" icon="fa-gear" @click="showMealSettings"></q-btn>
          
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
     
      </q-pull-to-refresh>

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
          <div v-for="(row,index) in mealdef.meal.rows" @click="clickRow(mealdef, row)" v-touch-hold="x => clickRow(mealdef, row)" :key="index">
            <div class="row food-portion">
              <div class="col">{{ row.foodName }} {{ row.quantity }} {{ row.portionName || 'g' }}</div>  
            </div>
            <div class="row nutrients">
              <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="index" align="center">
                <div v-if="nutrient.id == energyDistributionId">
                  <energy-distribution-bar v-bind:protein="row.nutrients[proteinId]" v-bind:carb="row.nutrients[carbId]" v-bind:fat="row.nutrients[fatId]"></energy-distribution-bar>
                </div>
                <div v-else>
                  {{ formatDecimal(row.nutrients[nutrient.id]) }}
                </div>
              </div>
            </div>
            <!--
            <q-btn round class="float-right" color="primary" style="top: -55px; right:-10px;" small icon="keyboard_arrow_up" v-on:click.stop="showFab=true"></q-btn>
            -->
          </div>
        </q-card-main>
        <q-card-actions align="end">
          <q-btn v-if="rowCopy" round glossy color="secondary" icon="fa-paste" small @click="pasteRows(mealdef)"></q-btn>
          <q-btn v-if="mealCopy" round glossy color="secondary" icon="fa-paste" small @click="pasteMeal(mealdef)"></q-btn>
          <q-btn glossy color="primary" icon="fa-plus" small @click="addRow(mealdef)">{{ $t('food') }}</q-btn>
        </q-card-actions>
      </q-card>
    </q-scroll-area>
  
    <meal-row-editor ref="editRow" @save="saveRow(arguments[0])" />
    <meal-settings ref="mealSettings" @save="saveSettings(arguments[0])" />
  </div>
</template>

<script src="./home.js">
</script>

<style lang="stylus">
.row.nutrients{ padding-bottom: 20px;}
</style>
