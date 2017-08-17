<template>
    <div>
    </div>
</template>

<script>
    var constants = require('../store/constants')
    var api = require('../api');

module.exports = {
    data () { return {}},
    components: {},
    methods: {},
    created: function () {
        var self = this;

        var client = self.$route.params.client;
        var refreshToken = self.$route.params.refreshToken;
        var accessToken = self.$route.params.accessToken;

        self.$store.dispatch(constants.STORE_TOKENS, {
            client,
            refreshToken,
            accessToken,
            success: function () {
                if(client == 'web'){
                    window.location = '/';
                }
            },
            failure: function () {
                toaster(self.$t('failed'));
            }
        });
    }
}
</script>

<style scoped>
</style>