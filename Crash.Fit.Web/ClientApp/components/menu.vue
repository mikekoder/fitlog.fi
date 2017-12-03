<template>
    <section class="sidebar">
        <ul class="sidebar-menu">
            <router-link tag="li" :to="{ name: 'home'}" exact>
                <a><i class="fa fa-home"></i> <span>{{ $t("home") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'profile'}" v-if="isLoggedIn">
                <a><i class="fa fa-user"></i> <span>{{ $t("profile") }}</span></a>
            </router-link>
            <li class="header">{{ $t("nutrition") }}</li>
            <router-link tag="li" :to="{ name: 'meals'}" v-if="isLoggedIn">
                <a><i class="fa fa-cutlery"></i> <span>{{ $t("meals") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'meal-details', params: { id: constants.NEW_ID } }" exact v-if="!isLoggedIn">
                <a><i class="fa fa-cutlery"></i> <span>{{ $t("mealCalculator") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'mealrhythm'}" v-if="isLoggedIn">
                <a><i class="fa fa-clock-o"></i> <span>{{ $t("mealRhythm") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'foods'}" v-if="isLoggedIn">
                <a><i class="fa fa-apple"></i> <span>{{ $t("foods") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'recipes'}" v-if="isLoggedIn">
                <a><i class="fa fa-book"></i> <span>{{ $t("recipes") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'nutrients'}" v-if="isLoggedIn">
                <a><i class="fa fa-flask"></i> <span>{{ $t("nutrients") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'nutrition-goals'}" v-if="isLoggedIn">
                <a><i class="fa fa-crosshairs"></i> <span>{{ $t("nutritionGoals") }}</span></a>
            </router-link>
            <li class="header">{{ $t("training") }}</li>
            <router-link tag="li" :to="{ name: 'workouts'}" v-if="isLoggedIn">
                <a><i class="fa fa-heartbeat"></i> <span>{{ $t("workouts") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'energy-expenditures'}" v-if="isLoggedIn">
                <a><i class="fa fa-fire"></i> <span>{{ $t("energyExpenditures") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'exercises'}" v-if="isLoggedIn">
                <a><i class="fa fa-universal-access"></i> <span>{{ $t("exercises") }}</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'routines'}" v-if="isLoggedIn">
                <a><i class="fa fa-calendar"></i> <span>{{ $t("routines") }}</span></a>
            </router-link>
            <!--
            <router-link tag="li" :to="{ name: 'training-goals'}" v-if="isLoggedIn">
                <a><i class="fa fa-crosshairs"></i> <span>{{ $t("trainingGoals") }}</span></a>
            </router-link>
                -->
            <router-link tag="li" :to="{ name: 'rep-calculator'}">
                <a><i class="fa fa-calculator"></i> <span>{{ $t("repCalculator") }}</span></a>
            </router-link>
            <li class="header" v-if="isLoggedIn">{{ $t("measurements") }}</li>
            <router-link tag="li" :to="{ name: 'measurements'}" v-if="isLoggedIn">
                <a><i class="fa fa-balance-scale"></i> <span>{{ $t("measurements") }}</span></a>
            </router-link>
            <!--
            <li class="header" v-if="isLoggedIn">{{ $t("getInvolved") }}</li>
            <router-link tag="li" :to="{ name: 'bugs'}" v-if="isLoggedIn">
              <a>
                <i class="fa fa-bug"></i>
                <span>{{ $t("bugs") }}</span>
              </a>
            </router-link>
            <router-link tag="li" :to="{ name: 'improvements'}" v-if="isLoggedIn">
              <a>
                <i class="fa fa-lightbulb-o"></i>
                <span>{{ $t("improvements") }}</span>
              </a>
            </router-link>
                -->
            <li class="header">{{ $t("follow") }}</li>
            <li>
                <a href="https://www.facebook.com/fitlog.fi/"><i class="fa fa-facebook"></i><span>fitlog.fi</span></a>
            </li>
            <li>
                <a href="https://www.instagram.com/fitlog.fi/"><i class="fa fa-instagram"></i><span>fitlog.fi</span></a>
            </li>
            
        </ul>
    </section>


</template>

<script>
    import constants from '../store/constants'
export default {
    //name: 'toolbar',
    data() {
        return {
            constants
        }
    },
    methods: {
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>