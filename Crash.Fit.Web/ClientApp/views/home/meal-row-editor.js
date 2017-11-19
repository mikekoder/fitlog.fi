﻿import constants from '../../store/constants'
import api from '../../api'
import formatters from '../../formatters'
import toaster from '../../toaster'
import FoodPicker from '../foods/food-picker'

export default {
    name: 'meal-row-editor',
    data () {
        return {
            food: undefined,
            quantity: undefined,
            portion: undefined
        }
    },
    props: {
        show: false,
        row: undefined
    },
    computed: {
        canSave() {
            return this.food && this.quantity;
        }
    },
    components: {
        FoodPicker
    },
    methods: {
        cancel() {
            this.$emit('close');
        },
        save() {
            var self = this;
            var row = {
                id: self.row.id,
                mealDefinitionId: self.row.mealDefinitionId,
                mealId: self.row.mealId,
                //food: self.food,
                foodId: self.food.id,
                foodName: self.food.name,
                quantity: self.quantity,
                //portion: self.portion,
                portionId: self.portion ? self.portion.id : undefined,
                portionName: self.portion ? self.portion.name : undefined
            };
            this.$emit('save', row);
        }
    },
    mounted() {
        var self = this;
        if (self.row.foodId) {
            self.$store.dispatch(constants.FETCH_FOOD, {
                id: self.row.foodId,
                success(food) {
                    self.food = food;
                    if (self.row.portionId) {
                        self.portion = food.portions.find(p => p.id == self.row.portionId);
                    }
                },
                failure() {
                }
            });
        }
        self.quantity = self.row.quantity;
    }
}