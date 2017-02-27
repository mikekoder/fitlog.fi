var Vue = require('vue')
import VueRouter from 'vue-router'
import routes from './routes'
import 'jquery'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.css'
import 'admin-lte'
import 'admin-lte/dist/css/adminLTE.css'
import 'admin-lte/dist/css/skins/skin-blue.css'
import 'font-awesome/css/font-awesome.css'
import './css/site.css'
var auth = require('./auth');

Vue.use(VueRouter)
Vue.mixin({
    computed:{
        isLoggedIn(){
            return auth.isLoggedIn();
        }
    },
    created(){
        console.log(document.cookie)
    }
});
//Vue.use(require('./components/datetime'));

let router = new VueRouter({
    routes: routes,
    linkActiveClass: 'active'
});
router.beforeEach((to, from, next) =>{
    if (!to.matched.some(record => record.meta.anon)) {
        console.log('authentication required');
        next();
    } else {
        next();
    }
});


import app from './app'

let App = Vue.component('app', app)

/* eslint-disable no-unused-vars */
const vm = new App({
    el: '#app',
    router
})
