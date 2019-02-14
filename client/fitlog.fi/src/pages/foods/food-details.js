import constants from '../../store/constants'
import utils from '../../utils'
import Help from './food-help'
import api from '../../api'
import PageMixin from '../../mixins/page'

var defaultNutrientPortion = { id: undefined, name: '100g', value: undefined, label: '100g' };

export default {
    mixins: [PageMixin],
    components: {
        'food-help': Help
    },
    data() {
        return {
            id: null,
            name: null,
            manufacturer: null,
            ean: null,
            nutrients: {},
            portions: [],
            tab: 'tab-1',
            selectedGroup: undefined,
            nutrientPortion: defaultNutrientPortion
        }
    },
    computed: {
        nutrientGroups() {
            return this.$store.state.nutrition.nutrientGroups;
        },
        nutrientsGrouped() {
            return this.$store.state.nutrition.nutrientsGrouped;
        },
        nutrientPortions() {
            var portions = [];
            portions.push({...defaultNutrientPortion, value: defaultNutrientPortion});
            if (this.portions.length === 0) {
                var portion = { name: this.$t('portion'), label:this.$t('portion'), nutrientPortion: true, weight: 100 }
                portions.push({...portion, value: portion});
            }
            else {
                this.portions.forEach(p => { portions.push({label: p.name, value: p}); });
            }
            return portions;
        },
        errors() {
            var errors = [];
            if (!this.name) {
                errors.push('Nimi puuttuu');
            }
            this.portions.forEach(p => {
                if (!p.name) {
                    errors.push('Annoksen nimi puuttuu');
                }
                else if (!p.weight || p.weight == '') {
                    errors.push('Annoksen "' + p.name + '" paino puuttuu');
                }
            });
            return errors;
        },
        isValid() {
            return this.errors.length === 0;
        },
        canSave(){
            return this.name && true;
        }
    },
    methods: {
        toggleGroup(group) {
            if (this.selectedGroup == group) {
                this.selectedGroup = undefined;
            }
            else {
                this.selectedGroup = group;
            }
        },
        changeNutrientPortion() {
            if (this.nutrientPortion.nutrientPortion && this.portions.length === 0) {
                this.portions.push(this.nutrientPortion);
            }
        },
        addPortion() {
            this.portions.push({ name: null, weight: null });
        },
        removePortion(index) {
            if (this.nutrientPortion == this.portions[index]) {
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
            var food = {
                id: this.id,
                name: this.name,
                manufacturer: this.manufacturer,
                ean: this.ean,
                nutrients: [],
                portions: this.portions ? this.portions.map(p => { return { id: p.id, name: p.name, weight: utils.parseFloat(p.weight), nutrientPortion: p === this.nutrientPortion } }) : []
            };
            for (var i in this.nutrients) {
                if (this.nutrients[i] || this.nutrients[i] == 0) {
                    food.nutrients.push({ nutrientId: i, amount: utils.parseFloat(this.nutrients[i]) });
                }
            }

            this.$store.dispatch(constants.SAVE_FOOD, {
                food
            }).then(_ => {
                this.$router.replace({ name: 'foods' });
            }).catch(_ => {
                this.notifyError(this.$t('saveFailed'));
            });
        },
        cancel() {
            this.$router.go(-1);
        },
        deleteFood() {
            this.$store.dispatch(constants.DELETE_FOOD, {
                food: { id: this.id }
            }).then(_ => {
                this.$router.push({ name: 'foods' });
            }).catch(_ => {
                this.notifyError(this.$t('deleteFailed'));
            });
        },
        populate(food) {
            this.id = food.id;
            this.name = food.name;
            this.manufacturer = food.manufacturer;
            this.ean = food.ean;
            this.portions = food.portions || [];
            if (food.nutrientPortionId) {
                this.nutrientPortion = this.portions.find(p => p.id === food.nutrientPortionId);
            }
            this.$store.dispatch(constants.FETCH_NUTRIENTS, { }).then(_ => {
                for (var i in this.nutrientsGrouped) {
                    var group = this.nutrientsGrouped[i];
                    for (var j in group) {
                        var nutrient = group[j];
                        var value = food.nutrients ? food.nutrients.find(n => n.nutrientId == nutrient.id) : undefined;
                        if (value) {
                            if (this.nutrientPortion && this.nutrientPortion != defaultNutrientPortion) {
                                this.$set(this.nutrients, nutrient.id, value.portionAmount);
                            }
                            else {
                              this.$set(this.nutrients, nutrient.id, value.amount);
                            }
                        }
                        else {
                          this.$set(this.nutrients, nutrient.id, undefined);
                        }
                    }
                }
                this.selectedGroup = this.nutrientGroups[0];
                this.$store.commit(constants.LOADING_DONE);
            }).catch(_ => {
                this.notifyError(this.$t('fetchFailed'));
            });
        },
        showHelp(){
            this.$refs.help.open();
        },
        readBarcode(){
            try {    
                cordova.plugins.barcodeScanner.scan(
                    result => {
                        if(!result.canceled){
                            this.ean = result.text;
                            this.loadInfoByEan();
                        }
                    },
                    error => {
                        this.notifyError(error);
                    }
                );
            }
            catch(err){
                this.notifyError(err.message);
            }
        },
        loadInfoByEan() {
            api.searchExternalFood(this.ean).then(response => {
                var food = response.data;
                if(!this.name){
                    this.name = food.name;
                }
                if(!this.manufacturer){
                    this.manufacturer = food.manufacturer;
                }
                food.nutrients.forEach(n => {
                    if(!this.nutrients[n.nutrientId]){
                      this.$set(this.nutrients, n.nutrientId, n.amount);
                    }
                });
                this.notifySuccess(this.$t('informationUpdated'));
            }).fail(xhr => {
                this.notifyError(this.$t('fetchFailed'));
            });
        }
    },
    created() {
        var id = this.$route.params.id;
        if (id == constants.NEW_ID) {
            this.populate({ id: undefined, name: undefined, nutrients: [] });
        }
        else {
            this.$store.dispatch(constants.FETCH_FOOD, {
                id
            }).then(food => {
                this.populate(food);
            }).catch(_ => {
                this.notifyError(this.$t('fetchFailed'));
            });
        }

    },
    mounted(){
        if(!this.name){
            this.$refs.nameInput.focus();
        }
    }
}
