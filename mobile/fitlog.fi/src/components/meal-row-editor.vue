<template>
    <q-modal ref="modal">
        <q-toolbar color="tertiary" glossy>
            <q-toolbar-title>
                <span v-if="id">{{ $t('editRow') }}</span>
                <span v-else>{{ $t('addRow') }}</span>
            </q-toolbar-title>
            <q-btn flat size="lg" icon="fa-barcode"  @click="readBarcode" v-if="selectFood"></q-btn>
        </q-toolbar>

        <q-tabs v-model="tab" v-if="selectFood" style="height: 82vh;" @select="changeTab">
            
            <q-tab slot="title" name="tab-1"  :label="$t('search')" />
            <q-tab slot="title" name="tab-2"  :label="$t('latest')" />
            <q-tab slot="title" name="tab-3" :label="$t('mostUsed')" />
            <q-tab slot="title" name="tab-4"  :label="$t('my')" />
           
            <q-scroll-area style="height: 75vh;">
            <q-tab-pane name="tab-1">
                <q-search v-model="searchText" :float-label="$t('search')" :placeholder="$t('food')" @input="search" :debounce="500" clearable></q-search>
                <q-list v-if="searchResults.length > 0">
                    <q-item v-for="(f, index) in searchResults" @click.native="load(f.id)" :class="{selected: food && f.id == food.id }" :key="index" :separator="true">{{ f.text }}</q-item>
                </q-list>
                <div v-else>
                    <span v-if="!searching && (searchText && searchText.length >= 2)">{{ $t('noFoods') }}</span>
                </div>
            </q-tab-pane>
            <q-tab-pane name="tab-2">
                <q-list>
                    <q-item v-for="(f, index) in latestFoods" @click.native="load(f.id)" :class="{selected: food && f.id == food.id }" :key="index" :separator="true">{{ f.name }}</q-item>
                </q-list>
            </q-tab-pane>
            <q-tab-pane name="tab-3">
                <q-list>
                    <q-item v-for="(f, index) in mostUsedFoods" @click.native="load(f.id)" :class="{selected: food && f.id == food.id }" :key="index" :separator="true">{{ f.name }}</q-item>
                </q-list>
            </q-tab-pane>
            <q-tab-pane name="tab-4">
                <q-list>
                    <q-item v-for="(f, index) in ownFoods" @click.native="load(f.id)" :class="{selected: food && f.id == food.id }" :key="index" :separator="true">{{ f.name }}</q-item>
                </q-list>
            </q-tab-pane>
            </q-scroll-area>
        </q-tabs>
      
      <div class="row q-ma-sm" v-if="!selectFood">     
        <q-btn @click="reselectFood" :label="food.manufacturer ? `${food.name} (${food.manufacturer})` : food.name"></q-btn>
      </div>
      <div class="row q-ma-sm" v-if="!selectFood">    
          <div class="col">
        <q-input v-model="quantity" type="number" v-if="food" :float-label="$t('quantity')" />
        </div> 
      </div>
      <div class="row q-ma-sm" v-if="!selectFood">   
          <div class="col">
            <q-select v-model="portion" v-if="food" :float-label="$t('portion')" :options="portions" :display-value="portionText" />
          </div>  
      </div>
      <div class="row q-ma-sm q-mt-lg">
        <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
        <q-btn glossy color="primary" @click="save" :label="$t('save')" :disabled="!canSave"></q-btn>
      </div>
        
    </q-modal>
</template>

<script>
    import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem } from 'quasar'
    import constants from '../store/constants'
    import api from'../api'

export default {
    name: 'meal-row-editor',
    data () {
        return {
            tab: 'tab-1',
            selectFood: true,

            id: undefined,
            mealDefinitionId: undefined,
            mealId: undefined,
            food: undefined,
            quantity: undefined,
            portion: undefined,
            portions: [],

            searchText: undefined,
            searchResults: [],
            searching: false
        }
    },
    props: {
        row: undefined
    },
    computed: {
        canSave() {
            return this.food && this.quantity;
        },
        portionText(){
            return this.portion && this.portion.name ? this.portion.name : 'g';
        },
        latestFoods(){
            return this.$store.state.nutrition.latestFoods.sort((a,b) => a.name < b.name ? -1 : 1);
        },
        mostUsedFoods(){
            return this.$store.state.nutrition.mostUsedFoods.sort((a,b) => a.name < b.name ? -1 : 1);
        },
        ownFoods(){
            return this.$store.state.nutrition.ownFoods.sort((a,b) => a.name < b.name ? -1 : 1);
        }
    },
    components: {
        QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem
    },
    methods: {
        show(row){
            var self = this;
            self.tab = 'tab-1';
            self.id = row.id;
            self.mealDefinitionId = row.mealDefinitionId;
            self.mealId = row.mealId;
            self.quantity = row.quantity;
            var foodId = row.food ? row.food.id : row.foodId;
            var portionId = row.portion ? row.portion.id : row.portionId;
        
            if(foodId){
                self.load(foodId, portionId);
            }
            
            self.$refs.modal.show();
        },
        changeTab(tab){
            if(tab == 'tab-1' && this.food){
                if(this.searchText != this.food.name){
                    this.searchText = this.food.name;
                    this.search();
                }
                
            }
        },
        search(text){
          var self = this;
          if(self.searchText.length >= 2){
            self.searching = true;
            api.searchFoods(self.searchText).then(results => {
                self.searchResults = results.map(f => { return { ...f, text: f.manufacturer ? `${f.name} (${f.manufacturer})` : f.name, icon: f.userId ? 'fa-user' : '' }});
                self.searching = false;
            });
          }
          else {
              self.searchResults = [];
          }
          if(self.food && self.searchText.length < self.food.name.length){
              self.food = undefined;
              self.portions = [];
              self.portion = undefined;
          }
        },
        foodSelected(food){
          this.load(food.id, undefined);
        },
        load(foodId, portionId){
            var self = this;
            self.$store.dispatch(constants.FETCH_FOOD, {
                id: foodId,
                success (food) {
                    //self.searchText = food.manufacturer ? `${food.name} (${food.manufacturer})` : food.name;
                    self.food = food;
                    self.selectFood = false;
                    var portions = food.portions.map(p => {return {...p, label: p.name, value: p }});
                    portions.splice(0,0,{ label: 'g', value: undefined});
                    self.portions = portions;
                    if(portionId){
                        self.portion = self.portions.find(p => p.id == portionId);
                    }
                    else{
                        self.portion = self.portions[0];        
                    }
                    
                },
                failure () {
                    self.notifyError(self.$t('fetchFailed'));
                }
            });
        },
        reselectFood(){
            this.selectFood = true;
        },
        tabChanged(tab){
            this.tab = tab;
        },
        cancel () {
            this.searchText = '';
            this.searchResults = [];
            this.food = undefined;
            this.quantity = undefined;
            this.portion = undefined;
            this.selectFood = true;
            this.$refs.modal.hide();
        },
        hide(){
            this.cancel();
        },
        save () {
            var self = this;
            var row = {
                id: self.id,
                mealDefinitionId: self.mealDefinitionId,
                mealId: self.mealId,
                food: self.food,
                foodId: self.food.id,
                foodName: self.food.name,
                quantity: self.quantity,
                portion: self.portion.value,
                portionId: self.portion ? self.portion.id : undefined,
                portionName: self.portion ? self.portion.name : undefined
            };
            this.$emit('save', row);
        },
        readBarcode(){
            try {
                var self = this;
                cordova.plugins.barcodeScanner.scan(
                    result => {
                        if(!result.canceled){
                            self.searchText = result.text;
                            self.search(result.text);
                            self.tab = 'tab-1';
                        }
                    },
                    error => {
                        self.notifyError(error);
                    }
                );
            }
            catch(err){
                self.notifyError(err.message);
            }
        }
    },
    mounted () {

    }
}
</script>

<style scoped>
/*
.q-select { min-width: 50%;}
button{margin-bottom: 10px;}

.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea { height: 100%;}
*/
</style>
