<template>
<layout >

  <span slot="title">{{ $t('exercise') }}</span>

  <div slot="toolbar">
    <q-btn flat size="lg" icon="help" @click="showHelp"></q-btn>
      <q-btn flat size="lg" icon="save" @click="save" :disabled="!canSave"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <div class="row">
      <q-input type="text" v-model="name" :float-label="$t('name')" ref="nameInput" />
    </div>
    <div class="row q-pa-sm" v-for="muscleGroup in muscleGroups">
      <q-toggle v-model="targets" :label="muscleGroup.name" :val="muscleGroup.id" />
    </div>
    <div class="row q-ma-sm">
      <q-input type="number" v-model="percentageBW" suffix="%" :float-label="$t('bodyWeightPercentage')" />
    </div>
    <exercise-help ref="help" />
  </q-page>
  </layout>
</template>

<script src="./exercise-details.js">
</script>

<style lang="stylus">
</style>
