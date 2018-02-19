<template>
  <div :class="{desktop: isDesktop }">
    <div class="row pad">
    <q-input type="text" v-model="name" :float-label="$t('name')" />
    </div>
    <q-tabs v-model="tab">
        <!-- Tabs - notice slot="title" -->
        <q-tab slot="title" name="tab-1" :label="$t('ingredients')" />
        <q-tab slot="title" name="tab-2" :label="$t('portions')" />
        <q-tab slot="title" name="tab-3" :label="$t('nutrients')" />
        <!-- Targets -->
        <q-tab-pane name="tab-1">
          <q-scroll-area style="height: 60vh;">
          <q-list>
              <q-item v-for="(row,index) in ingredients" @click="editIngredient(row)" :key="index" :separator="true">
                  <div class="row ingredient">
                    <div class="col-10">{{ row.food.name}} {{ row.quantity }} {{ row.portion ? row.portion.name : 'g' }}</div>  
                    <div class="col-2"><q-btn round glossy color="primary" small icon="fa-trash" v-on:click.stop="deleteIngredient(index)"></q-btn></div>
                </div>
              </q-item>
          </q-list>
            <q-btn round glossy color="primary" icon="fa-plus" small @click="addIngredient"></q-btn>

            <div class="row">
                <div class="col-6">
                    {{ $t("rawWeight") }}
                </div>
                <div class="col-3">
                    {{ formatDecimal(recipeWeight) }}
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    {{ $t("cookedWeight") }}
                </div>
           
            </div>
            <div class="row">
                <div class="col-3">
                    <q-input type="number" v-model="cookedWeight"  />
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    {{ $t("weightChange") }}
                </div>
                <div class="col-3">
                    <span v-if="cookedWeight">{{ formatDecimal(weightChange, 1) }} %</span>
                </div>
            </div>

            <meal-row-editor ref="editRow" v-on:save="saveIngredient(arguments[0])" />
            </q-scroll-area>
        </q-tab-pane>
        <q-tab-pane name="tab-2">
          <div class="row" v-for="(portion,index) in portions" :key="index">
            <div class="col col-5"><q-input type="text" v-model="portion.name" :float-label="$t('name')" /></div>
            <div class="col col-5"><q-input type="number" v-model="portion.amount" :float-label="$t('portions') + '/' + $t('recipe')" /></div>
            <div class="col col-1"><q-btn round small color="primary" icon="fa-trash" @click="removePortion(index)"></q-btn></div>
          </div>
          <div class="row">
            <q-btn round small color="primary" icon="fa-plus" @click="addPortion"></q-btn>
          </div>
        </q-tab-pane>
        <q-tab-pane name="tab-3">
            <q-scroll-area style="height: 60vh;">
                <table class="nutrients">
                    <thead>
                        <tr>
                            <th></th>
                            <th>{{ $t("recipe") }}</th>
                            <th>100g</th>
                            <template v-for="portion in portions">
                                <th>{{ portion.name }}</th>
                            </template>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody v-for="group in $nutrientGroups">
                        <tr>
                            <th class="clickable" colspan="2" @click="toggleGroup(group.id)">
                                <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                {{ group.name }}
                            </th>
                        </tr>
                        <tr v-for="nutrient in allNutrients[group.id]" v-if="groupOpenStates[group.id] && !nutrient.computed">
                            <td>{{ nutrient.name }}</td>
                            <td>{{ formatDecimal(recipeNutrients[nutrient.id], nutrient.precision) }}</td>
                            <td>{{ formatDecimal(recipeNutrients[nutrient.id] * 100 / recipeWeight, nutrient.precision) }}</td>
                            <template v-for="portion in portions">
                                <td><span v-if="portion.amount">{{ formatDecimal(recipeNutrients[nutrient.id] / recipeWeight * ((cookedWeight || recipeWeight)/portion.amount), nutrient.precision) }}</span></td>
                            </template>
                            <td>{{ formatUnit(nutrient.unit)}}</td>
                        </tr>
                    </tbody>
                </table>
            </q-scroll-area>
        </q-tab-pane>
    </q-tabs>
    <div class="row pad buttons">
      <q-btn @click="cancel">{{ $t('cancel') }}</q-btn>
      <q-btn color="primary" @click="save" v-if="name">{{ $t('save') }}</q-btn>
    </div>
  </div>
</template>

<script src="./recipe-details.js">
</script>

<style lang="stylus" scoped>
.q-tab-pane { height: 65vh;}
.scroll { height: 100%;}
.desktop .q-tab-pane { height: 70vh;}
.ingredient{ width: 100%;}
th { text-align: left;}
</style>
