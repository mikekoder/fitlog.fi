import constants from '../../store/constants'
import api from '../../api'
import formatters from '../../formatters'
import utils from '../../utils'
import toaster from '../../toaster'
var defaultNutrientPortion = { id: undefined, name: '100g' };
import nutrientsMixin from '../../mixins/nutrients'
import nutrientGroupsMixin from '../../mixins/nutrient-groups'

export default {
    mixins: [nutrientsMixin, nutrientGroupsMixin],
    data () {
        return {
            id: null,
            name: null,
            manufacturer: null,
            nutrients: {},
            portions: [],
            tab: 'nutrients',
            groupOpenStates: {},
            nutrientPortion: defaultNutrientPortion
        }
    },
    computed: {
        nutrientsGrouped() {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        nutrientPortions(){
            var portions = [];
            portions.push(defaultNutrientPortion);
            if(this.portions.length === 0){
                portions.push({ name: this.$t('portion'), nutrientPortion: true });
            }
            else {
                this.portions.forEach(p => { portions.push(p); });
            }
            return portions;
        },
        errors(){
            var errors = [];
            if(!this.name){
                errors.push('Nimi puuttuu');
            }
            this.portions.forEach(p => {
                if(!p.name){
                    errors.push('Annoksen nimi puuttuu');
                }
                else if(!p.weight || p.weight == ''){
                    errors.push('Annoksen "'+p.name+'" paino puuttuu');
                }
            });
            return errors;
        },
        isValid(){
            return this.errors.length === 0;
        }
    },
    components: {
    },
    methods: {
        changeNutrientPortion(){
            if(this.nutrientPortion.nutrientPortion && this.portions.length === 0){
                this.portions.push(this.nutrientPortion);
            }
        },
        addPortion(){
            this.portions.push({ name: null, weight: null});
        },
        deletePortion(index) {
            if(this.nutrientPortion == this.portions[index]){
                this.nutrientPortion = defaultNutrientPortion;
            }
            this.portions.splice(index, 1); 
        },
        weight(quantity, portion) {
            if (!quantity) {
                return '';
            }
            if (typeof (quantity) !== 'number') {
                quantity = parseFloat(quantity.replace(',', '.'));
            }

            if (portion) {
                return quantity * portion.weight;
            }
            return quantity;
        },
        save() {
            var self = this;
            var food = {
                id: self.id,
                name: self.name,
                nutrients: [],
                portions: self.portions ? self.portions.map(p => { return { id: p.id, name: p.name, weight: utils.parseFloat(p.weight), nutrientPortion: p === self.nutrientPortion }}) : []
            };
            for (var i in self.nutrients) {
                if (self.nutrients[i] || self.nutrients[i] == 0) {
                    food.nutrients.push({ nutrientId: i, amount: utils.parseFloat(self.nutrients[i]) });
                }
            }

            self.$store.dispatch(constants.SAVE_FOOD, {
                food,
                success() {
                    self.$router.replace({ name: 'foods' });
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            });
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteFood() {
            var self = this;
            self.$store.dispatch(constants.DELETE_FOOD, {
                food: { id: self.id },
                success() {
                    self.$router.push({ name: 'foods' });
                },
                failure() {
                    toaster.error(self.$t('deleteFailed'));
                }
            });
        },
        unit: formatters.formatUnit,
        decimal(value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        },
        toggleGroup(group) {
            this.$set(this.groupOpenStates, group, !(this.groupOpenStates[group] && true))
        },
        groupIsExpanded(group) {
            return this.groupOpenStates[group] && true;
        },
        populate(food) {
            var self = this;
            self.id = food.id;
            self.name = food.name;
            self.portions = food.portions || [];
            if(food.nutrientPortionId){
                self.nutrientPortion = self.portions.find(p => p.id === food.nutrientPortionId);
            }
            self.$store.dispatch(constants.FETCH_NUTRIENTS, {
                success() {
                    for (var i in self.nutrientsGrouped) {
                        var group = self.nutrientsGrouped[i];
                        for (var j in group) {
                            var nutrient = group[j];
                            var value = food.nutrients ? food.nutrients.find(n => n.nutrientId == nutrient.id) : undefined;
                            if (value) {
                                if(self.nutrientPortion && self.nutrientPortion != defaultNutrientPortion){
                                    self.nutrients[nutrient.id] = value.portionAmount;                                   
                                }
                                else {
                                    self.nutrients[nutrient.id] = value.amount;
                                }
                            }
                            else {
                                self.nutrients[nutrient.id] = undefined;
                            }
                        }
                    }
                    self.$store.commit(constants.LOADING_DONE);
                },
                failure() {
                    toaster.error(self.$t('foodDetails.fetchFailed'));
                }
            });
        }
    },
    created() {
        var self = this;
        var id = self.$route.params.id;
        if (id == constants.NEW_ID) {
            self.populate({ id: undefined, name: undefined, nutrients: []});
        }
        else {
            self.$store.dispatch(constants.FETCH_FOOD, {
                id,
                success(food) {
                    self.populate(food);
                },
                failure() {
                    toaster(self.$t('foodDetails.fetchFailed'));
                }
            });
        }

        self.toggleGroup(self.$nutrientGroups[0].id);
    }
}