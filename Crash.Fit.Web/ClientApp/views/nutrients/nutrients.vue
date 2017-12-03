<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutrients") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12 col-lg-9">
                    <div class="row hidden-xs">
                        <div class="col-sm-2"></div>
                        <div class="col-sm-3"></div>
                        <div class="col-sm-1"></div>
                        <div class="col-sm-6 center"><strong>{{ $t("visibility") }}</strong></div>
                    </div>
                    <div class="row hidden-xs">
                        <div class="col-sm-2"></div>
                        <div class="col-sm-3"></div>
                        <div class="col-sm-1"></div>
                        <div class="col-sm-3"><strong>{{ $t("summary") }}</strong></div>
                        <div class="col-sm-3"><strong>{{ $t("details") }}</strong></div>
                    </div>
                    <template v-for="group in groups">
                        <div class="row clickable" @click="toggleGroup(group.id)">
                            <div class="col-sm-12">
                                <h4>
                                    <i v-if="!groupOpenStates[group.id]" class="fa fa-chevron-down"></i>
                                    <i v-if="groupOpenStates[group.id]" class="fa fa-chevron-up"></i>
                                    {{ $t(group.id) }}
                                </h4>
                            </div>
                        </div>
                        <template v-for="(nutrient, index) in nutrientSettings[group.id]" v-if="groupOpenStates[group.id]">
                            <div class="row">
                                <div class="col-xs-4 col-sm-2">
                                    <button class="btn btn-sm" @click="moveNutrientUp(group.id, index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                    <button class="btn btn-sm" @click="moveNutrientDown(group.id, index)" :disabled="index === (nutrientSettings[group.id].length - 1)"><i class="fa fa-arrow-down"></i></button>
                                </div>
                                <div class="col-xs-6 col-sm-3">{{ nutrient.name }}</div>
                                <div class="col-xs-2 col-sm-1">{{ formatUnit(nutrient.unit) }}</div>
                                <div class="col-xs-12 col-sm-3">
                                    <div class="row">
                                        <div class="col-xs-4 hidden-sm hidden-md hidden-lg">{{ $t("summary") }}</div>
                                        <div class="col-xs-8">
                                            <select v-model="nutrient.userHideSummary">
                                                <option value="null">{{ $t("default") }} ({{ nutrient.defaultHideSummary ? $t("hide") : $t("show")}})</option>
                                                <option value="false">{{ $t("show") }}</option>
                                                <option value="true">{{ $t("hide") }}</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-12 col-sm-3">
                                    <div class="row">
                                        <div class="col-xs-4 hidden-sm hidden-md hidden-lg">{{ $t("details") }}</div>
                                        <div class="col-xs-8">
                                            <select v-model="nutrient.userHideDetails">
                                                <option value="null">{{ $t("default") }} ({{ nutrient.defaultHideDetails ? $t("hide") : $t("show")}})</option>
                                                <option value="false">{{ $t("show") }}</option>
                                                <option value="true">{{ $t("hide") }}</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row"><div class="col-xs-12"><br /></div></div>
                        </template>
                    </template>
                </div>
            </div>
            
            <hr />
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                </div>
            </div>

        </section>
        
    </div>
</template>

<script src="./nutrients.js">
</script>

<style scoped>
    .nutrient-settings thead th { text-align: center;}
    .nutrient-settings td { padding: 1px 5px;}
    .center { text-align: center; }
</style>