<template>
  <q-page class="q-pa-sm">
    <div class="q-tab-pane">
      <q-input v-model="email" type="text" :float-label="$t('email')" />
      <q-input v-model="password" type="password" :float-label="$t('password')" />
      <q-input v-model="password2" type="password" :float-label="$t('confirmPassword')" />
      <div class="row">
        <div class="col">
          <q-btn @click="register" :label="$t('register')"></q-btn>
        </div>
        <div class="col">
          <q-btn @click="$router.push({ name: 'login' })" :flat="true" :label="$t('login')"></q-btn>
        </div>
      </div>
      
    </div>
    <div class="q-tab-pane">
      <q-btn @click="fbLogin" icon="fa-facebook-official">Facebook</q-btn>
      <q-btn @click="googleLogin" icon="fa-google-plus-official">Google</q-btn>
    </div>
  </q-page>
</template>

<script>
import { openURL } from 'quasar'
import config from '../config'
import api from '../api'
import { Toast } from 'quasar'
import constants from '../store/constants'

export default {
  data () {
    return {
      email: undefined,
      password: undefined
    }
  },
  computed: {
  },
  methods: {
    register(){
      var self = this;
      var data = {
          email: this.email,
          password: this.password
      };
      api.register(data).then(function (response) {
          self.$store.dispatch(constants.STORE_TOKENS, {
              client: response.client,
              refreshToken: response.refreshToken,
              accessToken: response.accessToken,
              success() {
                  self.$router.replace({ name: 'meals' });
              },
              failure() {
                Toast.create(self.$t('failed'));
              }
          });
      }).fail(function (response) {
          Toast.create(self.$t('failed'));
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
        var ref = cordova.InAppBrowser.open(config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile', '_blank', 'location=no');
        ref.addEventListener('loadstop', function(event){
          if(event.url.includes('login-success')){
            var parts = event.url.split('/');
            self.refreshToken = parts[parts.length - 2];
            self.accessToken = parts[parts.length - 1];
            self.url = event.url;
            ref.close();
          }
        });
      }
      else{
        window.location = config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile&returnUrl='+ window.location;
      }
    }
  },
  created () {
    var self = this;
    self.$store.commit(constants.LOADING_DONE);
  },
  beforeDestroy () {
  }
}
</script>

<style lang="stylus">
</style>
