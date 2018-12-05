<template>
  <layout>
    <span slot="title">{{ $t('transferExerciseData') }}</span>

    <div slot="toolbar">
      <!--
      <q-btn flat icon="help" @click="showHelp"></q-btn>
      -->
    </div>
    <q-page class="q-pa-sm">
      <div class="row q-ma-sm">
          <div class="col">
            <span class="label">{{ $t('fromExercise')}}</span>
          </div>
        </div>
        <div class="row q-ma-sm" v-if="fromExercise">
          <div class="col">
            <q-input :value="fromExercise.name" readonly></q-input>
          </div>
        </div>
        <div class="row q-ma-sm">
          <div class="col">
            <span class="label">{{ $t('toExercise')}}</span>
          </div>
        </div>
        <div class="row q-ma-sm q-mb-lg">
            <div class="col" >
                <div v-if="toExercise">
                  <q-btn :label="toExercise.name" @click="selectExercise"></q-btn>
                </div>
                <div v-else>
                  <q-btn :label="$t('selectExercise')" @click="selectExercise"></q-btn>
                </div>
            </div>
        </div>
        <div class="row q-ma-sm">
          <div class="col">
            <q-toggle v-model="transferWorkouts" :label="$t('transferWorkouts')" />
          </div>
        </div>
        <div class="row q-ma-sm">
          <div class="col">
            <q-toggle v-model="transferRoutines" :label="$t('transferRoutines')" />
          </div>
        </div>
        <div class="row q-ma-sm">
          <div class="col">
            <q-toggle v-model="transfer1rm" :label="$t('transfer1rm')" />
          </div>
        </div>
        <div class="row q-ma-sm q-mt-lg">
            <div class="col">
              <!--
                <q-btn glossy @click="cancel" :label=" $t('cancel')"></q-btn>
                -->
                <q-btn glossy color="primary" @click="transfer" :disabled="!canTransfer" :label="$t('transfer')"></q-btn>
            </div>
        </div>
        
    </q-page>
    <exercise-picker ref="exercisePicker" @selected="exerciseSelected(arguments[0])" />
  </layout>
</template>

<script src="./exercise-transfer.js">
</script>

<style scoped>
</style>