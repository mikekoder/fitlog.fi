<template>
    <div>
        <section class="content-header"><h1>Aterialaskuri</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <meal-editor v-bind:meal="{}" v-bind:anon="true" />
                </div>
            </div>
        </section>
    </div>
</template>

<script>
module.exports = {
    data () {
        return {}
    },
    components: {
        'meal-editor': require('./meal-editor')
    }
}
</script>

<style scoped>
</style>