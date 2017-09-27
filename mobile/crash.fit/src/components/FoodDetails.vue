<template>
  <div :class="{desktop: isDesktop }">
    <div class="row pad">
    <q-input type="text" v-model="name" :float-label="$t('name')" />
    </div>
    <q-tabs v-model="tab">
        <!-- Tabs - notice slot="title" -->
        <q-tab slot="title" name="tab-1" :label="$t('nutrients')" />
        <q-tab slot="title" name="tab-2" :label="$t('portions')" />
        <!-- Targets -->
        <q-tab-pane name="tab-1">
          <q-scroll-area class="scroll">
            <div class="row">
                {{ $t("amount") }}/
                <select v-model="nutrientPortion" @change="changeNutrientPortion">
                    <option v-for="(portion,index) in nutrientPortions" v-bind:value="portion" :key="index">{{ portion.name }}</option>
                </select>
            </div>
            <template v-for="(group,index) in nutrientGroups">
              <div :key="index">
                <div class="row" @click="toggleGroup(group)">
                  <h6>
                    <i v-if="selectedGroup != group" class="fa fa-chevron-down"></i>
                    <i v-if="selectedGroup == group" class="fa fa-chevron-up"></i>
                    {{ $t(group.id) }}
                  </h6>
                </div>
                <div v-if="selectedGroup == group">
                  <div class="row" v-for="(nutrient,index_n) in nutrientsGrouped[group.id]" :key="index_n">
                      <template v-if="!nutrient.computed">
                          <div class="col col-sm-3 col-lg-2"><q-input type="number" v-model="nutrients[nutrient.id]" :float-label="nutrient.name" /></div>
                          <div class="col unit">{{ unit(nutrient.unit)}}</div>
                      </template>
                  </div>
                </div>
              </div>
            </template>
          </q-scroll-area>
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
        manufacturer: null,
        nutrients: {},
        portions: [],
        tab: 'tab-1',
        selectedGroup: undefined,
        nutrientPortion: defaultNutrientPortion
    }
  },
  components: {
        QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem
    },
    computed: {
        nutrientGroups() {
            return this.$store.state.nutrition.nutrientGroups;
        },
        nutrientsGrouped() {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        nutrientPortions(){
            var portions = [];
            portions.push(defaultNutrientPortion);
            if(this.portions.length === 0){
                portions.push({ name: this.$t('portion'), nutrientPortion: true });
            }
            else {
                this.portions.forEach(p => { portions.push(p); });
            }
            return portions;
        },
        errors(){
            var errors = [];
            if(!this.name){
                errors.push('Nimi puuttuu');
            }
            this.portions.forEach(p => {
                if(!p.name){
                    errors.push('Annoksen nimi puuttuu');
                }
                else if(!p.weight || p.weight == ''){
                    errors.push('Annoksen "'+p.name+'" paino puuttuu');
                }
            });
            return errors;
        },
        isValid(){
            return this.errors.length === 0;
        }
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
        changeNutrientPortion(){
            if(this.nutrientPortion.nutrientPortion && this.portions.length === 0){
                this.portions.push(this.nutrientPortion);
            }
        },
        addPortion(){
            this.portions.push({ name: null, weight: null});
        },
        removePortion(index) {
            if(this.nutrientPortion == this.portions[index]){
                this.nutrientPortion = defaultNutrientPortion;
            }
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
            var food = {
                id: self.id,
                name: self.name,
                nutrients: [],
                portions: self.portions ? self.portions.map(p => { return { id: p.id, name: p.name, weight: utils.parseFloat(p.weight), nutrientPortion: p === self.nutrientPortion }}) : []
            };
            for (var i in self.nutrients) {
                if (self.nutrients[i] || self.nutrients[i] == 0) {
                    food.nutrients.push({ nutrientId: i, amount: utils.parseFloat(self.nutrients[i]) });
                }
            }

            self.$store.dispatch(constants.SAVE_FOOD, {
                food,
                success() {
                    self.$router.replace({ name: 'foods' });
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            });
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteFood() {
            var self = this;
            self.$store.dispatch(constants.DELETE_FOOD, {
                food: { id: self.id },
                success() {
                    self.$router.push({ name: 'foods' });
                },
                failure() {
                    toaster.error(self.$t('deleteFailed'));
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
        populate(food) {
            var self = this;
            self.id = food.id;
            self.name = food.name;
            self.portions = food.portions || [];
            if(food.nutrientPortionId){
                self.nutrientPortion = self.portions.find(p => p.id === food.nutrientPortionId);
            }
            self.$store.dispatch(constants.FETCH_NUTRIENTS, {
                success() {
                    for (var i in self.nutrientsGrouped) {
                        var group = self.nutrientsGrouped[i];
                        for (var j in group) {
                            var nutrient = group[j];
                            var value = food.nutrients ? food.nutrients.find(n => n.nutrientId == nutrient.id) : undefined;
                            if (value) {
                                if(self.nutrientPortion && self.nutrientPortion != defaultNutrientPortion){
                                    self.nutrients[nutrient.id] = value.portionAmount;                                   
                                }
                                else {
                                    self.nutrients[nutrient.id] = value.amount;
                                }
                            }
                            else {
                                self.nutrients[nutrient.id] = undefined;
                            }
                        }
                    }
                    self.selectedGroup = self.nutrientGroups[0];
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure() {
                    toaster.error(self.$t('foodDetails.fetchFailed'));
                }
            });
        }
    },
  created () {
    var self = this;
    var id = self.$route.params.id;
    if (id == constants.NEW_ID) {
        self.populate({ id: undefined, name: undefined, nutrients: []});
    }
    else {
        self.$store.dispatch(constants.FETCH_FOOD, {
            id,
            success(food) {
                self.populate(food);
            },
            failure() {
                toaster(self.$t('foodDetails.fetchFailed'));
            }
        });
    }
    
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

.unit{ padding-top: 30px;}
</style>
