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
        [constants.STORE_TOKENS]({ commit, state }, { client, refreshToken, accessToken }) {
            storage.setItem('client', client);
            storage.setItem('refresh_token', refreshToken);
            storage.setItem('access_token', accessToken);

            return Promise.resolve();
        },
        [constants.REFRESH_TOKEN]({ commit, state }) {
            var refreshToken = storage.getItem('refresh_token');
            return api.refreshToken(refreshToken).then(response => {
                if(response.data.accessToken){
                    storage.setItem('access_token', response.data.accessToken);
                }
                else{
                    return Promise.reject(`Couldn't refresh token`);
                }
            });
        },
        [constants.FETCH_PROFILE]({ commit, state }, { forceRefresh}) {
            if (state.profile && !forceRefresh) {
                return Promise.resolve(state.profile);
            }
            return api.getProfile().then(response => {
                var profile = response.data;
                if (profile.doB) {
                    profile.doB = new Date(profile.doB);
                }
                commit(constants.FETCH_PROFILE_SUCCESS, { profile })
                return profile;
            });
        },
        [constants.SAVE_PROFILE]({ commit, state }, { profile }) {
            return api.saveProfile(profile).then(response => {
                var savedProfile = response.data;
                if (savedProfile.doB) {
                    savedProfile.doB = new Date(savedProfile.doB);
                }
                commit(constants.SAVE_PROFILE_SUCCESS, { profile: savedProfile })
                return savedProfile;
            });
        },
        [constants.LOGOUT]({ commit, state }) {
            return api.logout().then(response => {
                storage.removeItem('refresh_token');
                storage.removeItem('access_token');
                commit(constants.ACTIVITIES_CLEAR);
                commit(constants.CLIPBOARD_CLEAR);
                commit(constants.FEEDBACK_CLEAR);
                commit(constants.NUTRITION_CLEAR);
                commit(constants.PROFILE_CLEAR);
                commit(constants.TRAINING_CLEAR);
            });
        },
        [constants.UPDATE_LOGIN]({ commit, state }, { login }) {
            return api.updateLogin(login).then(response => {

            });
        },
        [constants.DELETE_PROFILE]({ commit, state }, { }) {
          return api.deleteProfile().then(response => {

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