<template>
  <div :class="{desktop: isDesktop }">
    <div class="row pad">
        <div class="col-8">
            <q-input type="text" v-model="name" :float-label="$t('name')" />
        </div>
        <div class="col-4 col-button">
            <q-btn color="primary" glossy small icon="fa-plus" @click="addWorkout">{{ $t('workout') }}</q-btn>
        </div>
      
    </div>
    <q-tabs v-model="tab">
      <q-tab slot="title" v-for="(workout, index) in workouts" :name="'tab-' + index" :label="workout.name" :key="index" />
      
      <q-scroll-area style="height: 66vh;">
          <q-tab-pane v-for="(workout, index) in workouts" :name="'tab-' + index" :key="index">
            <div class="row">
                <q-input type="text" v-model="workout.name" :float-label="$t('name')" />
            </div>
            <q-card v-for="(group, index) in workout.groups" :key="index">
                <q-card-title class="card-title bg-grey-3">
                    <div class="row">
                        <div class="col-8">
                            <div v-if="exercises.length > 20">
                                <!--
                                <q-input color="amber" v-model="group.exerciseName" :float-label="$t('exercise')" >
                                    <q-autocomplete @search="searchExercise" :min-characters="1" @selected="(exercise) => exerciseSelected(group,exercise)" />
                                </q-input>
                                -->
                            </div>
                            <div v-else>
                                <q-select v-model="group.exercise" :options="exercises" :float-label="$t('exercise')" :display-value="group.exercise ? group.exercise.name : ''"/>
                            </div>
                        </div>
                        <div class="col-4">
                        </div>
                    </div>    
                    
                </q-card-title>
                <q-card-separator />
                <q-card-main>
                    <div class="row pad set" v-for="(row,index) in group.rows" :key="index">
                        <div class="col-3">
                            <q-input v-model="row.sets" type="number" :float-label="index == 0 ? $t('sets') : ''" />
                        </div>
                        <div class="col-3">
                            <q-input v-model="row.reps" type="number" :float-label="index == 0 ? $t('reps') : ''" />
                        </div>
                        <div class="col-3">
                            <q-input v-model="row.loadFrom" type="number" :float-label="index == 0 ? $t('load') : ''" suffix="%" />
                        </div>
                        <div class="col-2">
                            <q-fab small flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                                <q-fab-action color="negative" @click="deleteRow(group, index)" icon="delete"></q-fab-action>
                                <q-fab-action color="secondary" @click="copyRow(group, row)" icon="content_copy"></q-fab-action>
                            </q-fab>
                        </div>
                    </div>
                </q-card-main>
                <q-card-actions align="end">
                    <q-btn color="primary" icon="fa-plus" small glossy @click="addRow(group)">{{ $t('set') }}</q-btn>
                </q-card-actions>
            </q-card>
            <div class="row pad buttons">
                <q-btn glossy color="primary" icon="fa-plus" small @click="addGroup(workout)">{{ $t('exercise') }}</q-btn>
            </div>  
          </q-tab-pane>
      </q-scroll-area>
      
    </q-tabs>
    <div class="row pad buttons">
        <q-btn @click="cancel">{{ $t('cancel') }}</q-btn>
        <q-btn color="primary" @click="save">{{ $t('save') }}</q-btn>
    </div>
  </div>
</template>

<script src="./routine-details-grouped.js">
</script>

<style lang="stylus" scoped>
.q-tab-pane { height: 60vh;}
.scroll { height: 100%;}
.desktop .q-tab-pane { height: 70vh;}
.desktop .q-scrollarea { height: 100%;}
.row.set div:not(:last-child) { padding-right: 5px; }
.card-title > .col { flex-wrap: nowrap; }
</style>
