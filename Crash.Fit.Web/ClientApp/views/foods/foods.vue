<template>
    <div>
        <div v-if="!selectedFood">
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
                                    <td><router-link :to="{ name: 'foods', params: { id: food.id } }">{{ food.name }}</router-link></td>
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
        <div v-if="selectedFood">
            <section class="content-header"><h1>{{ $t("foods.foodDetails") }}</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <food-editor v-bind:food="selectedFood" v-bind:saveCallback="saveFood" v-bind:cancelCallback="cancelFood" v-bind:deleteCallback="deleteFood" />
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var toaster = require('../../toaster');

module.exports = {
    data () {
        return {
            foods: [],
            selectedFood: null
        }
    },
    components: {
        'food-editor': require('./food-editor')
    },
    methods: {
        fetchFoods: function(){
            var self = this;
            self.foods = [];
            api.listFoods().then(function (foods) {
                for (var i in foods) {
                    self.foods.push(foods[i]);
                }
            }).fail(function () {
                toaster.error(self.$t('foods.fetchError'));
            });
        },
        createFood: function(){
            this.showFood({id: null, name: null});
        },
        editFood: function (id) {
            var self = this;
            api.getFood(id).then(function (foodDetails) {
                self.showFood(foodDetails);
            }).fail(function () {
                toaster.error('Ruoka-aineen haku epäonnistui');
            });
        },
        saveFood: function (food) {
            var self = this;
            api.saveFood(food).then(function (savedFood) {
                self.fetchFoods();
                self.$router.push({ name: 'foods' });
                self.showSummary();
            }).fail(function () {
                toaster.error(self.$t('foods.saveError'));
            });
        },
        cancelFood: function (food) {
            this.$router.push({ name: 'foods' });
            this.showSummary();
        },
        deleteFood: function (food) {
            var self = this;
            api.deleteFood(food.id).then(function () {
                self.fetchFoods();
                self.$router.push({ name: 'foods' });
                self.showSummary();
            }).fail(function () {
                toaster.error(self.$t('foods.deleteError'));
            });
            
        },
        showFood: function (food) {
            this.selectedFood = food;
        },
        showSummary() {
            this.selectedFood = null;
        },
        
    },
    created: function () {
        this.fetchFoods();
        var id = this.$route.params.id;
        if (id) {
            this.editFood(id);
        }
    },
    beforeRouteUpdate (to, from, next) {
        if (to.params.id) {
            this.editFood(to.params.id);
        }
        else {
            this.showSummary();
        }
        next();
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