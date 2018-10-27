<template>
<layout >

  <span slot="title">{{ $t('meals') }}</span>

  <div slot="toolbar">
    
  </div>

  <q-page class="q-pa-sm">
    <div class="row">
      <div class="col-4 q-pt-sm">
        <q-datetime :value="start" type="date" @input="val => changeStart(val)" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames"  />
      </div>
      <div class="col-1 q-pt-sm" style="text-align: center;"><q-icon name="fas fa-minus" /></div>
      <div class="col-4 q-pt-sm">
        <q-datetime :value="end" type="date" @input="val => changeEnd(val)" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" />
      </div>
      <div class="col-3 q-pt-md q-pl-sm" style="text-align: center;">
        <q-btn-dropdown glossy size="sm" color="primary" icon="fas fa-calendar" style="margin-top: -5px;">
            <q-list>
              <q-item @click.native="showDays(7)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>1 {{ $t('week') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showDays(14)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>2 {{ $t('weeks') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(1)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>1 {{ $t('month') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(2)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>2 {{ $t('months') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(3)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>3 {{ $t('months') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(6)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>6 {{ $t('months') }}</q-item-tile>
                </q-item-main>
              </q-item>
            </q-list>
          </q-btn-dropdown>
      </div>
    </div>
    <div class="row q-my-lg">
      <q-btn-group>
        <template v-for="group in groups">
          <q-btn glossy color="primary" :label="$t(group.id)" @click="selectGroup(group.id)" v-if="group.id == selectedGroup" />
          <q-btn glossy :label="$t(group.id)" @click="selectGroup(group.id)" v-else />
        </template>
        
      </q-btn-group>
    </div>
    <div class="row">
      <div class="outer" v-if="days.length > 0">
        <div class="inner">
            <table class="table" id="meal-list">
                <thead>
                    <tr>
                        <th></th>
                        <template v-for="col in visibleColumns">
                            <th class="nutrient" v-if="!col.hideSummary"><div><input type="checkbox" v-if="editNutrients" /><div>{{ col.title }}</div></div></th>
                        </template>
                        <th></th>
                    </tr>
                    <tr>
                        <th class="time freeze"></th>
                        <template v-for="col in visibleColumns">
                            <th class="unit" v-if="!col.hideSummary">{{ formatUnit(col.unit) }}</th>
                        </template>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <template v-for="day in days">
                        <tr class="day" @click="toggleDay(day)">
                            <td class="freeze text-weight-bold">{{ formatDate(day.date) }}
                                <!--
                                <q-icon name="fas fa-chevron-down" v-if="!dayStates[day.date.getTime()]"></q-icon>
                                <q-icon name="fas fa-chevron-up" v-if="dayStates[day.date.getTime()]"></q-icon>
                                -->
                            </td>
                            <template v-for="col in visibleColumns">
                                <td class="nutrient" v-if="!col.hideSummary">
                                    <div class="chart" v-if="col.key === energyDistributionId">
                                        <energy-distribution-bar :protein="day.nutrients[proteinId]" :carb="day.nutrients[carbId]" :fat="day.nutrients[fatId]"></energy-distribution-bar>
                                    </div>
                                    <div v-else>
                                        <nutrient-bar :goal="nutrientGoal(col.key, day.date)" :value="day.nutrients[col.key]" :precision="col.precision"></nutrient-bar>
                                    </div>
                                </td>
                            </template>
                            <td></td>
                        </tr>
                        <tr class="meal" v-if="dayStates[day.date.getTime()]" v-for="meal in day.meals">
                            <td class="freeze meal-name">{{ mealName(meal) }}</td>
                            <template v-for="col in visibleColumns">
                                <td class="nutrient" v-if="!col.hideSummary">
                                    <div class="chart" v-if="col.key === energyDistributionId">
                                        <energy-distribution-bar :protein="meal.nutrients[proteinId]" :carb="meal.nutrients[carbId]" :fat="meal.nutrients[fatId]"></energy-distribution-bar>
                                    </div>
                                    <div v-else>
                                        <nutrient-bar :goal="nutrientGoal(col.key, day.date, meal)" :value="meal.nutrients[col.key]" :precision="col.precision"></nutrient-bar>
                                    </div>
                                </td>
                            </template>
                            <td></td>
                        </tr>
                    </template>
                </tbody>
            </table>
        </div>
    </div>
    <div v-if="days.length == 0">
      {{ $t('noMeals') }}
    </div>
    </div>
  </q-page>
  </layout>
</template>

<script src="./meals.js">
</script>

<style>
  .outer {
      position: relative;
      width: 100%;
  }

  .inner {
      overflow-x: auto;
      overflow-y: visible;
      margin-left: 100px;
  }
    /*
    #meal-list {
        width: auto;
        table-layout: fixed;
        width: 100%;
    }
    */
  #meal-list td {
      padding-bottom: 0px;
  }

  #meal-list td span {
      margin: 5px;
  }

  .freeze {
      position: absolute;
      margin-left: -100px;
      width: 100px;
      text-align: right;
  }

  th.time {
      top: 112px;
      border-width: 0px;
      width: 100px;
  }

  #meal-list a {
      cursor: pointer;
  }

  th.nutrient {
      height: 120px;
      white-space: nowrap;
      width: 60px;
  }

  th.nutrient > div {
      transform: translate(41px, 30px) rotate(-45deg);
      width: 60px;
  }

  th.nutrient > div > div {
      border-bottom: 1px solid #ccc;
      padding: 5px 10px;
      width: 100px;
  }

  th.nutrient:nth-child(2) > div {
      transform: translate(41px, 30px) rotate(-45deg);
      width: 60px;
  }

  th.unit {
      padding: 0px;
      text-align: center;
      border-right: 1px solid #ccc;
  }

  td.nutrient {
      padding-left: 1px;
      padding-right: 1px;
      border-right: 1px solid #ccc;
      text-align: center;
      width: 60px;
      overflow: visible;
  }
  /*
  tr.day td {
    border-top: 1px solid #ccc;
  }*/
  .meal-name{
    padding-left:10px;
  }
</style>
