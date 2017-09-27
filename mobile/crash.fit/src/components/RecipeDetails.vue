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
            <div v-for="(row,index) in ingredients" @click="editIngredient(row)" :key="index">
                <div class="row food-portion">
                <div class="col">{{ row.foodName }} {{ row.quantity }} {{ row.portionName || 'g' }}</div>  
                </div>
                <!--
                <div class="row nutrients">
                <div class="col" v-for="nutrient in visibleNutrients" :key="nutrient.id">
                    <div v-if="nutrient.id == energyDistributionId">
                    <energy-distribution v-bind:protein="row.nutrients[proteinId]" v-bind:carb="row.nutrients[carbId]" v-bind:fat="row.nutrients[fatId]"></energy-distribution>
                    </div>
                    <div v-else>{{ decimal(row.nutrients[nutrient.id]) }}</div>
                    </div>
                </div>
                -->
                <q-btn round class="float-right" color="primary" style="top: -55px; right:-10px;" small icon="fa-trash" v-on:click.stop="deleteIngredient(index)"></q-btn>
            </div>
            <q-btn round color="primary" icon="fa-plus" small @click="addIngredient"></q-btn>
            <edit-row ref="editRow" v-on:save="saveIngredient(arguments[0])" />
        </q-tab-pane>
        <q-tab-pane name="tab-2">
          <div class="row" v-for="(portion,index) in portions" :key="index">
            <div class="col col-lg-4 col-xl-3"><q-input type="text" v-model="portion.name" :float-label="$t('name')" /></div>
            <div class="col col-lg-2 col-xl-1"><q-input type="number" v-model="portion.weight" :float-label="$t('weight')" /></div>
            <div class="col col-lg-1"><q-btn round small color="primary" icon="fa-trash" @click="removePortion(index)"></q-btn></div>
          </div>
          <div class="row">
            <q-btn round small color="primary" icon="fa-plus" @click="addPortion"></q-btn>
          </div>
        </q-tab-pane>
    </q-tabs>
    <div class="row pad buttons">
      <q-btn @click="cancel">{{ $t('cancel') }}</q-btn>
      <q-btn color="primary" @click="save">{{ $t('save') }}</q-btn>
    </div>
  </div>
</template>

<script>
 import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem } from 'quasar'
 import constants from '../store/constants'
 import formatters from '../formatters'
 import utils from '../utils'

var defaultNutrientPortion = { id: undefined, name: '100g' };
export default {
  data () {
    return {
        id: null,
        name: null,
        ingredients: [],
        portions: [],
        cookedWeight: undefined,
        tab: 'tab-1',
        selectedGroup: undefined,
        selectedRow: undefined
    }
  },
  components: {
        QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem,
        'edit-row': require('./edit-meal-row'),
        'energy-distribution': require('./energy-distribution-bar')
    },
    computed: {
        nutrientGroups() {
            return this.$store.state.nutrition.nutrientGroups;
        },
        nutrientsGrouped() {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        recipeNutrients() {
            var self = this;
            var nutrients = {};
            for (var i in this.ingredients) {
                var row = this.ingredients[i];
                if(!row.food || !row.quantity){
                    continue;
                }
                var weight = self.weight(row.quantity, row.portion);
                for (var j in row.food.nutrients) {
                    var foodNutrient = row.food.nutrients[j];
                    if (nutrients[foodNutrient.nutrientId]) {
                        nutrients[foodNutrient.nutrientId] += (weight * foodNutrient.amount / 100);
                    }
                    else {
                        nutrients[foodNutrient.nutrientId] = (weight * foodNutrient.amount / 100);
                    }
                }
            }
            return nutrients;
        },
        recipeWeight() {
            var self = this;
            var weight = 0;
            for (var i in this.ingredients) {
                var row = this.ingredients[i];
                if (!row.food || !row.quantity) {
                    continue;
                }
                var ingredientWeight = self.weight(row.quantity, row.portion);
                weight += ingredientWeight;
            }
            return weight;
        },
        weightChange() {
            return (this.cookedWeight - this.recipeWeight) / this.recipeWeight * 100;
        },
        visibleNutrients() {
          return this.$store.state.nutrition.nutrients.filter(n => n.homeOrder || n.homeOrder === 0).sort((n1,n2) => n1.homeOrder - n2.homeOrder);
      },
    },
  methods: {
    toggleGroup(group){
      if(this.selectedGroup == group){
        this.selectedGroup = undefined;
      }
      else{
        this.selectedGroup = group;
      }
    },
    addIngredient() {
        var row = { 
            food: undefined, 
            quantity: undefined, 
            portion: undefined
        };
       this.selectedRow = undefined;
      this.$refs.editRow.open(row);
    },
    editIngredient(row){
        this.selectedRow = row;
        this.$refs.editRow.open(row);
    },
    saveIngredient(row){
        this.$refs.editRow.close();

        var weight = this.weight(row.quantity, row.portion);
        row.nutrients = {};
        for(var i in row.food.nutrients){
            row.nutrients[row.food.nutrients[i].nutrientId] = row.food.nutrients[i].amount * weight / 100;
        }
        if(this.selectedRow){
            var index = this.ingredients.indexOf(this.selectedRow);
            this.ingredients.splice(index, 1, row);
        }
        else {
            this.ingredients.push(row);
        }
        this.selectedRow = undefined;
        
    },
    deleteIngredient(index){
        this.ingredients.splice(index, 1);
    },
    addPortion(){
        this.portions.push({ name: null, weight: null, number: null});
    },
    removePortion(index) {
        this.portions.splice(index, 1);
    },
    weight(quantity, portion) {
        if (!quantity) {
            return '';
        }
        if (typeof (quantity) !== 'number') {
            quantity = parseFloat(quantity.replace(',', '.'));
        }
        
        if (portion) {
            return quantity * portion.weight;
        }
        return quantity;
    },
    save() {
        var self = this;
        var recipe = {
            id: self.id,
            name: self.name,
            ingredients: self.ingredients.map(i => { return { foodId: i.food ? i.food.id : undefined, quantity: utils.parseFloat(i.quantity), portionId: i.portion ? i.portion.id : undefined } }),
            portions: self.portions ? self.portions.map(p => { return { id: p.id, name: p.name, amount: utils.parseFloat(p.amount), weight: utils.parseFloat(p.weight) } }) : [],
            cookedWeight: self.cookedWeight
        };
        self.$store.dispatch(constants.SAVE_RECIPE, {
            recipe,
            success() {
                self.$router.replace({ name: 'recipes' });
            },
            failure() {
                toaster.error(self.$t('recipeDetails.saveFailed'));
            }
        });
    },
    cancel() {
        this.$router.go(-1);
    },
    deleteRecipe() {
        var self = this;
        self.$store.dispatch(constants.DELETE_RECIPE, {
            recipe: { id: self.id },
            success() {
                self.$router.push({ name: 'recipes' });
            },
            failure() {
                toaster(self.$t('recipeDetails.deleteFailed'));
            }
        });
    },
    unit: formatters.formatUnit,
    decimal(value, precision) {
        if (!value) {
            return value;
        }
        return value.toFixed(precision);
    },
    toggleGroup(group) {
        this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
    },
    groupIsExpanded(group) {
        return this.groupOpenStates[group] && true;
    },
    populate(recipe) {
        var self = this;
        self.id = recipe.id;
        self.name = recipe.name;
        self.cookedWeight = recipe.cookedWeight;
        self.portions = recipe.portions || [];
        if(recipe.ingredients){
            var foodIds = recipe.ingredients.map(i => { return i.foodId });
            self.$store.dispatch(constants.FETCH_FOODS, {
                ids: foodIds,
                success(foods) {
                    self.ingredients = recipe.ingredients.map(i => {
                        var food = foods.find(f => f.id == i.foodId);
                        var portion = food.portions.find(p => p.id === i.portionId);
                        return { food: food, quantity: i.quantity, portion: portion};
                    });
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure() {
                    toaster(self.$t('recipeDetails.fetchFailed'));
                }
            });
        }
        else{
            self.$store.commit(constants.LOADING_DONE);
        }
    }
},
  created () {
    var self = this;
    var id = self.$route.params.id;
    if (id == constants.NEW_ID) {
        self.populate({ id: undefined, name: undefined });
    }
    else {
        self.$store.dispatch(constants.FETCH_RECIPE, {
            id,
            success(recipe) {
                self.populate(recipe);
            },
            failure() {
                toaster(self.$t('recipeDetails.fetchFailed'));
            }
        });
    }

    self.$store.dispatch(constants.FETCH_NUTRIENTS, {
        success() { },
        failure() { }
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
  },
  beforeDestroy () {

  }
}
</script>

<style lang="stylus" scoped>
.q-tab-pane { height: 60vh;}
.scroll { height: 100%;}
.desktop .q-tab-pane { height: 70vh;}
.desktop .q-scrollarea { height: 100%;}
.food-portion{ padding-bottom: 30px;}
</style>
