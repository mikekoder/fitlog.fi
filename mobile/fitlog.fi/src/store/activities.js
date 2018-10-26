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
        [constants.FETCH_ACTIVITY_PRESETS]({ commit, state }) {
            return api.listActivityPresets().then(response => {
                commit(constants.FETCH_ACTIVITY_PRESETS_SUCCESS, { presets: response.data });
                return response.data;
            });
        },
        [constants.SAVE_ACTIVITY_PRESETS]({ commit, state }, { presets }) {
            return api.saveActivityPresets(presets).then(response => {
                commit(constants.SAVE_ACTIVITY_PRESETS_SUCCESS, { presets: response.data });
                return response.data;
            });
        },
        [constants.FETCH_ACTIVITY_PRESET_DAYS]({ commit, state }, { start, end }) {
            if (state.activityPresetsStart && state.activityPresetsEnd) {
                if (moment(start).isBefore(state.activityPresetsStart) || moment(end).isAfter(state.activityPresetsEnd)) {
                    start = moment.min(moment(start), moment(state.activityPresetsEnd));
                    end = moment.max(moment(end), moment(state.activityPresetsStart));
                }
                else {
                   return Promise.resolve(state.activityPresetDays);
                }
            }

            return api.listActivityPresetDays(start, end).then(response => {
                commit(constants.FETCH_ACTIVITY_PRESET_DAYS_SUCCESS, { start, end, presets: response.data })
                return response.data;
            });
        },
        [constants.SAVE_ACTIVITY_PRESET_DAY]({ commit, state }, { date, preset }) {
            return api.saveActivityPresetForDay(date, preset.id).then(response => {
                commit(constants.SAVE_ACTIVITY_PRESET_DAY_SUCCESS, { date, preset });
                return preset;
            });
        }
    },
    mutations: {
        [constants.ACTIVITIES_CLEAR](state) {
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