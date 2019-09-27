<template>
    <div class="login-box">
        <div class="login-logo">
            <a href="/"><b>fitlog</b>.fi</a>
        </div>
        <div class="login-box-body">
            <div class="form-group has-feedback">
                <label>{{ $t("username") }}/{{ $t("email") }}</label>
                <input type="text" class="form-control" v-model="username" @keyup.enter="requestReset">
            </div>
            <div class="row">
                <div class="col-xs-12">
                    <button class="btn btn-primary btn-block btn-flat" @click="requestReset">{{ $t("sendPasswordResetRequest") }}</button>
                </div>
            </div>
            <div class="row" v-if="success">
                <div class="col-xs-12">
                    {{ $t('resetPasswordRequestSuccess') }}
                </div>
            </div>
            <router-link :to="{ name: 'login' }" class="text-center">{{ $t("login") }}</router-link>
        </div>
    </div>
</template>

<script>
    import constants from '../store/constants'
    import api from '../api'

export default {
    data ()
    {
        return {
            username: null,
            success: false
        }
    },
    components: {},
    methods: {
        requestReset() {
            var self = this;
            api.requestPasswordReset(this.username).then((response) => {
                self.success = true;
            });
        }
    },
    created() {
        this.$store.commit(constants.LOADING_DONE);
    }
}
</script>

<style scoped>
</style>