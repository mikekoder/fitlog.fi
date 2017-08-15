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
          <template v-for="item in items">
            <h3>{{ item.title }}</h3>
            </template>
        </section>
  </div>
</template>

<script>
    var constants = require('../../store/constants')

module.exports = {
    data () {
        return {
          type: '',
          items: []
        }
    },
    components: {},
    methods: {
      loadItems: function(){
        var self = this;
        self.type = self.$route.meta.type;
        if(self.type === 'Bug'){
          self.$store.dispatch(constants.FETCH_BUGS, {
            success: function (bugs) {
                self.items = bugs;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure: function () {
                toaster(self.$t('fetchFailed'));
            }
          });
        }
        else if(self.type === 'Improvement'){
          self.$store.dispatch(constants.FETCH_IMPROVEMENTS, {
            success: function (improvements) {
                self.items = improvements;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure: function () {
                toaster(self.$t('fetchFailed'));
            }
          });
        }
      },
      createFeedback: function(){
        if(this.type === 'Bug'){
          this.$router.push({ name: 'bug-details', params: { id: constants.NEW_ID } });
        }
        else if(this.type === 'Improvement'){
          this.$router.push({ name: 'improvement-details', params: { id: constants.NEW_ID } });
        }
      }
      
    },
    created () {
      this.loadItems();
    },
    watch: {
      $route: function(){
        this.loadItems();
      }
    }
}
</script>

<style scoped>
</style>