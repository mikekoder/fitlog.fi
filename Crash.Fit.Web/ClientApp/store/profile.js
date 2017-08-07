var api = require('../api')
var constants = require('./constants')
var moment = require('moment')

const state = {
    loading: false,
    profile: undefined
}

// actions
const actions = {
    [constants.FETCH_PROFILE] ({commit, state},{forceRefresh,success, failure}) {
        if(state.profile && !forceRefresh){
            if(success){
                success(state.profile);
            }
        }
        api.getProfile().then(function(profile){
            if(profile.doB){
                profile.doB = new Date(profile.doB);
            }
            commit(constants.FETCH_PROFILE_SUCCESS,{profile})
            if(success){
                success(profile);
            }
        }).fail(function(){
            commit(constants.FETCH_PROFILE_FAILURE)
            if(failure){
                failure();
            }
        });
    },
    [constants.SAVE_PROFILE]({commit, state}, {profile, success, failure}) {
        api.saveProfile(profile).then(function (savedProfile) {
            if (savedProfile.doB) {
                savedProfile.doB = new Date(savedProfile.doB);
            }
            commit(constants.SAVE_PROFILE_SUCCESS, { profile: savedProfile })
            if (success) {
                success(savedProfile);
            }
        }).fail(function () {
            commit(constants.FETCH_PROFILE_FAILURE)
            if (failure) {
                failure();
            }
        });
    },
    [constants.LOGOUT] ({commit, state},{success, failure}) {
        api.logout().then(function(){
            commit(constants.LOGOUT_SUCCESS)
            if(success){
                success();
            }
        }).fail(function(){
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
    [constants.FETCH_PROFILE_SUCCESS] (state, { profile }) {
        state.loading = false;
        state.profile = profile;
    },
    [constants.SAVE_PROFILE_SUCCESS](state, { profile }) {
        state.loading = false;
        state.profile = profile;
    },
    [constants.LOGOUT_SUCCESS] (state) {
        state.profile = undefined;
    },
    [constants.FETCH_PROFILE_FAILURE] (state) {
        state.loading = false;
    }
}


module.exports = { state, actions, mutations }