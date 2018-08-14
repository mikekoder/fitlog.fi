<template>
    <help-modal :visible="visible" @close="visible=false">
        <div>
            <div class="row text-bold">
                <div class="col">{{ $t('eaten') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.home.eaten') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('rmrAbbr') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.home.rmr') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('activity2') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.home.activity2') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('expenditure') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.home.expenditure') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('total') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.home.total') }}</div>
            </div>

            <div class="row q-mt-md text-bold text-secondary">
                <div class="col">{{ $t('settings') }} (<q-icon name="fa-cogs" />)</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('nutrients') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.home.nutrients') }}</div>
            </div>
            <div class="row q-mt-sm text-bold">
                <div class="col">{{ $t('activityLevel') }}</div>
            </div>
            <div class="row">
                <div class="col">{{ $t('help.home.activityLevel') }}</div>
            </div>
        </div>
    </help-modal>
</template>

<script>
import HelpModal from '../../components/help-modal.vue'
import HelpMixin from '../../mixins/help-modal'
export default {
    name: 'home-help',
    components: { HelpModal },
    mixins:[ HelpMixin ]
}
</script>

<style scoped>
</style>