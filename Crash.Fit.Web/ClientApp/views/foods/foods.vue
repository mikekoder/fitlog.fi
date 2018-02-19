<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("foods") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="createFood">{{ $t("create") }}</button>
                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <ul class="nav nav-tabs">
                        <li class="clickable" :class="{ active: tab === 'own' }"><a @click="tab = 'own'">{{ $t("own") }}</a></li>
                        <li class="clickable" :class="{ active: tab === 'search' }"><a @click="tab = 'search'">{{ $t("search") }}</a></li>
                        <li class="clickable" :class="{ active: tab === 'top100' }"><a @click="tab = 'top100'">{{ $t("top100") }}</a></li>
                    </ul>
                    <div v-if="tab === 'own'">
                        <div class="row" v-if="foods.length > 0">
                            <div class="col-sm-12">
                                <table class="table food-list">
                                    <thead>
                                        <tr>
                                            <th>{{ $t("name") }}</th>
                                            <th>
                                                <span class="hidden-xs">{{ $t("usageCount") }}</span>
                                                <span class="hidden-sm hidden-md hidden-lg" :title="$t('usageCount')" ><i class="fa fa-cutlery"></i></i></span>
                                            </th>
                                            <th>
                                                <span class="hidden-xs">{{ $t("nutrientCount") }}</span>
                                                <span class="hidden-sm hidden-md hidden-lg" :title="$t('nutrientCount')"><i class="fa fa-flask"></i></span>
                                            </th>
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
                                <br />
                                {{ $t("noFoods") }}
                            </div>
                        </div>
                    </div>
                    <div v-if="tab === 'search'">
                        <div class="row">
                            <div class="col-sm-6 col-lg-4">
                                <div class="form-group">
                                    <label>{{ $t("name") }}</label>
                                    <input type="text" class="form-control" v-model="searchText" @keyup="search" />
                                </div>
                            </div>
                        </div>
                        <div class="row" v-if="searchResults.length > 0">
                            <div class="col-sm-12">
                                <table class="table food-list">
                                    <thead>
                                        <tr>
                                            <th>{{ $t("name") }}</th>
                                            <th>
                                                <span class="hidden-xs">{{ $t("usageCount") }}</span>
                                                <span class="hidden-sm hidden-md hidden-lg" :title="$t('usageCount')" ><i class="fa fa-cutlery"></i></i></span>
                                            </th>
                                            <!--
                                            <th>
                                                <span class="hidden-xs">{{ $t("nutrientCount") }}</span>
                                                <span class="hidden-sm hidden-md hidden-lg" :title="$t('nutrientCount')"><i class="fa fa-flask"></i></span>
                                            </th>
                                            <th></th>
                                                -->
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="food in searchResults">
                                            <td><router-link :to="{ name: 'food-details', params: { id: food.id } }">{{ food.name }}</router-link></td>
                                            <td>{{ food.usageCount }}</td>
                                            <!--
                                            <td>{{ food.nutrientCount }}</td>
                                            <td><button class="btn btn-danger btn-xs" @click="deleteFood(food)">{{ $t("delete") }}</button></td>
                                            -->
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class="row" v-else>
                            <div class="col-sm-12">
                                <br />
                                {{ $t("noFoods") }}
                            </div>
                        </div>
                    </div>
                    <div v-if="tab === 'top100'">
                        <div class="row">
                            <div class="col-sm-4 col-lg-2">
                                <div class="form-group">
                                    <label>&nbsp;</label>
                                    <select class="form-control" v-model="topDirection" @change="searchTopFoods">
                                        <option value="most">{{ $t("most") }}</option>
                                        <option value="least">{{ $t("least") }}</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-6 col-lg-3">
                                <div class="form-group">
                                    <label>&nbsp;</label>
                                    <select class="form-control" v-model="topNutrient" @change="searchTopFoods">
                                        <option :value="undefined">{{ $t("chooseNutrient") }}</option>
                                        <option v-for="nutrient in nutrients" :value="nutrient">{{ nutrient.name }}</option>
                                    </select>
                                </div>
                                
                            </div>
                        </div>
                        <div class="row" v-if="topResults.length > 0">
                            <div class="col-sm-12">
                                <table class="table food-list">
                                    <thead>
                                        <tr>
                                            <th>{{ $t("name") }}</th>
                                            <th colspan="2">{{ $t("amount") }}</th>
                                            <!--
                                            <th>
                                                <span class="hidden-xs">{{ $t("usageCount") }}</span>
                                                <span class="hidden-sm hidden-md hidden-lg" :title="$t('usageCount')" ><i class="fa fa-cutlery"></i></i></span>
                                            </th>
                                            <th>
                                                <span class="hidden-xs">{{ $t("nutrientCount") }}</span>
                                                <span class="hidden-sm hidden-md hidden-lg" :title="$t('nutrientCount')"><i class="fa fa-flask"></i></span>
                                            </th>
                                            <th></th>
                                                -->
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="food in topResults">
                                            <td><router-link :to="{ name: 'food-details', params: { id: food.id } }">{{ food.name }}</router-link></td>
                                            <td>{{ formatDecimal(food.nutrientAmount) }}</td>
                                            <td>{{ formatUnit(topNutrient.unit) }}</td>
                                            <!--
                                            <td>{{ food.usageCount }}</td>
                                            <td>{{ food.nutrientCount }}</td>
                                            <td><button class="btn btn-danger btn-xs" @click="deleteFood(food)">{{ $t("delete") }}</button></td>
                                            -->
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class="row" v-else>
                            <div class="col-sm-12">
                                <br />
                                {{ $t("noFoods") }}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</template>

<script src="./foods.js">
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