<template>
  <comment-modal :visible="visible" :title="$t('comment')">
    <q-input v-model="comment" type="textarea" rows="7" />
    <div class="row q-ma-sm q-mt-lg">
        <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
        <q-btn glossy color="primary" @click="save" :label="$t('ok')"></q-btn>
    </div>
  </comment-modal>
</template>

<script>
import CommentModal from '../../components/comment-modal.vue'
export default {
    name: 'workout-comment',
    components: { CommentModal },
    data(){
      return {
        visible: false,
        comment: undefined
      }
    },
    methods: {
      open(comment){
        this.comment = comment;
        this.visible = true;
      },
      save(){
        this.$emit('ok', this.comment);
        this.visible = false;
      },
      cancel(){
        this.visible = false;
      }
    }
}
</script>

<style scoped>
</style>