<template>
    <q-page>
        <q-tabs v-model="tab">
            <q-tab slot="title" v-for="(preset, p_index) in presets" :name="'tab-' + p_index" :label="preset.name" :key="p_index" />
            <q-tab slot="title" :name="'tab-' + presets.length" :label="$t('preset')" icon="fa-plus" @click="addPreset" />
            <q-scroll-area style="height: 72vh;">
                <q-tab-pane v-for="(preset, p_index) in presets" :name="'tab-' + p_index" :key="p_index">
                    <div class="row">
                        <div class="col-10">
                            <q-input type="text" v-model="preset.name" :float-label="$t('name')" />
                        </div>
                        <div class="col-2">
                            <q-fab small flat color="primary" icon="more_vert" active-icon="more_horiz" direction="left">
                                <q-fab-action color="negative" @click="deletePreset(p_index)" icon="delete"></q-fab-action>
                            </q-fab>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6">
                            <q-input type="number" size="md" min="0" :step="0.5" v-model="preset.sleep" :float-label="$t('sleep')" :suffix="$t('hoursAbbr')" />
                         </div>
                    </div>
                    <div class="row">
                        <div class="col-6">
                            <q-input type="number" min="0" :step="0.5" v-model="preset.lightActivity" :float-label="$t('lightActivity')" :suffix="$t('hoursAbbr')" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6">
                            <q-input type="number" min="0" :step="0.5" v-model="preset.moderateActivity" :float-label="$t('moderateActivity')" :suffix="$t('hoursAbbr')" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6">
                            <q-input type="number" min="0" :step="0.5" v-model="preset.heavyActivity" :float-label="$t('heavyActivity')" :suffix="$t('hoursAbbr')" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6">
                            <q-input type="number" :value="calculateInactivity(preset)" :float-label="$t('inactivity')" :suffix="$t('hoursAbbr')" readonly />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6">
                            <q-input type="number" :value="calculateFactor(preset)" :float-label="$t('factor')" readonly />
                        </div>
                    </div>
                    <div class="row q-mt-md">
                        {{ $t('defaultTheseDays') }}
                    </div>
                    <div class="row">
                        <div class="col-1">
                            {{ $t("mondayShort") }}
                        </div>
                        <div class="col-1">
                             {{ $t("tuesdayShort") }}
                        </div>
                        <div class="col-1">
                             {{ $t("wednesdayShort") }}
                        </div>
                        <div class="col-1">
                             {{ $t("thursdayShort") }}
                        </div>
                        <div class="col-1">
                             {{ $t("fridayShort") }}
                        </div>
                        <div class="col-1">
                             {{ $t("saturdayShort") }}
                        </div>
                        <div class="col-1">
                             {{ $t("sundayShort") }}
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-1">
                            <q-checkbox v-model="preset.monday" @input="daysChanged(preset,'monday')" />
                        </div>
                        <div class="col-1">
                            <q-checkbox v-model="preset.tuesday" @input="daysChanged(preset,'tuesday')" />
                        </div>
                        <div class="col-1">
                            <q-checkbox v-model="preset.wednesday" @input="daysChanged(preset,'wednesday')" />
                        </div>
                        <div class="col-1">
                            <q-checkbox v-model="preset.thursday" @input="daysChanged(preset,'thursday')" />
                        </div>
                        <div class="col-1">
                            <q-checkbox v-model="preset.friday" @input="daysChanged(preset,'friday')" />
                        </div>
                        <div class="col-1">
                            <q-checkbox v-model="preset.saturday" @input="daysChanged(preset,'saturday')" />
                        </div>
                        <div class="col-1">
                            <q-checkbox v-model="preset.sunday" @input="daysChanged(preset,'sunday')" />
                        </div>
                    </div>
                </q-tab-pane>
            </q-scroll-area>
        </q-tabs>
        <div class="row q-pa-sm">
            <!--
            <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
            -->
            <q-btn glossy color="primary" @click="save" :label="$t('save')" :disabled="!canSave"></q-btn>
        </div>
    </q-page>
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