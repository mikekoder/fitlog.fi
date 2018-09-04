<template>
<layout >

  <span slot="title">{{ $t('mealRhythm') }}</span>

  <div slot="toolbar">
        <q-btn flat @click="addMeal" icon="fas fa-plus" :label="$t('meal')" class="q-mr-sm"></q-btn>
        <q-btn flat size="md" @click="save"><q-icon name="save"></q-icon></q-btn>
  </div>
<q-page class="q-pa-sm">
        <q-card v-for="(def, index) in definitions" :key="index" class="q-ma-sm">
          <q-card-main>
            <div class="row">
                  <div class="col-10"><q-input v-model="def.name" type="text" :float-label="$t('name')" /></div>
            </div>    
            <div class="row q-mt-sm">
                <div class="col-5">
                    <q-datetime v-model="def.start" format24h type="time" :float-label="$t('starts')" />
                </div>
                <div class="col-5">   
                    <q-datetime v-model="def.end" format24h type="time" :float-label="$t('ends')" />
                </div>
                <div class="col-2"><q-btn round glossy size="sm" color="primary" icon="fas fa-trash" @click="removeMeal(index)" /></div>
            </div>
          </q-card-main>
        </q-card>
        <q-card class="q-ma-sm">
        <q-card-main>
            <div class="row">
                <div class="col-10">
                    <q-input v-model="othersName" type="text" :float-label="$t('otherTimes')" />
                </div>
            </div>
        </q-card-main>
        </q-card>
        <!--
    <div class="row q-ma-sm q-my-lg">
        <q-btn glossy @click="addMeal" color="primary" icon="fas fa-plus" :label="$t('meal')" class="q-mr-sm"></q-btn>
        <q-btn glossy @click="save" color="primary" :label="$t('save')"></q-btn>
    </div>
    -->
</q-page>
</layout>
</template>

<script src="./meal-rhythm.js">
</script>

<style lang="stylus">
</style>
