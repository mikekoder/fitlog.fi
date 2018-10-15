<template>
  <transition name="fade">
    <router-view />
  </transition>
</template>

<script>

import constants from '../store/constants'
import storage from '../storage'

export default {
  name: 'LayoutDefault',
  data () {
    return {
    }
  },

  methods: {
    refreshTokens() {
      var self = this;
      self.$store.dispatch(constants.REFRESH_TOKEN, { }).then(() => {
        if(!self.isLoggedIn){
          self.$store.dispatch(constants.FETCH_PROFILE, { });
        }
      }).catch(reason => {
        if(self.$route.name != 'login' && self.$route.name != 'register'){
          self.$router.push({name: 'login'});
        }
      });
    }
  },
  created(){
      var self = this;
      self.refreshTokens();
      setInterval(() => {
          self.refreshTokens();
      }, 300000); 
  },
  beforeRouteUpdate(to, from, next) {
    this.$store.commit(constants.LOADING);
    this.refreshTokens();
    next();
  },
  beforeRouteLeave(to, from, next) {
      this.$store.commit(constants.LOADING);
      next();
  }
}
</script>

<style>
</style>
