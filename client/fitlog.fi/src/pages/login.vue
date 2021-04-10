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
      info: ''
    }
  },
  computed: {
  },
  methods: {
    login(){
      var data = {
        username: this.username,
        password: this.password,
        client: this.client
      };
      
      api.login(data).then(response => {
        this.$store.dispatch(constants.STORE_TOKENS, {
          client: response.data.client,
          refreshToken: response.data.refreshToken,
          accessToken: response.data.accessToken
        }).then(_ => {
          this.$router.replace({name: 'home'});
        }).catch(_ => {
          this.notifyError(this.$t('failed'));
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
    },
    checkLoggedIn(){
      if(this.isLoggedIn){
        this.$router.replace({name: 'home'});
      }
      else{
        setTimeout(() => {
          this.checkLoggedIn();
        }, 100);
      }
    }
  },
  created () {
    this.$store.commit(constants.LOADING_DONE);
    var refreshToken = this.$route.params.refreshToken;
    var accessToken = this.$route.params.accessToken;
    if(refreshToken && accessToken){
      this.finishLogin(refreshToken, accessToken);
    }
    this.checkLoggedIn();
  },
  beforeDestroy () {
  }
}
</script>

<style lang="stylus">
</style>
