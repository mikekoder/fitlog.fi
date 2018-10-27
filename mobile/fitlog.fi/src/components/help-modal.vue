<template>
    <q-modal v-model="visible" maximized>
        <q-layout view="LHh lpR fff" :container="!isCordova" :class="{'boxed': !isCordova }">
            <q-layout-header>
                <q-toolbar color="tertiary" glossy>
                    <q-toolbar-title>
                        {{ $t('help.title') }}
                    </q-toolbar-title>
                    <q-btn flat icon="close" @click="close" />
                </q-toolbar>
            </q-layout-header>
            <q-page-container>
                <q-page>
                    <div class="row q-pa-md">
                        <div class="col">
                            <slot></slot>
                        </div>
                    </div>
                </q-page>
            </q-page-container>
        </q-layout>
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