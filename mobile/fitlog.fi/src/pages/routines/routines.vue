<template>
<layout >

  <span slot="title">{{ $t('routines') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="fa-plus" @click="createRoutine" :label="$t('routine')"></q-btn>
  </div>
  <q-page class="q-pa-sm">
      <q-list v-if="routines.length > 0">
          <q-item v-for="(routine,index) in routines" @click.native="clickRoutine(routine)" :key="index" :separator="true">
            <div class="col-9">{{ routine.name }}</div>
            <div class="col-1"><q-icon name="fa-check" v-if="routine.active"></q-icon></div>
          </q-item>
      </q-list>
      <div v-else>{{ $t('noRoutines') }}</div>
  </q-page>
  </layout>
</template>

<script src="./routines.js">
</script>

<style lang="stylus">
</style>
