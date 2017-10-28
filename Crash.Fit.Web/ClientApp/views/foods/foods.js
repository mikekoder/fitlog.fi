import constants from '../../store/constants'
import api from '../../api'
import toaster from '../../toaster'
import nutrientsMixin from '../../mixins/nutrients'

export default {
    mixins:[nutrientsMixin],
    data () {
        return {
            foods: [],
            showOwn: true
        }
    },
    computed:{
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