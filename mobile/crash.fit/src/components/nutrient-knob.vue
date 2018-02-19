<template>
    <q-knob v-model="value" :min="minValue" :max="maxValue" :color="color"></q-knob>
</template>

<script>
    import { QKnob } from 'quasar'
    export default {
        name: 'nutrient-knob',
        components: { QKnob },
        data() {
            return {
            }
        },
        computed: {
            minValue(){
                return Math.abs(Math.min(this.value || 0, this.goal.min || 0)) * -1.1;
            },
            maxValue() {
                return Math.max(this.value || 0, this.goal.max || 0) * 1.1;
            },
            color(){
                return 'primary';
            }
        },
        props: {
            goal: Object,
            value: Number,
            precision: Number
        }
    }
</script>
<style scoped>
</style>