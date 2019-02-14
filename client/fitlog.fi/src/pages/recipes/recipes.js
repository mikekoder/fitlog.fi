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
    this.$store.dispatch(constants.DELETE_RECIPE, {
      recipe
    }).then(_ => {
      this.recipes.splice(this.recipes.findIndex(r => r.id == recipe.id), 1);
    }).catch(_ => {
      this.notifyError(this.$t('deleteFailed'));
    });
  },
  clickRecipe(recipe){
    this.$q.actionSheet({
      title: recipe.name,
      grid: true,
      actions: [
        {
          label: this.$t('edit'),
          icon: 'fas fa-edit',
          handler: () => {
            this.showRecipe(recipe);
          }
        },
        {
          label: this.$t('delete'),
          icon: 'fas fa-trash',
          handler: () => {
            this.deleteRecipe(recipe);
          }
        }
      ],
      dismiss: {
          label: this.$t('cancel'),
          handler: () => {
              
          }
      }
    });
  }
},
  created () {
    this.$store.dispatch(constants.FETCH_RECIPES, { }).then(recipes => {
      this.recipes = recipes;
      this.$store.commit(constants.LOADING_DONE, { });
    }).catch(_ => {
      this.notifyError(this.$t('fetchFailed'));
    });
    
  },
  beforeDestroy () {

  }
}
