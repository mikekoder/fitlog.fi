<template>
    <div>
        <section class="content-header"></section>
        <section class="content">
            <div v-if="isLoggedIn">
                <div class="row">
                    <div class="col-xs-12">
                        <button class="btn" @click="changeDate(-1)"><i class="fa fa-chevron-left"></i></button>
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                                {{ dateText }} <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu">
                                <li class="clickable"><a @click="changeDate('today')">{{ $t("today") }}</a></li>
                                <li class="clickable"><a @click="changeDate('yesterday')">{{ $t("yesterday") }}</a></li>
                                <li role="separator" class="divider"></li>
                                <li class="custom-date">
                                    <datetime-picker v-bind:value="selectedDate" v-bind:format="'DD.MM.YYYY'" v-on:change="changeDate(arguments[0])"></datetime-picker>
                                </li>
                            </ul>
                        </div>
                        <button class="btn" @click="changeDate(1)"><i class="fa fa-chevron-right"></i></button>

                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-xs-12">
                        <div class="box box-solid">
                            <div class="box-body">
                                <template v-if="!editNutrients">
                                    <div class="row">
                                        <div class="col-xs-12"><button class="btn pull-right" @click="editSettings" v-if="!editNutrients"><i class="fa fa-gear"></i></button></div>
                                    </div>
                                    <div class="row day-nutrients">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <span class="hidden-md hidden-lg">{{ nutrient.shortName }} <span v-if="nutrient.unit">({{ unit(nutrient.unit) }})</span></span>
                                            <span class="hidden-xs hidden-sm">{{ nutrient.name }} <span v-if="nutrient.unit">({{ unit(nutrient.unit) }})</span></span>
                                        </div>

                                    </div>
                                    <div class="row day-nutrients" v-if="meals.find(m => m.meal)">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <div v-if="nutrient.id == energyDistributionId">
                                                <chart-pie-energy v-bind:protein="dayNutrients[proteinId]" v-bind:carb="dayNutrients[carbId]" v-bind:fat="dayNutrients[fatId]"></chart-pie-energy>
                                            </div>
                                            <div v-else>
                                                <nutrient-bar v-bind:goal="nutrientGoal(nutrient.id)" v-bind:value="dayNutrients[nutrient.id]" v-bind:precision="nutrient.precision"></nutrient-bar>
                                            </div>
                                        </div>
                                    </div>
                                </template>
                                <template v-else>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <h4>{{ $t('nutrientsToShow') }}</h4>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-2" v-for="(id,i) in selectedNutrients">
                                            <select class="form-control" v-model="selectedNutrients[i]">
                                                <option v-bind:value="undefined"></option>
                                                <template v-for="group in nutrientGroups">
                                                    <option disabled>{{ group.name }}</option>
                                                    <option v-for="nutrient in group.nutrients" v-bind:value="nutrient.id">
                                                        {{ nutrient.name }} ({{ unit(nutrient.unit) }})
                                                    </option>
                                                </template>
                                            </select>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <button class="btn btn-primary pull-right" @click="saveSettings" v-if="editNutrients">{{ $t('save') }}</button>
                                            <span class="pull-right">&nbsp;</span>
                                            <button class="btn pull-right" @click="editNutrients=false" v-if="editNutrients">{{ $t('cancel') }}</button>
                                        </div>
                                    </div>
                                </template>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-12">
                        <div class="box box-solid box-primary" v-for="meal in meals">
                            <div class="box-header with-border">
                                <h3 class="box-title">{{ mealName(meal) }}</h3>
                                <button class="btn btn-sm btn-primary pull-right" @click="copyMeal(meal.meal)" :title="$t('copy')" v-if="meal.meal"><i class="fa fa-copy"></i></button>
                                <button class="btn btn-sm btn-primary pull-right" @click="pasteMeal(meal)" :title="$t('paste')" v-if="mealCopy"><i class="fa fa-paste"></i></button>
                            </div>
                            <div class="box-body" v-if="meal.meal">
                                <div class="row meal-nutrients" v-if="meal.meal.nutrients">
                                    <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                        <span class="hidden-md hidden-lg">{{ nutrient.shortName }} <span v-if="nutrient.unit">({{ unit(nutrient.unit) }})</span></span>
                                        <span class="hidden-xs hidden-sm">{{ nutrient.name }} <span v-if="nutrient.unit">({{ unit(nutrient.unit) }})</span></span>
                                        <div v-if="nutrient.id == energyDistributionId">
                                            <chart-pie-energy v-bind:protein="meal.meal.nutrients[proteinId]" v-bind:carb="meal.meal.nutrients[carbId]" v-bind:fat="meal.meal.nutrients[fatId]"></chart-pie-energy>
                                        </div>
                                        <div v-else>
                                            <nutrient-bar :goal="nutrientGoal(nutrient.id, meal.meal)" :value="meal.meal.nutrients[nutrient.id]" :precision="nutrient.precision"></nutrient-bar>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-12">
                                        <hr />
                                    </div>
                                </div>
                                <template v-for="row in meal.meal.rows">
                                    <div class="row">
                                        <div class="col-xs-8 col-sm-10 food"><span>{{ row.foodName }} {{ row.quantity }} {{ row.portionName || 'g' }}</span></div>
                                        <div class="col-xs-4 col-sm-2">
                                            <button class="btn pull-right icon-sm" @click="deleteRow(row)" :title="$t('delete')"><i class="fa fa-trash-o"></i></button>
                                            <span class="pull-right">&nbsp;</span>
                                            <button class="btn pull-right icon-sm" @click="copyRow(row)" :title="$t('copy')"><i class="fa fa-files-o"></i></button>
                                            <span class="pull-right">&nbsp;</span>
                                            <button class="btn pull-right icon-sm" @click="editRow(row)" :title="$t('edit')"><i class="fa fa-edit"></i></button>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <div v-if="nutrient.id == energyDistributionId">
                                                <chart-pie-energy v-bind:protein="row.nutrients[proteinId]" v-bind:carb="row.nutrients[carbId]" v-bind:fat="row.nutrients[fatId]"></chart-pie-energy>
                                            </div>
                                            <div v-else>{{ decimal(row.nutrients[nutrient.id], nutrient.precision) }}</div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <hr />
                                        </div>
                                    </div>
                                </template>

                            </div>
                            <div class="box-footer">
                                <div class="row">
                                    <div class="col-xs-12">
                                        <button class="btn" @click="addRow(meal)">{{ $t('addFood') }}</button>
                                        <button class="btn btn-sm icon-sm" @click="pasteRows(meal)" :title="$t('paste')" v-if="rowCopy"><i class="fa fa-paste"></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-if="!isLoggedIn">
                Fitlog on ilmainen ruoka- ja treenipäiväkirja.
            </div>
        </section>
        <section v-if="showEditMealRow">
            <edit-meal-row v-bind:show="showEditMealRow" v-bind:row="row" v-on:save="saveRow(arguments[0])" v-on:close="showEditMealRow=false"></edit-meal-row>
        </section>
    </div>
</template>

<script src="./home.js">
</script>

<style scoped>
    .day-nutrients, .meal-nutrients {
        font-weight: bold;
    }

    table.nutrition {
        width: auto;
    }

    table.nutrition td {
        width: 50px;
    }

    div.food {
        padding-top: 5px;
    }

    .box-body hr {
        margin: 2px 0px;
    }

    .box-body div.row:last-child hr {
        display: none;
    }

    option[disabled] {
        font-weight: bold;
    }

    div.row > button.pull-right {
        margin-right: 15px;
    }
</style>