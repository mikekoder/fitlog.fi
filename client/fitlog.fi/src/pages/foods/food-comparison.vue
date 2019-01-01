<template>
<layout >

  <span slot="title">{{ $t('foodComparison') }}</span>

  <div slot="toolbar">
    <!--
    <q-btn flat icon="fas fa-plus" @click="createFood" :label="$t('food')"></q-btn>
    -->
  </div>
  <q-page class="q-pa-sm">
    <div class="row">
      <div class="col">
        <q-btn-group>
          <template v-for="group in nutrientGroups">
            <q-btn glossy color="primary" :label="$t(group.id)" @click="selectGroup(group.id)" v-if="group.id == selectedGroup" />
            <q-btn glossy :label="$t(group.id)" @click="selectGroup(group.id)" v-else />
          </template>
        </q-btn-group>
      </div>
    </div>

    <div class="row" v-if="foods.length > 0">
      <div class="col">
        <div class="outer">
          <div class="inner">
            <table class="table">
              <thead>
                  <tr>
                    <th></th>
                    <template v-for="nutrient in visibleNutrients">
                      <th class="nutrient"><div><div>{{ nutrient.name }}</div></div></th>
                    </template>
                    <th></th>
                  </tr>
                  <tr>
                    <th class="time freeze"></th>
                    <template v-for="nutrient in visibleNutrients">
                        <th class="unit">{{ formatUnit(nutrient.unit) }}</th>
                    </template>
                    <th></th>
                  </tr>
              </thead>
              <tbody>
                <tr v-for="food in foods">
                  <td class="freeze text-weight-bold" :title="food.name">
                    <small>{{ food.name }}<span v-if="food.manufacturer" class="text-weight-light">({{ food.manufacturer }})</span></small>
                  </td>
                  <template v-for="nutrient in visibleNutrients">
                    <td class="nutrient">
                      <div class="chart" v-if="nutrient.id === energyDistributionId">
                        <energy-distribution-bar :protein="food.nutrients[proteinId]" :carb="food.nutrients[carbId]" :fat="food.nutrients[fatId]"></energy-distribution-bar>
                      </div>
                      <div class="text-weight-light" v-else>
                        <small v-if="food.nutrients[nutrient.id] >= 0">{{ formatDecimal(food.nutrients[nutrient.id], nutrient.precision)}}</small>
                        <small v-else>&nbsp;</small>
                      </div>
                    </td>
                  </template>
                  <td></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
    <div class="row q-mt-lg">
      <div class="col">
        <template v-for="(food, index) in foods">
          <q-chip closable @hide="deleteFood(index)">
            {{ food.name }}
            <span v-if="food.manufacturer" class="text-weight-light">&nbsp;({{ food.manufacturer }})</span>
          </q-chip>
        </template>
        <q-btn glossy color="primary" icon="fas fa-plus" size="sm" @click="addFood" :label="$t('food')"></q-btn>
      </div>
    </div>
    <div class="row">
      <div class="col">
        
      </div>
    </div>
    <food-picker ref="foodPicker" @selected="foodSelected" />
  </q-page>
  </layout>
</template>

<script src="./food-comparison.js">
</script>

<style lang="stylus">
.outer {
    position: relative;
    width: 100%;
  }

  .inner {
    overflow-x: auto;
    overflow-y: visible;
    margin-left: 150px;
  }

  .freeze {
    position: absolute;
    margin-left: -150px;
    width: 150px;
    padding-right: 5px;
    white-space: nowrap;
    overflow: hidden;
  }

  th.time {
    top: 112px;
    border-width: 0px;
    width: 150px;
  }


  th.nutrient {
    height: 120px;
    white-space: nowrap;
    width: 70px;
  }

  th.nutrient > div {
    transform: translate(49px, 28px) rotate(-45deg);
    width: 70px;
  }

  th.nutrient > div > div {
    border-bottom: 1px solid #ccc;
    padding: 5px 10px;
    width: 100px;
  }

  th.nutrient:nth-child(2) > div {
    transform: translate(45px, 30px) rotate(-45deg);
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

  .meal-name{
    padding-left:10px;
  }
</style>
