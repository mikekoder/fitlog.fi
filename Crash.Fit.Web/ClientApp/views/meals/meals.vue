<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("meals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <div class="btn-group">
                        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                            {{ date(start) }} - {{ date(end) }} <span class="caret"></span>
                        </button>
                        <ul class="dropdown-menu">
                            <li class="clickable"><a @click="showDay">{{ $t("today") }}</a></li>
                            <li class="clickable"><a @click="showWeek">{{ $t("currentWeek") }}</a></li>
                            <li class="clickable"><a @click="showMonth">{{ $t("currentMonth") }}</a></li>
                            <li role="separator" class="divider"></li>
                            <li class="clickable"><a @click="showDays(7)">7 {{ $t("days") }}</a></li>
                            <li class="clickable"><a @click="showDays(14)">14 {{ $t("days") }}</a></li>
                            <li class="clickable"><a @click="showDays(30)">30 {{ $t("days") }}</a></li>
                            <li role="separator" class="divider"></li>
                            <li class="custom-date"><span>{{ $t("timeInterval") }}</span></li>
                            <li class="custom-date">
                                <datetime-picker class="vue-picker1" name="picker1" v-bind:value="start" v-bind:format="'DD.MM.YYYY'" v-on:change="start=arguments[0]"></datetime-picker>
                                <datetime-picker class="vue-picker1" name="picker1" v-bind:value="end" v-bind:format="'DD.MM.YYYY'" v-on:change="end=arguments[0]"></datetime-picker>
                                <button class="btn btn-sm" @click="fetchMeals()">{{ $t("OK") }}</button>
                            </li>
                        </ul>
                    </div>
                    <button class="btn btn-primary" @click="createMeal()">{{ $t("log") }}</button>
                    <div class="btn-group" role="group" aria-label="...">
                        <template v-for="group in groups">
                            <button class="btn btn-default" v-bind:class="{ active: selectedGroup === group.id }" @click="selectGroup(group.id)">{{ $t(group.id) }}</button>
                        </template>
                        <button class="btn pull-right" @click="editSettings" v-if="!editNutrients"><i class="fa fa-gear"></i></button>
                        <button class="btn pull-right" @click="saveSettings" v-if="editNutrients">{{ $t('save') }}</button>
                    </div>

                    <div class="outer" v-if="days.length > 0">
                        <div class="inner">
                            <table class="table" id="meal-list">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <template v-for="col in visibleColumns">
                                            <th class="nutrient" v-if="!col.hideSummary"><div><input type="checkbox" v-if="editNutrients" /><div>{{ col.title }}</div></div></th>
                                        </template>
                                        <th></th>
                                    </tr>
                                    <tr>
                                        <th class="time freeze">{{ $t("time") }}</th>
                                        <template v-for="col in visibleColumns">
                                            <th class="unit" v-if="!col.hideSummary">{{ unit(col.unit) }}</th>
                                        </template>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <template v-for="day in days">
                                        <tr class="day" @click="toggleDay(day)">
                                            <td class="freeze clickable">
                                                <i v-if="!dayStates[day.date.getTime()]" class="fa fa-chevron-down"></i>
                                                <i v-if="dayStates[day.date.getTime()]" class="fa fa-chevron-up"></i>
                                                {{ date(day.date) }}
                                            </td>
                                            <template v-for="col in visibleColumns">
                                                <td class="nutrient" v-if="!col.hideSummary">
                                                    <div class="chart" v-if="col.key === energyDistributionId">
                                                        <chart-pie-energy v-bind:protein="day.nutrients[proteinId]" v-bind:carb="day.nutrients[carbId]" v-bind:fat="day.nutrients[fatId]"></chart-pie-energy>
                                                    </div>
                                                    <div v-else>
                                                        <nutrient-bar v-bind:goal="nutrientGoal(col.key, day.date)" v-bind:value="day.nutrients[col.key]" v-bind:precision="col.precision"></nutrient-bar>
                                                    </div>
                                                </td>
                                            </template>
                                            <td></td>
                                        </tr>
                                        <tr class="meal" v-if="dayStates[day.date.getTime()]" v-for="meal in day.meals">
                                            <td class="freeze"><router-link :to="{ name: 'meal-details', params: { id: meal.id } }">{{ mealName(meal) }}</router-link></td>
                                            <template v-for="col in visibleColumns">
                                                <td class="nutrient" v-if="!col.hideSummary">
                                                    <div class="chart" v-if="col.key === energyDistributionId">
                                                        <chart-pie-energy v-bind:protein="meal.nutrients[proteinId]" v-bind:carb="meal.nutrients[carbId]" v-bind:fat="meal.nutrients[fatId]"></chart-pie-energy>
                                                    </div>
                                                    <div v-else>
                                                        <nutrient-bar :goal="nutrientGoal(col.key, day.date, meal)" :value="meal.nutrients[col.key]" :precision="col.precision"></nutrient-bar>
                                                    </div>
                                                </td>
                                            </template>
                                            <td><button class="btn btn-danger btn-xs" @click="deleteMeal(meal)">{{ $t("delete") }}</button></td>
                                        </tr>
                                    </template>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div v-if="days.length == 0">
                        <br />
                        {{ $t("noMeals") }}
                    </div>

                    <div v-for="meal in meals">
                        <div class="alert alert-info" role="alert" v-if="meal.deleted">Ateria {{ datetime(meal.time) }} poistettu. <button class="btn btn-link" @click="restoreMeal(meal)">{{ $t("restore") }}</button></div>
                    </div>

                </div>
            </div>
        </section>
    </div>
</template>

<script src="./meals.js">
</script>

<style scoped>
    li.custom-date {
        padding: 3px 10px;
    }

        li.custom-date button {
            margin-top: 3px;
        }

    .outer {
        position: relative;
    }

    .inner {
        overflow-x: auto;
        overflow-y: visible;
        margin-left: 100px;
    }

    #meal-list {
        width: auto;
        table-layout: fixed;
        /*width: 100%;*/
    }

        #meal-list td {
            padding-bottom: 0px;
        }

            #meal-list td span {
                margin: 5px;
            }

    .freeze {
        position: absolute;
        margin-left: -100px;
        width: 100px;
        text-align: right;
    }

    th.time {
        top: 112px;
        border-width: 0px;
        width: 100px;
    }

    #meal-list a {
        cursor: pointer;
    }

    th.nutrient {
        height: 120px;
        white-space: nowrap;
    }

        th.nutrient > div {
            transform: translate(32px, 0px) rotate(-45deg);
            width: 40px;
        }

            th.nutrient > div > div {
                border-bottom: 1px solid #ccc;
                padding: 5px 10px;
                width: 100px;
            }

        th.nutrient:nth-child(2) > div {
            transform: translate(50px, -5px) rotate(-45deg);
            width: 60px;
        }

    th.unit {
        padding: 0px;
        text-align: center;
        border-right: 1px solid #ccc;
    }

    td.nutrient {
        padding-left: 1px;
        padding-right: 1px;
        border-right: 1px solid #ccc;
        text-align: center;
        width: 40px;
        overflow: visible;
    }
</style>