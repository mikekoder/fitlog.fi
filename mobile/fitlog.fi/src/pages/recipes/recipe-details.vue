<template>
  <q-page class="q-pa-sm">
    <div class="row q-mb-sm">
        <q-input type="text" v-model="name" :float-label="$t('name')" ref="nameInput" />
    </div>
    <q-tabs v-model="tab">
        <!-- Tabs - notice slot="title" -->
        <q-tab slot="title" name="tab-1" :label="$t('ingredients')" />
        <q-tab slot="title" name="tab-2" :label="$t('portions')" />
        <q-tab slot="title" name="tab-3" :label="$t('nutrients')" />
        <q-scroll-area style="height: 60vh;">
        <!-- Targets -->
        <q-tab-pane name="tab-1">
          <q-list v-if="ingredients.length > 0">
              <q-item v-for="(row,index) in ingredients" @click="editIngredient(row)" :key="index" :separator="true">
                  <div class="row ingredient">
                    <div class="col-10">{{ row.food.name}} {{ row.quantity }} {{ row.portion ? row.portion.name : 'g' }}</div>  
                    <div class="col-2"><q-btn round glossy color="primary" size="sm" icon="fa-trash" v-on:click.stop="deleteIngredient(index)"></q-btn></div>
                </div>
              </q-item>
          </q-list>
          <div class="row">
              <div class="col">
                <q-btn glossy color="primary" icon="fa-plus" size="sm" @click="addIngredient" :label="$t('food')"></q-btn>
              </div>
            </div>
            

            <div class="row q-my-md">
                <div class="col-12">
                    <q-input type="number" :value="formatDecimal(recipeWeight)" :float-label="$t('rawWeight')" readonly />
                </div>
            </div>
            <div class="row q-my-md">
                <div class="col-12">
                    <q-input type="number" v-model="cookedWeight" :float-label="$t('cookedWeight')" />
                </div>
            </div>
            <div class="row q-my-md">
                <div class="col-12">
                    <q-input type="number" :value="formatDecimal(weightChange, 1)" suffix="%" :float-label="$t('weightChange')" readonly />
                </div>
            </div>

            <meal-row-editor ref="editRow" v-on:save="saveIngredient(arguments[0])" />
        </q-tab-pane>
        <q-tab-pane name="tab-2">
          <div class="row" v-for="(portion,index) in portions" :key="index">
            <div class="col col-5"><q-input type="text" v-model="portion.name" :float-label="$t('name')" /></div>
            <div class="col col-5"><q-input type="number" v-model="portion.amount" :float-label="$t('portions') + '/' + $t('recipe')" /></div>
            <div class="col col-1"><q-btn round size="sm" color="primary" icon="fa-trash" @click="removePortion(index)"></q-btn></div>
          </div>
          <div class="row">
            <q-btn glossy size="sm" color="primary" icon="fa-plus" @click="addPortion" :label="$t('portion')"></q-btn>
          </div>
        </q-tab-pane>
        <q-tab-pane name="tab-3">
            <template v-for="(group,index) in nutrientGroups">
                <div :key="index">
                  <div class="row">
                      <q-btn flat @click="toggleGroup(group)" :label="$t(group.id)" :icon="selectedGroup == group ? 'fa-chevron-up' : 'fa-chevron-down'"></q-btn>
                  </div>
                  <div v-if="selectedGroup == group">
                    <div class="row" v-for="(nutrient,index_n) in nutrientsGrouped[group.id]" :key="index_n">
                        <template v-if="!nutrient.computed">
                            <div class="col"><q-input type="number" :value="formatDecimal(recipeNutrients[nutrient.id], nutrient.precision)" :float-label="nutrient.name" readonly /></div>
                            <div class="col q-pt-md">{{ formatUnit(nutrient.unit)}}</div>
                        </template>
                    </div>
                  </div>
                </div>
              </template>
        </q-tab-pane>
        </q-scroll-area>
    </q-tabs>
    
    <div class="row">
      <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
      <q-btn glossy color="primary" @click="save" :label="$t('save')" :disabled="!canSave"></q-btn>
    </div>
  </q-page>
</template>

<script src="./recipe-details.js">
</script>

<style lang="stylus" scoped>
/*
.q-tab-pane { height: 65vh;}
.scroll { height: 100%;}
.desktop .q-tab-pane { height: 70vh;}
.ingredient{ width: 100%;}
th { text-align: left;}
*/
</style>
