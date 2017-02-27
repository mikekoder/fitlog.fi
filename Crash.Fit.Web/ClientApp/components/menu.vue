<template>
    <section class="sidebar">
        <ul class="sidebar-menu">
            <li class="header">RAVINTO</li>
            <router-link tag="li" :to="{ name: 'meals'}" exact v-if="isLoggedIn">
                <a><i class="fa fa-cutlery"></i> <span>Ateriat</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'meal-calculator'}" exact v-if="!isLoggedIn">
                <a><i class="fa fa-cutlery"></i> <span>Aterialaskuri</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'foods'}">
                <a><i class="fa fa-apple"></i> <span>Ruoka-aineet</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'recipes'}" v-if="isLoggedIn">
                <a><i class="fa fa-book"></i> <span>Reseptit</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'nutrients'}">
                <a><i class="fa fa-flask"></i> <span>Ravintoaineet</span></a>
            </router-link>
            <li class="header">TREENAUS</li>
            <router-link tag="li" :to="{ name: 'workouts'}" v-if="isLoggedIn">
                <a><i class="fa fa-trophy"></i> <span>Treenit</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'exercises'}" v-if="isLoggedIn">
                <a><i class="fa fa-universal-access"></i> <span>Liikkeet</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'routines'}" v-if="isLoggedIn">
                <a><i class="fa fa-calendar"></i> <span>Ohjelmat</span></a>
            </router-link>
            <router-link tag="li" :to="{ name: 'rep-calculator'}">
                <a><i class="fa fa-calculator"></i> <span>Sarjapainolaskuri</span></a>
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