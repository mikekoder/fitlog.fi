import constants from './constants'
import api from '../api'
import moment from 'moment'

export default {
    state: {
        activityPresetsLoaded: false,
        activityPresets: [],

        activityPresetsStart: null,
        activityPresetsEnd: null,
        activityPresetDays: []
    },
    actions: {
        [constants.FETCH_ACTIVITY_PRESETS]({ commit, state }, { success, failure }) {
            api.listActivityPresets().then(presets => {
                commit(constants.FETCH_ACTIVITY_PRESETS_SUCCESS, { presets });
                if (success) {
                    success(presets);
                }
            }).fail(() => {
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
            }).fail(() => {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.FETCH_ACTIVITY_PRESET_DAYS]({ commit, state }, { start, end, success, failure }) {
            if (state.activityPresetsStart && state.activityPresetsEnd) {
                if (moment(start).isBefore(state.activityPresetsStart) || moment(end).isAfter(state.activityPresetsEnd)) {
                    start = moment.min(moment(start), moment(state.activityPresetsEnd));
                    end = moment.max(moment(end), moment(state.activityPresetsStart));
                }
                else {
                    // within already loaded period
                    if (success) {
                        success(state.activityPresetDays);
                    }
                    return;
                }
            }

            api.listActivityPresetDays(start, end).then(presets => {
                commit(constants.FETCH_ACTIVITY_PRESET_DAYS_SUCCESS, { start, end, presets })
                if (success) {
                    success(presets);
                }
            }).fail(() => {
                if (failure) {
                    failure();
                }
            });
        },
        [constants.SAVE_ACTIVITY_PRESET_DAY]({ commit, state }, { date, preset, success, failure }) {
            api.saveActivityPresetForDay(date, preset.id).then(_ => {
                commit(constants.SAVE_ACTIVITY_PRESET_DAY_SUCCESS, { date, preset });
                if (success) {
                    success();
                }
            }).fail(() => {
                if (failure) {
                    failure();
                }
            });
        }
    },
    mutations: {
        [constants.LOGOUT_SUCCESS](state) {
            state.activityPresetsLoaded = false;
            state.activityPresets = [];
            state.activityPresetsStart = undefined;
            state.activityPresetsEnd = undefined;
            state.activityPresetDays = [];
        },
        [constants.FETCH_ACTIVITY_PRESETS_SUCCESS](state, { presets }) {
            state.activityPresetsLoaded = true;
            state.activityPresets = presets;
        },
        [constants.SAVE_ACTIVITY_PRESETS_SUCCESS](state, { presets }) {
            state.activityPresetsLoaded = true;
            state.activityPresets = presets;
        },
        [constants.SAVE_ACTIVITY_PRESET_DAY_SUCCESS](state, { date, preset }) {
            var index = state.activityPresetDays.findIndex(p => p.date.getDate() == date.getDate());
            if (index >= 0) {
                state.activityPresetDays.splice(index, 1);
            }
            state.activityPresetDays.push({ date, activityPresetId: preset.id });
        },
        [constants.FETCH_ACTIVITY_PRESET_DAYS_SUCCESS](state, { start, end, presets }) {
            presets.forEach(preset => {
                preset.date = new Date(preset.date);
                var index = state.activityPresetDays.findIndex(p => p.date.getDate() == preset.date.getDate());
                if (index >= 0) {
                    state.activityPresetDays.splice(index, 1);
                }
                state.activityPresetDays.push(preset);
            });
            if (!state.activityPresetsStart || moment(start).isBefore(state.activityPresetsStart)) {
                state.activityPresetsStart = start;
            }
            if (!state.activityPresetsEnd || moment(end).isAfter(state.activityPresetsEnd)) {
                state.activityPresetsEnd = end;
            }
        },
    }
}