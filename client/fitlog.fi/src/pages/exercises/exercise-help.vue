<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold">
                <div class="col">{{ $t('muscleGroups') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.exercise.muscleGroups') }}</div>
            </div>

            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('bodyWeightPercentage') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.exercise.bodyWeightPercentage') }}</div>
            </div>
        </div>
    </help-modal>
</template>

<script>
import HelpModal from '../../components/help-modal.vue'
import HelpMixin from '../../mixins/help-modal'
export default {
    name: 'exercise-help',
    components: { HelpModal },
    mixins:[ HelpMixin ]
}
</script>

<style scoped>
</style>