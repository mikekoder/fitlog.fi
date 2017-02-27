<template>
    <transition name="md-router">
        <router-view></router-view>
    </transition>
</template>

<script>
module.exports =  {
    name: 'App',
    data: function () {
        return { }
    },
    components: {
        'mainmenu': require('./components/menu')
    }
}
</script>

<style>
</style>