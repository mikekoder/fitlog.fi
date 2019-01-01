import constants from '../../store/constants'
import nutrientsMixin from '../../mixins/nutrients'
import PageMixin from '../../mixins/page'
import FoodPicker from '../../components/food-picker'
import EnergyDistributionBar from '../../components/energy-distribution-bar'

export default {
  mixins:[nutrientsMixin, PageMixin],
  components: {
    EnergyDistributionBar,
    'food-picker': FoodPicker
  },
  data () {
    return {
      selectedGroup: '',
      foods: [],
      energyId: constants.ENERGY_ID,
      proteinId: constants.PROTEIN_ID,
      proteinEnergyId: constants.PROTEIN_ENERGY_ID,
      carbId: constants.CARB_ID,
      carbEnergyId: constants.CARB_ENERGY_ID,
      fatId: constants.FAT_ID,
      fatEnergyId: constants.FAT_ENERGY_ID,
      energyDistributionId: constants.ENERGY_DISTRIBUTION_ID,
    }
  },
  computed: {
    nutrientGroups() {
      return this.$store.state.nutrition.nutrientGroups;
    },
    nutrientsGrouped() {
        return this.$store.state.nutrition.nutrientsGrouped;
    },
    visibleNutrients(){
      if(this.selectedGroup){
        return this.nutrientsGrouped[this.selectedGroup].filter(n => !n.hideSummary);
      }
      return [];
    }
  },
  methods: {
    addFood(){
      this.$refs.foodPicker.show();
    },
    foodSelected(food){
      food.nutrients = food.nutrients.reduce((obj,item) => (obj[item.nutrientId] = item.amount, obj),{});
      if(food.nutrients[this.energyId]){
        food.nutrients[this.proteinEnergyId] = 100 * (4 * food.nutrients[this.proteinId]) / food.nutrients[this.energyId];
        food.nutrients[this.carbEnergyId] = 100 *  (4 * food.nutrients[this.carbId]) / food.nutrients[this.energyId];
        food.nutrients[this.fatEnergyId] = 100 *  (9 * food.nutrients[this.fatId]) / food.nutrients[this.energyId];
      }
    
      this.foods.push(food);
      this.$refs.foodPicker.hide();
    },
    deleteFood(index){
      this.foods.splice(index,1);
    },
    selectGroup(groupId) {
      this.selectedGroup = groupId;
    },
  },
  created(){
    this.$store.dispatch(constants.FETCH_NUTRIENTS, { }).then(() => {
      this.selectedGroup = this.nutrientGroups[0].id;
      this.$store.commit(constants.LOADING_DONE);
    });
    this.$store.dispatch(constants.FETCH_LATEST_FOODS, { });
    this.$store.dispatch(constants.FETCH_MOST_USED_FOODS, { });
    this.$store.dispatch(constants.FETCH_MY_FOODS, { });
  }
}
