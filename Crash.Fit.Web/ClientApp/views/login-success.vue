<template>
    <div>
    </div>
</template>

<script>
    import constants from '../store/constants'
    import api from '../api'

export default {
    data () { return {}},
    components: {},
    methods: {},
    created() {
        var self = this;

        var client = self.$route.params.client;
        var refreshToken = self.$route.params.refreshToken;
        var accessToken = self.$route.params.accessToken;

        self.$store.dispatch(constants.STORE_TOKENS, {
            client,
            refreshToken,
            accessToken,
            success() {
                if(client == 'web'){
                    window.location = '/';
                }
            },
            failure() {
                toaster(self.$t('failed'));
            }
        });
    }
}
</script>

<style scoped>
</style>