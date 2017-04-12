<template>
    <div>
        <div v-if="!selectedFood">
            <section class="content-header"><h1>Ruoka-aineet</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createFood"><i class="glyphicon glyphicon-plus"></i> Uusi ruoka-aine</button>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <table class="table" id="food-list">
                            <thead>
                                <tr>
                                    <th>Nimi</th>
                                    <th>K&auml;ytt&ouml;kerrat</th>
                                    <th>Ravintoarvoja</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="food in foods">
                                    <td>{{ food.name }}</td>
                                    <td>{{ food.usageCount }}</td>
                                    <td>{{ food.nutrientCount }}</td>
                                    <td><button class="btn" @click="editFood(food)">Tiedot</button></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedFood">
            <section class="content-header"><h1>Ruoka-aineen tiedot</h1></section>
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
            });
        },
        createFood: function(){
            this.showFood({id: null, name: null});
        },
        editFood: function (food) {
            var self = this;
            api.getFood(food.id).then(function (foodDetails) {
                self.showFood(foodDetails);
            });
        },
        saveFood: function (food) {
            var self = this;
            api.saveFood(food).then(function (savedFood) {
                self.fetchFoods();
                self.showSummary();
            });
        },
        cancelFood: function (food) {
            this.showSummary();
        },
        deleteFood: function (food) {
            var self = this;
            api.deleteFood(food.id).then(function () {
                self.fetchFoods();
                self.showSummary();
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