<template>
    <div>
        <section class="content-header">
            <h1>Crash.Fit</h1>
        </section>
        <section class="content" v-if="isLoggedIn">
            Olet kirjautunut
        </section>
        <section class="content" v-if="!isLoggedIn">
            Et ole kirjautunut
        </section>
</div>
</template>

<script>
module.exports = {
    data () {
        return {
        }
    },
    components: {},
    methods: { }
}
</script>

<style scoped>
</style>