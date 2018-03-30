<template>
  <q-page class="q-pa-sm">
    <div class="q-tab-pane">
      <q-input v-model="username" type="email" :float-label="$t('username')+'/' + $t('email')" />
      <q-input v-model="password" type="password" :float-label="$t('password')" />
      <div class="row q-pt-md">
        <div class="col">
          <q-btn glossy color="primary" @click="login" :label="$t('login')"></q-btn>
        </div>
        <div class="col">
          <q-btn @click="$router.push({ name: 'register' })" :flat="true" color="primary" :label="$t('register')"></q-btn>
        </div>
      </div>
      
    </div>
    <div class="q-tab-pane">
      <q-btn glossy color="primary" @click="fbLogin" icon="fa-facebook-official">Facebook</q-btn>
      <q-btn glossy color="primary" @click="googleLogin" icon="fa-google-plus-official">Google</q-btn>
    </div>
    <div class="q-tab-pane">
      {{ url }}<br />
      {{ debugInfo }}
    </div>
  </q-page>
</template>

<script>
import { openURL } from 'quasar'
import config from '../config'
import constants from '../store/constants'
import api from '../api'

export default {
  data () {
    return {
      url: undefined,
      refreshToken: undefined,
      accessToken: undefined,
      username: '',
      password: '',
      debugInfo: ''
    }
  },
  computed: {
  },
  methods: {
    login(){
      var self = this;
      var data = {
        username: this.username,
        password: this.password,
        client: this.client
      };
      
      api.login(data).done((response) => {
        self.$store.dispatch(constants.STORE_TOKENS, {
          client: response.client,
          refreshToken: response.refreshToken,
          accessToken: response.accessToken,
          success() {
            self.$router.replace({name: 'home'});
          },
          failure() {
            self.notifyError(self.$t('failed'));
          }
        });
      }).fail(xhr => {

      });
    },
    fbLogin(){
      this.socialLogin('Facebook');
    },
    googleLogin(){
      this.socialLogin('Google');
    },
    socialLogin(provider){
      var self = this;
      if(self.$q.platform.is.cordova){
        if(provider == 'Google'){
          window.plugins.googleplus.login(
            {
              'webClientId': config.googleWebClientId
            },
            function (obj) {
              self.debugInfo = JSON.stringify(obj);
              if(obj.idToken){
                self.debugInfo = obj.idToken;
                api.loginWithToken('Google', obj.idToken).then(response => {
                  self.debugInfo = 'done: ' + JSON.stringify(response) +' '+ response.refreshToken + ' ' + response.accessToken;
                  self.finishLogin(response.refreshToken, response.accessToken);
                }).fail(xhr => {
                  self.debugInfo = 'fail: ' + JSON.stringify(xhr);
                });
              }
            },
            function (msg) {
              self.notifyError('error: ' + msg);
            }
          );
        }
        else {
          var ref = cordova.InAppBrowser.open(config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile', '_blank', 'location=no');
          ref.addEventListener('loadstop', function(event){
            if(event.url.includes('login-success')){
              var parts = event.url.split('/');
              var refreshToken = parts[parts.length - 2];
              var accessToken = parts[parts.length - 1];
              self.url = event.url;

              ref.close();
              if(refreshToken && accessToken){
                self.finishLogin(refreshToken, accessToken);
              }
            }
          });
        }
      }
      else {
        window.location = config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile&returnUrl='+ encodeURIComponent(window.location.href);
      }
    },
    finishLogin(refreshToken, accessToken){
      var self = this;
      if(refreshToken && accessToken){
        self.$store.dispatch(constants.STORE_TOKENS, {
          refreshToken,
          accessToken,
          success() {
            self.$store.dispatch(constants.FETCH_PROFILE, {
              success(){
                self.$router.replace({name: 'home'});
              }
            });
          },
          failure() {
            self.notifyError(self.$t('failed'));
          }
        });
      }
    },
    checkLoggedIn(){
      var self = this;
      if(self.isLoggedIn){
        self.$router.replace({name: 'home'});
      }
      else{
        setTimeout(() => {
          self.checkLoggedIn();
        }, 100);
      }
    }
  },
  created () {
    var self = this;
    self.$store.commit(constants.LOADING_DONE);
    var refreshToken = self.$route.params.refreshToken;
    var accessToken = self.$route.params.accessToken;
    if(refreshToken && accessToken){
      self.finishLogin(refreshToken, accessToken);
    }

    self.checkLoggedIn();

  },
  beforeDestroy () {
  }
}
</script>

<style lang="stylus">
</style>
