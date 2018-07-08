<template>
<layout >

  <span slot="title">{{ $t('workout') }}</span>

  <div slot="toolbar">
      <q-btn flat icon="save" @click="save"></q-btn>
  </div>
    <!--
    <q-toolbar slot="footer">
        <q-btn flat round dense icon="menu"/>
        <q-toolbar-title>
            Footer
        </q-toolbar-title>
        <q-btn flat icon="fa-plus" @click="addGroup" :label="$t('exercise')"></q-btn>
    </q-toolbar>
    -->
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
                    <q-input v-model="energyExpenditure" type="number" :float-label="$t('expenditure')" suffix="kcal" @blur="energySpecified=(energyExpenditure && true)" />
                </div>
            </div>
            
        
        </q-card-main>
    </q-card>

    <q-card v-for="(group, index) in groups" :key="index" class="q-ma-sm">
    <q-card-title class="card-title" :class="cardTitleBackground">
        <div class="row">
            <div class="col-10">
                <div>
                    <q-select v-model="group.exercise" :options="exercises" :float-label="$t('exercise')" :display-value="group.exercise ? group.exercise.name : ''"/>
                </div>
            </div>
            <div class="col-2 group-actions">
                <q-fab size="sm" flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
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
                <q-btn round glossy color="primary" icon="fa-plus" size="sm" @click="addSet(group)"></q-btn>
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
                <q-btn round glossy color="primary" icon="fa-plus" size="sm" @click="addSet(group)" v-if="index == group.sets.length - 1"></q-btn>
            </div>
        </div>
        
    </q-card-main>
    <q-card-actions align="end">
        <!--
        <q-btn round glossy color="primary" icon="fa-plus" size="sm" @click="addSet(group)"></q-btn>
        -->
    </q-card-actions>
    </q-card>
    
    <div class="row q-my-md">
        <q-btn glossy color="primary" icon="fa-plus" @click="addGroup" :label="$t('exercise')"></q-btn>
    </div>
  </q-page>
  </layout>
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
