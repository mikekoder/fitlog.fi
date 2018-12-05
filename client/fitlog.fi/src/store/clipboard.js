import constants from './constants'

export default {
    state: {
        type: undefined,
        data: undefined,
    },
    actions: {
        [constants.CLIPBOARD_COPY]({ commit, state }, { type, data }) {
            commit(constants.CLIPBOARD_COPY_SUCCESS, { type, data });
        },
        [constants.CLIPBOARD_CLEAR]({ commit, state }, { }) {
            commit(constants.CLIPBOARD_CLEAR_SUCCESS, { });
        }
    },
    mutations: {
        [constants.CLIPBOARD_CLEAR](state) {
            state.type = undefined;
            state.data = undefined;
        },
        [constants.CLIPBOARD_COPY_SUCCESS](state, { type, data }) {
            state.type = type;
            state.data = data;
        },
        [constants.CLIPBOARD_CLEAR_SUCCESS](state, { type, data }) {
            state.type = undefined;
            state.data = undefined;
        }
    }
}