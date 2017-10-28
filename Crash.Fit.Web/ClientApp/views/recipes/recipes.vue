<template>
    <div v-if="!loading">     
        <section class="content-header"><h1>{{ $t('recipes') }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="createRecipe">{{ $t('create') }}</button>
                </div>
            </div>
            <div class="row" v-if="recipes.length > 0">
                <div class="col-sm-12">
                    <table class="table" id="recipe-list">
                        <thead>
                            <tr>
                                <th>{{ $t('name') }}</th>
                                <th>{{ $t('usageCount') }}</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="recipe in recipes">
                                <td><router-link :to="{ name: 'recipe-details', params: { id: recipe.id } }">{{ recipe.name }}</router-link></td>
                                <td>{{ recipe.usageCount }}</td>
                                <td><button class="btn btn-danger btn-xs" @click="deleteRecipe(recipe)">{{ $t('delete') }}</button></td>
                            </tr>   
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="recipes.length == 0">
                <div class="col-sm-12">
                  <br />
                    {{ $t('noRecipes') }}
                </div>
            </div>
        </section>
       
    </div>
</template>

<script src="./recipes.js">
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