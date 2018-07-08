<template>
<layout >

  <span slot="title">{{ $t('routine') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="help" @click="showHelp"></q-btn>
    <q-btn flat icon="save" @click="save" :disabled="!canSave"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <div class="row q-mx-sm q-mb-sm">
        <div class="col-8">
            <q-input type="text" v-model="name" :float-label="$t('name')" ref="nameInput" />
        </div>
        <div class="col-4 col-button">
        </div>
    </div>
    <q-tabs v-model="tab">
      <q-tab slot="title" v-for="(workout, t_index) in workouts" :name="'tab-' + t_index" :label="workout.name" :key="t_index" />
      <q-tab slot="title" :name="'tab-' + workouts.length" :label="$t('workout')" icon="fa-plus" @click="addWorkout" />
          <q-tab-pane v-for="(workout, w_index) in workouts" :name="'tab-' + w_index" :key="w_index">
            <div class="row q-mx-md">
                <div class="col-6">
                    <q-input type="text" v-model="workout.name" :float-label="$t('name')" />
                </div>
                 <div class="col-4 q-pl-sm">
                     <q-select v-model="workout.frequency" :options="frequencyPresets" :float-label="$t('frequency')"></q-select>
                </div>
                <div class="col-2">
                    <q-fab size="sm" flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                        <q-fab-action color="negative" @click="deleteWorkout(w_index)" icon="delete"></q-fab-action>
                     </q-fab>
                </div>
                
            </div>
            <q-card v-for="(group, g_index) in workout.groups" :key="g_index" class="q-mx-sm q-mb-sm">
                <q-card-title class="bg-grey-3">
                    <div class="row">
                        <div class="col-10"><!--
                            <div v-if="exercises.length > 20">
                                
                                <q-input color="amber" v-model="group.exerciseName" :float-label="$t('exercise')" >
                                    <q-autocomplete @search="searchExercise" :min-characters="1" @selected="(exercise) => exerciseSelected(group,exercise)" />
                                </q-input>
                                
                            </div>-->
                            <div>
                                <q-select v-model="group.exercise" :options="exercises" :float-label="$t('exercise')" :display-value="group.exercise ? group.exercise.name : ''"/>
                            </div>
                        </div>
                        <div class="col-2">
                            <q-fab size="sm" flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                                <q-fab-action color="negative" @click="deleteGroup(workout, g_index)" icon="delete"></q-fab-action>
                            </q-fab>
                        </div>
                    </div>
                </q-card-title>
                <q-card-separator />
                <q-card-main>
                    <div class="row" v-for="(row, r_index) in group.rows" :key="r_index">
                        <div class="col-3 q-pr-sm">
                            <q-input v-model="row.sets" type="number" :float-label="r_index == 0 ? $t('sets') : ''" />
                        </div>
                        <div class="col-3 q-pr-sm">
                            <q-input v-model="row.reps" type="number" :float-label="r_index == 0 ? $t('reps') : ''" />
                        </div>
                        <div class="col-4">
                            <q-input v-model="row.loadFrom" type="number" :float-label="r_index == 0 ? $t('load') : ''" suffix="%" />
                        </div>
                        <div class="col-2">
                            <q-fab size="sm" flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                                <q-fab-action color="negative" @click="deleteRow(group, r_index)" icon="delete"></q-fab-action>
                                <q-fab-action color="secondary" @click="copyRow(group, row)" icon="content_copy"></q-fab-action>
                            </q-fab>
                        </div>
                    </div>
                </q-card-main>
                <q-card-actions align="end">
                    <q-btn size="sm" glossy color="primary" icon="fa-plus" @click="addRow(group)" :label="$t('set')"></q-btn>
                </q-card-actions>
            </q-card>
            <div class="row q-ma-sm">
                <div class="col">
                    <q-btn size="md" glossy color="primary" icon="fa-plus" @click="addGroup(workout)" :label="$t('exercise')"></q-btn>
                </div>
                <div class="col">
                    
                </div>
                

            </div>  
          </q-tab-pane>
    </q-tabs>
    <routine-help ref="help" />
  </q-page>
  </layout>
</template>

<script src="./routine-details-grouped.js">
</script>

<style lang="stylus" scoped>
</style>
