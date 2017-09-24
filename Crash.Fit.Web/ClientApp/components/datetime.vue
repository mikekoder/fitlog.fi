<template>
    <span>{{ str }}</span>
</template>

<script>
import moment from 'moment'
export default {
    data() {
        return {
            str: null
        }
    },
    props:{
        value: {},
        format: {}
    },
    methods: {
    },
    mounted() {
        var format = this.format || 'DD.MM.YYYY HH:mm';
        if (this.value) {
            var m = moment(this.value);
            this.str = m.format(format);
        }
    }
}
</script>