<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("foods") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="createFood">{{ $t("create") }}</button>
                </div>
            </div>
            <!--
            <div class="row">
                <div class="col-sm-12">
                    <ul class="nav nav-tabs">
                        <li class="clickable" v-bind:class="{ active: showOwn }"><a @click="">{{ $t("own") }}</a></li>
                        <li class="clickable" v-bind:class="{ active: !showOwn }"><a @click="">{{ $t("all") }}</a></li>
                    </ul>
                </div>
            </div>
                -->
            <div v-if="showOwn">
                 <div class="row" v-if="foods.length > 0">
                <div class="col-sm-12">
                    <table class="table" id="food-list">
                        <thead>
                            <tr>
                                <th>{{ $t("name") }}</th>
                                <th>
                                    <span class="hidden-xs">{{ $t("usageCount") }}</span>
                                    <span class="hidden-sm hidden-md hidden-lg" v-bind:title="$t('usageCount')" ><i class="fa fa-cutlery"></i></i></span>
                                </th>
                                <th>
                                    <span class="hidden-xs">{{ $t("nutrientCount") }}</span>
                                    <span class="hidden-sm hidden-md hidden-lg" v-bind:title="$t('nutrientCount')"><i class="fa fa-flask"></i></span>
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
           <div v-if="!showOwn">
               <div class="row">
                   <div class="col-sm-12">

                   </div>
               </div>
           </div>
        </section>
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import api from '../../api'
    import toaster from '../../toaster'

export default {
    data () {
        return {
            foods: [],
            showOwn: true
        }
    },
    computed:{
    },
    methods: {
        createFood(){
            this.$router.push({ name: 'food-details', params: { id: constants.NEW_ID } });
        },
        deleteFood(food) {
            var self = this;
            this.$store.dispatch(constants.DELETE_FOOD, {
                food,
                success() {
                    self.foods.splice(self.foods.findIndex(f => f.id == food.id), 1);
                },
                failure() {
                    toaster(this.$t('deleteFailed'));
                }
            });
        },
    },
    created() {
        var self = this;
        self.foods = [];
        self.$store.dispatch(constants.FETCH_MY_FOODS, {
            success(foods) {
                self.foods = foods;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() {
                toaster(this.$t('fetchFailed'));
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