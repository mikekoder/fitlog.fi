<template>
  <q-page class="q-pa-sm">
      <q-scroll-area style="height: 72vh;">
        <q-list v-if="ownExercises.length > 0">
            <q-item class="text-bold">
              <div class="col-8"></div>            
              <div class="col-2">{{ $t('sets') }}</div>
              <div class="col-2">{{ $t('1RM') }}<br />(1 {{ $t('monthsAbbr') }})</div>
            </q-item>
            <q-item v-for="(exercise,index) in ownExercises"  :key="index" :separator="true"  @click.native="clickExercise(exercise)">
              <div class="col-9">{{ exercise.name }}</div>
              <div class="col-2">{{ exercise.usageCount }}</div>
              <div class="col-2">{{ formatDecimal(exercise.oneRepMax) }}</div>
              </div>
            </q-item>
        </q-list>
        <div v-else>{{ $t('noExercises') }}</div>
      </q-scroll-area>
    <div class="row q-my-lg">
      <q-btn color="primary" glossy icon="fa-plus" @click="createExercise" :label="$t('exercise')"></q-btn>
    </div>
  </q-page>
</template>

<script src="./exercises.js">
</script>

<style lang="stylus">
</style>
