<template>
    <div>
        <ul class="nav nav-sidebar">
            <li>NUTRITION</li>
            <router-link tag="li" to="/meals">
                <a>Meals</a>
            </router-link>
            <router-link tag="li" to="/foods">
                <a>Foods</a>
            </router-link>
            <router-link tag="li" to="/nutrients">
                <a>Nutriens</a>
            </router-link>
        </ul>
        <ul class="nav nav-sidebar">
            <li>TRAINING</li>
            <router-link tag="li" to="/workouts">
                <a>Workouts</a>
            </router-link>
            <router-link tag="li" to="/exercises">
                <a>Exercises</a>
            </router-link>
            <router-link tag="li" to="/routines">
                <a>Routines</a>
            </router-link>
        </ul>
    </div>
</template>

<script>


module.exports = {
    //name: 'toolbar',
    data: function() {
        return { }
    },
    methods: {
    },
    created () {
        // this.updateLoggedInStatus()
        // then register hook
        this.$router.beforeEach((to, from, next) => {
            // this.updateLoggedInStatus()
            next()
        })
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>