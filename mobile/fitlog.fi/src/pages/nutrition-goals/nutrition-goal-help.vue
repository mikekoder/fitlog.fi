<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold text-primary">
                <div class="col">{{ $t('nutritionGoals') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.nutritionGoals.intro') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('onlyDays') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.nutritionGoals.onlyDays') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('perMeal') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.nutritionGoals.perMeal') }}</div>
            </div>

            <div class="row q-mt-sm text-bold">
                <div class="col"><q-icon name="fas fa-heartbeat" /></div>
            </div>
            <div class="row">
                <div class="col">{{ $t('exerciseDay') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col"><q-icon name="fas fa-bed" /></div>
            </div>
            <div class="row">
                <div class="col">{{ $t('restDay') }}</div>
            </div>
        </div>
    </help-modal>
</template>

<script>
import HelpModal from '../../components/help-modal.vue'
import HelpMixin from '../../mixins/help-modal'
export default {
    name: 'nutrition-goal-help',
    components: { HelpModal },
    mixins:[ HelpMixin ]
}
</script>

<style scoped>
</style>