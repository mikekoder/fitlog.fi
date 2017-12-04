<template>
    <q-modal ref="modal" :class="{desktop: isDesktop }">
      <h5>Ruoka-aine</h5>
      
        <q-tabs v-model="tab" v-if="selectFood">
            <!-- Tabs - notice slot="title" -->
            <q-tab slot="title" name="tab-1" icon="fa-search" />
            <q-tab slot="title" name="tab-2" icon="fa-clock-o" />
            <q-tab slot="title" name="tab-3" icon="fa-star" />
            <q-tab slot="title" name="tab-4" icon="fa-user" />
            <!-- Targets -->
            <q-tab-pane name="tab-1">
                <q-search v-model="searchText" :placeholder="$t('food')" @change="foodChange">
                    <q-autocomplete @search="search" @selected="foodSelected" :min-characters="2" :max-results="20" />
                </q-search>
                <q-btn @click="selectFood=false" v-if="food">{{ $t('ok') }}</q-btn>
            </q-tab-pane>
            <q-tab-pane name="tab-2">
                <q-scroll-area>
                    <q-list>
                        <q-item v-for="(f, index) in latestFoods" @click="load(f.id)" v-bind:class="{selected: food && f.id == food.id }" :key="index">{{ f.name }}</q-item>
                    </q-list>
                </q-scroll-area>
            </q-tab-pane>
            <q-tab-pane name="tab-3">
                <q-scroll-area>
                    <q-list>
                        <q-item v-for="(f, index) in mostUsedFoods" @click="load(f.id)" v-bind:class="{selected: food && f.id == food.id }" :key="index">{{ f.name }}</q-item>
                    </q-list>
                </q-scroll-area>
            </q-tab-pane>
            <q-tab-pane name="tab-4">
                <q-scroll-area>
                    <q-list>
                        <q-item v-for="(f, index) in ownFoods" @click="load(f.id)" v-bind:class="{selected: food && f.id == food.id }" :key="index">{{ f.name }}</q-item>
                    </q-list>
                </q-scroll-area>
            </q-tab-pane>
        </q-tabs>
      
      <div class="row pad" v-if="!selectFood">     
        <q-btn @click="reselectFood">{{ food.name }}</q-btn>
      </div>
      <div class="row pad" v-if="!selectFood">     
        <q-input v-model="quantity" type="number" v-if="food" :float-label="$t('quantity')" />
      </div>
      <div class="row pad" v-if="!selectFood">     
        <q-select v-model="portion" v-if="food" :float-label="$t('portion')" :options="portions" :display-value="portionText" />
      </div>
      <div class="row pad buttons">
        <q-btn @click="close">{{ $t('cancel') }}</q-btn>
        <q-btn color="primary" @click="save" v-if="food && quantity">{{ $t('save') }}</q-btn>
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

            searchText: undefined
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
        open(row){
            var self = this;
            self.id = row.id;
            self.mealDefinitionId = row.mealDefinitionId;
            self.mealId = row.mealId;
            self.quantity = row.quantity;
            var foodId = row.food ? row.food.id : row.foodId;
            var portionId = row.portion ? row.portion.id : row.portionId;
        
            if(foodId){
                self.load(foodId, portionId);
            }
            
            self.$refs.modal.open();
        },
        search(text, done){
          var self = this;
          api.searchFoods(text).then(results => {
            done(results.map(f => { return { ...f, value: f.name, label: f.name, icon: f.userId ? 'fa-user' : '' }}));
          });
          
        },
        foodChange(text){
            if(this.food && text.length < this.food.name.length){
                this.food = undefined;
                this.portions = [];
                this.portion = undefined;
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
                    self.searchText = food.name;
                    self.food = food;
                    self.selectFood = false;
                    var portions = food.portions.map(p => {return {...p, label: p.name, value: p }});
                    portions.splice(0,0,{ label: 'g', value: undefined});
                    self.portions = portions;
                    if(portionId){
                        self.portion = self.portion.find(p => p.id == portionId);
                    }
                    else{
                        self.portion = self.portions[0];        
                    }
                    
                },
                failure () {
                    //toaster.error(self.$t('fetchFailed'));
                }
            });
        },
        reselectFood(){
            this.selectFood = true;
        },
        tabChanged(tab){
            this.tab = tab;
        },
        close () {
            this.searchText = '';
            this.food = undefined;
            this.quantity = undefined;
            this.portion = undefined;
            this.selectFood = true;
            this.$refs.modal.close();
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
        }
    },
    mounted () {

    }
}
</script>

<style scoped>
.q-select { min-width: 50%;}
button{margin-bottom: 10px;}
.selected{background: hsla(0,0%,74%,.5);}
.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea { height: 100%;}
</style>