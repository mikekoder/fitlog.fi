<template>
    <q-modal ref="modal">
        <q-toolbar color="tertiary" glossy>
            <q-toolbar-title>
                {{ $t('selectFood') }}
            </q-toolbar-title>
            <q-btn flat icon="fas fa-barcode"  @click="readBarcode" v-if="isCordova"></q-btn>
            <q-btn flat icon="close" @click="hide" />
        </q-toolbar>

        <q-tabs v-model="tab" style="height: 82vh;" @select="changeTab">
            
            <q-tab slot="title" name="tab-1"  :label="$t('search')" />
            <q-tab slot="title" name="tab-2"  :label="$t('latest')" />
            <q-tab slot="title" name="tab-3" :label="$t('mostUsed')" />
            <q-tab slot="title" name="tab-4"  :label="$t('my')" />
           
            <q-scroll-area style="height: 75vh;">
            <q-tab-pane name="tab-1">
                <q-search v-model="searchText" :float-label="$t('search')" :placeholder="$t('food')" @input="search" :debounce="500" clearable></q-search>
                <q-list v-if="searchResults.length > 0">
                    <q-item v-for="(f, index) in searchResults" @click.native="foodSelected(f.id)" :class="{selected: food && f.id == food.id }" :key="index" :separator="true">
                        {{ f.text }}
                    </q-item>
                </q-list>
                <div v-else>
                    <span v-if="!searching && (searchText && searchText.length >= 2)">{{ $t('noFoods') }}</span>
                </div>
            </q-tab-pane>
            <q-tab-pane name="tab-2">
                <q-list>
                    <q-item v-for="(f, index) in latestFoods" @click.native="foodSelected(f.id)" :class="{selected: food && f.id == food.id }" :key="index" :separator="true">
                        {{ f.name }}
                    </q-item>
                </q-list>
            </q-tab-pane>
            <q-tab-pane name="tab-3">
                <q-list>
                    <q-item v-for="(f, index) in mostUsedFoods" @click.native="foodSelected(f.id)" :class="{selected: food && f.id == food.id }" :key="index" :separator="true">
                        {{ f.name }}
                    </q-item>
                </q-list>
            </q-tab-pane>
            <q-tab-pane name="tab-4">
                <q-list>
                    <q-item v-for="(f, index) in ownFoods" @click.native="foodSelected(f.id)" :class="{selected: food && f.id == food.id }" :key="index" :separator="true">
                        {{ f.name }}
                    </q-item>
                </q-list>
            </q-tab-pane>
            </q-scroll-area>
        </q-tabs>
    </q-modal>
</template>

<script>  
import constants from '../store/constants'
import api from'../api'

export default {
    name: 'food-picker',
    data () {
      return {
        tab: 'tab-1',
        food: undefined,

        searchText: undefined,
        searchResults: [],
        searching: false
      }
    },
    computed: {
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
    
    methods: {
      show(foodId){     
        this.$refs.modal.show();
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
        if(this.searchText.length >= 2){
          this.searching = true;
          api.searchFoods(this.searchText).then(response => {
            this.searchResults = response.data.map(f => { return { ...f, text: f.manufacturer ? `${f.name} (${f.manufacturer})` : f.name, icon: f.userId ? 'fas fa-user' : '' }});
            this.searching = false;
          });
        }
        else {
          this.searchResults = [];
        }
        if(this.food && this.searchText.length < this.food.name.length){
          this.food = undefined;
          this.portions = [];
          this.portion = undefined;
        }
      },
      foodSelected(foodId){
        this.$store.dispatch(constants.FETCH_FOOD, {id: foodId}).then(food => {
          this.$emit('selected', food);
        });
      },
      tabChanged(tab){
        this.tab = tab;
      },
      hide(){
        this.$refs.modal.hide();
      },
      readBarcode(){
        try {
          cordova.plugins.barcodeScanner.scan(
            result => {
              if(!result.canceled){
                this.searchText = result.text;
                this.search(result.text);
                this.tab = 'tab-1';
              }
            },
            error => {
              this.notifyError(error);
            }
          );
        }
        catch(err){
          this.notifyError(err.message);
        }
      }
    }
}
</script>

<style scoped>
</style>
