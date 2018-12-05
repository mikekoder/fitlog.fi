<template>
<layout>

  <span slot="title">{{ $t('workouts') }}</span>

  <div slot="toolbar">
    <!--
    <q-btn flat icon="help" @click="showHelp"></q-btn>
    -->
    <q-btn-dropdown flat icon="fas fa-plus" :label="$t('workout')"  v-if="activeRoutine && activeRoutine.workouts.length > 0" id="workout-options">
      <q-list>
        <q-list-header>{{ activeRoutine.name }}</q-list-header>
        <q-item v-for="workout in activeRoutine.workouts" @click.native="createWorkout(activeRoutine.id, workout.id)">
          <q-item-main>
            <q-item-tile label>{{ workout.name }}</q-item-tile>
          </q-item-main>
        </q-item>
        <q-item-separator />
        <q-item @click.native="createWorkout(undefined, undefined)">
          <q-item-main>
            <q-item-tile label>{{ $t('freeWorkout') }}</q-item-tile>
          </q-item-main>
        </q-item>
      </q-list>
    </q-btn-dropdown>
    <q-btn flat icon="fas fa-plus" v-else :label="$t('workout')" @click="createWorkout(undefined, undefined)"></q-btn>
  </div>

  <q-page class="q-pa-sm">
    <q-pull-to-refresh :handler="refresh" :pull-message="$t('')" :release-message="$t('')" :refresh-message="$t('')" :distance="20" style="height: 70px;">
    <q-card>
      <q-card-main>
        <div class="row">
          <div class="col-4 q-pt-sm">
            <q-datetime :value="start" type="date" @change="changeStart" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames"  />
          </div>
          <div class="col-1 q-pt-sm" style="text-align: center;"><q-icon name="fas fa-minus" /></div>
          <div class="col-4 q-pt-sm">
            <q-datetime :value="end" type="date" @change="changeEnd" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" />
          </div>
          <div class="col-3 q-pt-md q-pl-sm" style="text-align: center;">
            <q-btn-dropdown glossy size="sm" color="primary" icon="fas fa-calendar" style="margin-top: -5px;">
                <q-list>
                  <q-item @click.native="showDays(7)" v-close-overlay>
                    <q-item-main>
                      <q-item-tile label>1 {{ $t('week') }}</q-item-tile>
                    </q-item-main>
                  </q-item>
                  <q-item @click.native="showDays(14)" v-close-overlay>
                    <q-item-main>
                      <q-item-tile label>2 {{ $t('weeks') }}</q-item-tile>
                    </q-item-main>
                  </q-item>
                  <q-item @click.native="showMonths(1)" v-close-overlay>
                    <q-item-main>
                      <q-item-tile label>1 {{ $t('month') }}</q-item-tile>
                    </q-item-main>
                  </q-item>
                  <q-item @click.native="showMonths(2)" v-close-overlay>
                    <q-item-main>
                      <q-item-tile label>2 {{ $t('months') }}</q-item-tile>
                    </q-item-main>
                  </q-item>
                  <q-item @click.native="showMonths(3)" v-close-overlay>
                    <q-item-main>
                      <q-item-tile label>3 {{ $t('months') }}</q-item-tile>
                    </q-item-main>
                  </q-item>
                  <q-item @click.native="showMonths(6)" v-close-overlay>
                    <q-item-main>
                      <q-item-tile label>6 {{ $t('months') }}</q-item-tile>
                    </q-item-main>
                  </q-item>
                </q-list>
              </q-btn-dropdown>
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
  </layout>
</template>

<script src="./workouts.js">
</script>

<style lang="stylus">
#workout-options .q-btn-dropdown-arrow {
  display:none;
}
</style>
