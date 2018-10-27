<template>
<layout >

  <span slot="title">{{ $t('login') }}</span>

  <div slot="toolbar"></div>
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
      <q-btn glossy color="primary" @click="fbLogin" icon="fas fa-facebook-official">Facebook</q-btn>
      <q-btn glossy color="primary" @click="googleLogin" icon="fas fa-google-plus-official">Google</q-btn>
    </div>
  </q-page>
  </layout>
</template>

<script>
import { openURL } from 'quasar'
import config from '../config'
import constants from '../store/constants'
import api from '../api'
import PageMixin from '../mixins/page'

export default {
  mixins: [PageMixin],
  data () {
    return {
      refreshToken: undefined,
      accessToken: undefined,
      username: '',
      password: '',
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
      
      api.login(data).then(response => {
        self.$store.dispatch(constants.STORE_TOKENS, {
          client: response.data.client,
          refreshToken: response.data.refreshToken,
          accessToken: response.data.accessToken
        }).then(_ => {
          self.$router.replace({name: 'home'});
        }).catch(_ => {
          self.notifyError(self.$t('failed'));
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
      if(self.isCordova){
        if(provider == 'Google'){
          window.plugins.googleplus.login(
            {
              'webClientId': config.googleWebClientId
            },
            function (obj) {
              if(obj.idToken){
                api.loginWithToken('Google', obj.idToken).then(response => {
                  self.finishLogin(response.data.refreshToken, response.data.accessToken);
                }).fail(xhr => {
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
          accessToken
        }).then(_ => {
          self.$router.replace({name: 'home'});
        }).catch(_ => {
          self.notifyError(self.$t('failed'));
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
