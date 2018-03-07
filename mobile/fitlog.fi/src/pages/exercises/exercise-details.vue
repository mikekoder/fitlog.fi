<template>
  <q-page class="q-pa-sm">
    <div class="row">
      <q-input type="text" v-model="name" :float-label="$t('name')" ref="nameInput" />
    </div>
    <div class="row q-pa-sm" v-for="muscleGroup in muscleGroups">
      <q-toggle v-model="targets" :label="muscleGroup.name" :val="muscleGroup.id" />
    </div>
    <div class="row">
      <q-input type="number" v-model="percentageBW" suffix="%" :float-label="$t('bodyWeightPercentage')" />
    </div>
    <div class="row">
      <q-btn glossy @click="cancel" :label="$t('cancel')"></q-btn>
      <q-btn glossy color="primary" @click="save" :label="$t('save')"></q-btn>
    </div>
  </q-page>
</template>

<script src="./exercise-details.js">
</script>

<style lang="stylus">
</style>
