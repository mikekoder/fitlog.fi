var api = require('../api')
var constants = require('./constants')

const state = {
    loading: false,
    bugsLoaded: false,
    improvementsLoaded: false,
    votesLoaded: false,
    bugs: [],
    improvements: [],
    votes: []
}

// actions
const actions = {
    [constants.FETCH_BUGS] ({commit, state},{forceRefresh, success, failure}) {
        if(state.bugsLoaded && !forceRefresh){
            if(success){
                success(state.bugs);
            }
        }
        api.listBugs().then(function(bugs){
            commit(constants.FETCH_BUGS_SUCCESS, { bugs })
            if(success){
                success(bugs);
            }
        }).fail(function(){
            commit(constants.FETCH_BUGS_FAILURE)
            if(failure){
                failure();
            }
        });
    },
    [constants.FETCH_IMPROVEMENTS]({ commit, state }, { forceRefresh, success, failure }) {
        if (state.improvementsLoaded && !forceRefresh) {
            if (success) {
                success(state.improvements);
            }
        }
        api.listImprovements().then(function (improvements) {
            commit(constants.FETCH_IMPROVEMENTS_SUCCESS, { improvements })
            if (success) {
                success(improvements);
            }
        }).fail(function () {
            commit(constants.FETCH_IMPROVEMENTS_FAILURE)
            if (failure) {
                failure();
            }
        });
    },
    [constants.SAVE_FEEDBACK]({commit, state}, {feedback, success, failure}) {
        api.saveFeedback(feedback).then(function (savedFeedback) {
            commit(constants.SAVE_FEEDBACK_SUCCESS, { feedback: savedFeedback })
            if (success) {
                success(savedFeedback);
            }
        }).fail(function () {
            commit(constants.SAVE_FEEDBACK_FAILURE)
            if (failure) {
                failure();
            }
        });
    },
    [constants.FETCH_VOTES]({ commit, state }, { forceRefresh, success, failure }) {
        if (state.votesLoaded && !forceRefresh) {
            if (success) {
                success(state.votes);
            }
        }
        api.getVotes().then(function (votes) {
            commit(constants.FETCH_VOTES_SUCCESS, { votes })
            if (success) {
                success(votes);
            }
        }).fail(function () {
            if (failure) {
                failure();
            }
        });
    },
    [constants.SAVE_VOTE]({commit, state}, {feedbackId, success, failure}) {
        api.saveVote(feedbackId).then(function () {
            commit(constants.SAVE_VOTE_SUCCESS, { feedbackId })
            if (success) {
                success();
            }
        }).fail(function () {
            commit(constants.SAVE_FEEDBACK_FAILURE)
            if (failure) {
                failure();
            }
        });
    }
}

// mutations
const mutations = {
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


module.exports = { state, actions, mutations }