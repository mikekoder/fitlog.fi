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
        this.type = this.$route.meta.type;
        if(this.type === 'Bug'){
          this.$store.dispatch(constants.FETCH_BUGS, { }).then(bugs => {
            this.items = bugs;
            this.$store.commit(constants.LOADING_DONE, { });
        }).catch(_ => {
          this.notifyError(this.$t('fetchFailed'));
        });
        }
        else if(this.type === 'Improvement'){
          this.$store.dispatch(constants.FETCH_IMPROVEMENTS, { }).then(improvements => {
            this.items = improvements;
            this.$store.commit(constants.LOADING_DONE, { });
        }).catch(_ => {
          this.notifyError(this.$t('fetchFailed'));
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
        this.$store.dispatch(constants.SAVE_VOTE, {
          feedbackId: feedback.id
        }).then(_ => {
          feedback.score++;
        }).catch(_ => {
          this.notifyError(this.$t('saveFailed'));
        });
      }
    },
    created() {
      this.$store.dispatch(constants.FETCH_VOTES, { }).catch(_ => {
        this.notifyError(this.$t('fetchFailed'));
        });
        this.loadItems();
    },
    watch: {
      $route(){
        this.loadItems();
      }
    }
}