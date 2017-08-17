var Vue = require('vue')
import VueRouter from 'vue-router'
import VueI18n from 'vue-i18n'
import VueAnalytics from 'vue-analytics'
import store from './store/store'
import routes from './routes'
import 'jquery'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.css'
import 'admin-lte'
import 'admin-lte/dist/css/adminLTE.css'
import 'admin-lte/dist/css/skins/skin-blue-light.css'
import 'font-awesome/css/font-awesome.css'
import './css/site.css'
import 'bootstrap-notify'
var $ = require('jquery');
var translations = require('./translations')
Vue.config.productionTip = false

Vue.use(VueRouter)
Vue.use(VueI18n)


const i18n = new VueI18n({
    locale: 'fi',
    messages: translations,
})

Vue.mixin({
    computed:{
        isLoggedIn(){
            return this.$store.state.profile.profile && true;
        }
    },
    created(){
    }
});

let router = new VueRouter({
    routes: routes,
    linkActiveClass: 'active'
});
router.beforeEach((to, from, next) =>{
    if (!to.matched.some(record => record.meta.anon)) {
        next();
    } else {
        next();
    }
});
router.afterEach((to, from) => {
    if (window.matchMedia('(max-width: 767px)').matches) {
        $('body').addClass('sidebar-collapse').removeClass('sidebar-open');
    }
})
Vue.use(VueAnalytics, {
    id: 'UA-10474486-3',
    router
})

import app from './app'

let App = Vue.component('app', app)

/* eslint-disable no-unused-vars */
const vm = new App({
    el: '#app',
    router,
    store,
    i18n
})
