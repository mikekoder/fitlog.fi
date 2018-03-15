<template>
  <q-page class="q-pa-sm">
    <div class="row">
      <q-input type="text" v-model="title" :float-label="$t('title')" />
    </div>
    <div class="row q-my-sm">
      <q-input type="textarea" v-model="description" :float-label="$t('description')" rows="5" />
    </div>
    <div class="row">
      <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
      <q-btn glossy color="primary" @click="save" :label="$t('save')" :disabled="!canSave"></q-btn>
    </div>
  </q-page>
</template>

<script src="./feedback-details.js">
</script>

<style scoped>
</style>