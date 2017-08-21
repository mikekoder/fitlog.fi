<template>
    <div>
        <section class="content-header">
        </section>
        <section class="content">
          Fitlog on ilmainen ruoka- ja treenipäiväkirja.
          <div  v-if="isLoggedIn"></div>
          <div  v-if="!isLoggedIn"></div>
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