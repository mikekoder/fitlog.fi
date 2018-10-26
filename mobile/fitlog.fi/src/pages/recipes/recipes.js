import constants from '../../store/constants'
import PageMixin from '../../mixins/page'

export default {
  mixins: [PageMixin],
  data () {
    return {
      tab: 'tab-1',
      recipes: []
    }
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
      recipe
    }).then(_ => {
      self.recipes.splice(self.recipes.findIndex(r => r.id == recipe.id), 1);
    }).catch(_ => {
      self.notifyError(self.$t('deleteFailed'));
    });
  },
  clickRecipe(recipe){
    var self = this;
    this.$q.actionSheet({
      title: recipe.name,
      grid: true,
      actions: [
        {
          label: self.$t('edit'),
          icon: 'fas fa-edit',
          handler: () => {
            self.showRecipe(recipe);
          }
        },
        {
          label: self.$t('delete'),
          icon: 'fas fa-trash',
          handler: () => {
            self.deleteRecipe(recipe);
          }
        }
      ],
      dismiss: {
          label: self.$t('cancel'),
          handler: () => {
              
          }
      }
    });
  }
},
  created () {
    var self = this;
    self.$store.dispatch(constants.FETCH_RECIPES, { }).then(recipes => {
      self.recipes = recipes;
      self.$store.commit(constants.LOADING_DONE, { });
    }).catch(_ => {
      self.notifyError(self.$t('fetchFailed'));
    });
    
  },
  beforeDestroy () {

  }
}
