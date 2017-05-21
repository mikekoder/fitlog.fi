var api = require('../api')
var constants = require('./constants')
var moment = require('moment')

const state = {
    loading: false,
    profile: undefined
}

// actions
const actions = {
    [constants.FETCH_PROFILE] ({commit, state},{success, failure}) {
        api.getProfile().then(function(profile){
            if(profile.doB){
                profile.doB = new Date(profile.doB);
            }
            commit(constants.FETCH_PROFILE_SUCCESS,{profile})
            if(success){
                success();
            }
        }).fail(function(){
            commit(constants.FETCH_PROFILE_FAILURE)
            if(failure){
                failure();
            }
        });
    }
}

// mutations
const mutations = {
    [constants.FETCH_PROFILE_STARTED] (state) {
        state.loading = true;
    },
    [constants.FETCH_PROFILE_SUCCESS] (state, {profile}) {
        state.loading = false;
        state.profile = profile;
    },
    [constants.FETCH_PROFILE_FAILURE] (state) {
        state.loading = false;
    }
}


module.exports = { state, actions, mutations }