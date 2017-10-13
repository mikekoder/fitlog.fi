<template>
    <transition name="md-router">
        <router-view></router-view>
    </transition>
</template>

<script>
export default {
    name: 'App',
    data() {
        return { }
    },
    methods: {
        onResize() {
            if (window.matchMedia('(max-width: 980px)').matches) {
                $('body').addClass('sidebar-collapse').removeClass('sidebar-open');
            }
            else {
                $('body').removeClass('sidebar-collapse').addClass('sidebar-open');
            }
        }
    },
    mounted() {
        window.addEventListener('resize', this.onResize);
    }
}
</script>

<style>
</style>