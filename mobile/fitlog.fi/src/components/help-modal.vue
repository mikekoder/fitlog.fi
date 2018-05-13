<template>
    <q-modal v-model="visible" maximized>
        <q-toolbar color="tertiary" glossy>
            <q-toolbar-title>
                {{ $t('help.title') }}
            </q-toolbar-title>
            <q-btn flat size="lg" icon="close" @click="close" />
        </q-toolbar>
        <div class="row q-pa-md">
            <div class="col">
                <slot></slot>
            </div>
        </div>
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