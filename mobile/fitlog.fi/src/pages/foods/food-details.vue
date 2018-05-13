<template>
<layout >

  <span slot="title">{{ $t('food') }}</span>

  <div slot="toolbar">
    <q-btn flat size="lg" icon="help" @click="showHelp"></q-btn>
    <q-btn flat size="lg" icon="save" @click="save" :disabled="!canSave"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <div class="row">
      <div class="col">
        <div class="row">
          <div class="col">
            <q-input type="text" v-model="name" :float-label="$t('name')" ref="nameInput" />
          </div>
        </div>
        <div class="row q-my-sm">
          <div class="col">
            <q-input type="text" v-model="manufacturer" :float-label="$t('manufacturer')" />
          </div>
        </div>
      </div>
    </div>
    
    <q-tabs v-model="tab">

        <q-tab slot="title" name="tab-1" :label="$t('nutrients')" />
        <q-tab slot="title" name="tab-2" :label="$t('portions')" />
        <q-tab slot="title" name="tab-3" :label="$t('furtherInformation')" />

        <q-tab-pane name="tab-1">
          
            <div class="row">
              <div class="col-2" style="padding-top: 20px">{{ $t("amount") }}/</div>
              <div class="col-4"><q-select v-model="nutrientPortion" :options="nutrientPortions" @change="changeNutrientPortion" /></div>
            </div>
            <template v-for="(group,index) in nutrientGroups">
              <div :key="index">
                <div class="row">
                  <q-btn flat @click="toggleGroup(group)" :label="$t(group.id)" :icon="selectedGroup == group ? 'fa-chevron-up' : 'fa-chevron-down'"></q-btn>
                </div>
                <div v-if="selectedGroup == group">
                  <div class="row" v-for="(nutrient,index_n) in nutrientsGrouped[group.id]" :key="index_n">
                      <template v-if="!nutrient.computed">
                          <div class="col"><q-input type="number" v-model="nutrients[nutrient.id]" :float-label="nutrient.name" /></div>
                          <div class="col q-pt-md">{{ formatUnit(nutrient.unit)}}</div>
                      </template>
                  </div>
                </div>
              </div>
            </template>
          
        </q-tab-pane>
        <q-tab-pane name="tab-2">
          <div class="row" v-for="(portion,index) in portions" :key="index">
            <div class="col col-8 q-pr-sm"><q-input type="text" v-model="portion.name" :float-label="$t('name')" /></div>
            <div class="col col-3"><q-input type="number" v-model="portion.weight" :float-label="$t('weight')" /></div>
            <div class="col col-1 q-pa-sm"><q-btn round size="sm" glossy color="primary" icon="fa-trash" @click="removePortion(index)"></q-btn></div>
          </div>
          <div class="row q-mt-md">
            <q-btn size="sm" glossy color="primary" icon="fa-plus" @click="addPortion" :label="$t('portion')"></q-btn>
          </div>
        </q-tab-pane>
        <q-tab-pane name="tab-3">
          <div class="row" >
            <div class="col">
              <q-input type="text" v-model="ean" :float-label="$t('ean')" @blur="loadInfoByEan" />
            </div>

          </div>
        </q-tab-pane>
    </q-tabs>
    <food-help ref="help" />
  </q-page>
  </layout>
</template>

<script src="./food-details.js">
</script>

<style lang="stylus" scoped>
/*

.q-tab-pane { height: 66vh;}

.desktop .q-tab-pane { height: 70vh;}
.desktop .q-scrollarea { height: 100%;}

.unit{ padding-top: 30px;}
*/
</style>
