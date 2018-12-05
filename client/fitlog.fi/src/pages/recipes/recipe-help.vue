<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold">
                <div class="col">{{ $t('rawWeight') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.recipe.rawWeight') }}</div>
            </div>
            <div class="row text-bold">
                <div class="col">{{ $t('cookedWeight') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.recipe.cookedWeight') }}</div>
            </div>
            <div class="row text-bold">
                <div class="col">{{ $t('weightChange') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.recipe.weightChange') }}</div>
            </div>
        </div>
    </help-modal>
</template>

<script>
import HelpModal from '../../components/help-modal.vue'
import HelpMixin from '../../mixins/help-modal'
export default {
    name: 'recipe-help',
    components: { HelpModal },
    mixins:[ HelpMixin ]
}
</script>

<style scoped>
</style>