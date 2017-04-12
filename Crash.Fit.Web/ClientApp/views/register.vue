<template>
    <div class="register-box">
        <div class="register-logo">
            <a href="/"><b>Crash</b>FIT</a>
        </div>
        <div class="register-box-body">
                <div class="form-group has-feedback">
                    <label>Sähköpostiosoite</label> <span class="error">{{ emailError}}</span>
                    <input type="email" class="form-control" v-model="email" @blur="checkEmail">
                </div>
                <div class="form-group has-feedback">
                    <label>Salasana</label>
                    <input type="password" class="form-control" v-model="password" @blur="checkPassword">
                </div>
                <div class="form-group has-feedback">
                    <label>Salasana uudestaan</label> <span class="error">{{ password2Error}}</span>
                    <input type="password" class="form-control" v-model="password2" @blur="checkPassword">
                </div>
                <div class="row">
                    <div class="col-xs-8">
                        Jatkamalla hyväksyn <a href="#">käyttöehdot</a>
                    </div>
                    <div class="col-xs-4">
                        <button type="submit" class="btn btn-primary btn-block btn-flat" @click="register" :disabled="!isValid">Luo tili</button>
                    </div>
                </div>
            <div class="social-auth-links text-center">
                <p>- TAI -</p>
                <a href="#" class="btn btn-block btn-social btn-facebook btn-flat">
                    <i class="fa fa-facebook"></i> Kirjaudu Facebook-tunnuksilla
                </a>
                <a href="#" class="btn btn-block btn-social btn-google btn-flat">
                    <i class="fa fa-google-plus"></i> Kirjaudu Google-tunnuksilla
                </a>
            </div>
            <a href="#/kirjaudu" class="text-center">Minulla on jo tunnukset</a>
        </div>
        <!-- /.form-box -->
    </div>
</template>

<script>
    var api = require('../api');
    var auth = require('../auth');

module.exports = {
    data () {
        return {
            email: null,
            password: null,
            password2: null,
            emailError: null,
            password2Error: null
        }
    },
    computed: {
        isValid(){
            return this.email && this.password && this.password2 && this.password === this.password2;
        }
    },
    components: {},
    methods: {
        checkEmail() {
            if (!this.email) {
                this.emailError = null;
            }
            else if (this.email.indexOf('@') < 0 || this.email.indexOf('.') < 0) {
                this.emailError = 'Ei näytä sähköpostiosoitteelta';
            }
            else {
                api.checkEmail(this.email).then(function () {

                });
            }
        },
        checkPassword(){
            if (this.password && this.password2 && this.password !== this.password2) {
                this.password2Error = 'Salasanat eivät täsmää';
            }else{
                this.password2Error = null;
            }
        },
        register() {
            var data = {
                email: this.email,
                password: this.password,
                password2:this.password2
            };
            api.register(data).then(function () {
                api.getUser().then(function (user) {
                    auth.setUserInfo(user.id);
                    window.location = '/';
                });
            });
            
        }
    }
}
</script>

<style scoped>
    input[type=checkbox]{
        width: 20px;
        height:20px;
    }
    span.error {
        color:red;
    }
</style>