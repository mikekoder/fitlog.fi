<template>
<layout >

  <span slot="title">{{ $t('progress') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="help" @click="showHelp"></q-btn>
  </div>
  <q-page class="q-pa-sm">
    <div class="row">
      <div class="col">
        <q-select v-model="exercise" @input="loadData" :options="exercises" :float-label="$t('exercise')" :display-value="exercise ? exercise.name : ''"/>
      </div>
    </div>
    <div class="row">
      <div class="col-4 q-pt-sm">
        <q-datetime v-model="start" type="date" @input="loadData" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames"  />
      </div>
      <div class="col-1 q-pt-sm" style="text-align: center;"><q-icon name="fas fa-minus" /></div>
      <div class="col-4 q-pt-sm">
        <q-datetime v-model="end" type="date" @input="loadData" :format="$t('dateFormat')" :monday-first="true" :no-clear="true" :ok-label="$t('OK')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" />
      </div>
      <div class="col-3 q-pt-md q-pl-sm" style="text-align: center;">
        <q-btn-dropdown glossy size="sm" color="primary" icon="fas fa-calendar" style="margin-top: -5px;">
            <q-list>
              <q-item @click.native="showMonths(1)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>1 {{ $t('month') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(2)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>2 {{ $t('months') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(3)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>3 {{ $t('months') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(6)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>6 {{ $t('months') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(12)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>1 {{ $t('year') }}</q-item-tile>
                </q-item-main>
              </q-item>
              <q-item @click.native="showMonths(24)" v-close-overlay>
                <q-item-main>
                  <q-item-tile label>2 {{ $t('years') }}</q-item-tile>
                </q-item-main>
              </q-item>
            </q-list>
          </q-btn-dropdown>
      </div>
    </div>
    <div class="row q-pa-sm">
      <div class="col">
          <graph-bar :chartData="data" :options="options" v-if="data"/>
          <div v-else>{{ $t('noData') }}</div>
      </div>
    </div>
    <div class="row q-pa-sm">
      <div class="col">
        <table>
          <thead>
            <tr>
              <th class="q-pr-md">{{ $t('time') }}</th>
              <th class="q-pr-md">{{ $t('1rm') }}</th>
              <th class="q-pr-md">{{ $t('1rmBW') }}</th>
              <th class="q-pr-md">{{ $t('1rmInclBW') }}</th>
              <th>{{ $t('volume') }} / {{ $t('workout') }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in tableData">
              <td class="q-pr-md">{{ formatDateTime(row.time) }}</td>
              <td class="q-pr-md">{{ formatDecimal(row.max, 2) }}</td>
              <td class="q-pr-md">{{ formatDecimal(row.maxBW, 2) }}</td>
              <td class="q-pr-md">{{ formatDecimal(row.maxInclBW, 2) }}</td>
              <td>{{ formatDecimal(row.totalVolume, 2) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <exercise-progress-help ref="help" />
  </q-page>
  </layout>
</template>

<script src="./exercise-progress.js">
</script>

<style scoped>
</style>