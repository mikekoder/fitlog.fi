<template>
  <layout >

    <span slot="title">{{ $t('exercise') }}</span>

    <div slot="toolbar">
      <q-btn flat icon="help" @click="showHelp"></q-btn>
        <q-btn flat icon="save" @click="save" :disabled="!canSave"></q-btn>
    </div>
    <q-page class="q-pa-sm">
      <div class="row">
        <q-input type="text" v-model="name" :float-label="$t('name')" ref="nameInput" />
      </div>
      <div class="row q-my-sm">
        <q-select v-model="equipments" multiple :float-label="$t('equipment')" :options="equipmentOptions" clearable />
      </div>
      <div class="row q-my-sm">
        <q-select v-model="targets" multiple :float-label="$t('primaryMuscleGroups')" :options="muscleGroups" clearable />
      </div>
      <div class="row q-my-sm">
        <q-select v-model="secondaryTargets" multiple :float-label="$t('secondaryMuscleGroups')" :options="muscleGroups" clearable />
      </div>
      <div class="row q-my-sm">
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
