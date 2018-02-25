<template>
  <div :class="{desktop: isDesktop }">
    <q-tabs v-model="tab">
      <q-tab slot="title" name="tab-1" :label="$t('own')" />
      <q-tab slot="title" name="tab-2" :label="$t('search')" />
      <q-tab slot="title" name="tab-3" :label="$t('top100')" />
      
      <q-scroll-area style="height: 75vh;">
          <q-tab-pane name="tab-1">
          <q-list>
              <q-item v-for="(food,index) in ownFoods" @click="showFood(food)" :key="index" :separator="true">{{ food.name }}</q-item>
          </q-list>
          </q-tab-pane>
          <q-tab-pane name="tab-2">
            <q-search v-model="searchText" :float-label="$t('search')" placeholder="" @change="search" />
            <q-list>
              <q-item v-for="(food,index) in searchResults" @click="showFood(food)" :key="index" :separator="true">{{ food.name }}</q-item>
            </q-list>
          </q-tab-pane>
          <q-tab-pane name="tab-3">
            <div class="row">
              <div class="col-3">
                <q-select v-model="topDirection" :options="topDirections" :float-label="' '" @change="searchTopNutrients" />
              </div>
              <div class="col-9">
                <q-select v-model="topNutrient" :options="nutrients" :float-label="$t('nutrient')" @change="searchTopNutrients" />
              </div>
            </div>
            
            <q-list>
              <q-item v-for="(food,index) in topResults" @click="showFood(food)" :key="index" :separator="true">
                  <div class="col-9">{{ food.name }}</div>
                  <div class="col-2">{{ formatDecimal(food.nutrientAmount,topNutrient.precision) }}</div>
                  <div class="col-1">{{ formatUnit(topNutrient.unit) }}</div>
              </q-item>
            </q-list>
          </q-tab-pane>
      </q-scroll-area>
      
    </q-tabs>
      
    <div class="row pad">
      <q-btn color="primary" glossy icon="fa-plus" @click="createFood">{{ $t('food') }}</q-btn>
    </div>
  </div>
</template>

<script src="./foods.js">
</script>

<style lang="stylus">
.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea, .desktop .scroll { height: 100%; min-height: 300px;}
.q-tab-pane { height: 66vh;}
</style>
