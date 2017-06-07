<template>
    <div class="login-box">
        <div class="login-logo">
            <a href="/"><b>fitlog</b>.fi</a>
        </div>
        <div class="login-box-body">
            <div class="form-group has-feedback">
                <label>{{ $t("login.username") }}</label>
                <input type="text" class="form-control" v-model="username">
            </div>
            <div class="form-group has-feedback">
                <label>{{ $t("login.password") }}</label>
                <input type="password" class="form-control" v-model="password">
            </div>
            <div class="row">
                <div class="col-xs-4 col-xs-offset-8">
                    <button class="btn btn-primary btn-block btn-flat" @click="login">{{ $t("login.login") }}</button>
                </div>
            </div>
            <div class="social-auth-links text-center">
                <p>- {{ $t("login.or") }} -</p>
                <a class="btn btn-block btn-social btn-facebook btn-flat" @click="loginFacebook">
                    <i class="fa fa-facebook"></i> {{ $t("login.useFacebook") }}
                </a>
                <a class="btn btn-block btn-social btn-google btn-flat" @click="loginGoogle">
                    <i class="fa fa-google-plus"></i> {{ $t("login.useGoogle") }}
                </a>
            </div>
            <router-link :to="{ name: 'register' }" class="text-center">{{ $t("login.register") }}</router-link>
        </div>
    </div>
</template>

<script>
    var constants = require('../store/constants')
    var api = require('../api');
module.exports = {
    data ()
    {
        return {
            username: null,
            password: null
        }
    },
    components: {},
    methods: {
        login() {
            var self = this;
            var data = {
                username: this.username,
                password: this.password
            };
            api.login(data).then(function () {
                window.location = '/';
            });
        },
        loginGoogle() {
            //dialog = window.open(api.baseUrl+'users/external-login/?provider=Google','_blank','width=400,height=600');
            window.location = api.baseUrl + 'users/external-login/?provider=Google';
        },
        loginFacebook() {
            //this.dialog = window.open(api.baseUrl + 'users/external-login/?provider=Facebook', '_blank', 'width=400,height=600');
            window.location = api.baseUrl + 'users/external-login/?provider=Facebook';
        }
    },
    created(){
        this.$store.commit(constants.LOADING_DONE);
    },
    mounted() {
        var self = this;
        window.closeDialog = function () {
            console.log(self.dialog);
            self.dialog.close();
            window.location = '/';
        };
    }
}
</script>

<style scoped>
</style>