import api from '../api'
import constants from './constants'
import moment from 'moment'

export default {
    state: {
        date: new Date()
    },
    actions: {
        [constants.SELECT_HOME_DATE]({ commit, state }, { date, success, failure }) {
            state.date = date;
        },
        [constants.SAVE_HOME_SETTINGS]({ commit, state }, { settings, success, failure }) {
            api.saveHomeSettings(settings).then(function () {
                commit(constants.SAVE_HOME_NUTRIENTS_SUCCESS, { nutrients: settings.nutrients })
                if (success) {
                    success();
                }
            }).fail(function () {
                if (failure) {
                    failure();
                }
            });
        }
    },
    mutations: {

    }
}