<template>
<layout >

  <span slot="title">{{ $t('nutritionGoal') }}</span>

  <div slot="toolbar">
        <q-btn flat size="lg" @click="showHelp" icon="help"></q-btn>
        <q-btn flat size="lg" @click="save" icon="save" :disabled="!canSave"></q-btn>
  </div>
    <q-page class="q-pa-sm">
        <div class="row q-mx-sm q-mb-sm">
            <div class="col">
                <q-input type="text" v-model="name" :float-label="$t('name')" />
            </div>
        </div>
        <q-tabs v-model="tab" two-lines>
            <q-tab slot="title" v-for="(period, p_index) in periods" :name="'tab-' + p_index" :label="daysFormatted(period) + ' | ' + mealsFormatted(period)" :key="p_index" />
            <q-tab slot="title" :name="'tab-' + periods.length" :label="$t('preset')" icon="fa-plus" @click="addPeriod" />
                <q-tab-pane v-for="(period, p_index) in periods" :name="'tab-' + p_index" :key="p_index">
                    <div class="row">
                        <div class="col-10">
                            <div class="row">
                                <div class="col-10 text-bold">
                                    {{ $t('onlyDays') }}
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-1">
                                    {{ $t("mondayShort") }}
                                </div>
                                <div class="col-1">
                                        {{ $t("tuesdayShort") }}
                                </div>
                                <div class="col-1">
                                        {{ $t("wednesdayShort") }}
                                </div>
                                <div class="col-1">
                                        {{ $t("thursdayShort") }}
                                </div>
                                <div class="col-1">
                                        {{ $t("fridayShort") }}
                                </div>
                                <div class="col-1">
                                        {{ $t("saturdayShort") }}
                                </div>
                                <div class="col-1">
                                        {{ $t("sundayShort") }}
                                </div>
                                <div class="col-1">
                                    <q-icon name="fa-heartbeat" />
                                </div>
                                <div class="col-1">
                                    <q-icon name="fa-bed" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-1">
                                    <q-checkbox v-model="period.monday" />
                                </div>
                                <div class="col-1">
                                    <q-checkbox v-model="period.tuesday" />
                                </div>
                                <div class="col-1">
                                    <q-checkbox v-model="period.wednesday" />
                                </div>
                                <div class="col-1">
                                    <q-checkbox v-model="period.thursday" />
                                </div>
                                <div class="col-1">
                                    <q-checkbox v-model="period.friday" />
                                </div>
                                <div class="col-1">
                                    <q-checkbox v-model="period.saturday" />
                                </div>
                                <div class="col-1">
                                    <q-checkbox v-model="period.sunday" />
                                </div>
                                <div class="col-1">
                                    <q-checkbox v-model="period.exerciseDay" />
                                </div>
                                <div class="col-1">
                                    <q-checkbox v-model="period.restDay" />
                                </div>
                            </div>
                        </div>
                        <div class="col-2">
                            <q-fab size="sm" flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                                <q-fab-action color="negative" @click="deletePeriod(period)" icon="delete"></q-fab-action>
                            </q-fab>
                        </div>
                    </div>
                    
                    
                    <div class="row q-my-md">
                        <div class="col">
                            <q-radio v-model="period.wholeDay" :val="true" :label="$t('wholeDay')"></q-radio>
                            <q-radio v-model="period.wholeDay" :val="false" :label="$t('perMeal')"></q-radio>
                        </div>
                    </div>
                    <div v-if="!period.wholeDay">
                        <div class="row q-ma-md" v-for="mealdef in $mealDefinitions">
                            <div class="col">
                                <q-toggle v-model="period.mealDefinitions[mealdef.id]" :label="mealdef.name" />
                            </div>
                        </div>
                    </div>
                    
                    <template v-for="(group,index) in nutrientGroups">  
                        <div class="row">
                            <div class="col">
                                <q-btn flat @click="toggleGroup(group)" :label="$t(group.id)" :icon="selectedGroup == group ? 'fa-chevron-up' : 'fa-chevron-down'"></q-btn>
                            </div>
                        </div>
                        <div v-if="selectedGroup == group">
                            <div class="row" v-for="(nutrient,index_n) in nutrientsGrouped[group.id]" :key="index_n" v-if="nutrient.id != energyDistributionId">
                                <div class="col-6 q-pt-sm">{{ nutrient.name }}</div>
                                <div class="col-2"><q-input type="number" v-model="period.nutrients[nutrient.id].min" /></div>
                                <div class="col-1 q-pt-sm" style="text-align: center"><q-icon name="fa-minus" /></div>
                                <div class="col-2"><q-input type="number" v-model="period.nutrients[nutrient.id].max" /></div>
                                <div class="col-1 q-pt-sm">{{ formatUnit(nutrient.unit)}}</div>
                            </div>
                        </div>
                    </template>
                        
                </q-tab-pane>
        </q-tabs>
    <nutrition-goal-help ref="help" />
</q-page>
</layout>
</template>

<script src="./nutrition-goal-details.js">
</script>

<style scoped>
</style>