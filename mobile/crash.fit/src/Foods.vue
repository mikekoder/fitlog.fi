<template>
  <div :class="{desktop: isDesktop }">
    <q-tabs v-model="tab">
        <!-- Tabs - notice slot="title" -->
        <q-tab slot="title" name="tab-1" icon="fa-user" :label="$t('own')" />
        <!--
        <q-tab slot="title" name="tab-2" icon="fa-search" :label="$t('search')" />
        -->
        <!-- Targets -->
        <q-tab-pane name="tab-1">
            <q-scroll-area>
                <q-list>
                    <q-item v-for="(food,index) in ownFoods" @click="showFood(food)" :key="index">{{ food.name }}</q-item>
                </q-list>
            </q-scroll-area>
        </q-tab-pane>
        <q-tab-pane name="tab-2">

        </q-tab-pane>
    </q-tabs>
    <div class="row pad">
      <q-btn round color="primary" small icon="fa-plus" @click="createFood"></q-btn>
    </div>
  </div>
</template>

<script>
 import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem } from 'quasar'
 import constants from '../store/constants'

export default {
  data () {
    return {
      tab: 'tab-1',
    }
  },
  components: {
        QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem
    },
  computed: {
    ownFoods(){
      return this.$store.state.nutrition.ownFoods;
    }
  },
  methods: {
    showFood(food){
      this.$router.push({name: 'food-details',params:{id:food.id}});
    },
    createFood(){
      this.$router.push({ name: 'food-details', params: { id: constants.NEW_ID } });
    },
  },
  created () {
    var self = this;
    
    self.$store.dispatch(constants.FETCH_MY_FOODS, {
      success(){
        self.$store.commit(constants.LOADING_DONE);
      },
      failure(){
        self.$store.commit(constants.LOADING_DONE);
      }
    });
    
  },
  beforeDestroy () {

  }
}
</script>

<style lang="stylus">
.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea { height: 100%;}
</style>
