<template>
<layout >

  <span slot="title" v-if="type == 'Improvement'" >
      {{ $t('improvement') }}
  </span>
  <span slot="title" v-if="type == 'Bug'" >
       {{ $t('bug') }}
  </span>

  <div slot="toolbar">
    <q-btn flat icon="save" @click="save" :disabled="!canSave"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <div class="row">
      <div class="col">
        <q-input type="text" v-model="title" :float-label="$t('title')" />
      </div>
    </div>
    <div class="row q-my-sm">
        <div class="col">
          <q-input type="textarea" v-model="description" :float-label="$t('description')" rows="5" />
        </div>
    </div>
  </q-page>
  </layout>
</template>

<script src="./feedback-details.js">
</script>

<style scoped>
</style>