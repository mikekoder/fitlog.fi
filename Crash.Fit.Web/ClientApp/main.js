import Vue from 'vue'
import VueRouter from 'vue-router'
import routes from './routes'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.css'
import './css/site.css'

Vue.use(VueRouter)

let router = new VueRouter({
    routes: routes,
    linkActiveClass: 'active'
})

router.afterEach((currentRoute) => {
  let mainContent = document.querySelector('.main-content')

  if (mainContent) {
    mainContent.scrollTop = 0
  }
})


import app from './app'

let App = Vue.component('app', app)

/* eslint-disable no-unused-vars */
const vm = new App({
  el: '#app',
  router
})
