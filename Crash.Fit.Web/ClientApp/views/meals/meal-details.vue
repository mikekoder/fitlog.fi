<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("mealDetails") }}</h1></section>
        <section class="content">
            <div class="row" v-if="isLoggedIn">
                <div class="col-xs-12">                  
                        <label>{{ $t("time") }}</label><br />
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">{{ formatDate(date) }} <span class="caret"></span></button>
                            <ul class="dropdown-menu">
                                <li>
                                    <a @click="setDate(0)">{{ $t("today") }}</a>
                                </li>
                                <li>
                                    <a @click="setDate(-1)">{{ $t("yesterday") }}</a>
                                </li>             
                                <li role="separator" class="divider"></li>
                                <li>
                                    <datetime-picker v-bind:value="date" v-on:change="date=arguments[0]" v-bind:format="'DD.MM.YYYY'"></datetime-picker>
                                </li>
                            </ul>
                        </div> 
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">{{ mealName }} <span class="caret"></span></button>
                            <ul class="dropdown-menu">
                                <li v-for="meal in mealDefinitions">
                                    <a @click="setMealDefinition(meal)">{{ meal.name }}</a>
                                </li>
                                <li>
                                    <div class="input-group">
                                      <span class="input-group-addon" style="border: 0px;">klo</span>
                                      <datetime-picker v-bind:value="selectedTime" v-on:change="selectedTime=arguments[0]" v-bind:format="'HH:mm'" v-on:click="selectedTime=arguments[0]"></datetime-picker>
                                      <span class="input-group-btn">
                                        <button class="btn btn-secondary" type="button" @click="setTime">{{ $t('use') }}</button>
                                      </span>
                                    </div>
                                </li>
                            </ul>
                        </div> 
                    
                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <ul class="nav nav-tabs">
                        <li class="clickable" v-bind:class="{ active: !showNutrients }"><a @click="toggleNutrients(false)">{{ $t("foods") }}</a></li>
                        <li class="clickable" v-bind:class="{ active: showNutrients }"><a @click="toggleNutrients(true)">{{ $t("nutrients") }}</a></li>
                    </ul>
                    <div v-if="!showNutrients">
                        <div class="row hidden-xs">
                            <div class="col-sm-4 col-text-40">
                              <label>{{ $t("food") }}</label>
                              <router-link :to="{ name: 'food-details', params: { id: constants.NEW_ID } }" target="_blank" v-if="!copyMode && isLoggedIn">{{ $t("createFood") }}</router-link>
                              <span v-if="!copyMode && isLoggedIn">|</span>
                              <router-link :to="{ name: 'recipe-details', params: { id: constants.NEW_ID } }" target="_blank" v-if="!copyMode && isLoggedIn">{{ $t("createRecipe") }}</router-link>
                            </div>
                            <div class="col-sm-2 col-number-5"><label>{{ $t("amount") }}</label></div>
                            <div class="col-sm-3 col-text-20"><label>{{ $t("portion") }}</label></div>
                            <div class="col-sm-1 col-number-5"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1 col-actions-1">&nbsp;</div>
                        </div>
                        <template v-for="(row,index) in rows">
                            <div class="row">
                                <div class="col-sm-4 col-text-40">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t('food') }}</label>
                                    <router-link class="hidden-sm hidden-md hidden-lg" :to="{ name: 'food-details', params: { id: constants.NEW_ID } }" target="_blank" v-if="!copyMode && isLoggedIn">{{ $t("createFood") }}</router-link>
                                    <span class="hidden-sm hidden-md hidden-lg" v-if="!copyMode && isLoggedIn">|</span>
                                    <router-link class="hidden-sm hidden-md hidden-lg" :to="{ name: 'recipe-details', params: { id: constants.NEW_ID } }" target="_blank" v-if="!copyMode && isLoggedIn">{{ $t("createRecipe") }}</router-link>
                                    <div v-if="copyMode">
                                        <input type="checkbox" v-model="row.copy" />
                                        <span>{{ row.food ? row.food.name : '' }}</span>
                                    </div>
                                    <food-picker v-else v-bind:value="row.food" v-on:change="setRowFood(row, arguments[0])" />
                                </div>
                                <div class="col-xs-5 col-sm-2 col-number-5">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("amount") }}</label>
                                    <span v-if="copyMode">{{ row.quantity }}</span>
                                    <input v-else type="number" class="form-control" v-model="row.quantity" />
                                </div>
                                <div class="col-xs-7 col-sm-3 col-text-20">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("portion") }}</label>
                                    <div v-if="row.food">
                                        <span v-if="copyMode">{{ row.portion ? row.portion.name : '' }}</span>
                                        <select v-else class="form-control" v-model="row.portion">
                                            <option v-bind:value="undefined">g</option>
                                            <option v-for="portion in row.food.portions" v-bind:value="portion">
                                                {{ portion.name }}
                                            </option>
                                        </select>
                                    </div>
                                    <select v-else class="form-control" disabled></select>
                                </div>
                                <div class="col-xs-6 col-sm-1 col-number-5">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("weight") }}</label>
                                    <div v-if="row.food">{{ weight(row.quantity, row.portion) }}</div>
                                    <div v-if="!row.food">&nbsp;</div>
                                </div>
                                <div class="col-xs-12 col-sm-1 col-actions-1">
                                    <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                    <button v-if="!copyMode" class="btn btn-danger btn-sm" @click="deleteRow(index)">{{ $t("delete") }}</button>
                                </div>
                            </div>
                            <div class="separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                        </template>
                        <div class="row table-actions">
                            <div class="col-sm-12">
                                <div v-if="copyMode"><input type="checkbox" v-model="copyAllRows" /> <strong>{{ $t("all") }}</strong></div>
                                <button v-else class="btn" @click="addRow">{{ $t("add") }}</button>
                            </div>
                        </div>

                    </div>
                    <div v-if="showNutrients">
                        <table>
                            <thead>
                                <tr><th></th><th></th><th></th></tr>
                            </thead>
                            <tbody v-for="group in groups">
                                <tr>
                                    <th colspan="2" class="clickable" @click="toggleGroup(group.id)">
                                        <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                        <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                        {{ $t(group.id) }}
                                    </th>
                                </tr>
                                <tr v-for="nutrient in allNutrients[group.id]" v-if="groupOpenStates[group.id]">
                                    <td>{{ nutrient.name }}</td>
                                    <td>{{ decimal(mealNutrients[nutrient.id], nutrient.precision) }}</td>
                                    <td>{{ unit(nutrient.unit)}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <hr />
            <div class="row main-actions" v-if="isLoggedIn">
                <div class="col-sm-12">
                    <button class="btn btn-primary" v-if="!copyMode" @click="save">{{ $t("save") }}</button>
                    <button class="btn" v-if="!copyMode" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn" v-if="id && !copyMode" @click="startCopy">{{ $t("copy") }}</button>
                    <button class="btn btn-primary" v-if="copyMode" @click="confirmCopy">{{ $t("confirmCopy") }}</button>
                    <button class="btn" v-if="copyMode" @click="cancelCopy">{{ $t("cancelCopy") }}</button>
                    <button class="btn btn-danger" v-if="id && !copyMode" @click="deleteMeal">{{ $t("delete") }}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script src="./meal-details.js">
</script>
<style scoped>
</style>