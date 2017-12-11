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
                                    <datetime-picker :value="selectedDate" :format="'DD.MM.YYYY'" @change="changeDate"></datetime-picker>
                                </li>
                            </ul>
                        </div>
                        <button class="btn" @click="changeDate(1)"><i class="fa fa-chevron-right"></i></button>

                    </div>
                </div>
                <div class="row">&nbsp;</div>
                <div class="row">
                    <div class="col-xs-12">
                        <div class="box box-solid">
                            <div class="box-body">
                               <div class="row">
                                   <div class="col-xs-2 col-text-10">{{ $t('eaten') }}</div>
                                   <div class="col-xs-1 operator"></div>
                                   <div class="col-xs-2 col-text-10" :title="$t('rmr')">{{ $t('rmrAbbr') }}</div>
                                   <div class="col-xs-1 operator"></div>
                                   <div class="col-xs-2 col-text-10">{{ $t('expenditure') }}</div>
                                   <div class="col-xs-1 operator"></div>
                                   <div class="col-xs-2 col-text-10">{{ $t('total' )}}</div>
                               </div>
                                <div class="row total-energy">
                                   <div class="col-xs-2 col-text-10">{{ formatDecimal(eatenEnergy) }}</div>
                                   <div class="col-xs-1 operator">-</div>
                                   <div class="col-xs-2 col-text-10">{{ formatDecimal(rmr) }}</div>
                                   <div class="col-xs-1 operator">-</div>
                                   <div class="col-xs-2 col-text-10">{{ formatDecimal(energyExpenditure) }}</div>
                                   <div class="col-xs-1 operator">=</div>
                                   <div class="col-xs-2 col-text-10" :class="totalClass" :title="totalTitle">{{ formatDecimal(totalEnergy) }} {{ formatUnit('KCAL') }}</div>
                               </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-12">
                        <div class="box box-solid">
                            <div class="box-body">
                                <template v-if="!editNutrients">
                                    <div class="row">
                                        <div class="col-xs-12"><button class="btn btn-sm pull-right" @click="editSettings" v-if="!editNutrients"><i class="fa fa-gear"></i></button></div>
                                    </div>
                                    <div class="row day-nutrients">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <span class="hidden-md hidden-lg">{{ nutrient.shortName }} <span v-if="nutrient.unit">({{ formatUnit(nutrient.unit) }})</span></span>
                                            <span class="hidden-xs hidden-sm">{{ nutrient.name }} <span v-if="nutrient.unit">({{ formatUnit(nutrient.unit) }})</span></span>
                                        </div>

                                    </div>
                                    <div class="row day-nutrients" v-if="meals.find(m => m.meal)">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <div v-if="nutrient.id == energyDistributionId">
                                                <energy-distribution-bar :protein="dayNutrients[proteinId]" :carb="dayNutrients[carbId]" :fat="dayNutrients[fatId]"></energy-distribution-bar>
                                            </div>
                                            <div v-else>
                                                <nutrient-bar :goal="nutrientGoal(nutrient.id)" :value="dayNutrients[nutrient.id]" :precision="nutrient.precision"></nutrient-bar>
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
                                                <option :value="undefined"></option>
                                                <template v-for="group in nutrientGroups">
                                                    <option disabled>{{ group.name }}</option>
                                                    <option v-for="nutrient in group.nutrients" :value="nutrient.id">
                                                        {{ nutrient.name }} ({{ formatUnit(nutrient.unit) }})
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
                                
                                <div class="box-tools pull-right">
                                    <button class="btn btn-box-tool" @click="copyMeal(meal.meal)" :title="$t('copyMeal')" v-if="meal.meal"><i class="fa fa-copy"></i></button>
                                    <button class="btn btn-box-tool" @click="pasteMeal(meal)" :title="$t('pasteMeal')" v-if="mealCopy"><i class="fa fa-paste"></i></button>
                                </div>
                            </div>
                            <div class="box-body" v-if="meal.meal">
                                <div class="row meal-nutrients" v-if="meal.meal.nutrients">
                                    <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                        <span class="hidden-md hidden-lg">{{ nutrient.shortName }} <span v-if="nutrient.unit">({{ formatUnit(nutrient.unit) }})</span></span>
                                        <span class="hidden-xs hidden-sm">{{ nutrient.name }} <span v-if="nutrient.unit">({{ formatUnit(nutrient.unit) }})</span></span>
                                        <div v-if="nutrient.id == energyDistributionId">
                                            <energy-distribution-bar :protein="meal.meal.nutrients[proteinId]" :carb="meal.meal.nutrients[carbId]" :fat="meal.meal.nutrients[fatId]"></energy-distribution-bar>
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
                                            <button class="btn pull-right icon-sm" @click="copyRow(row)" :title="$t('copyRow')"><i class="fa fa-files-o"></i></button>
                                            <span class="pull-right">&nbsp;</span>
                                            <button class="btn pull-right icon-sm" @click="editRow(row)" :title="$t('edit')"><i class="fa fa-edit"></i></button>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-2" v-for="nutrient in visibleNutrients">
                                            <div v-if="nutrient.id == energyDistributionId">
                                                <energy-distribution-bar :protein="row.nutrients[proteinId]" :carb="row.nutrients[carbId]" :fat="row.nutrients[fatId]"></energy-distribution-bar>
                                            </div>
                                            <div v-else>{{ formatDecimal(row.nutrients[nutrient.id], nutrient.precision) }}</div>
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
                                        <button class="btn btn-sm icon-sm" @click="pasteRows(meal)" :title="$t('pasteRow')" v-if="rowCopy"><i class="fa fa-paste"></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div v-if="!isLoggedIn">
                Voit testata palvelua tunnuksilla <strong>testi@fitlog.fi</strong> / <strong>testi123</strong>
            </div>
        </section>
        <section v-if="showEditMealRow">
            <meal-row-editor :show="showEditMealRow" :row="row" @save="saveRow" @close="showEditMealRow=false" @createFood="name => createFood(name)" @createRecipe="name => createRecipe(name)"></meal-row-editor>
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
    .operator{
        width: 30px;
    }
    .total-energy
    { 
        font-weight: bold;
        font-size: larger;
    }
    /*
    .total-start, .total-end{
        border: 1px solid red;
    }
    .total-start {
        border-right: 0px;
    }
    .total-end {
        border-left: 0px;
    }
        */
</style>