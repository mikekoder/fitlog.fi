<template>
  <q-page class="q-pa-sm">
    <div class="row pad">
        <div class="col-8">
            <q-input type="text" v-model="name" :float-label="$t('name')" />
        </div>
        <div class="col-4">
            <q-btn round color="primary" glossy size="sm" icon="fa-plus" @click="addWorkout"></q-btn>
        </div>
      
    </div>
    <q-tabs v-model="tab">
      <q-tab slot="title" v-for="(workout, index) in workouts" :name="'tab-' + index" :label="workout.name" :key="index" />
      
      <q-scroll-area style="height: 75vh;">
          <q-tab-pane v-for="(workout, index) in workouts" :name="'tab-' + index" :key="index">
            <div class="row">
                <q-input type="text" v-model="workout.name" :float-label="$t('name')" />
            </div>
            <div class="row pad" v-for="(exercise,index) in workout.exercises" :key="index">
            <div class="col-6">
                <div v-if="exercises.length > 20">
                    <q-input color="amber" v-model="exercise.exerciseName" :float-label="$t('exercise')" >
                        <q-autocomplete @search="searchExercise" :min-characters="1" @selected="(e) => exerciseSelected(exercise,e)" />
                    </q-input>
                </div>
                <div v-else>
                    <q-select v-model="exercise.exerciseName" :options="exercises" :float-label="$t('exercise')" @change="(e) => exerciseSelected(exercise,e)" />
                </div>
            </div> 
            <div class="col-2">
                <q-input v-model="exercise.sets" type="number" :float-label="$t('sets')" />
            </div>
             <div class="col-2">
                <q-input v-model="exercise.reps" type="number" :float-label="$t('reps')" />
            </div>
            <div class="col-2">
                <q-input v-model="exercise.loadFrom" type="number" :float-label="$t('load')" />
            </div>
            <div class="col-2">
                <q-fab size="sm" flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                    <q-fab-action color="negative" @click="deleteExercise(workout, index)" icon="delete"></q-fab-action>
                    <q-fab-action color="secondary" @click="copyExercise(workout, index)" icon="content_copy"></q-fab-action>
                </q-fab>
            </div>
        </div>
        <div class="row pad buttons">
            <q-btn round glossy color="primary" icon="fa-plus" size="sm" @click="addExercise(workout)"></q-btn>
        </div>  
          </q-tab-pane>
      </q-scroll-area>
      
    </q-tabs>
  </q-page>
</template>

<script src="./routine-details.js">
</script>

<style lang="stylus" scoped>
/*
.q-tab-pane { height: 60vh;}
.scroll { height: 100%;}
.desktop .q-tab-pane { height: 70vh;}
.desktop .q-scrollarea { height: 100%;}
*/
</style>
