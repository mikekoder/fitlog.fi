<template>
  <q-page class="q-pa-sm">
    <q-scroll-area style="height: 90vh;">
        <q-card>
            <q-card-main>
                <q-datetime v-model="time" type="datetime" :format="$t('datetimeFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" :float-label="$t('time')" format24h ref="timeInput" />
            </q-card-main>
        </q-card>

        <q-card v-for="(group, index) in groups" :key="index" class="q-ma-sm">
        <q-card-title class="card-title" :class="cardTitleBackground">
            <div class="row">
                <div class="col-10">
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
                <div class="col-2 group-actions">
                    <q-fab small flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                        <q-fab-action color="negative" @click="deleteGroup(index)" icon="delete"></q-fab-action>
                        <q-fab-action color="secondary" @click="copyGroup(group)" icon="content_copy"></q-fab-action>
                    </q-fab>
                    
                </div>
            </div>
        </q-card-title>
        <q-card-separator />
        <q-card-main>
            <div class="row q-my-md" v-if="group.sets.length == 0">
                <div class="col-10">
                </div>
                <div class="col-2">
                    <q-btn round glossy color="primary" icon="fa-plus" small @click="addSet(group)"></q-btn>
                </div>
            </div>
            <div class="row q-my-md" v-else v-for="(set,index) in group.sets" :key="index">
                <div class="col-3 q-pr-sm">
                    <q-input v-model="set.reps" type="number" :float-label="index == 0 ? $t('reps') : ''" />
                </div>
                <div class="col-3">
                    <q-input v-model="set.weights" type="number" :float-label="index == 0 ? $t('weights') : ''" />
                </div>
                <div class="col-2">
                    <q-fab small flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                        <q-fab-action color="negative" @click="deleteSet(group, index)" icon="delete"></q-fab-action>
                        <q-fab-action color="secondary" @click="copySet(group, set)" icon="content_copy"></q-fab-action>
                    </q-fab>
                </div>
                <div class="col-2">
                </div>
                <div class="col-2">
                    <q-btn round glossy color="primary" icon="fa-plus" small @click="addSet(group)" v-if="index == group.sets.length - 1"></q-btn>
                </div>
            </div>
            
        </q-card-main>
        <q-card-actions align="end">
            <!--
            <q-btn round glossy color="primary" icon="fa-plus" small @click="addSet(group)"></q-btn>
            -->
        </q-card-actions>
      </q-card>
        
        <div class="row q-my-md">
            <q-btn glossy color="primary" icon="fa-plus" @click="addGroup" :label="$t('exercise')"></q-btn>
        </div>

        <div class="row q-my-md">
            <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
            <q-btn glossy color="primary" @click="save" :label="$t('save')" :disabled="!canSave"></q-btn>
        </div>
    </q-scroll-area>
    
  </q-page>
</template>

<script src="./workout-details-grouped.js">
</script>

<style lang="stylus" scoped>
/*
.q-tab-pane { height: 60vh;}
.scroll { height: 100%;}
.desktop .q-tab-pane { height: 70vh;}
.desktop .q-scrollarea { height: 100%;}
.row.set > div:not(:last-child) { padding-right: 5px; }
.card-title > .col { flex-wrap: nowrap; }
.group-actions {padding: 20px 0px 0px 10px; }
.exercise .q-card-container { padding-top: 0px; padding-bottom: 0px;}
.set > div:last-child{ padding-top: 12px; }
*/
</style>
