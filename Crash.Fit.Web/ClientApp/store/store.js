import Vue from 'vue'
import Vuex from 'vuex'
import nutrition from './nutrition'
import training from './training'
import profile from './profile'

Vue.use(Vuex)


export default new Vuex.Store({
    modules: {
        nutrition,
        training,
        profile
    }
})