import Vue from 'vue'
import Vuex from 'vuex'
import home from './home'
import nutrition from './nutrition'
import training from './training'
import profile from './profile'
import feedback from './feedback'
import constants from './constants'

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        loading: false
    },
    actions: {},
    mutations: {
        [constants.LOADING] (state){
            state.loading = true;
        },
        [constants.LOADING_DONE] (state){
            state.loading = false;
        }
    },
    modules: {
        home,
        nutrition,
        training,
        profile,
        feedback
    }
})