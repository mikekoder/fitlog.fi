<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold">
                <div class="col">{{ $t('1rm') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.exerciseProgress.1rm') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('1rmBW') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.exerciseProgress.1rmBW') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('1rmInclBW') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.exerciseProgress.1rmInclBW') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('volume') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.exerciseProgress.volume') }}</div>
            </div>

        </div>
    </help-modal>
</template>

<script>
import HelpModal from '../../components/help-modal.vue'
import HelpMixin from '../../mixins/help-modal'
export default {
    name: 'exercise-progress-help',
    components: { HelpModal },
    mixins:[ HelpMixin ]
}
</script>

<style scoped>
</style>