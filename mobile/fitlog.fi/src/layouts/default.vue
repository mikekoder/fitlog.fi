<template>
  <q-layout view="lHh Lpr lFf">
    <q-layout-header>
      <q-toolbar color="tertiary" glossy>
        <q-btn flat dense round @click="leftDrawerOpen = !leftDrawerOpen">
          <q-icon name="menu" />
        </q-btn>
        <q-toolbar-title>
          {{ title }}
        </q-toolbar-title>
        <q-btn flat dense round @click="showHelp" v-if="hasHelp">
          <q-icon name="help" />
        </q-btn>
      </q-toolbar>
    </q-layout-header>

    <q-layout-drawer v-model="leftDrawerOpen" content-class="bg-grey-2">
      <q-list no-border link inset-delimiter v-if="!isLoggedIn">
        <q-item :to="{ name: 'login' }">  
          <q-item-side icon="fa-sign-in-alt" />
          <q-item-main :label="$t('login')" />
        </q-item>
      </q-list>

      <q-list no-border link inset-delimiter v-if="isLoggedIn">
         <q-item :to="{ name: 'home' }" exact>  
          <q-item-side icon="fa-utensils" />
          <q-item-main :label="$t('diary')" />
        </q-item>
        <q-item :to="{ name: 'activity-levels' }" exact>  
          <q-item-side icon="fa-tachometer-alt" />
          <q-item-main :label="$t('activityLevels')" />
        </q-item>
        <q-list-header>{{ $t('nutrition') }}</q-list-header>
       
        <q-item :to="{ name: 'foods' }">  
          <q-item-side icon="kitchen" />
          <q-item-main :label="$t('foods')" />
        </q-item>
        <q-item :to="{ name: 'meal-rhythm' }">  
          <q-item-side icon="fa-clock" />
          <q-item-main :label="$t('mealRhythm')" />
        </q-item>
        <q-item :to="{ name: 'recipes' }">  
          <q-item-side icon="fa-book" />
          <q-item-main :label="$t('recipes')" />
        </q-item>
        <q-item :to="{ name: 'nutrition-goals' }">  
          <q-item-side icon="fa-crosshairs" />
          <q-item-main :label="$t('nutritionGoals')" />
        </q-item>
        
        <q-list-header>{{ $t('training') }}</q-list-header>
        <q-item :to="{ name: 'workouts' }">  
          <q-item-side icon="fa-heartbeat" />
          <q-item-main :label="$t('workouts')" />
        </q-item>
        <q-item :to="{ name: 'exercises' }">  
          <q-item-side icon="fitness_center" />
          <q-item-main :label="$t('exercises')" />
        </q-item>
        <q-item :to="{ name: 'routines' }">  
          <q-item-side icon="fa-calendar" />
          <q-item-main :label="$t('routines')" />
        </q-item>
        <q-item :to="{ name: 'rep-calculator' }">  
          <q-item-side icon="fa-calculator" />
          <q-item-main :label="$t('repCalculator')" />
        </q-item>
        <q-item :to="{ name: 'energy-expenditures' }">  
          <q-item-side icon="fa-fire" />
          <q-item-main :label="$t('energyExpenditures')" />
        </q-item>
        <q-list-header>{{ $t('measurements') }}</q-list-header>
        <q-item item :to="{ name: 'measurements' }">  
          <q-item-side icon="fa-balance-scale" />
          <q-item-main :label="$t('measurements')" />
        </q-item>

        <q-list-header>{{ $t('profile') }}</q-list-header>
        <q-item :to="{ name: 'profile' }">  
          <q-item-side icon="fa-user" />
          <q-item-main :label="$t('profile')" />
        </q-item>
        <q-item @click.native="logout">
          <q-item-side icon="fa-sign-out-alt" />
          <q-item-main :label="$t('logout')" />
        </q-item>

        <q-list-header>{{ $t('getInvolved') }}</q-list-header>
        <q-item :to="{ name: 'improvements' }">  
          <q-item-side icon="fa-lightbulb" />
          <q-item-main :label="$t('improvements')" />
        </q-item>
        <q-item :to="{ name: 'bugs' }">  
          <q-item-side icon="fa-bug" />
          <q-item-main :label="$t('bugs')" />
        </q-item>
        <!--
        <q-list-header>Debug</q-list-header>
        <q-item v-if="profile">{{ profile.username }}</q-item>
        <q-item>{{ token }}</q-item>
        -->
      </q-list>
      

    </q-layout-drawer>

    <q-page-container>
      <router-view v-show="!loading" ref="page" />
      <q-inner-loading :visible="loading">
        <q-spinner-dots size="100" />
      </q-inner-loading>
    </q-page-container>
  </q-layout>
</template>

<script>

import constants from '../store/constants'
import storage from '../storage'

export default {
  name: 'LayoutDefault',
  data () {
    return {
      title: 'fitlog',
      leftDrawerOpen: true,
      hasHelp: false
    }
  },
  computed: {
    profile(){
      return this.$store.state.profile.profile;
    },
    token(){
      var token = storage.getItem('refresh_token');
      return token;
    },
    loading() {
          return this.$store.state.loading;
    }
  },
  watch:{
    $route(){
      this.handleMeta();
    }
  },
  methods: {
    refreshTokens() {
      var self = this;
      self.$store.dispatch(constants.REFRESH_TOKEN, {
          success: () => { 
            if(!self.isLoggedIn){
              self.$store.dispatch(constants.FETCH_PROFILE, {});
            }
          },
          failure: () => {
            if(self.$route.name != 'login' && self.$route.name != 'register'){
              self.$router.push({name: 'login'});
            }
          }
      });
    },
    handleMeta(){
      var title = this.$route.meta.title;
      this.title = this.$t(title);

      this.help = this.$route.meta.help;
      this.$nextTick(() => {
        var page = this.$refs.page;
        console.log(page);
        this.hasHelp = page.showHelp && true;
      });
      
    },
    logout(){
      var self = this;
      self.$store.dispatch(constants.LOGOUT, {
          success: () => {
            self.$router.replace({name: 'login'});
           },
          failure: (err) => {
            console.log(err);
           }
      });
    },
    showHelp(){
      this.$refs.page.showHelp();
    }
  },
  created(){
console.log(process);

      var self = this;
      self.handleMeta();
      self.refreshTokens();
      setInterval(() => {
          self.refreshTokens();
      }, 300000); 
  },
  mounted () {
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
