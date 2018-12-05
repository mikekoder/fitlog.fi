<template>
<layout >

  <span slot="title">{{ $t('nutritionChart') }}</span>

  <div slot="toolbar">
    <!--
    <q-btn flat icon="help" @click="showHelp"></q-btn>
    -->
  </div>
  <q-page class="q-pa-sm">
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
    <div class="row">
      <div class="col col-xs-12 col-sm-6 col-md-3 q-pa-sm" v-for="(nutrient,index) in selectedNutrients">
        <q-select v-model="selectedNutrients[index]" :options="nutrients" :float-label="`${$t('dataset')} ${index + 1}`" @input="showGraph"></q-select>
      </div>
    </div>
    <div class="row q-pa-sm">
      <div class="col">
          <graph-bar :chartData="data" :options="options" v-if="data && options"/>
          <div v-else>{{ $t('noData') }}</div>
      </div>
    </div>

    <!--
    <exercise-progress-help ref="help" />
    -->
  </q-page>
  </layout>
</template>

<script src="./nutrition-chart.js">
</script>

<style scoped>
</style>