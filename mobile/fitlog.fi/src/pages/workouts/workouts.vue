<template>
  <q-page class="q-pa-sm">
    <q-pull-to-refresh :handler="refresh" :pull-message="$t('')" :release-message="$t('')" :refresh-message="$t('')" :distance="20" style="height: 70px;">
    <q-card>
      <q-card-main>
        <div class="row">
          <div class="col-4 q-pt-sm">
            <q-datetime :value="start" type="date" @change="changeStart" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames"  />
          </div>
          <div class="col-1 q-pt-sm" style="text-align: center;"><q-icon name="fa-minus" /></div>
          <div class="col-4 q-pt-sm">
            <q-datetime :value="end" type="date" @change="changeEnd" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" />
          </div>
          <div class="col-3 q-pl-sm">
            <q-btn-dropdown glossy color="primary" icon="fa-plus" split @click="createWorkout" v-if="activeRoutine && activeRoutine.workouts.length > 0"  style="margin-top: -5px;">
              <!-- dropdown content -->
              <q-list>
                <q-item v-for="workout in activeRoutine.workouts" @click.native="createWorkout(activeRoutine.id, workout.id)">
                  <q-item-main>
                    <q-item-tile label>{{ workout.name }}</q-item-tile>
                  </q-item-main>
                </q-item>
                <q-item @click.native="createWorkout">
                  <q-item-main>
                    <q-item-tile label>{{ $t('freeWorkout') }}</q-item-tile>
                  </q-item-main>
                </q-item>
              </q-list>
            </q-btn-dropdown>
            <q-btn glossy size="sm" color="primary" icon="fa-plus" v-else :label="$t('workout')" @click="createWorkout"></q-btn>
          </div>
        </div>
      </q-card-main>
    </q-card>
    </q-pull-to-refresh>
    <template v-for="(workout,index) in workouts">
      <q-card :key="index" class="q-mt-sm" @click.native="clickWorkout(workout)">
        <q-card-title :class="cardTitleBackground">
          {{ formatDateTime(workout.time) }}
        </q-card-title>
        <q-card-separator />
        <q-card-main>
          <span v-for="exercise in workout.exercises" class="q-pa-sm">{{ exercise }}</span>
        </q-card-main>
      </q-card>
      </template>
      <workouts-help ref="help" />
  </q-page>
</template>

<script src="./workouts.js">
</script>

<style lang="stylus">
</style>
