<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t('recipeDetails') }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-6 col-md-5 col-lg-4">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input class="form-control" v-model="name" />
                    </div>

                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <ul class="nav nav-tabs">
                        <li class="clickable" :class="{ active: tab === 'ingredients' }"><a @click="tab = 'ingredients'">{{ $t("foods") }}</a></li>
                        <li class="clickable" :class="{ active: tab === 'portions' }"><a @click="tab = 'portions'">{{ $t("portions") }}</a></li>
                        <li class="clickable" :class="{ active: tab === 'nutrients' }"><a @click="tab = 'nutrients'">{{ $t("nutrients") }}</a></li>
                    </ul>
                    <div v-if="tab === 'ingredients'">
                        <div class="row hidden-xs">
                            <div class="col-sm-4 col-text-40">
                              <label>{{ $t("food") }}</label>
                              <router-link :to="{ name: 'food-details', params: { id: constants.NEW_ID } }" target="_blank">{{ $t("createFood") }}</router-link>
                            </div>
                            <div class="col-sm-2 col-number-5"><label>{{ $t("amount") }}</label></div>
                            <div class="col-sm-3 col-text-20"><label>{{ $t("portion") }}</label></div>
                            <div class="col-sm-1 col-number-5"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1 col-actions-3">&nbsp;</div>
                        </div>
                        <template v-for="(row,index) in ingredients">
                            <div class="row">
                                <div class="col-sm-4 col-text-40">
                                    <label class="hidden-sm hidden-md hidden-lg">Ruoka</label>
                                    <router-link class="hidden-sm hidden-md hidden-lg" :to="{ name: 'food-details', params: { id: constants.NEW_ID } }" target="_blank">{{ $t("createFood") }}</router-link>
                                    <food-picker :value="row.food" @change="val => row.food=val" :disableCreation="true" />
                                </div>
                                <div class="col-xs-3 col-number-5">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("amount") }}</label>
                                    <input type="number" class="form-control" v-model="row.quantity" />
                                </div>
                                <div class="col-xs-7 col-text-20">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("portion") }}</label>
                                    <div v-if="row.food">
                                        <select class="form-control" v-model="row.portion">
                                            <option :value="undefined">g</option>
                                            <option v-for="portion in row.food.portions" :value="portion">
                                                {{ portion.name }}
                                            </option>
                                        </select>
                                    </div>
                                    <div v-if="!row.food"><select class="form-control" disabled></select></div>
                                </div>
                                <div class="col-xs-2 col-number-5">
                                    <label class="hidden-sm hidden-md hidden-lg">Paino</label>
                                    <div v-if="row.food">{{ weight(row.quantity, row.portion) }}</div>
                                    <div v-if="!row.food">&nbsp;</div>
                                </div>
                                <div class="actions col-sm-1 col-xs-12">
                                    <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                    <button class="btn btn-danger btn-sm" @click="deleteIngredient(index)">{{ $t("delete") }}</button>
                                </div>
                            </div>
                            <div class="separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                        </template>
                        <div class="row">
                            <div class="col-sm-6 col-text-40">
                                <button class="btn" @click="addIngredient">{{ $t("add") }}</button>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xs-6 col-text-20">
                                {{ $t("rawWeight") }}
                            </div>
                            <div class="col-xs-3 col-number-5">
                                {{ decimal(recipeWeight) }}
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xs-6 col-text-20">
                                {{ $t("cookedWeight") }}
                            </div>
                            <div class="col-xs-3 col-number-5">
                                <input type="number" class="form-control" v-model="cookedWeight" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xs-6 col-text-20">
                                {{ $t("weightChange") }}
                            </div>
                            <div class="col-xs-3 col-number-5">
                                <span v-if="cookedWeight">{{ decimal(weightChange, 1) }} %</span>
                            </div>
                        </div>


                    </div>
                    <div v-if="tab === 'portions'">
                        <div class="row hidden-xs">
                            <div class="col-sm-4 col-text-20"><label>{{ $t("name") }}</label></div>
                            <div class="col-sm-4 col-text-20"><label>{{ $t("portions") }}/{{ $t("recipe") }}</label></div>
                            <div class="col-sm-2 col-number-5"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1 col-actions-3">&nbsp;</div>
                        </div>
                        <template v-for="(portion,index) in portions">
                            <div class="recipe-row row">
                                <div class="col-sm-4 col-text-20">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("name") }}</label>
                                    <input type="text" class="form-control" v-model="portion.name" />
                                </div>
                                <div class="col-xs-4 col-text-20">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("portions") }}/{{ $t("recipe") }}</label>
                                    <input type="number" class="form-control" v-model="portion.amount" />
                                </div>
                                <div class="col-xs-3 col-number-5">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("weight") }} (g)</label>
                                    <span v-if="portion.amount">{{ decimal((cookedWeight || recipeWeight)/portion.amount) }} </span>
                                </div>

                                <div class="col-xs-12 col-actions-3">
                                    <label>&nbsp;</label>
                                    <button class="btn btn-danger btn-sm" @click="deletePortion(index)">{{ $t("delete") }}</button>
                                </div>
                            </div>
                            <div class="recipe-row-separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                        </template>
                        <div class="row table-actions">
                            <div class="col-sm-12"><button class="btn" @click="addPortion">{{ $t("add") }}</button></div>
                        </div>
                    </div>
                    <div v-if="tab === 'nutrients'">
                        <table>
                            <thead>
                                <tr>
                                    <th></th>
                                    <th>{{ $t("recipe") }}</th>
                                    <th>100g</th>
                                    <template v-for="portion in portions">
                                        <th>{{ portion.name }}</th>
                                    </template>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody v-for="group in $nutrientGroups">
                                <tr>
                                    <th class="clickable" colspan="2" @click="toggleGroup(group.id)">
                                        <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                        <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                        {{ group.name }}
                                    </th>
                                </tr>
                                <tr v-for="nutrient in allNutrients[group.id]" v-if="groupOpenStates[group.id]">
                                    <td>{{ nutrient.name }}</td>
                                    <td>{{ decimal(recipeNutrients[nutrient.id], nutrient.precision) }}</td>
                                    <td>{{ decimal(recipeNutrients[nutrient.id] * 100 / recipeWeight, nutrient.precision) }}</td>
                                    <template v-for="portion in portions">
                                        <td><span v-if="portion.amount">{{ decimal(recipeNutrients[nutrient.id] / recipeWeight * ((cookedWeight || recipeWeight)/portion.amount), nutrient.precision) }}</span></td>
                                    </template>
                                    <td>{{ unit(nutrient.unit)}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <hr />
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger" @click="deleteRecipe" v-if="id">{{ $t("delete") }}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script src="./recipe-details.js">
</script>
<style scoped>
</style>