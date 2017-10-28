<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutritionGoals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input class="form-control" v-model="name" />
                    </div>

                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="addPeriod">{{ $t("addPeriod") }}</button>
                    <button class="btn btn-danger" @click="deletePeriod" v-if="selectedPeriod">{{ $t("deleteSelectedPeriod") }}</button>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-xs-4 col-sm-3 col-md-2 master">
                    <div class="box box-solid" v-for="period in periods" v-bind:class="{ selected: period == selectedPeriod }" @click="selectPeriod(period)">
                        <div class="box-header with-border">
                            {{ daysFormatted(period) }}
                        </div>
                        <div class="box-body">
                            {{ mealsFormatted(period) }}
                        </div>
                    </div>
                </div>
                <div class="col-xs-8" v-if="selectedPeriod">
                    <label>{{ $t('onlyDays') }}</label>
                    <table class="days">
                        <tr>
                            <td>{{ $t("mondayShort") }}</td>
                            <td>{{ $t("tuesdayShort") }}</td>
                            <td>{{ $t("wednesdayShort") }}</td>
                            <td>{{ $t("thursdayShort") }}</td>
                            <td>{{ $t("fridayShort") }}</td>
                            <td>{{ $t("saturdayShort") }}</td>
                            <td>{{ $t("sundayShort") }}</td>
                            <td class="divider"></td>
                            <td><i class="fa fa-heartbeat"></i></td>
                            <td><i class="fa fa-bed"></i></td>
                        </tr>
                        <tr>
                            <td><input type="checkbox" v-model="selectedPeriod.monday" v-bind:title="$t('monday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.tuesday" v-bind:title="$t('tuesday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.wednesday" v-bind:title="$t('wednesday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.thursday" v-bind:title="$t('thursday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.friday" v-bind:title="$t('friday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.saturday" v-bind:title="$t('saturday')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.sunday" v-bind:title="$t('sunday')" /></td>
                            <td class="divider"></td>
                            <td><input type="checkbox" v-model="selectedPeriod.exerciseDay" v-bind:title="$t('exerciseDay')" /></td>
                            <td><input type="checkbox" v-model="selectedPeriod.restDay" v-bind:title="$t('restDay')" /></td>
                        </tr>
                    </table>
                    
                    <table>
                        <tbody>
                            <tr>
                                <td><input type="radio" :value="true" v-model="selectedPeriod.wholeDay" /></td>
                                <td>{{ $t('wholeDay') }}</td>
                            </tr>
                            <tr>
                                <td><input type="radio" :value="false" v-model="selectedPeriod.wholeDay" /></td>
                                <td>{{ $t('perMeal') }}</td>
                            </tr>
                            <template v-if="!selectedPeriod.wholeDay">
                            <tr>
                                <td colspan="2"><label>{{ $t('onlyMeals') }}</label></td>
                            </tr>
                            <tr v-for="mealdef in $mealDefinitions">
                                <td><input type="checkbox" v-model="selectedPeriod.mealDefinitions[mealdef.id]"/></td>
                                <td>{{ mealdef.name }}</td>
                            </tr>
                            </template>
                        </tbody>
                    </table>
                     <table class="nutrient-goals">
                        <tbody v-for="group in $nutrientGroups">
                            <tr>
                                <th class="clickable" @click="toggleGroup(group.id)">
                                    <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                    <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                    {{ $t(group.id) }}
                                </th>
                                <td>&nbsp;</td>
                            </tr>
                            <tr v-for="nutrient in nutrients[group.id]" v-if="groupOpenStates[group.id]">
                                <td><span class="name">{{ nutrient.name }}</span> <span class="unit">{{ unit(nutrient.unit) }}</span></td>
                                <td><input type="number" class="form-control input-4" v-model="selectedPeriod.nutrients[nutrient.id].min" /> - <input type="number" class="form-control input-4" v-model="selectedPeriod.nutrients[nutrient.id].max" /></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row" v-if="periods.length == 0">
              <div class="col-sm-12">
                <br />
                {{ $t("noPeriods") }}
              </div>
            </div>
            <hr />
            <div class="row" v-if="periods.length > 0">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t('save') }}</button>
                </div>
            </div>
          
            <div class="row">
                <div class="col-sm-12">
                    <p>
                        {{ $t('nutritionGoalsInfo') }}
                    </p>
                </div>
            </div>
        </section>
        
    </div>
</template>

<script src="./nutrition-goal-details.js">
</script>

<style scoped>
    div.selected {
        border: 1px solid #00c0ef;
    }
    table.nutrient-goals{
        width: auto;
        table-layout: fixed; 
    }
    table.nutrient-goals input 
    {
        padding-left: 6px;
        padding-right: 2px;
    }
    table.nutrient-goals thead > tr > th {
        text-align:center;
    }
    table.nutrient-goals thead > tr > th:nth-child(n+2),
    table.nutrient-goals tbody > tr > th:nth-child(n+2), 
    table.nutrient-goals tbody > tr >  td:nth-child(n+2){
        height: 20px;
        border-right: 1px solid black;
        border-left: 1px solid black;
    }
    table.days td{text-align:center;}
    table.days td.divider{width: 2px; border-left:1px solid black;}
    table input[type=checkbox], table input[type=radio]{ width: 25px; height: 25px;}
    input.form-control { display:initial;}
</style>