<template>
<layout >

  <span slot="title">{{ $t("privacy") }}</span>

  <div slot="toolbar"></div>
  <q-page class="q-pa-sm" v-html="$t('privacyInfo')">
  </q-page>
  </layout>
</template>

<script>
import PageMixin from '../mixins/page'
import constants from '../store/constants'

export default {
  mixins: [PageMixin],
  data () {
    return { }
  },
  created () {
    this.$store.commit(constants.LOADING_DONE);
  }
}
</script>

<style lang="stylus">
</style>
