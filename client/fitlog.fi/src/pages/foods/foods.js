import constants from '../../store/constants'
import api from '../../api'
import nutrientsMixin from '../../mixins/nutrients'
import PageMixin from '../../mixins/page'

export default {
  mixins:[nutrientsMixin, PageMixin],
  data () {
    return {
      tab: 'tab-1',
      searchText: undefined,
      searchResults: [],
      topDirections: [],
      topDirection: undefined,
      topNutrient: undefined,
      topResults: []
    }
  },
  computed: {
    ownFoods(){
      return this.$store.state.nutrition.ownFoods;
    },
    nutrients(){
      return this.$nutrients.filter(n => !n.computed).map(n =>{return  {...n, label: n.name, value: n}});
    }
  },
  methods: {
    showFood(food){
      this.$router.push({name: 'food-details',params:{id:food.id}});
    },
    createFood(){
      this.$router.push({ name: 'food-details', params: { id: constants.NEW_ID } });
    },
    search(text){
      if(text.length >= 2){
        api.searchFoods(text).then(response => {
          this.searchResults = response.data;
        });
      }
      else {
        this.searchResults = [];
      }
    },
    searchTopNutrients(){
      this.topResults = [];
      if(this.topDirection == 'most'){
        api.searchFoodsMostNutrients(this.topNutrient.id).then(response => {
          this.topResults = response.data;
        });
      }
      else{
        api.searchFoodsLeastNutrients(this.topNutrient.id).then(response => {
          this.topResults = response.data;
        });
      }
    }
  },
  created () {
    this.topDirections = [
      { label: this.$t('most'),  value: 'most' }, 
      { label: this.$t('least'), value:'least' } 
    ];
    this.topDirection = this.topDirections[0].value;
    this.$store.dispatch(constants.FETCH_MY_FOODS, { }).then(_ => {
      this.$store.commit(constants.LOADING_DONE, { });
    }).catch(_ => {
      this.$store.commit(constants.LOADING_DONE, { });
    });
    
  },
  beforeDestroy () {

  }
}
