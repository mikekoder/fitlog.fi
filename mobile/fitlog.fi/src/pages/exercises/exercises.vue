<template>
<layout >

  <span slot="title">{{ $t('exercises') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="fas fa-plus" @click="createExercise" :label="$t('exercise')"></q-btn>
  </div>
  <q-page class="q-pa-sm">
        <q-list v-if="ownExercises.length > 0">
            <q-item class="text-bold">
              <q-item-main>
                <div class="row">
                  <div class="col-8"></div>            
                  <div class="col-2">{{ $t('sets') }}</div>
                  <div class="col-2">{{ $t('1RM') }}<br />(1 {{ $t('monthsAbbr') }})</div>
              </div>
              </q-item-main>
            </q-item>
            <q-item v-for="(exercise,index) in ownExercises"  :key="index" :separator="true"  @click.native="clickExercise(exercise)">
              <q-item-main>
                <div class="row">
                  <div class="col-8">{{ exercise.name }}</div>
                  <div class="col-2">{{ exercise.usageCount }}</div>
                  <div class="col-2">{{ formatDecimal(exercise.oneRepMax) }}</div>
                </div>
              </q-item-main>
            </q-item>
        </q-list>
        <div v-else>{{ $t('noExercises') }}</div>
  </q-page>
  </layout>
</template>

<script src="./exercises.js">
</script>

<style lang="stylus">
</style>
