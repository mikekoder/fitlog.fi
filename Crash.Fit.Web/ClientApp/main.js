import Vue from 'vue'
import VueRouter from 'vue-router'
import VueI18n from 'vue-i18n'
import VueAnalytics from 'vue-analytics'
import store from './store/store'
import routes from './routes'
import 'bootstrap'
import 'bootstrap/dist/css/bootstrap.css'
import 'admin-lte'
import 'admin-lte/dist/css/adminLTE.css'
import 'admin-lte/dist/css/skins/skin-blue-light.css'
//import 'font-awesome/css/font-awesome.css'
import '@fortawesome/fontawesome-free/css/all.css'
import './css/site.css'
import 'bootstrap-notify'
import $ from 'jquery';
import translations from './translations'
import moment from 'moment'
Vue.config.productionTip = false

Vue.use(VueRouter)
Vue.use(VueI18n)


const i18n = new VueI18n({
    locale: 'fi',
    messages: translations
})

Vue.mixin({
    computed:{
        isLoggedIn(){
            return this.$store.state.profile.profile && true;
        },
        loading() {
            return this.$store.state.loading;
        },
        $profile() {
            return this.$store.state.profile.profile;
        }
    },
    methods: {
        formatDate(value) {
            if(!value){
                return '-';
            }
            var m = new moment(value);
            return m.format('DD.MM.YYYY');
        },
        formatTime(value) {
            if(!value){
                return '-';
            }
            var m = new moment(value);
            return m.format('HH:mm');
        },
        formatDateTime(value, format) {
            if(!value){
                return '-';
            }
            var m = new moment(value);
            format = format || 'DD.MM.YYYY HH:mm';
            return m.format(format);
        },
        formatDuration(hours, minutes) {
            var time = '01.01.2000 ' + (hours || 0) + ':' + (minutes || 0);
            return this.formatTime(time);
        },
        formatUnit(unit) {
            switch(unit){
                case 'G':
                    return 'g';
                case 'MG':
                    return 'mg';
                case 'UG':
                    return '\u03BCg';
                case'KCAL':
                    return 'kcal';
                case 'KJ':
                    return 'kJ';
                case 'CM':
                    return 'cm';
                case 'KG':
                    return 'kg';
                case 'KCAL_DAY':
                    return 'kcal/' + this.$t('day');
                default:
                    return unit;
            }
        },
        formatDecimal(value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        }
    }
});

let router = new VueRouter({
    routes: routes,
    linkActiveClass: 'active'
});
/*
router.beforeEach((to, from, next) =>{
    if (!to.matched.some(record => record.meta.anon)) {
        next();
    } else {
        next();
    }
});
*/
router.afterEach((to, from) => {
    if (window.matchMedia('(max-width: 900px)').matches) {
         window.sidebarExpanded = false;
        $('body').addClass('sidebar-collapse').removeClass('sidebar-open');
    }
    window.scrollTo(0, 0);
})


/* HACK: scrolling on mobile fires resize event causing collapsing of the sidebar
    which is not desirable
*/
$("body").on("expanded.pushMenu", function(){
    window.sidebarExpanded = true;
});
$("body").on("collapsed.pushMenu", function(){
    window.sidebarExpanded = false;
});
$(window).resize(() => {
    if (window.sidebarExpanded && window.matchMedia('(max-width: 900px)').matches) {
        setTimeout(() => {
            $('body').addClass('sidebar-open').removeClass('sidebar-collapse');
        }, 10);
    }
});
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
