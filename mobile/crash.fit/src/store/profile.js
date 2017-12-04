import api from '../api'
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
            }).fail(function () {
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
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_PROFILE]({ commit, state }, { profile, success, failure }) {
            api.saveProfile(profile).then(function (savedProfile) {
                if (savedProfile.doB) {
                    savedProfile.doB = new Date(savedProfile.doB);
                }
                commit(constants.SAVE_PROFILE_SUCCESS, { profile: savedProfile })
                if (success) {
                    success(savedProfile);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.LOGOUT]({ commit, state }, { success, failure }) {
            api.logout().then(function () {
                storage.removeItem('refresh_token');
                storage.removeItem('access_token');
                commit(constants.LOGOUT_SUCCESS);
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.UPDATE_LOGIN]({ commit, state }, { login, success, failure }) {
            api.updateLogin(login).then(function () {

            }).fail(function () {

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
        [constants.LOGOUT_SUCCESS](state) {
            state.profile = undefined;
        },
        [constants.FETCH_PROFILE_FAILURE](state) {
            state.loading = false;
        }
    }
}