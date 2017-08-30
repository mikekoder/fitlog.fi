var api = require('../api')
var constants = require('./constants')
var moment = require('moment')

const state = {
    date: new Date()
}

// actions
const actions = {

    [constants.SELECT_HOME_DATE]({ commit, state }, { date, success, failure }) {
        state.date = date;
    }
}

// mutations
const mutations = {
    
}

module.exports = { state, actions, mutations }