<template>
    <div class="login-box">
        <div class="login-logo">
            <a href="/"><b>Admin</b>LTE</a>
        </div>
        <div class="login-box-body">
            <div class="form-group has-feedback">
                <label>K&auml;ytt&auml;j&auml;tunnus/S&auml;hk&ouml;postiosoite</label>
                <input type="text" class="form-control" v-model="username">
            </div>
            <div class="form-group has-feedback">
                <label>Salasana</label>
                <input type="password" class="form-control" v-model="password">
            </div>
            <div class="row">
                <div class="col-xs-4 col-xs-offset-8">
                    <button class="btn btn-primary btn-block btn-flat" @click="login">Kirjaudu</button>
                </div>
            </div>
            <div class="social-auth-links text-center">
                <p>- TAI -</p>
                <a class="btn btn-block btn-social btn-facebook btn-flat" @click="loginFacebook">
                    <i class="fa fa-facebook"></i> Kirjaudu Facebook-tunnuksilla
                </a>
                <a class="btn btn-block btn-social btn-google btn-flat" @click="loginGoogle">
                    <i class="fa fa-google-plus"></i> Kirjaudu Google-tunnuksilla
                </a>
            </div>
            <a href="#/luo-tunnus" class="text-center">Luo tili</a>
        </div>
    </div>
</template>

<script>
    var api = require('../api');
module.exports = {
    data ()
    {
        return {
            dialog: null
        }
    },
    components: {},
    methods: {
        login(){
            var data = {
                username: this.username,
                password: this.password
            };
            api.login(data).then(function(){
                window.location = '#login-success';
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