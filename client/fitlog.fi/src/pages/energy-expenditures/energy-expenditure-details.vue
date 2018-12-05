<template>
    <q-modal ref="modal" maximized>
        <q-toolbar color="tertiary" glossy>
            <q-toolbar-title>
                {{ $t('energyExpenditure') }}
            </q-toolbar-title>
        </q-toolbar>
        <div class="row q-ma-sm q-mb-md">
            <div class="col">
                <q-radio v-model="manual" :val="false" :label="$t('selectActivity')"></q-radio>
                <q-radio v-model="manual" :val="true" :label="$t('inputManual')"></q-radio>
            </div>
        </div>
        <div class="row q-ma-sm">
            <div class="col">
                <q-datetime v-model="time" type="datetime" :format="$t('datetimeFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" :float-label="$t('time')" format24h ref="timeInput" />
            </div>
        </div>
        <div v-if="manual">
            <div class="row  q-ma-sm">
                <div class="col">
                    <q-input type="text" v-model="activityName" :float-label="$t('activityDescription')" ref="nameInput" />
                </div>
            </div>
            <div class="row q-ma-sm">
                <div class="col">
                    <q-input type="number" v-model="energyKcal" min="0" :float-label="$t('energyExpenditure')"></q-input>
                </div>
            </div>
        </div>
        <div v-else>
            <div class="row q-ma-sm">
                <div class="col">
                    <q-select v-model="activity" :options="activities" :filter="filter" :float-label="$t('activity')" :display-value="activity ? activity.name : ''"/>
                </div>
            </div>
            <div class="rol q-ma-sm">
                <div class="col">
                    <q-datetime v-model="duration" type="time" :format="$t('timeFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" :float-label="$t('duration')" format24h />
                </div>
            </div>
            <div class="row q-ma-sm" v-if="estimate">
                <div class="col">
                    <q-input :value="formatDecimal(estimate)" :float-label="$t('calculation')" suffix="kcal" readonly></q-input>
                </div>
            </div>
        </div>

        <div class="row q-ma-sm">
            <div class="col">
                <q-btn glossy @click="cancel" :label=" $t('cancel')"></q-btn>
                <q-btn glossy color="primary" @click="save" :disabled="!canSave" :label="$t('save')"></q-btn>
            </div>
        </div>
    </q-modal>
</template>

<script src="./energy-expenditure-details.js">
</script>

<style scoped>
</style>