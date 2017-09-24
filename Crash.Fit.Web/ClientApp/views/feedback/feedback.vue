<template>
    <div>
        <section class="content-header">
          <h1 v-if="type == 'Bug'">{{ $t("bugs") }}</h1>
          <h1 v-if="type == 'Improvement'">{{ $t("improvements") }}</h1>
        </section>
        <section class="content">
          <div class="row">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="createFeedback">
                    <span v-if="type == 'Bug'">{{ $t("reportBug") }}</span>
                    <span v-if="type == 'Improvement'">{{ $t("askImprovement") }}</span>
                </button>
            </div>
          </div>
        <div class="row">
            <div class="col-sm-12">
                <br />
            </div>
          </div>
          <div class="row">
            <div class="col-sm-12">
                <template v-for="item in itemsSorted">
                    <div class="box box-solid" v-bind:class="{voted: userHasVoted(item.id)}">
                        <div class="box-header with-border">
                            <h3 class="box-title">{{ item.title }}</h3>
                        </div>
                        <div class="box-body">
                            <div class="row">
                                <div class="col-sm-12">
                                    {{ item.description }}
                                    <template v-if="userHasVoted(item.id)">
                                        <a class="btn btn-app pull-right" v-bind:title="$t('youHaveVoted')">
                                            <span class="badge bg-red">{{ item.score }}</span>
                                            <i class="fa fa-thumbs-o-up"></i>
                                        </a>
                                    </template>
                                    <template v-else>
                                        <button class="btn btn-app pull-right" @click="vote(item)" :disabled="item.locked">
                                            <span class="badge bg-red">{{ item.score }}</span>
                                            <i class="fa fa-thumbs-o-up"></i> {{ $t('vote') }}
                                        </button>
                                    </template>
                                </div>
                            </div>
                            <div class="row" v-if="item.adminComment">
                                <div class="col-sm-12">
                                    <pre>{{ item.adminComment }}</pre>
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
            </div>
        </div>
          
        </section>
  </div>
</template>

<script>
    import constants from '../../store/constants'
    import toaster from '../../toaster'
export default {
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
                toaster.error(self.$t('fetchFailed'));
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
                toaster.error(self.$t('fetchFailed'));
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
                toaster.error(self.$t('saveFailed'));
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
                toaster.error(self.$t('fetchFailed'));
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
</script>

<style scoped>
    .box.voted i { color: blue; }
</style>