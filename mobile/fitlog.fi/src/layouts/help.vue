<template>
  <q-layout view="lHh Lpr lFf">
    <q-layout-header>
      <q-toolbar color="tertiary" glossy>
        <q-btn flat dense round @click="close">
          <q-icon name="keyboard_arrow_left" />
        </q-btn>
        <q-toolbar-title>
          {{ $t('help.title') }}
        </q-toolbar-title>

      </q-toolbar>
    </q-layout-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script>
export default {
  name: 'LayoutMenu',
  data () {
    return {
      title: 'fitlog'
    }
  },
  computed: {
  },
  watch:{
    $route(){
      this.handleMeta();
    }
  },
  methods: {
    handleMeta(){
      var title = this.$route.meta.title;
      this.title = this.$t(title);
    },
    close(){
      this.$router.go(-1);
    }
  }
}
</script>

<style>
</style>
