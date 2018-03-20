<template>
  <q-page class="q-pa-sm">
      <q-scroll-area style="height: 80vh;">
        <q-list v-if="measures.length > 0">
            <q-item>
              <div class="col-7"></div>
              <div class="col-2">{{ $t('measure') }}</div>
              <div class="col-2">{{ $t('time') }}</div>
            </q-item>
            <q-item v-for="(measure,index) in measures" @click.native="showProgress(measure)" :key="index" :separator="true">
              <div class="col-7">{{ measure.name }}</div>
              <div class="col-2">{{ measure.latestValue }}<span v-if="measure.latestValue"> {{ formatUnit(measure.unit) }}</span></div>
              <div class="col-2">{{ formatDateTime(measure.latestTime) }}</div>
            </q-item>
        </q-list>
        <div v-else>{{ $t('noMeasurements') }}</div>
      </q-scroll-area>
    <div class="row q-mt-lg">
      <q-btn color="primary" glossy icon="fa-plus" @click="createMeasurements" :label="$t('create')"></q-btn>
    </div>
  </q-page>
</template>

<script src="./measurements.js">
</script>

<style scoped>
</style>