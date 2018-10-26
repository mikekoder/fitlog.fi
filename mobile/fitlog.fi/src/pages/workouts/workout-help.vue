<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold text-primary">
                <div class="col">{{ $t('help.workout.actionsMenu') }} (<q-icon name="more_vert" />)</div>
            </div>
            <div class="row">
                <div class="col"><q-icon name="content_copy" /> {{ $t('copy') }} {{ $t('exercise')}} / {{ $t('set')}}</div>
            </div>
            <div class="row">
                <div class="col"><q-icon name="delete" /> {{ $t('delete') }} {{ $t('exercise')}} / {{ $t('set')}}</div>
            </div>
            

        </div>
    </help-modal>
</template>

<script>
import HelpModal from '../../components/help-modal.vue'
import HelpMixin from '../../mixins/help-modal'
export default {
    name: 'workout-help',
    components: { HelpModal },
    mixins:[ HelpMixin ]
}
</script>

<style scoped>
</style>