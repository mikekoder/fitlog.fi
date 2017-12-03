<template>
    <div class="modal fade" id="modal-default" style="display: block;" :class="{ in: show }">
        <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
            <button type="button" class="close" @click="cancel">
                <span aria-hidden="true">×</span></button>
            <h4 class="modal-title">{{ $t('energyExpenditure') }}</h4>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-sm-12">
                        <input type="radio" :value="false" v-model="manual" /> <label>{{ $t('selectActivity')}}</label><br />
                        <input type="radio" :value="true" v-model="manual" /> <label>{{ $t('inputManual')}}</label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <hr />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6 col-text-40">
                        <div class="form-group">
                                <label>{{ $t("time") }}</label><br />
                                <datetime-picker :value="time" @change="val => time=val"></datetime-picker>
                            </div> 
                    </div>
                </div>
                <div v-if="manual">
                    <div class="row">
                        <div class="col-sm-6 col-text-40">
                            <div class="form-group">
                                <label>{{ $t("activityDescription") }}</label>
                                <input type="text" v-model="activityName" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6 col-text-40">
                            <div class="form-group">
                                <label>{{ $t("energyExpenditure") }} (kcal)</label>
                                <input type="number" min="0" v-model="energyKcal" class="form-control input-4" />
                            </div>
                        </div>
                    </div>
                </div>
                <div v-else>
                    <div class="row">
                        <div class="col-sm-6 col-text-40">
                            <div class="form-group">
                                <label>{{ $t("activity") }}</label>
                                <select  class="form-control" v-model="activity">
                                    <option :value="undefined"></option>
                                    <option v-for="activity in activities" :value="activity">{{ activity.name }}</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-text-40">
                            <div class="form-group">
                                <label>{{ $t("duration") }}</label><br />
                                <datetime-picker :value="duration" :format="'HH:mm'" @change="val => duration=val"></datetime-picker>
                            </div> 
                        </div>
                    </div>
                     <div class="row">
                        <div class="col-sm-12 col-text-40">
                            <div class="form-group">
                                <label>{{ $t("calculation") }}</label><br />
                                <span v-if="estimate">{{ formatDecimal(estimate) }} kcal</span> 
                            </div> 
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-default pull-left" @click="cancel">{{ $t('cancel')}}</button>
                <button class="btn btn-primary" @click="save" :disabled="!canSave">{{ $t('save')}}</button>
            </div>
        </div>
        </div>
    </div>
</template>

<script src="./energy-expenditure-editor.js">
</script>

<style scoped>
    input[type=radio]{
        width: 20px;
        height: 20px;
        position: relative;
        top: 5px;
    }
</style>