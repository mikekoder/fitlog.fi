<template>
<layout >

  <span slot="title">{{ $t('foods') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="fa-plus" @click="createFood" :label="$t('food')"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <q-tabs v-model="tab">
      <q-tab slot="title" name="tab-1" :label="$t('own')" />
      <q-tab slot="title" name="tab-2" :label="$t('search')" />
      <q-tab slot="title" name="tab-3" :label="$t('top100')" />
      
      <q-scroll-area style="height: 85vh;">
          <q-tab-pane name="tab-1">
            <q-list v-if="ownFoods.length > 0">
                <q-item v-for="(food,index) in ownFoods" @click.native="showFood(food)" :key="index" :separator="true">{{ food.name }}<span v-if="food.manufacturer" class="text-weight-light">&nbsp;({{ food.manufacturer }})</span></q-item>
            </q-list>
            <div v-else>{{ $t('noFoods') }}</div>
          </q-tab-pane>
          <q-tab-pane name="tab-2">
            <q-search v-model="searchText" :float-label="$t('search')" placeholder="" @input="search" class="q-mb-sm"/>
            <q-list v-if="searchResults.length > 0">
              <q-item v-for="(food,index) in searchResults" @click.native="showFood(food)" :key="index" :separator="true">{{ food.name }}<span v-if="food.manufacturer" class="text-weight-light">&nbsp;({{ food.manufacturer }})</span></q-item>
            </q-list>
            <div v-else><span v-if="searchText && searchText.length >= 2">{{ $t('noFoods') }}</span></div>
          </q-tab-pane>
          <q-tab-pane name="tab-3">
            <div class="row q-mb-sm">
              <div class="col-3">
                <q-select v-model="topDirection" :options="topDirections" :float-label="' '" @input="searchTopNutrients" />
              </div>
              <div class="col-9">
                <q-select v-model="topNutrient" :options="nutrients" :float-label="$t('nutrient')" @input="searchTopNutrients" />
              </div>
            </div>
            <q-list v-if="topResults.length > 0">
              <q-item v-for="(food,index) in topResults" @click.native="showFood(food)" :key="index" :separator="true">
                  <div class="col-9">{{ food.name }}<span v-if="food.manufacturer" class="text-weight-light">&nbsp;({{ food.manufacturer }})</span></div>
                  <div class="col-2">{{ formatDecimal(food.nutrientAmount,topNutrient.precision) }}</div>
                  <div class="col-1">{{ formatUnit(topNutrient.unit) }}</div>
              </q-item>
            </q-list>
            <div v-else><span v-if="topDirection && topNutrient">{{ $t('noFoods') }}</span></div>
          </q-tab-pane>
      </q-scroll-area>
      
    </q-tabs>
  </q-page>
  </layout>
</template>

<script src="./foods.js">
</script>

<style lang="stylus">
/*
.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea, .desktop .scroll { height: 100%; min-height: 300px;}
.q-tab-pane { height: 66vh;}
*/
</style>
