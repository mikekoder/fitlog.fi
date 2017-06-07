<template>
    <div>
        <section class="content-header">
        </section>
        <section class="content" v-if="isLoggedIn">
        </section>
        <section class="content" v-if="!isLoggedIn">
        </section>
</div>
</template>

<script>
    var constants = require('../store/constants')

module.exports = {
    data () {
        return {
        }
    },
    components: {},
    methods: {},
    created () {
        this.$store.commit(constants.LOADING_DONE);
    }
}
</script>

<style scoped>
</style>