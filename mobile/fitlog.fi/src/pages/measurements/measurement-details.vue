<template>
<q-page class="q-pa-sm"> 
    <q-scroll-area style="height: 80vh;">
        <div class="row">
            <div class="col">
                <q-datetime v-model="time" type="datetime" :format="$t('datetimeFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" :float-label="$t('time')" format24h ref="timeInput" />
            </div>
        </div>
        <q-list>
            <q-item v-for="(measurement,index) in measurements">
                <div class="col-5">
                    <div v-if="measurement.id">{{ measurement.name }}</div>
                    <q-input type="text" v-else v-model="measurement.name" :float-label="$t('name')" />
                </div>
                <div class="col-3 q-px-sm">
                    <q-input type="number" v-model="measurement.value" :class="{ 'new': !measurement.id }" />
                </div>
                <div class="col-3">
                    <div v-if="measurement.id">{{ formatUnit(measurement.unit) }}</div>
                    <q-select v-model="measurement.unit" :options="units" v-if="!measurement.id" :float-label="$t('unit')"></q-select>
                </div>
                <div class="col-1">
                    <q-fab small flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left" v-if="!measurement.id">
                        <q-fab-action color="negative" @click="deleteMeasurement(index)" icon="delete"></q-fab-action>
                    </q-fab>
                </div>
            </q-item>
        </q-list>
    </q-scroll-area>
    <div class="row q-mt-sm">
        <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
        <q-btn glossy color="primary" @click="addMeasurement" icon="fa-plus" :label="$t('measure')" class="q-mr-sm" />
        <q-btn glossy color="primary" @click="save" :label="$t('save')" :disabled="!canSave"></q-btn>
    </div>
</q-page>
</template>

<script src="./measurement-details.js">
</script>
<style scoped>
.new {margin-top: 11px;}
</style>