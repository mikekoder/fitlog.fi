<template>
  <q-page class="q-pa-sm">

    <q-scroll-area style="height: 90vh;">
        <div class="row q-my-md">
            <q-datetime v-model="time" type="datetime" :format="$t('datetimeFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" :float-label="$t('time')" format24h />
        </div>
        <div class="row q-my-md" v-for="(set,index) in sets" :key="index">
            <div class="col-6">
                <div v-if="exercises.length > 20">
                    <q-input color="amber" v-model="set.exerciseName" :float-label="$t('exercise')" >
                        <q-autocomplete @search="searchExercise" :min-characters="1" @selected="(exercise) => exerciseSelected(set,exercise)" />
                    </q-input>
                </div>
                <div v-else>
                    <q-select v-model="set.exerciseName" :options="exercises" :float-label="$t('exercise')" @change="(exercise) => exerciseSelected(set,exercise)" />
                </div>
            </div> 
             <div class="col-2">
                <q-input v-model="set.reps" type="number" :float-label="$t('reps')" />
            </div>
            <div class="col-2">
                <q-input v-model="set.weights" type="number" :float-label="$t('weights')" />
            </div>
            <div class="col-2">
                <q-fab small flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                    <q-fab-action color="negative" @click="deleteSet(index)" icon="delete"></q-fab-action>
                    <q-fab-action color="secondary" @click="copySet(index)" icon="content_copy"></q-fab-action>
                </q-fab>
            </div>
        </div>
        
        <div class="row q-my-md">
            <q-btn glossy small color="primary" icon="fa-plus" @click="addSet" :label="$t('exercise')"></q-btn>
        </div>

        <div class="row q-my-md">
            <q-btn glossy @click="cancel" :label="$t('cancel')"></q-btn>
            <q-btn glossy color="primary" @click="save" :label="$t('save')"></q-btn>
        </div>
    </q-scroll-area>
    
  </q-page>
</template>

<script src="./workout-details.js">
</script>

<style lang="stylus" scoped>
/*
.scroll { height: 100%;}
.desktop .q-tab-pane { height: 70vh;}
.desktop .q-scrollarea { height: 100%;}
*/
</style>
