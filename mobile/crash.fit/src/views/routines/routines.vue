<template>
  <div :class="{desktop: isDesktop }">
      <q-scroll-area>
          <q-list>
              <q-item v-for="(routine,index) in routines" @click="showRoutine(routine)" :key="index" :separator="true">
                <div class="col-9">{{ routine.name }}</div>
              </q-item>
          </q-list>
      </q-scroll-area>
    <div class="row pad">
      <q-btn round color="primary" glossy small icon="fa-plus" @click="createRoutine"></q-btn>
    </div>
  </div>
</template>

<script src="./routines.js">
</script>

<style lang="stylus">
</style>
