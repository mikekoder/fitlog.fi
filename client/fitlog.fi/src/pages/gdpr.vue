<template>
<layout >

  <span slot="title">{{ $t("privacy") }}</span>

  <div slot="toolbar"></div>
  <q-page class="q-pa-sm">
    {{ $t('privacyInfo') }}
  </q-page>
  </layout>
</template>

<script>

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
