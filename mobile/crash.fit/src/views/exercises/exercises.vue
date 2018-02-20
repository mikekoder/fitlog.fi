<template>
  <div :class="{desktop: isDesktop }">
      <q-scroll-area>
          <q-list>
              <q-item v-for="(exercise,index) in ownExercises" @click="showExercise(exercise)" :key="index" :separator="true">
                <div class="col-9">{{ exercise.name }}</div>
                  <div class="col-2">{{ formatDecimal(exercise.oneRepMax) }}</div>
                </q-item>
          </q-list>
      </q-scroll-area>
    <div class="row pad">
      <q-btn round color="primary" glossy small icon="fa-plus" @click="createExercise"></q-btn>
    </div>
  </div>
</template>

<script src="./exercises.js">
</script>

<style lang="stylus">
</style>
