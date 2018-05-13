import constants from '../../store/constants'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    data () {
        return {
          type: '',
          items: []
        }
    },
    computed: {
        itemsSorted() {
            return this.items.sort((a, b) => { return a.score < b.score; });
        },
        votes() {
            return this.$store.state.feedback.votes;
        }
    },
    components: {},
    methods: {
      loadItems(){
        var self = this;
        self.type = self.$route.meta.type;
        if(self.type === 'Bug'){
          self.$store.dispatch(constants.FETCH_BUGS, {
            success(bugs) {
                self.items = bugs;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() {
                self.notifyError(self.$t('fetchFailed'));
            }
          });
        }
        else if(self.type === 'Improvement'){
          self.$store.dispatch(constants.FETCH_IMPROVEMENTS, {
            success(improvements) {
                self.items = improvements;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() {
                self.notifyError(self.$t('fetchFailed'));
            }
          });
        }
      },
      createFeedback(){
        if(this.type === 'Bug'){
          this.$router.push({ name: 'bug-details', params: { id: constants.NEW_ID } });
        }
        else if(this.type === 'Improvement'){
          this.$router.push({ name: 'improvement-details', params: { id: constants.NEW_ID } });
        }
      },
      userHasVoted(feedbackId) {
          return this.votes.includes(feedbackId);
      },
      vote(feedback) {
          var self = this;
          self.$store.dispatch(constants.SAVE_VOTE, {
            feedbackId: feedback.id,
            success() {
                feedback.score++;
            },
            failure() {
                self.notifyError(self.$t('saveFailed'));
            }
          });
      }
    },
    created() {
        var self = this;
        self.$store.dispatch(constants.FETCH_VOTES, {
            success(votes) {
            },
            failure() {
                self.notifyError(self.$t('fetchFailed'));
            }
          });
        self.loadItems();
    },
    watch: {
      $route(){
        this.loadItems();
      }
    }
}