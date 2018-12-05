<template>
  <q-modal v-model="visible" maximized>
    <q-modal-layout view="LHh lpR fff" :container="isDesktop" :class="{'boxed': isDesktop }">           
      <q-toolbar color="tertiary" glossy slot="header">
        <q-toolbar-title>
          {{ $t('help.title') }}
        </q-toolbar-title>
        <q-btn flat icon="close" @click="close" />
      </q-toolbar>
      <div class="row q-pa-md">
        <div class="col">
          <slot></slot>
        </div>
      </div>
    </q-modal-layout>
  </q-modal>
</template>
<script>
export default {
  name: 'help-modal',
  props: {
    visible: false
  },
  methods:{
    close(){
      this.$emit('close');
    }
  }
}
</script>
<style>
.boxed {
  width: 600px; 
  min-height: 500px; 
  margin: auto;
}
</style>