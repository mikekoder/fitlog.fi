<template>
    <q-modal v-model="visible" maximized>
        <div class="row q-pa-md  q-display-1">
            <div class="col-10 q-pt-sm">
                {{ $t('help.title') }}
            </div>
            <div class="col-2 q-pr-md">
                <q-btn flat size="lg" icon="close" @click="close" />
            </div>
        </div>
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