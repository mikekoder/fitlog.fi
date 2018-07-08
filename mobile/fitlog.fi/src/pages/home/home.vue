<template>
<layout>

  <span slot="title">{{ $t('diary') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="help" @click="showHelp"></q-btn>
    <q-btn-dropdown flat icon="fa-cogs" id="home-options">
        <q-list>
          <q-item @click.native="showMealSettings" v-close-overlay>
            <q-item-main>
              <q-item-tile label>{{ $t('nutrients') }}</q-item-tile>
            </q-item-main>
          </q-item>
          <q-item-separator />
          <q-list-header v-if="$activityPresets.length > 0">{{ $t('activityLevel') }}</q-list-header>
          <q-item v-for="preset in $activityPresets" @click.native="changeActivityPreset(preset)" :class="{selected: activityPreset && activityPreset.id == preset.id }" v-close-overlay>
            <q-item-main>
              <q-item-tile label>{{ preset.name }}</q-item-tile>
            </q-item-main>
          </q-item>
        </q-list>
      </q-btn-dropdown>
  </div>

  <q-page v-touch-swipe.horizontal="swipe">
    <q-pull-to-refresh :handler="refresh" :pull-message="$t('')" :release-message="$t('')" :refresh-message="$t('')" :distance="20" style="height: 175px;">
      <q-card>
        <q-card-title class=" bg-grey-4">
          <div class="row">
            <div class="col col-lg-2" align="right">
              <q-btn round size="md" glossy color="grey-6" @click="changeDate(-1)"><q-icon name="fa-chevron-left" /></q-btn>
            </div>
            <div class="col col-lg-2" align="center">
              <q-datetime :value="selectedDate" type="date" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" @change="changeDate" @blur="datepickerVisible=false;" ref="datepicker" v-show="datepickerVisible" />
              <q-btn :flat="true" @click="()=> {datepickerVisible=true; $refs.datepicker.show();}" :label="dateText"></q-btn>
              </div>
            <div class="col col-lg-2" align="left">
              <q-btn round size="md" glossy color="grey-6" @click="changeDate(1)"><q-icon name="fa-chevron-right" /></q-btn>
            </div>
          </div>
        </q-card-title>
        <q-card-separator />
        <q-card-main>
          <div class="row">
              <div class="col">
                <table style="width: 100%">
                  <tr>
                    <td>{{ $t('eaten') }}</td>
                    <td></td>
                    <td>{{ $t('rmrAbbr') }}</td>
                    <td></td>
                    <td>{{ $t('activity2') }}</td>
                    <td></td>
                    <td>{{ $t('expenditure') }}</td>
                    <td></td>
                    <td>{{ $t('total') }}</td>
                    <td></td>
                  </tr>
                  <tr>
                    <td>{{ formatDecimal(eatenEnergy) }}</td>
                    <td>-</td>
                    <td>{{ formatDecimal(rmr) }}</td>
                    <td>-</td>
                    <td>{{ formatDecimal(activityLevelEnergy) }}</td>
                    <td>-</td>
                    <td>{{ formatDecimal(energyExpenditure) }} </td>
                    <td>=</td>
                    <td>{{ formatDecimal(totalEnergy) }}</td>
                    <td>{{ formatUnit('KCAL') }}</td>
                  </tr>
                </table>
              </div>
          </div>
          <hr />
          <div class="row">
            <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="'nutrient'+index" align="center">{{ nutrient.shortName }}</div>
          </div>
          <div class="row">
            <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="'day'+index"  align="center">
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
      <q-card v-for="(mealdef, index) in meals" :key="index" class="q-mb-sm">
        <q-card-title class="card-title bg-grey-3" @click.native="clickMeal(mealdef)" v-touch-hold="x => clickMeal(mealdef)">
          <div class="row text-weight-medium">{{ mealName(mealdef) }}</div>
          <div class="row" v-if="mealdef.meal">
            <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="'meal'+index" align="center">
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
              <div class="col">{{ row.foodName }} <span class="text-weight-light">{{ row.quantity }} {{ row.portionName || 'g' }}</span></div>  
            </div>
            <div class="row nutrients">
              <div class="col" v-for="(nutrient,index) in visibleNutrients" :key="'row'+index" align="center">
                <div v-if="nutrient.id == energyDistributionId">
                  <energy-distribution-bar v-bind:protein="row.nutrients[proteinId]" v-bind:carb="row.nutrients[carbId]" v-bind:fat="row.nutrients[fatId]"></energy-distribution-bar>
                </div>
                <div v-else class="text-weight-light">
                  {{ formatDecimal(row.nutrients[nutrient.id], nutrient.precision) }}
                </div>
              </div>
            </div>
          </div>
        </q-card-main>
        <q-card-actions align="end">
          <q-btn v-if="rowCopy" round glossy color="secondary" icon="fa-paste" size="sm" @click="pasteRows(mealdef)"></q-btn>
          <q-btn v-if="mealCopy" round glossy color="secondary" icon="fa-paste" size="sm" @click="pasteMeal(mealdef)"></q-btn>
          <q-btn glossy color="primary" icon="fa-plus" size="sm" @click="addRow(mealdef)" :label="$t('food')"></q-btn>
        </q-card-actions>
      </q-card>
    </q-scroll-area>
  
    <meal-row-editor ref="editRow" @save="saveRow(arguments[0])" />
    <meal-settings ref="mealSettings" @save="saveSettings(arguments[0])" />
    <home-help ref="help" />
  </q-page>
  </layout>
</template>

<script src="./home.js">
</script>

<style lang="stylus">
.q-card-title {
    font-size: 14px;
    line-height: normal;
    /*
    font-weight: 400;
    letter-spacing: normal;
    line-height: 2rem;
    */
}
#home-options .q-btn-dropdown-arrow {
  display:none;
}
</style>
