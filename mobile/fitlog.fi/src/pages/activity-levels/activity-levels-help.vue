<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold text-primary">
                <div class="col">{{ $t('activityLevels') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.activityLevels.intro') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('sleep') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.activityLevels.sleep') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('lightActivity') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.activityLevels.light') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('moderateActivity') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.activityLevels.moderate') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('heavyActivity') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.activityLevels.heavy') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('inactivity') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.activityLevels.inactivity') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('factor') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.activityLevels.factor') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('defaultTheseDays') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.activityLevels.defaultDays') }}</div>
            </div>
        </div>
    </help-modal>
</template>

<script>
import HelpModal from '../../components/help-modal.vue'
import HelpMixin from '../../mixins/help-modal'
export default {
    name: 'activity-levels-help',
    components: { HelpModal },
    mixins:[ HelpMixin ]
}
</script>

<style scoped>
</style>