import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem } from 'quasar'
import constants from '../../store/constants'

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
  showRecipe(recipe){
    this.$router.push({name: 'recipe-details',params:{id:recipe.id}});
  },
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
