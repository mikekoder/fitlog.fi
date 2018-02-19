import constants from '../../store/constants'
import api from '../../api'
import toaster from '../../toaster'
import nutrientsMixin from '../../mixins/nutrients'

export default {
    mixins:[nutrientsMixin],
    data () {
        return {
            tab: 'own',
            foods: [],
            searchText: undefined,
            searchResults: [],
            topDirection: 'most',
            topNutrient: undefined,
            topResults: []
        }
    },
    computed: {
        nutrients() {
            return this.$nutrients.filter(n => !n.computed);
        }
    },
    methods: {
        createFood(){
            this.$router.push({ name: 'food-details', params: { id: constants.NEW_ID } });
        },
        deleteFood(food) {
            var self = this;
            this.$store.dispatch(constants.DELETE_FOOD, {
                food,
                success() {
                    self.foods.splice(self.foods.findIndex(f => f.id == food.id), 1);
                },
                failure() {
                    toaster(this.$t('deleteFailed'));
                }
            });
        },
        search(){
            var self = this;
            if(self.searchText.length >= 2){
                api.searchFoods(self.searchText).then(results => {
                    self.searchResults = results;
                });
            }
            else {
                self.searchResults = [];
            }
        },
        searchTopFoods(){
            var self = this;
            self.topResults = [];
            if (!self.topNutrient) {
                return;
            }
            if(self.topDirection == 'most'){
                api.searchFoodsMostNutrients(self.topNutrient.id).then(results => {
                    self.topResults = results;
                });
            }
            else{
                api.searchFoodsLeastNutrients(self.topNutrient.id).then(results => {
                    self.topResults = results;
                });
            }
        }
    },
    created() {
        var self = this;
        self.foods = [];
        self.$store.dispatch(constants.FETCH_MY_FOODS, {
            success(foods) {
                self.foods = foods;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() {
                toaster(this.$t('fetchFailed'));
            }
        });
    }
}