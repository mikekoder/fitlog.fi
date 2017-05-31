<template>
    <section class="sidebar">
        <ul class="sidebar-menu">
            <li class="header">{{ $t("menu.nutritionHeader") }}</li>
            <router-link tag="li" :to="{ name: 'meals'}" v-if="isLoggedIn">
                <a><i class="fa fa-cutlery"></i> <span>{{ $t("menu.meals") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'meal-calculator'}" exact v-if="!isLoggedIn">
                <a><i class="fa fa-cutlery"></i> <span>{{ $t("menu.mealCalculator") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'foods'}" v-if="isLoggedIn">
                <a><i class="fa fa-apple"></i> <span>{{ $t("menu.foods") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'recipes'}" v-if="isLoggedIn">
                <a><i class="fa fa-book"></i> <span>{{ $t("menu.recipes") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'nutrients'}" v-if="isLoggedIn">
                <a><i class="fa fa-flask"></i> <span>{{ $t("menu.nutrients") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'nutrition-targets'}" v-if="isLoggedIn">
                <a><i class="fa fa-crosshairs"></i> <span>{{ $t("menu.targets") }}</span></a>
            </router-link>
            <li class="header">{{ $t("menu.trainingHeader") }}</li>
            <router-link tag="li" :to="{ name: 'workouts'}" v-if="isLoggedIn">
                <a><i class="fa fa-trophy"></i> <span>{{ $t("menu.workouts") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'exercises'}" v-if="isLoggedIn">
                <a><i class="fa fa-universal-access"></i> <span>{{ $t("menu.exercises") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'routines'}" v-if="isLoggedIn">
                <a><i class="fa fa-calendar"></i> <span>{{ $t("menu.routines") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'rep-calculator'}">
                <a><i class="fa fa-calculator"></i> <span>{{ $t("menu.repCalculator") }}</span></a>
            </router-link>
            <li class="header" v-if="isLoggedIn">{{ $t("menu.measurementsHeader") }}</li>
            <router-link tag="li" :to="{ name: 'measurements'}" v-if="isLoggedIn">
                <a><i class="fa fa-balance-scale"></i> <span>{{ $t("menu.measurements") }}</span></a>
            </router-link>
            <!--
            <li class="treeview">
                <a href="#">
                    <i class="fa fa-link"></i> <span>Multilevel</span>
                    <span class="pull-right-container">
                        <i class="fa fa-angle-left pull-right"></i>
                    </span>
                </a>
                <ul class="treeview-menu">
                    <li><a href="#">Link in level 2</a></li>
                    <li><a href="#">Link in level 2</a></li>
                </ul>
            </li>-->
        </ul>
    </section>


</template>

<script>

module.exports = {
    //name: 'toolbar',
    data: function() {
        return {
        }
    },
    methods: {
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>