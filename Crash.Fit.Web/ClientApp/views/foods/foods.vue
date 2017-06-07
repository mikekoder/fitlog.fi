<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("foods.title") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="createFood">{{ $t("foods.create") }}</button>
                </div>
            </div>
            <div class="row" v-if="foods.length > 0">
                <div class="col-sm-12">
                    <table class="table" id="food-list">
                        <thead>
                            <tr>
                                <th>{{ $t("foods.columns.name") }}</th>
                                <th>{{ $t("foods.columns.usageCount") }}</th>
                                <th>{{ $t("foods.columns.nutrientCount") }}</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="food in foods">
                                <td><router-link :to="{ name: 'food-details', params: { id: food.id } }">{{ food.name }}</router-link></td>
                                <td>{{ food.usageCount }}</td>
                                <td>{{ food.nutrientCount }}</td>
                                <td><button class="btn btn-danger btn-xs" @click="deleteFood(food)">{{ $t("delete") }}</button></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="foods.length == 0">
                <div class="col-sm-12">
                    {{ $t("foods.noFoods") }}
                </div>
            </div>
        </section>
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var api = require('../../api');
    var toaster = require('../../toaster');

module.exports = {
    data () {
        return {
            foods: []
        }
    },
    computed:{
        loading: function() {
            return this.$store.state.loading;
        }
    },
    methods: {
        createFood: function(){
            this.$router.push({ name: 'food-details', params: { id: constants.NEW_ID } });
        },
        deleteFood: function (food) {
            var self = this;
            this.$store.dispatch(constants.DELETE_FOOD, {
                food,
                success: function () {
                    self.foods.splice(self.foods.findIndex(f => f.id == food.id), 1);
                },
                failure: function () {
                    toaster(this.$t('foods.deleteFailed'));
                }
            });
        },
    },
    created: function () {
        var self = this;
        self.foods = [];
        self.$store.dispatch(constants.FETCH_MY_FOODS, {
            success: function (foods) {
                self.foods = foods;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure: function () {
                toaster(this.$t('foods.fetchFailed'));
            }
        });
    }
}
</script>

<style scoped>
   #food-list
   {
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    #food-list td {
        padding-bottom: 0px;
    }
    #food-list td span{
        margin: 5px;
    }
   
</style>