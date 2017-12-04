<template>
  <q-layout ref="layout" view="lHh Lpr fff" :left-class="{'bg-grey-2': true}">
    <q-toolbar slot="header" class="glossy">
      <q-btn flat @click="$refs.layout.toggleLeft()">
        <q-icon name="menu" />
      </q-btn>

      <q-toolbar-title>
        {{ title }}
      </q-toolbar-title>
    </q-toolbar>

    <div slot="left">
      <q-list no-border link inset-delimiter v-if="!isLoggedIn">
        <q-side-link item :to="{ name: 'login' }">  
          <q-item-side icon="fa-key" />
          <q-item-main :label="$t('login')" />
        </q-side-link>
      </q-list>
      <q-list no-border link inset-delimiter v-if="isLoggedIn">

        <q-list-header>{{ $t('nutrition') }}</q-list-header>
        <q-side-link item :to="{ name: 'meals' }">  
          <q-item-side icon="fa-cutlery" />
          <q-item-main :label="$t('diary')" />
        </q-side-link>
        <q-side-link item :to="{ name: 'foods' }">  
          <q-item-side icon="fa-apple" />
          <q-item-main :label="$t('foods')" />
        </q-side-link>
        <q-side-link item :to="{ name: 'meal-rhythm' }">  
          <q-item-side icon="fa-clock-o" />
          <q-item-main :label="$t('mealRhythm')" />
        </q-side-link>
        <q-side-link item :to="{ name: 'recipes' }">  
          <q-item-side icon="fa-book" />
          <q-item-main :label="$t('recipes')" />
        </q-side-link>
        <q-side-link item :to="{ name: 'nutrition-goals' }">  
          <q-item-side icon="fa-crosshairs" />
          <q-item-main :label="$t('nutritionGoals')" />
        </q-side-link>

        <q-list-header>{{ $t('training') }}</q-list-header>
        <q-side-link item :to="{ name: 'workouts' }">  
          <q-item-side icon="fa-heartbeat" />
          <q-item-main :label="$t('workouts')" />
        </q-side-link>
        <q-side-link item :to="{ name: 'exercises' }">  
          <q-item-side icon="fa-universal-access" />
          <q-item-main :label="$t('exercises')" />
        </q-side-link>
        <q-side-link item :to="{ name: 'routines' }">  
          <q-item-side icon="fa-calendar" />
          <q-item-main :label="$t('routines')" />
        </q-side-link>
        <q-side-link item :to="{ name: 'rep-calculator' }">  
          <q-item-side icon="fa-calculator" />
          <q-item-main :label="$t('repCalculator')" />
        </q-side-link>

        <q-list-header>{{ $t('measurements') }}</q-list-header>
        <q-side-link item :to="{ name: 'measurements' }">  
          <q-item-side icon="fa-balance-scale" />
          <q-item-main :label="$t('measurements')" />
        </q-side-link>


        <q-list-header>{{ $t('profile') }}</q-list-header>
        <q-item @click="logout">
          <q-item-side icon="fa-key" />
          <q-item-main :label="$t('logout')" />
        </q-item>
        <q-list-header>Debug</q-list-header>
        <q-item v-if="profile">{{ profile.username }}</q-item>
        <q-item>{{ token }}</q-item>
      </q-list>
    </div>
    <router-view v-show="!loading" />
    <q-inner-loading :visible="loading">
      <q-spinner-dots size="100" />
    </q-inner-loading>
  </q-layout>
</template>

<script>
import {dom, event, openURL, QLayout, QToolbar, QToolbarTitle, QBtn, QIcon, QList, QListHeader, QItem, QItemSide, QItemMain, QSideLink, QInnerLoading, QSpinnerDots} from 'quasar'
import constants from '../store/constants'
import storage from '../storage'

//var constants = require('../store/constants')
export default {
  components: {
    QLayout, QToolbar, QToolbarTitle, QBtn, QIcon, QList, QListHeader, QItem, QItemSide, QItemMain, QSideLink, QInnerLoading, QSpinnerDots
  },
  data () {
    return {
      title: 'fitlog'
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
      },
  },
  watch:{
    $route(){
      this.updateTitle();
    }
  },
  methods: {
    refreshTokens() {
      var self = this;
      self.$store.dispatch(constants.REFRESH_TOKEN, {
          success: function () { 
            if(!self.isLoggedIn){
              self.$store.dispatch(constants.FETCH_PROFILE, {});
            }
          },
          failure: function () {
            self.$router.push({name: 'login'});
          }
      });
    },
    updateTitle(){
      var title = this.$route.meta.title;
      this.title = this.$t(title);
    },
    logout(){
      var self = this;
      self.$store.dispatch(constants.LOGOUT, {
          success: function () { },
          failure: function () { }
      });
    }
  },
  created(){
      var self = this;
      self.updateTitle();
      //self.$store.dispatch(constants.FETCH_PROFILE, {});
      self.refreshTokens();
      setInterval(function () {
          self.refreshTokens();
      }, 300000); 
  },
  mounted () {
  },
  beforeRouteUpdate(to, from, next) {
    this.$store.commit(constants.LOADING);
    next();
  },
  beforeRouteLeave(to, from, next) {
      this.$store.commit(constants.LOADING);
      next();
  }
}
</script>

<style lang="stylus">
.q-tab-pane { border: 0px;}
.q-list { border: 0px;}
.pad{ padding: 0px 12px; }
.buttons button{ margin-right: 5px; margin-bottom: 10px;}
</style>
