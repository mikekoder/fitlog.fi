<template>
<layout >

  <span slot="title">{{ $t('register') }}</span>

  <div slot="toolbar"></div>
  <q-page class="q-pa-sm">
    <div class="q-tab-pane">
      <q-input v-model="email" type="text" :float-label="$t('email')" />
      <q-input v-model="password" type="password" :float-label="$t('password')" />
      <q-input v-model="password2" type="password" :float-label="$t('confirmPassword')" />
      <div class="row q-pt-md">
        <div class="col">
          <q-btn glossy color="primary" @click="register" :label="$t('register')" :disabled="!isValid"></q-btn>
        </div>
        <div class="col">
          <q-btn color="primary" @click="$router.push({ name: 'login' })" :flat="true" :label="$t('login')"></q-btn>
        </div>
      </div>
      
    </div>
    <div class="q-tab-pane">
      <q-btn glossy color="primary" @click="fbLogin" icon="fas fa-facebook-official">Facebook</q-btn>
      <q-btn glossy color="primary" @click="googleLogin" icon="fas fa-google-plus-official">Google</q-btn>
    </div>
    <div class="row">
      <div class="col">
        {{ info }}
      </div>
    </div>
  </q-page>
  </layout>
</template>

<script>
import { openURL } from 'quasar'
import config from '../config'
import api from '../api'
import constants from '../store/constants'
import PageMixin from '../mixins/page'

export default {
  mixins: [PageMixin],
  data () {
    return {
      email: undefined,
      password: undefined,
      password2: undefined,
      info: ''
    }
  },
  computed: {
    isValid(){
      return this.password && this.password.length >= 6 && this.password == this.password2 && this.email;
    }
  },
  methods: {
    register(){
      var data = {
          email: this.email,
          password: this.password,
          password2: this.password2
      };
      api.register(data).then(response => {
          this.$store.dispatch(constants.STORE_TOKENS, {
              client: response.data.client,
              refreshToken: response.data.refreshToken,
              accessToken: response.data.accessToken
          }).catch(_ => {
            this.notifyError(this.$t('failed'));
          });
      }).then(_ => {
          this.$router.replace({ name: 'meals' });
      }).catch(_ => {
          this.notifyError(this.$t('failed'));
      });
    },
    fbLogin(){
      this.socialLogin('Facebook');
    },
    googleLogin(){
      this.socialLogin('Google');
    },
    socialLogin(provider){
      if(this.$q.platform.is.cordova){
        if(provider == 'Google'){
          window.plugins.googleplus.login(
            {
              'webClientId': config.googleWebClientId
            },
            (obj) => {
              if(obj.idToken){
                api.loginWithToken('Google', obj.idToken).then(response => {
                  this.finishLogin(response.data.refreshToken, response.data.accessToken);
                }).fail(xhr => {
                  this.changeInfo(xhr);
                });
              }
            },
            (msg) => {
              this.changeInfo(msg);
            }
          );
        }
        else {
          var ref = cordova.InAppBrowser.open(config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile', '_blank', 'location=no');
          ref.addEventListener('loadstop', (event) => {
            if(event.url.includes('login-success')){
              var parts = event.url.split('/');
              var refreshToken = parts[parts.length - 2];
              var accessToken = parts[parts.length - 1];

              ref.close();
              if(refreshToken && accessToken){
                this.finishLogin(refreshToken, accessToken);
              }
            }
          });
        }
      }
      else{
        window.location = config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile&returnUrl='+ window.location;
      }
    },
    finishLogin(refreshToken, accessToken){
      if(refreshToken && accessToken){
        this.$store.dispatch(constants.STORE_TOKENS, {
          refreshToken,
          accessToken
        }).then(_ => {
          this.$router.replace({name: 'home'});
        }).catch(_ => {
          this.notifyError(this.$t('failed'));
        });
      }
    },
    changeInfo(data){
      //this.info = JSON.stringify(data);
    }
  },
  created () {
    this.$store.commit(constants.LOADING_DONE);
  },
  beforeDestroy () {
  }
}
</script>

<style lang="stylus">
</style>
