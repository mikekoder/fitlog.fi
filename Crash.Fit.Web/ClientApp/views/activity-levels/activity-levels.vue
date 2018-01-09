<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("activityLevels") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <div class="row hidden-xs hidden-sm hidden-md">
                        <div class="col-lg-3 col-text-20"></div>
                        <div class="col-lg-1 col-text-20">{{ $t('activityHoursTitle') }}</div>
                        <div class="col-lg-1 col-number-8"></div>
                        <div class="col-lg-1 col-number-10"></div>
                        <div class="col-lg-1 col-number-8"></div>
                        <div class="col-lg-1 col-number-8"></div>
                        <div class="col-lg-2 col-text-20">{{ $t('defaultTheseDays') }}</div>
                        <div class="col-lg-1 col-actions-3"></div>
                    </div>
                    <div class="row hidden-xs hidden-sm hidden-md">
                        <div class="col-lg-3 col-text-20"><label>{{ $t('name') }}</label></div>
                        <div class="col-lg-1 col-number-8"><label>{{ $t('sleep') }}</label></div>
                        <div class="col-lg-1 col-number-8"><label :title="$t('lightActivityInfo')" data-toggle="tooltip" data-placement="top">{{ $t('lightActivityAbbr') }} <i class="fa fa-question-circle-o"></i></label></div>
                        <div class="col-lg-1 col-number-10"><label :title="$t('moderateActivityInfo')" data-toggle="tooltip" data-placement="top">{{ $t('moderateActivityAbbr') }} <i class="fa fa-question-circle-o"></i></label></div>
                        <div class="col-lg-1 col-number-8"><label :title="$t('heavyActivityInfo')" data-toggle="tooltip" data-placement="top">{{ $t('heavyActivityAbbr') }} <i class="fa fa-question-circle-o"></i></label></div>
                        <div class="col-lg-1 col-number-8"><label>{{ $t('inactivityAbbr') }}</label></div>
                        <div class="col-lg-1 col-number-5"><label>{{ $t('factor') }}</label></div>
                        <div class="col-lg-2 col-text-20"></div>
                        <div class="col-lg-1 col-actions-3"></div>
                    </div>
                    
                    <template v-for="(preset, index) in presets">
                        <div class="row">
                            <div class="col-lg-3 col-text-20">
                                <label class="hidden-lg">{{ $t('name') }}</label>
                                <input class="form-control" v-model="preset.name" />
                            </div>
                            <div class="hidden-md hidden-lg"><label>{{ $t('activityHoursTitle') }}</label></div>
                            <div class="col-lg-1 col-number-8">
                                <label class="hidden-lg">{{ $t('sleep') }}</label>
                                <input type="number" min="0" step="0.5" class="form-control" v-model="preset.sleep" />

                            </div>
                            <div class="col-lg-1 col-number-8">
                                <label class="hidden-lg">{{ $t('lightActivity') }}</label>
                                <input type="number" min="0" step="0.5" class="form-control" v-model="preset.lightActivity" />
                            </div>
                            <div class="col-lg-1 col-number-10">
                                <label class="hidden-lg">{{ $t('moderateActivity') }}</label>
                                <input type="number" min="0" step="0.5" class="form-control" v-model="preset.moderateActivity" />
                            </div>
                            <div class="col-lg-1 col-number-8">
                                <label class="hidden-lg">{{ $t('heavyActivity') }}</label>
                                <input type="number" min="0" step="0.5" class="form-control" v-model="preset.heavyActivity" />
                            </div>
                            <div class="col-lg-1 col-number-8">
                                <label class="hidden-lg">{{ $t('inactivity') }}</label>
                                <span class="inactivity">{{ calculateInactivity(preset) }}</span>
                            </div>
                            <div class="col-lg-1 col-number-5">
                                <label class="hidden-lg">{{ $t('factor') }}</label>
                                <span class="inactivity">{{ calculateFactor(preset) }}</span>
                            </div>
                            <div class="col-lg-2 col-text-20">
                                <label class="hidden-lg">{{ $t('defaultTheseDays') }}</label>
                                <table class="days">
                                    <tr>
                                        <td>{{ $t("mondayShort") }}</td>
                                        <td>{{ $t("tuesdayShort") }}</td>
                                        <td>{{ $t("wednesdayShort") }}</td>
                                        <td>{{ $t("thursdayShort") }}</td>
                                        <td>{{ $t("fridayShort") }}</td>
                                        <td>{{ $t("saturdayShort") }}</td>
                                        <td>{{ $t("sundayShort") }}</td>
                                    </tr>
                                    <tr>
                                        <td><input type="checkbox" v-model="preset.monday" :title="$t('monday')" @change="daysChanged(preset,'monday')" /></td>
                                        <td><input type="checkbox" v-model="preset.tuesday" :title="$t('tuesday')" @change="daysChanged(preset,'tuesday')"  /></td>
                                        <td><input type="checkbox" v-model="preset.wednesday" :title="$t('wednesday')" @change="daysChanged(preset,'wednesday')"  /></td>
                                        <td><input type="checkbox" v-model="preset.thursday" :title="$t('thursday')" @change="daysChanged(preset,'thursday')"  /></td>
                                        <td><input type="checkbox" v-model="preset.friday" :title="$t('friday')" @change="daysChanged(preset,'friday')"  /></td>
                                        <td><input type="checkbox" v-model="preset.saturday" :title="$t('saturday')" @change="daysChanged(preset,'saturday')"  /></td>
                                        <td><input type="checkbox" v-model="preset.sunday" :title="$t('sunday')" @change="daysChanged(preset,'sunday')"  /></td>
                                    </tr>
                                </table>
                            </div>
                            <div class="col-md-1 col-actions-1">
                                <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                <button class="btn btn-sm btn-danger" @click="deletePreset(index)">{{ $t('delete') }}</button>
                            </div>
                        </div>

                        <div class="separator row hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                    </template>

                    <div class="row table-actions">
                        <div class="col-sm-6"><button class="btn" @click="addPreset">{{ $t('add') }}</button></div>
                        <div class="col-sm-6"></div>
                    </div>
                </div>
            </div>
            <hr />
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t('save')}}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script src="./activity-levels.js">
</script>

<style scoped>
    table.days td {
        text-align: center;
        position: relative;
    }

    table.days td.divider {
        width: 2px;
        border-left: 1px solid black;
    }

    table input[type=checkbox], table input[type=radio] {
        width: 25px;
        height: 25px;
    }

    input.form-control {
        display: initial;
    }

    @media (min-width: 992px) {
        table.days td {
            top: -20px;
        }

        span.inactivity {
            position:relative;
            top: 7px;
        }
    }
</style>