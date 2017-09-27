<template>
  <div>
    <div class="q-tab-pane">
      <q-input v-model="username" type="text" :float-label="$t('username')+'/' + $t('email')" />
      <q-input v-model="password" type="password" :float-label="$t('password')" />
      <div class="row">
        <div class="col">
          <q-btn @click="login">{{ $t('login') }}</q-btn>
        </div>
        <div class="col">
          <q-btn @click="$router.push({ name: 'register' })" :flat="true">{{ $t('register') }}</q-btn>
        </div>
      </div>
      
    </div>
    <div class="q-tab-pane">
      <q-btn @click="fbLogin" icon="fa-facebook-official">Facebook</q-btn>
      <q-btn @click="googleLogin" icon="fa-google-plus-official">Google</q-btn>
    </div>
  </div>
</template>

<script>
import { openURL, QBtn, QField, QInput, QSideLink } from 'quasar'
import config from '../config'
import constants from '../store/constants'
import { Toast } from 'quasar'

export default {
  components: {
    QBtn, QField, QInput, QSideLink
  },
  data () {
    return {
      url: undefined,
      refreshToken: undefined,
      accessToken: undefined,
      username: undefined,
      password: undefined
    }
  },
  computed: {
  },
  methods: {
    login(){

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
      else {
        window.location = config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile&returnUrl='+ window.location.href;
      }
    },
  },
  created () {
    var self = this;
    self.$store.commit(constants.LOADING_DONE);
    var accessToken = self.$route.params.accessToken;
    var refreshToken = self.$route.params.refreshToken;
    if(accessToken && refreshToken){
      self.$store.dispatch(constants.STORE_TOKENS, {
            client,
            refreshToken,
            accessToken,
            success() {
                self.$router.replace({name: 'meals'});
            },
            failure() {
                Toast.create(self.$t('failed'));
            }
        });
    }
  },
  beforeDestroy () {
  }
}
</script>

<style lang="stylus">
</style>
