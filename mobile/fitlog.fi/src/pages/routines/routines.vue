<template>
  <q-page class="q-pa-sm">
      <q-scroll-area>
          <q-list v-if="routines.legth > 0">
              <q-item v-for="(routine,index) in routines" @click.native="showRoutine(routine)" :key="index" :separator="true">
                <div class="col-9">{{ routine.name }}</div>
              </q-item>
          </q-list>
          <div v-else>{{ $t('noRoutines') }}</div>
      </q-scroll-area>
    <div class="row q-my-lg">
      <q-btn color="primary" glossy icon="fa-plus" @click="createRoutine" :label="$t('routine')"></q-btn>
    </div>
  </q-page>
</template>

<script src="./routines.js">
</script>

<style lang="stylus">
</style>
