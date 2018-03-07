<template>
  <q-page class="q-pa-sm">
      <q-scroll-area>
        <q-list>
            <q-item>
              <div class="col-8"></div>
              <div class="col-2">{{ $t('sets') }}</div>
              <div class="col-2">{{ $t('1RM') }}</div>
            </q-item>
            <q-item v-for="(exercise,index) in ownExercises" @click.native="showExercise(exercise)" :key="index" :separator="true">
              <div class="col-8">{{ exercise.name }}</div>
              <div class="col-2">{{ exercise.usageCount }}</div>
              <div class="col-2">{{ formatDecimal(exercise.oneRepMax) }}</div>
            </q-item>
        </q-list>
      </q-scroll-area>
    <div class="row pad">
      <q-btn color="primary" glossy icon="fa-plus" @click="createExercise" :label="$t('exercise')"></q-btn>
    </div>
  </q-page>
</template>

<script src="./exercises.js">
</script>

<style lang="stylus">
</style>
