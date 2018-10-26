import api from '../api'
import constants from './constants'

export default {
    state: {
        bugsLoaded: false,
        improvementsLoaded: false,
        votesLoaded: false,
        bugs: [],
        improvements: [],
        votes: []
    },
    actions: {
        [constants.FETCH_BUGS]({ commit, state }, { forceRefresh }) {
            if (state.bugsLoaded && !forceRefresh) {
                return Promise.resolve(state.bugs);
            }
            return api.listBugs().then(response => {
                commit(constants.FETCH_BUGS_SUCCESS, { bugs: response.data })
                return response.data;
            });
        },
        [constants.FETCH_IMPROVEMENTS]({ commit, state }, { forceRefresh }) {
            if (state.improvementsLoaded && !forceRefresh) {
               return Promise.resolve(state.improvements);
            }
            return api.listImprovements().then(response => {
                commit(constants.FETCH_IMPROVEMENTS_SUCCESS, { improvements: response.data })
                return response.data;
            });
        },
        [constants.SAVE_FEEDBACK]({ commit, state }, { feedback }) {
            return api.saveFeedback(feedback).then(response =>  {
                commit(constants.SAVE_FEEDBACK_SUCCESS, { feedback: response.data })
                return response.data;
            });
        },
        [constants.FETCH_VOTES]({ commit, state }, { forceRefresh }) {
            if (state.votesLoaded && !forceRefresh) {
                return Promise.resolve(state.votes);
            }
            return api.getVotes().then(response => {
                commit(constants.FETCH_VOTES_SUCCESS, { votes: response.data })
                return response.data;
            });
        },
        [constants.SAVE_VOTE]({ commit, state }, { feedbackId }) {
            return api.saveVote(feedbackId).then(response => {
                commit(constants.SAVE_VOTE_SUCCESS, { feedbackId })
                return response.data;
            });
        }
    },
    mutations: {
        [constants.FEEDBACK_CLEAR](state) {
            state.bugsLoaded = false;
            state.bugs = [];
            state.improvementsLoaded = false;
            state.improvements = [];
            state.votesLoaded = false;
            state.votes = [];
        },
        [constants.SAVE_FEEDBACK_SUCCESS](state, { feedback }) {
            if (feedback.type === 'Bug') {
                state.bugs.push(feedback);
            }
            else if (feedback.type === 'Improvement') {
                state.improvements.push(feedback);
            }
        },
        [constants.FETCH_BUGS_SUCCESS](state, { bugs }) {
            state.bugsLoaded = true;
            state.bugs = bugs;
        },
        [constants.FETCH_IMPROVEMENTS_SUCCESS](state, { improvements }) {
            state.improvementsLoaded = true;
            state.improvements = improvements;
        },
        [constants.FETCH_VOTES_SUCCESS](state, { votes }) {
            state.votes = votes;
            state.votesLoaded = true;
        },
        [constants.SAVE_VOTE_SUCCESS](state, { feedbackId }) {
            state.votes.push(feedbackId);
        }
    }
}