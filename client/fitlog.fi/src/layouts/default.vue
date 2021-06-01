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
      this.$store.dispatch(constants.REFRESH_TOKEN, { }).then(() => {
        if(!this.isLoggedIn){
          this.$store.dispatch(constants.FETCH_PROFILE, { });
        }
      }).catch(reason => {
        if(this.$route.name != 'login' && this.$route.name != 'register' && this.$route.name != 'gdpr'){
          this.$router.push({name: 'login'});
        }
      });
    }
  },
  created(){
    this.refreshTokens();
    setInterval(() => {
      this.refreshTokens();
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
