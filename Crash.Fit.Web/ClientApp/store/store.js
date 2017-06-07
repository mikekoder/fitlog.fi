import Vue from 'vue'
import Vuex from 'vuex'
import nutrition from './nutrition'
import training from './training'
import profile from './profile'
var constants = require('./constants')

Vue.use(Vuex)

const state = {
    loading: false
};
const actions = {
};
const mutations = {
    [constants.LOADING] (state){
        state.loading = true;
    },
    [constants.LOADING_DONE] (state){
        state.loading = false;
    }
};

export default new Vuex.Store({
    state,
    actions,
    mutations,
    modules: {
        nutrition,
        training,
        profile
    }
})