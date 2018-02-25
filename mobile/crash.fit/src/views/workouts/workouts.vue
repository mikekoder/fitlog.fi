<template>
  <div>
    <q-card>
      <q-card-main>
        <div class="row">
          <div class="col-4">
            <q-datetime v-model="start" type="date" @change="changeStart" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('ok')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames"  />
          </div>
          <div class="col-2" style="padding: 20px;">-</div>
          <div class="col-4">
            <q-datetime v-model="end" type="date" @change="changeEnd" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('ok')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" />
          </div>
          <div class="col-2">
            <q-fab small flat color="primary" icon="more_vert" active-icon="more_horiz" direction="down" @open="showTooltips" @close="hideTooltips">
                <q-fab-action color="negative" icon="fa-plus"><q-tooltip anchor="center left" self="center right" :offset="[20, 0]" ref="tooltip1">Yläkroppa</q-tooltip></q-fab-action>
                <q-fab-action color="secondary" icon="fa-plus" @click="createWorkout"></q-fab-action>
            </q-fab>
            <!--
            <q-btn round glossy small color="primary" class="pull-right" icon="fa-plus" @click="() => createWorkout()"></q-btn>
            -->
          </div>
        </div>
      </q-card-main>
    </q-card>
    <template v-for="(workout,index) in workouts">
      <q-card :key="index" @click="showWorkout(workout)">
        <q-card-title :class="cardTitleBackground">
          {{ formatDateTime(workout.time) }}
        </q-card-title>
        <q-card-separator />
        <q-card-main>
          <span v-for="exercise in workout.exercises" style="padding: 0px 5px;">{{ exercise }}</span>
        </q-card-main>
      </q-card>
      </template>
  </div>
</template>

<script src="./workouts.js">
</script>

<style lang="stylus">
</style>
