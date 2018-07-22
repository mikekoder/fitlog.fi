<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold">
                <div class="col"><q-icon name="fa-plus" /><q-icon name="arrow_drop_down" /></div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.workouts.create') }}</div>
            </div>
        </div>
    </help-modal>
</template>

<script>
import HelpModal from '../../components/help-modal.vue'
import HelpMixin from '../../mixins/help-modal'
export default {
    name: 'workouts-help',
    components: { HelpModal },
    mixins:[ HelpMixin ]
}
</script>

<style scoped>
</style>