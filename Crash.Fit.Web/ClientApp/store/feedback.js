var api = require('../api')
var constants = require('./constants')

const state = {
    loading: false,
    bugsLoaded: false,
    improvementsLoaded: false,
    bugs: [],
    improvements: []
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
    }

}


module.exports = { state, actions, mutations }