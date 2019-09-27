<template>
    <div class="register-box">
        <div class="register-logo">
            <a href="/"><b>fitlog</b>.fi</a>
        </div>
        <div class="register-box-body">
            <div class="form-group has-feedback">
                <label>{{ $t("password") }}</label> ({{ $t("min") }} 6 {{ $t("characters") }})<span class="error">{{ passwordError}}</span>
                <input type="password" class="form-control" v-model="password" @blur="checkPassword">
            </div>
            <div class="form-group has-feedback">
                <label>{{ $t("confirmPassword") }}</label> <span class="error">{{ password2Error}}</span>
                <input type="password" class="form-control" v-model="password2" @blur="checkPassword">
            </div>
            <div class="row">
                <div class="col-xs-12">
                    <button type="submit" class="btn btn-primary btn-block btn-flat" @click="changePassword" :disabled="!isValid">{{ $t("changePassword") }}</button>
                </div>
            </div>
            <div class="row" v-if="success">
                <div class="col-xs-12">
                    {{ $t('resetPasswordSuccess') }}
                </div>
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
            password: null,
            password2: null,
            passwordError: null,
            password2Error: null,
            success: false
        }
    },
    computed: {
        passwordIsValid(){
            return this.password && this.password.length >= 6;
        },
        password2IsValid(){
            return this.password2 === this.password;
        },
        isValid(){
            return this.passwordIsValid && this.password2IsValid;
        }
    },
    components: {},
    methods: {
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
        changePassword() {
            var self = this;
            var data = {
                userId: this.$route.params.userId,
                token: this.$route.params.token,
                password: this.password
            };
            api.changePassword(data).then((response) => {
                self.success = true;
            }).fail((response) => {
                toaster.error(self.$t('changePassword.fixErrors'));
            });
            
        },
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