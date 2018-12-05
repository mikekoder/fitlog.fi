<template>
<layout >

  <span slot="title">{{ $t('measurements') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="fas fa-plus" @click="createMeasurements" :label="$t('create')"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <q-list v-if="measures.length > 0">
        <q-item>
          <q-item-main>
            <div class="row">
              <div class="col-7"></div>
              <div class="col-2">{{ $t('measure') }}</div>
              <div class="col-2">{{ $t('time') }}</div>
            </div>
          </q-item-main>

        </q-item>
        <q-item v-for="(measure,index) in measures" @click.native="showProgress(measure)" :key="index" :separator="true">
          <q-item-main>
            <div class="row">
              <div class="col-7">{{ measure.name }}</div>
              <div class="col-2">{{ measure.latestValue }}<span v-if="measure.latestValue"> {{ formatUnit(measure.unit) }}</span></div>
              <div class="col-2">{{ formatDateTime(measure.latestTime) }}</div>
            </div>
          </q-item-main>

        </q-item>
    </q-list>
    <div v-else>{{ $t('noMeasurements') }}</div>
  </q-page>
  </layout>
</template>

<script src="./measurements.js">
</script>

<style scoped>
</style>