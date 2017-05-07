<template>
    <div>
        <div v-if="!selectedRecipe">
            <section class="content-header"><h1>Reseptit</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createRecipe"><i class="glyphicon glyphicon-plus"></i> Uusi resepti</button>
                    </div>
                </div>
                <div class="row" v-if="recipes.length > 0">
                    <div class="col-sm-12">
                        <table class="table" id="recipe-list">
                            <thead>
                                <tr>
                                    <th>Nimi</th>
                                    <th>K&auml;ytt&ouml;kerrat</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="recipe in recipes">
                                    <td><router-link :to="{ name: 'recipes', params: { id: recipe.id } }">{{ recipe.name }}</router-link></td>
                                    <td>{{ recipe.usageCount }}</td>
                                    <td><button class="btn btn-danger btn-xs" @click="deleteRecipe(recipe)">Poista</button></td>
                                </tr>   
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="row" v-if="recipes.length == 0">
                    <div class="col-sm-12">
                        Ei reseptejä
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
    var toaster = require('../../toaster');

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
            }).fail(function () {
                toaster.error('Reseptien haku epäonnistui');
            });
        },
        showNutrients: function(group){
            this.selectedGroup = group;
        },

        createRecipe: function(){
            this.showRecipe({});
        },
        editRecipe: function (id) {
            var self = this;
            api.getRecipe(id).then(function (recipeDetails) {
                self.showRecipe(recipeDetails);
            }).fail(function () {
                toaster.error('Reseptin haku epäonnistui');
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
                self.$router.push({ name: 'recipes' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Reseptin tallennus epäonnistui');
            });
        },
        cancelRecipe: function (recipe) {
            this.$router.push({ name: 'recipes' });
            this.showSummary();
        },
        deleteRecipe: function (recipe) {
            var self = this;
            api.deleteRecipe(recipe.id).then(function () {
                self.fetchRecipes();
                self.$router.push({ name: 'recipes' });
                self.showSummary();
            }).fail(function () {
                toaster.error('Reseptin poistaminen epäonnistui');
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
        var id = this.$route.params.id;
        if (id) {
            this.editRecipe(id);
        }
    },
    beforeRouteUpdate (to, from, next) {
        if (to.params.id) {
            this.editRecipe(to.params.id);
        }
        else {
            this.showSummary();
        }
        next();
    }
}
</script>

<style scoped>
    #recipe-list
    {
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    #recipe-list td {
        padding-bottom: 0px;
    }
    #recipe-list td span{
        margin: 5px;
    }
   
</style>