<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold text-primary">
                <div class="col">{{ $t('routines') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.routine.intro') }}</div>
            </div>

            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('load') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.routine.load') }}</div>
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