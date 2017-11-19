import constants from '../../store/constants'
import utils from '../../utils'
import api from '../../api'
import formatters from '../../formatters'
import toaster from '../../toaster'
import nutrientsMixin from '../../mixins/nutrients'

export default {
    mixins:[nutrientsMixin],
    data () {
        return {
            nutrientSettings: {},
            groupOpenStates: {}
        }
    },
    computed: {
        groups(){
            return this.$store.state.nutrition.nutrientGroups;
        }
    },
    components: {},
    methods: {
        $nutrientsLoaded(nutrients) {
            var self = this;
            var grouped = {};
            for (var i in nutrients) {
                var nutrient = nutrients[i];
                if (grouped[nutrient.fineliGroup]) {
                    grouped[nutrient.fineliGroup].push(nutrient);
                }
                else {
                    grouped[nutrient.fineliGroup] = [nutrient];
                }

            }
            self.nutrientSettings = grouped;
            self.$store.commit(constants.LOADING_DONE);
        },
        toggleGroup(group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        moveNutrientUp(group, index) {
            var group = this.nutrientSettings[group];
            var nutrient = group[index];
            group.splice(index, 1);
            group.splice(index - 1, 0, nutrient);
        },
        moveNutrientDown(group, index) {
            var group = this.nutrientSettings[group];
            var nutrient = group[index];
            group.splice(index, 1);
            group.splice(index + 1, 0, nutrient);
        },
        save() {
            var self = this;
            var settings = [];
            for (var i in self.nutrientSettings) {
                for (var j in self.nutrientSettings[i]) {
                    var nutrient = self.nutrientSettings[i][j];
                    settings.push({ nutrientId: nutrient.id, userHideSummary: nutrient.userHideSummary, userHideDetails: nutrient.userHideDetails })
                }
            }
            self.$store.dispatch(constants.SAVE_NUTRIENT_SETTINGS, {
                settings,
                success() {
                    toaster.info(self.$t('saveSuccessful'));
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            });
        },
        unit: formatters.formatUnit
    },
    created() {
    }
}