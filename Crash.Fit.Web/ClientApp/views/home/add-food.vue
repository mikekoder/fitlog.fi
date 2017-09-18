<template>
    <div class="modal fade" id="modal-default" style="display: block;" v-bind:class="{ in: show }">
        <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
            <button type="button" class="close" @click="cancel">
                <span aria-hidden="true">×</span></button>
            <h4 class="modal-title">{{ $t('addFood') }}</h4>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-sm-12">
                        <label>{{ $t('food') }}</label>
                        <food-picker v-bind:value="food" v-on:change="food=arguments[0]" />
                    </div>
                </div>
                <div class="row" v-if="food">
                    <div class="col-sm-3">
                        <label>{{ $t('quantity') }}</label>
                        <input class="form-control" type="number" v-model="quantity" />
                    </div>
                    <div class="col-sm-9">
                        <label>{{ $t('portion') }}</label>
                        <select class="form-control" v-model="portion">
                            <option v-bind:value="undefined">g</option>
                            <option v-for="p in food.portions" v-bind:value="p">
                                {{ p.name }}
                            </option>
                        </select>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-default pull-left" @click="cancel">{{ $t('cancel')}}</button>
                <button class="btn btn-primary" @click="save" :disabled="!canSave">{{ $t('save')}}</button>
            </div>
        </div>
        </div>
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import api from '../../api'
    import formatters from '../../formatters'
    import toaster from '../../toaster'

export default {
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
        'food-picker': require('../foods/food-picker'),
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
                    toaster.error(self.$t('fetchFailed'));
                }
            });
        }
        self.quantity = self.row.quantity;
    }
}
</script>

<style scoped>
</style>