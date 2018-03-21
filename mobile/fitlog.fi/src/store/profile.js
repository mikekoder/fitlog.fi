﻿import api from '../api'
import constants from './constants'
import moment from 'moment'
import storage from '../storage'

export default
{
    state: {
        loading: false,
        profile: undefined
    },
    actions: {
        [constants.STORE_TOKENS]({ commit, state }, { client, refreshToken, accessToken, success, failure }) {
            storage.setItem('client', client);
            storage.setItem('refresh_token', refreshToken);
            storage.setItem('access_token', accessToken);

            if (success) {
                success();
            }
        },
        [constants.REFRESH_TOKEN]({ commit, state }, { success, failure }) {
            var refreshToken = storage.getItem('refresh_token');
            api.refreshToken(refreshToken).then(function (response) {
                if(response.accessToken){
                    storage.setItem('access_token', response.accessToken);
                    if (success) {
                        success();
                    }
                }
                else if (failure) {
                    failure();
                }
            }).fail(() =>{
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_PROFILE]({ commit, state }, { forceRefresh, success, failure }) {
            if (state.profile && !forceRefresh) {
                if (success) {
                    success(state.profile);
                }
            }
            api.getProfile().then(function (profile) {
                if (profile.doB) {
                    profile.doB = new Date(profile.doB);
                }
                commit(constants.FETCH_PROFILE_SUCCESS, { profile })
                if (success) {
                    success(profile);
                }
            }).fail(() =>{
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_PROFILE]({ commit, state }, { profile, success, failure }) {
            api.saveProfile(profile).then(savedProfile => {
                if (savedProfile.doB) {
                    savedProfile.doB = new Date(savedProfile.doB);
                }
                commit(constants.SAVE_PROFILE_SUCCESS, { profile: savedProfile })
                if (success) {
                    success(savedProfile);
                }
            }).fail(() => {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.LOGOUT]({ commit, state }, { success, failure }) {
            api.logout().then(() => {
                storage.removeItem('refresh_token');
                storage.removeItem('access_token');
                commit(constants.ACTIVITIES_CLEAR);
                commit(constants.CLIPBOARD_CLEAR);
                commit(constants.FEEDBACK_CLEAR);
                commit(constants.NUTRITION_CLEAR);
                commit(constants.PROFILE_CLEAR);
                commit(constants.TRAINING_CLEAR);
                if (success) {
                    success();
                }
            }).fail((xhr) => {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.UPDATE_LOGIN]({ commit, state }, { login, success, failure }) {
            api.updateLogin(login).then(function () {

            }).fail(() => {

            });
        }
    },
    mutations: {
        [constants.FETCH_PROFILE_STARTED](state) {
            state.loading = true;
        },
        [constants.FETCH_PROFILE_SUCCESS](state, { profile }) {
            state.loading = false;
            state.profile = profile;
        },
        [constants.SAVE_PROFILE_SUCCESS](state, { profile }) {
            state.loading = false;
            state.profile = profile;
        },
        [constants.PROFILE_CLEAR](state) {
            console.log('profile.logout 1');
            state.profile = undefined;
            console.log('profile.logout 2');
        },
        [constants.FETCH_PROFILE_FAILURE](state) {
            state.loading = false;
        }
    }
}