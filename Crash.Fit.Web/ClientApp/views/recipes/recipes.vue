<template>
    <div>
        <div v-if="!selectedRecipe">
            <section class="content-header"><h1>Reseptit</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createRecipe"><i class="glyphicon glyphicon-plus"></i> Uusi resepti</button>
                        <table class="table" id="recipe-summary">
                            <thead>
                                <tr>
                                    <th>Nimi</th>
                                    <th></th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="recipe in recipes">
                                    <td><span>{{ recipe.name }}</span></td>
                                    <td>
                                        <button @click="editRecipe(recipe)">Muokkaa</button>
                                    </td>
                                    <td></td>
                                </tr>   
                            </tbody>
                        </table>
                    </div>
                </div>
            </section>
        </div>
        <div v-if="selectedRecipe">
            <section class="content-header"><h1>Reseptin tiedot</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <recipe-editor v-bind:recipe="selectedRecipe" v-bind:saveCallback="saveRecipe" v-bind:cancelCallback="cancelRecipe" v-bind:deleteCallback="deleteRecipe" />
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var formatters = require('../../formatters')
    var c3 = require('c3');
    var moment = require('moment');

module.exports = {
    data () {
        return {
            recipes: [],
            selectedRecipe: null
        }
    },
    components: {
        'recipe-editor': require('./recipe-editor')
    },
    methods: {
        fetchRecipes: function () {
            var self = this;
            self.recipes = [];
            api.listRecipes().then(function (recipes) {
                for (var i in recipes) {
                    self.recipes.push(recipes[i]);
                }
            });
        },
        showNutrients: function(group){
            this.selectedGroup = group;
        },

        createRecipe: function(){
            this.showRecipe({});
        },
        editRecipe: function (recipe) {
            var self = this;
            api.getRecipe(recipe.id).then(function (recipeDetails) {
                self.showRecipe(recipeDetails);
            });
            
        },
        saveRecipe: function (recipe) {
            var ingredients = [];
            var portions = [];
            for (var i in recipe.ingredients) {
                var recipeIngredient = recipe.ingredients[i];
                var ingredient = {};
                if (recipeIngredient.quantity) {
                    if (typeof (recipeIngredient.quantity === 'number')) {
                        ingredient.quantity = recipeIngredient.quantity;
                    }
                    else {
                        ingredient.quantity = parseFloat(recipeIngredient.quantity.replace(',', '.'));
                    }
                }
                if (recipeIngredient.food) {
                    ingredient.foodId = recipeIngredient.food.id;
                }
                if (recipeIngredient.portion) {
                    ingredient.portionId = recipeIngredient.portion.id;
                }
                ingredients.push(ingredient);
            }
            for (var i in recipe.portions) {
                var recipePortion = recipe.portions[i];
                var portion = { id: recipePortion.id, name: recipePortion.name };
                if (recipePortion.weight) {
                    if (typeof (recipePortion.weight === 'number')) {
                        portion.weight = recipePortion.weight;
                    }
                    else {
                        portion.weight = parseFloat(recipePortion.weight.replace(',', '.'));
                    }
                }
                portions.push(portion);
            }
            var recipe = {
                id: recipe.id,
                name: recipe.name,
                ingredients: ingredients,
                portions: portions
            };
            var self = this;
            api.saveRecipe(recipe).then(function (savedRecipe) {
                self.fetchRecipes();
                self.showSummary();
            });
        },
        cancelRecipe: function (recipe) {
            this.showSummary();
        },
        deleteRecipe: function (recipe) {
            var self = this;
            api.deleteRecipe(recipe.id).then(function () {
                self.showSummary();
            });
           
        },
        showRecipe: function (recipe) {
            this.selectedRecipe = recipe;
        },
        showSummary: function(){
            this.selectedRecipe = undefined;
        },
        date: formatters.formatDate,
        time: formatters.formatTime,
        unit: formatters.formatUnit
    },
    watch:{
        $route: function(){
            console.log(this.$route);
        }
    },
    created: function () {
        this.fetchRecipes();
    }
}
</script>

<style scoped>
    li.custom-date{
        padding: 3px 10px;
    }
    li.custom-date button{
        margin-top: 3px;
    }
    .outer {
      position: relative;
    }
    .inner {
      overflow-x: auto;
      overflow-y: visible;
      margin-left: 100px;
    }
    #recipe-summary{
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    #recipe-summary td {
        padding-bottom: 0px;
    }
    #recipe-summary td span{
        margin: 5px;
    }
    .freeze {
      position: absolute;
      margin-left: -100px;
      width: 80px;
    }


   
    th.time{
        width: 80px;
    }

    th.nutrient{
        width: 70px;
    }
    td div.chart{
        position: relative;
        top: -10px;
    }
    
   
</style>