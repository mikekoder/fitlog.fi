<template>
    <div>
        <div v-if="!selectedFood">
            <section class="content-header"><h1>Ruoka-aineet</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createFood"><i class="glyphicon glyphicon-plus"></i> Uusi ruoka-aine</button>
                    
                        <table class="table" id="meal-summary">
                            <thead>
                                <tr>
                                    <th>Nimi</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="food in foods">
                                    <td>{{ food.name }}</td>
                                    <td><button class="btn">Tiedot</button></td>
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
        fetchFoods: function () {
            var self = this;
            api.listFoods().then(function (foods) {
            });
        },
        createFood: function(){
            this.showFood({});
        },
        editFood: function(food){
            this.showFood(food);
        },
        saveFood: function (food) {
        },
        cancelFood: function (food) {
            this.showList();
        },
        deleteFood: function (food) {
            this.showList();
        },
        showFood: function (food) {
            this.selectedFood = food;
        },
        showSummary() {
            this.selectedFood = null;
        },
        
    },
    watch:{
        $route: function(){
            console.log(this.$route);
        }
    },
    mounted: function () {
        this.fetchFoods();
    }
}
</script>

<style scoped>
</style>