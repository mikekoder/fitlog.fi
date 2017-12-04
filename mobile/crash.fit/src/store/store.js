import Vue from 'vue'
import Vuex from 'vuex'
import nutrition from './nutrition'
import training from './training'
import profile from './profile'
import feedback from './feedback'
import clipboard from './clipboard'
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
        nutrition,
        training,
        profile,
        feedback,
        clipboard
    }
})