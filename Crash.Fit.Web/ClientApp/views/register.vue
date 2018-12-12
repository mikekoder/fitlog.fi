<template>
    <div class="register-box">
        <div class="register-logo">
            <a href="/"><b>Crash</b>FIT</a>
        </div>
        <div class="register-box-body">
                <div class="form-group has-feedback">
                    <label>{{ $t("email") }}</label> <span class="error">{{ emailError }}</span>
                    <input type="email" class="form-control" v-model="email" @blur="checkEmail">
                </div>
                <div class="form-group has-feedback">
                    <label>{{ $t("password") }}</label> ({{ $t("min") }} 6 {{ $t("characters") }})<span class="error">{{ passwordError}}</span>
                    <input type="password" class="form-control" v-model="password" @blur="checkPassword">
                </div>
                <div class="form-group has-feedback">
                    <label>{{ $t("confirmPassword") }}</label> <span class="error">{{ password2Error}}</span>
                    <input type="password" class="form-control" v-model="password2" @blur="checkPassword">
                </div>
                <div class="row">
                    <div class="col-xs-8">
                        <!--
                        {{ $t("agree") }} <a href="#">{{ $t("terms") }}</a>
                            -->
                    </div>
                    <div class="col-xs-4">
                        <button type="submit" class="btn btn-primary btn-block btn-flat" @click="register" :disabled="!isValid">{{ $t("register") }}</button>
                    </div>
                </div>
            <div class="social-auth-links text-center">
                <p>- {{ $t("or") }} -</p>
                <a class="btn btn-block btn-social btn-facebook btn-flat" @click="loginFacebook">
                  <i class="fab fa-facebook"></i> {{ $t("useFacebook") }}
                </a>
                <a class="btn btn-block btn-social btn-google btn-flat" @click="loginGoogle">
                  <i class="fab fa-google-plus"></i> {{ $t("useGoogle") }}
                </a>
            </div>
            <router-link :to="{ name: 'login' }" class="text-center">{{ $t("login") }}</router-link>
        </div>
        <!-- /.form-box -->
    </div>
</template>

<script>
    import constants from '../store/constants'
    import api from '../api'
    import toaster from '../toaster'

export default {
    data () {
        return {
            email: null,
            password: null,
            password2: null,
            emailError: null,
            passwordError: null,
            password2Error: null
        }
    },
    computed: {
        emailIsValid(){
            return this.email && this.email.indexOf('@') >= 0 && this.email.indexOf('.') >= 0;
        },
        passwordIsValid(){
            return this.password && this.password.length >= 6;
        },
        password2IsValid(){
            return this.password2 === this.password;
        },
        isValid(){
            return this.emailIsValid && this.passwordIsValid && this.password2IsValid;
        }
    },
    components: {},
    methods: {
        checkEmail() {
            this.emailError = null;
            if (this.email && !this.emailIsValid) {
                this.emailError = this.$t('invalidEmail');
            }
        },
        checkPassword() {
            this.passwordError = null;
            this.password2Error = null;

            if (this.password && this.password.length < 6) {
                this.passwordError = this.$t('passwordTooShort');
            }
            if (this.password && this.password2 && this.password !== this.password2) {
                this.password2Error = this.$t('passwordsDontMatch');
            }
        },
        register() {
            var self = this;
            var data = {
                email: this.email,
                password: this.password,
                password2:this.password2
            };
            api.register(data).then((response) => {
                self.$store.dispatch(constants.STORE_TOKENS, {
                    client: response.client,
                    refreshToken: response.refreshToken,
                    accessToken: response.accessToken,
                    success() {
                        window.location = '/';
                    },
                    failure() {
                        toaster(self.$t('failed'));
                    }
                });
            }).fail((response) => {
                toaster.error(self.$t('register.fixErrors'));
                if (response.responseJSON && response.responseJSON.errorCodes) {
                    var errorCodes = response.responseJSON.errorCodes;
                    if (errorCodes.includes('PasswordTooShort')) {
                        self.passwordError = self.$t('passwordTooShort');
                    }
                }
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
    created() {
        this.$store.commit(constants.LOADING_DONE);
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