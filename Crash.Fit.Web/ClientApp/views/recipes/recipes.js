import constants from '../../store/constants'
import api from '../../api'
import toaster from '../../toaster'
import nutrientsMixin from '../../mixins/nutrients'

export default {
    mixins:[nutrientsMixin],
    data () {
        return {
            recipes: []
        }
    },
    computed:{
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
    created() {
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
    }
}