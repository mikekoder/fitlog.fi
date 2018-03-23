<template>
  <q-page class="q-pa-sm">
      <q-card>
      <q-card-main>
        <div class="row">
          <div class="col-4 q-pt-sm">
            <q-datetime :value="start" type="date" @change="changeStart" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames"  />
          </div>
          <div class="col-1 q-pt-sm" style="text-align: center;"><q-icon name="fa-minus" /></div>
          <div class="col-4 q-pt-sm">
            <q-datetime :value="end" type="date" @change="changeEnd" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" />
          </div>
          <div class="col-3 q-pl-sm">
            <q-btn glossy size="sm" color="primary" icon="fa-plus" :label="$t('expenditure')" @click="createEnergyExpenditure"></q-btn>
          </div>
        </div>
      </q-card-main>
    </q-card>
      <q-scroll-area style="height: 72vh;">
        <q-list v-if="energyExpenditures.length > 0">
            <q-item class="text-bold">
              <div class="col-4">{{ $t("activity") }}</div>
              <div class="col-2">{{ $t("duration") }}</div>
              <div class="col-2">{{ $t("expenditure") }} (kcal)</div>
              <div class="col-4">{{ $t("time") }}</div>
            </q-item>
            <q-item v-for="(expenditure, index) in energyExpenditures"  :key="index" :separator="true"  @click.native="clickEnergyExpenditure(expenditure)">
              <div class="col-4">
                <span v-if="expenditure.workoutId">{{ $t('workout') }}</span>
                <span v-else>{{ expenditure.activityName }}</span>
              </div>
              <div class="col-2">{{ formatDuration(expenditure.hours, expenditure.minutes) }}</div>
              <div class="col-2">{{ formatDecimal(expenditure.energyKcal) }}</div>
              <div class="col-4">{{ formatDateTime(expenditure.time) }}</div>
            </q-item>
        </q-list>
        <div v-else>{{ $t('noData') }}</div>
      </q-scroll-area>
    <div class="row q-my-lg">
      <q-btn color="primary" glossy icon="fa-plus" @click="createEnergyExpenditure" :label="$t('expenditure')"></q-btn>
    </div>
    <energy-expenditure-details ref="editEnergyExpenditureDetails" @save="saveEnergyExpenditure(arguments[0])" />
  </q-page>
</template>

<script src="./energy-expenditures.js">
</script>

<style scoped>
</style>