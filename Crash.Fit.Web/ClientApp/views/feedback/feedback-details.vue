<template>
    <div>
        <section class="content-header">
          <h1 v-if="type == 'Bug'">{{ $t("bugs") }}</h1>
          <h1 v-if="type == 'Improvement'">{{ $t("improvements") }}</h1>
        </section>
        <section class="content">
          <div class="row">
            <div class="col-sm-12">
              <div class="form-group">
                <label>{{ $t('title') }}</label><br />
                <input type="text" class="form-control" v-model="title" />
              </div>
            </div>
          </div>
          <div class="row">
            <div class="col-sm-12">
              <div class="form-group">
                <label>{{ $t('description') }}</label>
                <br />
                <textarea class="form-control" v-model="description"></textarea>
              </div>
            </div>
          </div>
          <div class="row main-actions">
            <div class="col-sm-12">
              <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
              <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
              <button class="btn btn-danger" @click="deleteFeedback" v-if="id">{{ $t("delete") }}</button>
            </div>
          </div>
        </section>
  </div>
</template>

<script>
    import constants from '../../store/constants'

export default {
    data () {
        return {
          id: undefined,
          type: undefined,
          title: '',
          description: ''
        }
    },
    components: {},
    methods: {
      save() {
        var self = this;
        var feedback = {
            id: self.id,
            type: self.type,
            title: self.title,
            description: self.description
        };
        self.$store.dispatch(constants.SAVE_FEEDBACK, {
            feedback,
            success() {
                if(feedback.type === 'Bug'){
                  self.$router.replace({ name: 'bugs' });
                }
                else if(feedback.type === 'Improvement'){
                  self.$router.replace({ name: 'improvements' });
                }
            },
            failure() {
                toaster(self.$t('saveFailed'));
            }
        });
      },
      cancel() {
        this.$router.go(-1);
      },
      deleteFeedback: function(){}
    },
    created () {
        this.type = this.$route.meta.type;
        this.$store.commit(constants.LOADING_DONE);
    },
    watch: {
      $route: function(){
        this.type = this.$route.meta.type;
        this.$store.commit(constants.LOADING_DONE);
      }
    }
}
</script>

<style scoped>
</style>