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
                <div class="row" v-if="foods.length > 0">
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
                                    <td><router-link :to="{ name: 'foods', params: { id: food.id } }">{{ food.name }}</router-link></td>
                                    <td>{{ food.usageCount }}</td>
                                    <td>{{ food.nutrientCount }}</td>
                                    <td><button class="btn btn-danger btn-xs" @click="deleteFood(food)">Poista</button></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="row" v-if="foods.length == 0">
                    <div class="col-sm-12">
                        Ei ruoka-aineita
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
                toaster.error('Ruoka-aineiden haku epäonnistui');
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
                toaster.error('Ruoka-aineen tallennus epäonnistui');
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
                toaster.error('Ruoka-aineen poistaminen epäonnistui');
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