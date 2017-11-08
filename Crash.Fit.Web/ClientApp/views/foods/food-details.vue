<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("foodDetails") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-6 col-text-40">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input class="form-control" v-model="name" />
                    </div>

                </div>
            </div>
            <!--
            <div class="row">
                <div class="col-sm-6 col-text-40">
                    <div class="form-group">
                        <label>{{ $t("manufacturer") }}</label>
                        <input class="form-control" v-model="manufacturer" />
                    </div>

                </div>
            </div>
                -->
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <ul class="nav nav-tabs">
                        <li class="clickable" :class="{ active: tab === 'nutrients' }"><a @click="tab = 'nutrients'">{{ $t("nutrients") }}</a></li>
                        <li class="clickable" :class="{ active: tab === 'portions' }"><a @click="tab = 'portions'">{{ $t("portions") }}</a></li>
                    </ul>
                    <div v-if="tab === 'portions'">
                        <div class="row hidden-xs">
                            <div class="col-sm-4 col-text-30"><label>{{ $t("name") }}</label></div>
                            <div class="col-sm-2 col-number-8"><label>{{ $t("weight") }} (g)</label></div>
                            <div class="col-sm-1 col-actions-1">&nbsp;</div>
                        </div>
                        <template v-for="(portion,index) in portions">
                            <div class="row">
                                <div class="col-sm-4 col-text-30">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("name") }}</label>
                                    <input type="text" class="form-control" v-model="portion.name" />
                                </div>
                                <div class="col-xs-3 col-number-8">
                                    <label class="hidden-sm hidden-md hidden-lg">{{ $t("weight") }} (g)</label>
                                    <input type="number" class="form-control" v-model="portion.weight" />
                                </div>
                                <div class="col-xs-12 col-actions-1">
                                    <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
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
                                    <th>{{ $t("amount") }}/
                                        <select v-model="nutrientPortion" @change="changeNutrientPortion">
                                            <option v-for="portion in nutrientPortions" :value="portion">{{ portion.name }}</option>
                                        </select>
                                    </th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody v-for="group in $nutrientGroups">
                                <tr>
                                    <th class="clickable" colspan="2" @click="toggleGroup(group.id)">
                                        <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                        <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                        {{ $t(group.id) }}
                                    </th>
                                </tr>
                                <tr v-for="nutrient in nutrientsGrouped[group.id]" v-if="groupOpenStates[group.id]">
                                    <template v-if="!nutrient.computed">
                                        <td class="col-text-30">{{ nutrient.name }}</td>
                                        <td class="col-number-8"><input type="number" class="form-control" v-model="nutrients[nutrient.id]" /></td>
                                        <td>{{ unit(nutrient.unit)}}</td>
                                    </template>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <hr />
            <!--
            <div class="row errors" v-if="!isValid">
                <div class="col-sm-12">
                    <div class="alert alert-danger"><span v-for="error in errors">{{ error }}<br /></span></div>
                </div>
            </div>
            -->
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger" v-if="id" @click="deleteFood">{{ $t("delete") }}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script src="./food-details.js">
</script>
<style scoped>
    div.recipe-row {
        margin-bottom: 5px;
    }

    div.recipe-row-separator {
        padding: 0px;
    }

        div.recipe-row-separator hr {
            border: 1px solid #00c0ef;
        }

    div.food, div.quantity, div.portion, div.weight, div.actions {
        padding-right: 2px;
    }

    div.weight {
        padding-top: 5px;
    }

    @media (max-width: 767px) {
        div.food, div.quantity, div.portion, div.weight, div.actions {
            padding-right: 15px;
        }

        div.actions {
            text-align: right;
        }

            div.actions button {
                margin-top: 10px;
                margin-right: 0px;
            }
    }
</style>