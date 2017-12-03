<template>
    <div v-if="!loading">    
        <section class="content-header"><h1>{{ $t("energyExpenditures") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <div class="btn-group">
                        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                            {{ formatDate(start) }} - {{ formatDate(end) }} <span class="caret"></span>
                        </button>
                        <ul class="dropdown-menu">
                            <li class="clickable"><a @click="showWeek">{{ $t("currentWeek") }}</a></li>
                            <li class="clickable"><a @click="showMonth">{{ $t("currentMonth") }}</a></li>
                            <li role="separator" class="divider"></li>
                            <li class="clickable"><a @click="showDays(7)">7 {{ $t("days") }}</a></li>
                            <li class="clickable"><a @click="showDays(14)">14{{ $t("days") }}</a></li>
                            <li class="clickable"><a @click="showDays(30)">30{{ $t("days") }}</a></li>
                            <li role="separator" class="divider"></li>
                            <li class="custom-date"><span>{{ $t("timeInterval") }}</span></li>
                            <li class="custom-date">
                                <datetime-picker :value="start" :format="'DD.MM.YYYY'" @change="changeStart"></datetime-picker>
                                <datetime-picker :value="end" :format="'DD.MM.YYYY'" @change="changeEnd"></datetime-picker>
                            </li>
                        </ul>
                    </div>
                    <button class="btn btn-primary" @click="createEnergyExpenditure()">{{ $t("create") }}</button>
                    
                </div>
            </div>
            <div class="row" v-if="energyExpenditures.length > 0">
                <div class="col-sm-12">
                    <table class="table">
                        <thead>
                            <tr>
                                <th>{{ $t("activity") }}</th>
                                <th>{{ $t("duration") }}</th>
                                <th>{{ $t("energyExpenditure") }} (kcal)</th>
                                <th>{{ $t("time") }}</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="expenditure in energyExpenditures">
                                <td>
                                    <router-link :to="{ name: 'workout-details', params: { id: expenditure.workoutId } }" v-if="expenditure.workoutId">{{ $t('workout') }}</router-link>
                                    <a class="clickable" v-else @click="editEnergyExpenditure(expenditure)">{{ expenditure.activityName }}</a>
                                </td>
                                <td>{{ formatDuration(expenditure.hours, expenditure.minutes) }}</td>
                                <td>{{ formatDecimal(expenditure.energyKcal) }}</td>
                                <td>{{ formatDateTime(expenditure.time) }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="energyExpenditures.length == 0">
                <div class="col-sm-12">
                    <br />
                    {{ $t("noData") }}
                </div>
            </div>
        </section>
        <section v-if="showEditEnergyExpenditure">
            <energy-expenditure-editor :show="showEditEnergyExpenditure" :energyExpenditure="energyExpenditure" @save="saveEnergyExpenditure" @close="showEditEnergyExpenditure=false"></energy-expenditure-editor>
        </section>
</div>
</template>

<script src="./energy-expenditures.js">
</script>

<style scoped>
    table{ width: auto;}
</style>