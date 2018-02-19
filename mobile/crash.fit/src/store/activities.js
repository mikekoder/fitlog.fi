import constants from './constants'
import api from '../api'

export default {
    state: {
        activityPresetsLoaded: false,
        activityPresets: []
    },
    actions: {
        [constants.FETCH_ACTIVITY_PRESETS]({ commit, state }, { success, failure }) {
            api.listActivityPresets().then(presets => {
                commit(constants.FETCH_ACTIVITY_PRESETS_SUCCESS, { presets });
                if (success) {
                    success(presets);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
            
        },
        [constants.SAVE_ACTIVITY_PRESETS]({ commit, state }, { presets, success, failure }) {
            api.saveActivityPresets(presets).then(savedPresets => {
                commit(constants.SAVE_ACTIVITY_PRESETS_SUCCESS, { presets: savedPresets });
                if (success) {
                    success(savedPresets);
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
           
        }
    },
    mutations: {
        [constants.FETCH_ACTIVITY_PRESETS_SUCCESS](state, { presets }) {
            state.activityPresetsLoaded = true;
            state.activityPresets = presets;
        },
        [constants.SAVE_ACTIVITY_PRESETS_SUCCESS](state, { presets }) {
            state.activityPresetsLoaded = true;
            state.activityPresets = presets;
        }
    }
}