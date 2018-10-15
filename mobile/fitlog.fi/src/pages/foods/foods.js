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
      var self = this;
      if(text.length >= 2){
        api.searchFoods(text).then(response => {
          self.searchResults = response.data;
        });
      }
      else {
        self.searchResults = [];
      }
    },
    searchTopNutrients(){
      var self = this;
      self.topResults = [];
      if(self.topDirection == 'most'){
        api.searchFoodsMostNutrients(self.topNutrient.id).then(response => {
          self.topResults = response.data;
        });
      }
      else{
        api.searchFoodsLeastNutrients(self.topNutrient.id).then(response => {
          self.topResults = response.data;
        });
      }
    }
  },
  created () {
    var self = this;
    self.topDirections = [
      { label: self.$t('most'),  value: 'most' }, 
      { label:self.$t('least'), value:'least' } ];
    self.topDirection = self.topDirections[0].value;
    self.$store.dispatch(constants.FETCH_MY_FOODS, { }).then(_ => {
      self.$store.commit(constants.LOADING_DONE, { });
    }).catch(_ => {
      self.$store.commit(constants.LOADING_DONE, { });
    });
    
  },
  beforeDestroy () {

  }
}
