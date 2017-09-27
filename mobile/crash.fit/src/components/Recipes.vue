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
                    <q-item v-for="(recipe,index) in recipes" @click="showRecipe(recipe)" :key="index">{{ recipe.name }}</q-item>
                </q-list>
            </q-scroll-area>
        </q-tab-pane>
        <q-tab-pane name="tab-2">

        </q-tab-pane>
    </q-tabs>
    <div class="row pad">
      <q-btn round color="primary" small icon="fa-plus" @click="createRecipe"></q-btn>
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
      recipes: []
    }
  },
  components: {
        QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem
    },
  computed: {
  },
  methods: {
    createRecipe(){
      this.$router.push({ name: 'recipe-details', params: { id: constants.NEW_ID } });
    },
    deleteRecipe(recipe) {
        var self = this;
        self.$store.dispatch(constants.DELETE_RECIPE, {
            recipe,
            success() {
                self.recipes.splice(self.recipes.findIndex(r => r.id == recipe.id), 1);
            },
            failure() {
                toaster(self.$t('recipes.deleteFailed'));
            }
        });
    }
  },
  created () {
    var self = this;
    self.$store.dispatch(constants.FETCH_RECIPES, {
      success(recipes) {
        self.recipes = recipes;
        self.$store.commit(constants.LOADING_DONE);
      },
      failure() {
        toaster(self.$t('recipes.fetchFailed'));
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
