<template>
<layout>

  <span slot="title">{{ $t('workout') }}</span>

  <div slot="toolbar">
      <q-btn flat icon="help" @click="showHelp"></q-btn>
      <q-btn flat icon="comment" :color="comment ? 'secondary': 'default'" @click="showComment"></q-btn>
      <q-btn flat icon="save" @click="save"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <q-card>
      <q-card-main>
        <div class="row">
            <div class="col-6">
              <q-datetime v-model="time" type="datetime" :format="$t('datetimeFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" :float-label="$t('time')" format24h ref="timeInput" />
            </div>
            <div class="col-3 q-pl-sm">
              <q-datetime v-model="duration" type="time" :format="$t('timeFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" :float-label="$t('duration')" format24h />
            </div>
            <div class="col-3 q-pl-sm">
              <q-input v-model="energyExpenditure" type="number" :float-label="$t('expenditure') + ' (kcal)'" @blur="energySpecified=(energyExpenditure && true)" />
            </div>
        </div>
      </q-card-main>
    </q-card>

    <q-card v-for="(group, index) in groups" :key="index" class="q-ma-sm">
    <q-card-title class="card-title" :class="cardTitleBackground">
        <div v-if="group.exercise" @click="selectExercise(group)">
            {{ group.exercise.name }}
        </div>
        <div v-else @click="selectExercise(group)">
            {{ $t('selectExercise') }}
        </div>
        <q-fab slot="right" flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
          <q-fab-action color="negative" @click="deleteGroup(index)" icon="delete"></q-fab-action>
          <q-fab-action color="secondary" @click="copyGroup(group)" icon="content_copy"></q-fab-action>
          <q-fab-action color="primary" @click="selectExercise(group)" icon="fitness_center"></q-fab-action>
        </q-fab>
        <div slot="subtitle" style="width:100%;" v-if="group.exercise" @click="selectExercise(group)">
          {{ $t('1rm') }}: {{ formatDecimal(group.exercise.oneRepMax, 1) || '-'}} 
          {{ $t('latestWeights') }}: {{ group.exercise.latestWeights || '-'}}
        </div>
    </q-card-title>
    <q-card-separator />
    <q-card-main>
        <div v-if="group.collapsed">
            <span v-for="(set,index) in group.sets">
                <span v-if="set.reps">{{ set.reps }} x {{ set.weights }}
                    <span v-if="index < group.sets.length - 1">, </span>
                </span>
            </span>
        </div>
        <div v-else>
            <div class="row q-my-md" v-if="group.sets.length == 0">
                <div class="col-10">
                </div>
                <div class="col-2">
                    <q-btn class="float-right btn-add-set" round glossy color="primary" icon="fas fa-plus" size="sm" @click="addSet(group)"></q-btn>
                </div>
            </div>
            <div class="row q-mt-sm" v-else v-for="(set,index) in group.sets" :key="index">
                <div class="col-3 q-pr-sm">
                    <q-input v-model="set.reps" type="number" :float-label="index == 0 ? $t('reps') : ''" />
                </div>
                <div class="col-3">
                    <q-input v-model="set.weights" type="number" :float-label="index == 0 ? $t('weights') : ''" />
                </div>
                <div class="col-2">
                    <q-fab size="sm" flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                        <q-fab-action color="negative" @click="deleteSet(group, index)" icon="delete"></q-fab-action>
                        <q-fab-action color="secondary" @click="copySet(group, set)" icon="content_copy"></q-fab-action>
                    </q-fab>
                </div>
                <div class="col-2">
                </div>
                <div class="col-2">
                    <q-btn class="float-right btn-add-set" round glossy color="primary" icon="fas fa-plus" size="sm" @click="addSet(group)" v-if="index == group.sets.length - 1"></q-btn>
                </div>
            </div>
        </div>
    </q-card-main>
    <q-card-actions align="center">
        <q-btn flat color="primary" icon="expand_more" size="sm" @click="() => group.collapsed = false" v-if="group.collapsed"></q-btn>
        <q-btn flat color="primary" icon="expand_less" size="sm" @click="() => group.collapsed = true" v-if="!group.collapsed"></q-btn>
    </q-card-actions>
    </q-card>
    
    <div class="row q-my-md">
        <q-btn glossy color="primary" icon="fas fa-plus" @click="addGroup" :label="$t('exercise')"></q-btn>
    </div>
    <workout-help ref="help" />
    <workout-comment ref="comment" @ok="commentOk" />
  </q-page>
  <exercise-picker ref="exercisePicker" @selected="exerciseSelected(arguments[0])" />
  </layout>
</template>

<script src="./workout-details-grouped.js">
</script>

<style lang="stylus" scoped>
.q-card-primary{
    padding-top:2px;
    padding-bottom: 0px;
}

.q-card-actions {
    padding: 0px;
}
.btn-add-set{
  margin-top: 8px;
  margin-right: 12px;
}
</style>
