<template>
<layout >

  <span slot="title">{{ $t('exercises') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="fas fa-plus" @click="createExercise" :label="$t('exercise')"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <q-tabs v-model="tab">
      <q-tab slot="title" name="tab-1" :label="$t('own')" />
      <q-tab slot="title" name="tab-2" :label="$t('search')" />
      <q-scroll-area style="height: 85vh;">
        <q-tab-pane name="tab-1">
          <q-list v-if="ownExercises.length > 0">
            <q-item class="text-bold">
              <q-item-main>
                <div class="row">
                  <div class="col-8"></div>            
                  <div class="col-2">{{ $t('sets') }}</div>
                  <div class="col-2">{{ $t('1rm') }}<br />(1 {{ $t('monthsAbbr') }})</div>
              </div>
              </q-item-main>
            </q-item>
            <q-item v-for="(exercise,index) in ownExercises"  :key="index" :separator="true" @click.native="clickExercise(exercise)">
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
        </q-tab-pane>
        <q-tab-pane name="tab-2">
          <div class="row">
              <div class="col">
                  <q-select v-model="muscleGroup" :float-label="$t('muscleGroup')" :options="muscleGroups" :display-value="muscleGroupText" @input="search" clearable />
              </div>
                <div class="col">
                    <q-select v-model="equipment" :float-label="$t('equipment')" :options="equipments" :display-value="equipmentText" @input="search" clearable />
                </div>
          </div>
          <q-search v-model="searchText" :float-label="$t('search')" :placeholder="$t('exercise')" @input="search" :debounce="500" clearable></q-search>
          <q-list v-if="searchResults.length > 0">
              <q-item class="text-bold">
              <q-item-main>
                <div class="row">
                  <div class="col-8"></div>            
                  <div class="col-2">{{ $t('sets') }}</div>
                  <div class="col-2">{{ $t('1rm') }}<br />(1 {{ $t('monthsAbbr') }})</div>
              </div>
              </q-item-main>
            </q-item>
            <q-item v-for="(exercise,index) in searchResults" :key="index" :separator="true" @click.native="clickExercise(exercise)">
              <q-item-main>
                <div class="row">
                  <div class="col-8">{{ exercise.name }}</div>
                  <div class="col-2">{{ exercise.usageCount }}</div>
                  <div class="col-2">{{ formatDecimal(exercise.oneRepMax) }}</div>
                </div>
              </q-item-main>
            </q-item>
          </q-list>
          <div v-else>
              <span v-if="!searching && (searchText && searchText.length >= 2)">{{ $t('noExercises') }}</span>
          </div>
        </q-tab-pane>
      </q-scroll-area>
    </q-tabs> 
  </q-page>
  </layout>
</template>

<script src="./exercises.js">
</script>

<style lang="stylus">
</style>
